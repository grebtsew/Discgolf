%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  File:    simulate_flight.m
%%  By:      Sarah Hummel
%%  Date:    July 2003
%%
%%  This MATLAB program allows the simulation of a single frisbee flight
%%  given initial conditions and a set of aerodynamic coefficients.
%%  Calls subroutine discfltEOM.m, the equations of motion for the frisbee.
%%  Inertial xyz coordinates = forward, right and down
%%
%%   before executing code (as described below):
%%  1) specify value for "CoefUsed"
%%  2) specify which values for the damping coefficients, use long flight or short
%%    flight values.
%%  3) specify Simulation set of initial conditions: thetao, speedo, betao, and po
%%  4) specify which "x0" command to use
%%  5) specify values for "tfinal" and "nsteps"
%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
clear
format short
global m g Ia Id A d rho
global CLo CLa CDo CDa CMo CMa CRr
global CL_data CD_data CM_data CRr_rad CRr_AdvR CRr_data
global CMq CRp CNr
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  Set "CoefUsed" = 1 OR 2
%%  This chooses the values of coefficients (specifies a set of if/then statements)
%%  to use for CLo CLa CDo CDa CMo CMa CRr.
%%  CoefUsed = 1 ... choose for using estimated short flights lift, drag, moment coefficients
%%  CoefUsed = 2 ... choose for using Potts and Crowther (2002) lift, drag, moment coefficients
CoefUsed=2;
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  define non-aerodynamic parameters
m = 0.175;   % Kg
g = 9.7935;  % m/s^2
A = 0.057;   % m^2
d = 2*sqrt(A/pi);  % diameter
rho = 1.23; % Kg/m^3
Ia  = 0.002352;  % moment of inertia about the spinning axis
Id  = 0.001219; % moment of inertia about the planar axis'
 %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  THE THREE ESTIMATED COEFFICIENTS
%CMq= -0.005,     CRp =-0.0055,   CNr =  0.0000071       % short (three) flights
 CMq= -1.44E-02 ; CRp =-1.25E-02; CNr = -3.41E-05;       % long flight f2302
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  THE seven COEFFICIENTS estimated from three flights
CLo=  0.3331;
CLa=  1.9124;
CDo=  0.1769;
CDa=  0.685;
CMo= -0.0821;
CMa=  0.4338;
CRr=  0.00171;  % for nondimensionalization = sqrt(d/g), magnitude of CRr changes
        % with nondimensionalization

    %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  angle and angular velocities in rad and rad/sec respectively
%%  phi   = angle about the x axis  phidot       = angular velocity
%%  theta = angle about the y axis    thetadot    = angular velocity
%%  gamma = angle about the z axis   gd(gammadot)   = angular velocity

%%  For reference, two sets of previously used initial conditions...
%%  Long flight (f2302) release conditions:
%%      thetao = 0.211;  speedo = 13.5;  betao = 0.15;  gd=54
%%  Common release conditions:
%%    thetao = 0.192;  speedo = 14; betao = 0.105; gd=50
%%  Define Simulation set initial conditions, enter your choosen values here:
      thetao =  .192;    % initial pitch angle
      speedo = 13.7;  % magnitude, m/sec
      betao  = .105;     % flight path angle in radians between velocity vector and horizontal
      gd=50;            % initial spin
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
alphao = thetao - betao;          % inital alpha
vxo = speedo * cos(betao);        % velocity component in the nx direction
vzo = -(speedo * sin(betao));     % velocity component in the nz direction
                                 %    (note: nz is positive down)
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  x0= vector of initial conditions
%%  x0= [   x    y     z  vx  vy  vz   phi theta   phidot  thetadot gd   gamma]
%%  Specify the set of inital conditions to use:
%%    the first set of conditions is for a disc released:
%%      theta, speed, and spin as specified above (thetao, speedo, gd),
%%        1 meter above the ground, forward and right 0.001,
%%        no roll angle, no velocity in the the y direction
%%        and for positive alpha, disc is pitched up, with a neg. w component
%%    the second set is the long flight f2302 estimated initial conditions
%%  First set:
%x0= [ 0.001 0.001 -1  vxo 0   vzo  0   thetao  0.001   0.001    gd  0    ];
%%  Second set:
x0=[-9.03E-01 0 -9.13E-01 1.34E+01 -4.11E-01 1.12E-03 -7.11E-02 2.11E-01 -1.49E+01 -1.48E+00 5.43E+01 5.03E+00];
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  Enter values for tfinal and nsteps:
%tfinal = 12;  % length of flight
%nsteps = 200;   % number of time steps for data
%tspan=[0:tfinal/nsteps:tfinal];
%options=[];
%options = odeset('AbsTol', 0.000001,'RelTol', 0.00000001,'OutputFcn','odeplot');

%%  Calls the ODE and integrate the frisbee equations of motions in the
%%    subroutine, discfltEOM.m
x = x0;
counter = 0;
while x(2) >= -100
x = discfltEOM(x);
counter = counter + 1 ;
end
counter
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  Remaining commands are associated with creating plots of the output. A "v"
%%  is put at the end of the variable to differentiate it from the variable
%%  calculated in discfltEOM.m
%%  give states names  .... v for view for making plots


function x = discfltEOM(x)
% Equations of Motion for the frisbee
% The inertial frame, xyz = forward, right and down
global m g Ia Id A d rho
global CLo CLa CDo CDa CMo CMa CRr
global CL_data CD_data CM_data CRr_rad CRr_AdvR CRr_data
global CMq CRp CNr
% x = [ x y z vx vy vz f th  fd  thd  gd gamma]
%       1 2 3 4  5  6  7  8   9   10  11  12
%% give states normal names
vx = x(4);
vy = x(5);
vz = x(6);
f  = x(7);
th = x(8);
st = sin(th);
ct = cos(th);
sf = sin(f);
cf = cos(f);
fd = x(9);
thd= x(10);
gd  = x(11);

%% Define transformation matrix
%% [c]=[T_c_N] * [N]
T_c_N=[ct st*sf -st*cf; 0 cf sf; st -ct*sf ct*cf];
%% [d]=[T_d_N] * [N]
%T_d_N(1,:)=[cg*ct  sg*cf+sf*st*cg  sf*sg-st*cf*cg];
%T_d_N(2,:)=[ -sg*ct cf*cg-sf*sg*st sf*cg+sg*st*cf];
%T_d_N(3,:)=[ st -sf*ct cf*ct]

[~,eval]=eig(T_c_N);
eigM1=diag(eval);
m1=norm(eigM1(1));
m2=norm(eigM1(2));
m3=norm(eigM1(3));

c1=T_c_N(1,:);      % c1 expressed in N frame
c2=T_c_N(2,:);      % c2 expressed in N frame
c3=T_c_N(3,:);      % c3 expressed in N frame

%% calculate aerodynamic forces and moments
%% every vector is expressed in the N frame
vel = [vx vy vz];   %expressed in N
vmag = norm(vel);

vc3=dot(vel,c3);     % velocity (scalar) in the c3 direction
vp= [vel-vc3*c3];     % subtract the c3 velocity component to get the velocity vector
% projected onto the plane of the disc, expressed in N
alpha = atan(vc3/norm(vp));
Adp = A*rho*vmag*vmag/2;
uvel  = vel/vmag;            % unit vector in vel direction, expressed in N
uvp   = vp/norm(vp);      % unit vector in the projected velocity direction, expressed in N
ulat  = cross(c3,uvp); % unit vec perp to v and d3 that points to right, right?
%% first calc moments in uvp (roll), ulat(pitch) directions, then express in n1,n2,n3
omegaD_N_inC = [fd*ct thd  fd*st+gd];       % expressed in c1,c2,c3
omegaD_N_inN = T_c_N'*omegaD_N_inC';      % expressed in n1,n2,n3

omegavp   = dot(omegaD_N_inN,uvp);
omegalat  = dot(omegaD_N_inN,ulat);
omegaspin = dot(omegaD_N_inN,c3);            % omegaspin = p1=fd*st+gd

AdvR= d*omegaspin/2/vmag ;                    % advanced ration
CL  = CLo + CLa*alpha;
alphaeq = -CLo/CLa;    % this is angle of attack at zero lift
CD  = CDo + CDa*(alpha-alphaeq)*(alpha-alphaeq);
CM=CMo + CMa*alpha;
Mvp = Adp*d* (sqrt(d/g)*CRr*omegaspin  + CRp*omegavp)*uvp;   % expressed in N

lift  = CL*Adp;
drag  = CD*Adp;
ulift = -cross(uvel,ulat);          % ulift always has - d3 component
udrag = -uvel;
Faero = lift*ulift + drag*udrag;     % aero force in N
FgN   = [ 0 0 m*g]';                 % gravity force in N
F = Faero' + FgN;
Mlat  = Adp*d* (CM + CMq*omegalat)*ulat;    % Pitch moment expressed in N
Mspin = [0 0 +CNr*(omegaspin)];              % Spin Down moment expressed in C
M = T_c_N*Mvp' + T_c_N*Mlat' + Mspin';     % Total moment expressed in C

% set moments equal to zero if wanted...
% M=[0 0 0];

% calculate the derivatives of the states
xdot = vel';
xdot(4)  = (F(1)/m);     %accx
xdot(5)  = (F(2)/m);     %accy
xdot(6)  = (F(3)/m);     %accz
xdot(7)  = fd;
xdot(8)  = thd;
xdot(9)  = (M(1) + Id*thd*fd*st - Ia*thd*(fd*st+gd) + Id*thd*fd*st)/Id/ct;
xdot(10) = (M(2) + Ia*fd*ct*(fd*st +gd) - Id*fd*fd*ct*st)/Id;
fdd=xdot(9);
xdot(11) = (M(3) - Ia*(fdd*st + thd*fd*ct))/Ia;
xdot(12) = x(11);
xdott=xdot';

% calculate angular momentum
H = [Id 0 0 ; 0 Id 0; 0 0 Ia]*omegaD_N_inC';
format long;
magH = norm(H);
format short;
state=x';
x = xdott;
end
