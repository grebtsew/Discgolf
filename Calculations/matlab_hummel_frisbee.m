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
%%  THE seven COEFFICIENTS from Potts and Crowther (2002) 
%%  CL =[   rad     CL       deg]
CL_data=[  -0.1745 -0.2250 -10 
           -0.05236  0  -3 
            0     0.150  0 
            0.08727 0.4500 5 
            0.17453 0.7250 10 
            0.26180 0.9750 15 
            0.34907 1.2000 20 
            0.43633 1.4500 25 
            0.52360 1.6750 30]; 
         
%%  CD =[    rad      CD       deg]
CD_data=[  -0.1745 0.1500 -10 
           -0.05236  0.0800 -3 
            0      0.1000  0 
            0.08727 0.1500 5 
            0.1745   0.2600 10 
            0.26180 0.3900 15 
            0.3491 0.5700 20 
            0.4363   0.7500 25 
            0.5236   0.9200 30]; 
%%  CM =[      rad      CM       deg]
CM_data=[-0.174532925 -0.0380 -10 
        -0.087266463  -0.0220  -5 
        -0.052359878  -0.0140  -3 
        0             -0.0060  0 
        0.052359878     -0.0060  3 
        0.104719755     -0.0020 6 
         0.157079633    0.0000 9 
         0.20943951    0.0100 12 
        0.261799388    0.0220 15 
         0.34906585     0.0440 20 
        0.401425728    0.0600 23 
         0.453785606    0.0840 26 
        0.523598776     0.1100 30]; 
%%CRr_deg=[-5    -4     -3     -2     -1     0     1     2     3     4     5     6     7    8     9     10     11    12     13     14     15      30    ]
CRr_rad = [-0.0873 -0.0698 -0.0524 -0.0349 -0.0175 0.0000 0.0175 0.0349 0.0524 0.0698 0.0873 0.1047 0.1222 0.1396 0.1571 0.1745 0.1920 0.2094 0.2269 0.2443 0.2618 0.5236]; 

CRr_AdvR= [2 1.04 0.69 0.35 0.17 0]; 


CRr_data =[-0.0172 -0.0192 -0.0180 -0.0192 -0.0180 -0.0172 -0.0172 -0.0168 -0.0188 -0.0164 -0.0136 
-0.0100 -0.0104 -0.0108 -0.0084 -0.0080 -0.0080 -0.0060 -0.0048 -0.0064 -0.0080 -0.0030 
-0.0112 -0.0132 -0.0120 -0.0132 -0.0120 -0.0112 -0.0112 -0.0108 -0.0128 -0.0104 -0.0096
-0.0068 -0.0072 -0.0076 -0.0052 -0.0048 -0.0048 -0.0028 -0.0032 -0.0048 -0.0064 -0.003
-0.0056  -0.0064 -0.0064 -0.0068 -0.0064 -0.0064 -0.0052 -0.0064 -0.0028 -0.0028 -0.004
-0.002 -0.004 -0.002 -0.0016   0   0   0   0 -0.002 -0.0048 -0.003
-0.0012  -0.0016 -0.0004 -0.0028 -0.0016 -0.0016 -0.0004 0.0004 0.0004 0.0008 0.0004
0.0008 0.0012 0.0008 0.002 0.0028 0.0032 0.0024 0.0028 0.0004 -0.0012 -0.003
-0.0012  -0.0012 -0.0016 -0.0016 -0.0012 -0.0004 0.0004 0.0008 0.0008 0.0016 0.0004
0.0020 0.0004 0.0016 0.002 0.002 0.002 0.0012 0.0012   0    -0.0012 -0.003
-0.0012  -0.0012 -0.0004 -0.0008 -0.0008 -0.0008 0.0004 0.0004 0.0004 0.0008 0.0004
0.0008 -0.0004   0.0000   0.0000 0.0004   0.0000   0.0000 0.0004 -0.002 -0.0012 -0.003];
CRr_data = reshape(CRr_data,length(CRr_AdvR),length(CRr_rad));
CRr_AdvR = repmat(CRr_AdvR',[1 size(CRr_data,2)]);
CRr_rad = repmat(CRr_rad,[ size(CRr_data,1) 1]);    
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
      speedo = 100.7;  % magnitude, m/sec 
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
%x0= [ 0.001 0.001 -1  vxo 0   vzo  0   thetao  0.001   0.001    gd  0    ]
%%  Second set: 
x0=[-9.03E-01 -6.33E-01 -9.13E-01 20 -4.11E-01 1.12E-03 -7.11E-02 2.11E-01 -1.49E+01 -1.48E+00 5.43E+01 5.03E+00];
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  Enter values for tfinal and nsteps: 
tfinal = 12;  % length of flight 
nsteps = 200;   % number of time steps for data 
tspan=[0:tfinal/nsteps:tfinal];
options=[];
%options = odeset('AbsTol', 0.000001,'RelTol', 0.00000001,'OutputFcn','odeplot');  
   
%%  Calls the ODE and integrate the frisbee equations of motions in the  
%%    subroutine, discfltEOM.m  
[t,x]=ode45(@discfltEOM,tspan,x0,options,CoefUsed); 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  Remaining commands are associated with creating plots of the output. A "v"  
%%  is put at the end of the variable to differentiate it from the variable  
%%  calculated in discfltEOM.m
%%  give states names  .... v for view for making plots 
vxv = x(:,4); 
vyv = x(:,5); 
vzv = x(:,6); 
fv  = x(:,7); 
thv = x(:,8); 
stv = sin(thv); 
ctv = cos(thv); 
sfv = sin(fv); 
cfv = cos(fv); 
fdv = x(:,9); 
thdv= x(:,10);
pv  = x(:,11);
velv = [vxv vyv vzv]; %expressed in N 
omegaD_N_inCv = [fdv.*ctv thdv  fdv.*stv + pv]; % expressed in c1,c2,c3 
for i=1:size(t)
     i=i; 
    vmagv(i) = norm(velv(i,:)); % a nx1 row vector 
     
    %% T_c_N=[ct      st*sf           -st*cf;  
    %%         0       cf               sf;  
    %%         st     -ct*sf            ct*cf]
   
    T_c_N=[  ctv(i)   stv(i)*sfv(i)   -stv(i)*cfv(i);  
             0        cfv(i)            sfv(i); 
             stv(i)  -ctv(i)*sfv(i)    ctv(i)*cfv(i)]; 
         
    c3v(i,:)=T_c_N(3,:);                 % c3 expressed in N frame
    vc3v(i,:)=dot(velv(i,:),c3v(i,:));   % velocity (scalar) in the c3 direction 
    vpv(i,:)= [velv(i,:)-vc3v(i,:).*c3v(i,:)];  % subtract c3 velocity component to get the 
    %                                          velocity vector projected onto the plane  
    %                                         of the disc, expressed in N 
    vpmagv(i) = norm(vpv(i,:)); 
    uvpv(i,:)=vpv(i,:)/vpmagv(i); 
    ulatv(i,:)=cross(c3v(i,:)',uvpv(i,:)')'; % unit vector perp. to v and c3, points right 
    alphav(i) = atan(vc3v(i,:)/norm(vpv(i,:))); 
    omegaD_N_inNv(i,:) = (T_c_N'*omegaD_N_inCv(i,:)')';       % expressed in n1,n2,n3 
     
    omegavpv(i,:) = dot(omegaD_N_inNv(i,:),uvpv(i,:));         %omega about vp axis 
    omegalatv(i,:) = dot(omegaD_N_inNv(i,:),ulatv(i,:));       
    omegaspinv(i,:)= dot(omegaD_N_inNv(i,:),c3v(i,:)); 
end   %for i=1:size(t) 
    Adpv = A*rho*vmagv.*vmagv/2; 
    wuns=ones(size(alphav));
    AdvRv=d*omegaspinv/2./vmagv'; 
     
if    CoefUsed==1 % using short flights coefficients 
    alphaeq= -CLo/CLa;  % this is angle of attack at zero lift 
   CLv = CLo*ones(size(alphav)) + CLa*alphav; 
CDv = CDo*ones(size(alphav)) + CDa*(alphav-alphaeq*ones(size(alphav))).*...
        (alphav-alphaeq*ones(size(alphav))); 
    CMv = CMo*wuns + CMa*alphav; 
    CRrv=CRr; 
    %CRrv= CRr*d*omegaspinv/2./vmagv';
    %CRrv= CRr*sqrt(d/g)*omegaspinv;     
    % above line produces NaN, so leave it in Mvp equation 
     
    %Mvp = Adp*d* (CRrv*d*omegaspin/2/vmag  + CRp*omegavp)*uvp;         % expressed in N 
    Mvpv = Adpv*d* (sqrt(d/g)*CRrv*omegaspinv  + CRp*omegavpv)*uvpv;    % Roll moment 
end   % if    CoefUsed==1 % using short flights coefficients  
if     CoefUsed==2 % using Potts and Crowther (2002) coefficients 
    %% interpolation of Potts and Crowther (2002) data 
    CLv  = interp1(CL_data(:,1), CL_data(:,2), alphav,'spline');     
   CDv  = interp1(CD_data(:,1), CD_data(:,2), alphav,'spline');     
     CMv = interp1(CM_data(:,1), CM_data(:,2), alphav,'spline');      
    CRrv = interp2(CRr_rad,CRr_AdvR,CRr_data,alphav,AdvRv','spline');  
    Mvpv = Adpv'*d.* (CRrv'  + CRp*omegavpv);                           % Roll moment 
end   % CoefUsed==2 % using potts coefficients    
     
liftv=CLv.*Adpv; 
dragv=CDv.*Adpv; 
     
Mlatv = Adpv'*d.* (CMv' + CMq*omegalatv);   % Pitch Moment 
Mspinv= [CNr*(fdv.*stv +pv)] ;              % Spin Down Moment 
%%  Plot four subplots in one figure, the force and moments of the simulations 
figure(1)  
    clf 
    subplot(2,2,1),plot(t,liftv) 
        title('Liftv')         
 subplot(2,2,2),plot(t,dragv)
        title('Dragv') 
 subplot(2,2,3),plot(t,Mvpv,t,Mlatv) 
    title(' Mvpv, Mlatv') 
        xlabel('time(sec)')
 subplot(2,2,4),plot(t,Mspinv) 
    title(' Mspinv') 
       xlabel('time(sec)') 
      
    veloc=sqrt(x(:,4).^2 +x(:,5).^2 +x(:,6).^2);  
    mechenrgy=m*(-2*g*x(:,3) + x(:,4).^2 +x(:,5).^2 +x(:,6).^2) /2; 
    Ho = mechenrgy/m/g; 
figure(2) 
    plot(t,mechenrgy,t,veloc,'.',t,Ho,t,AdvRv); 
    legend('mechenrgy','vel','Ho','AdvRv')
    xlabel('time(sec)') 
figure(3) 
    plot(t,alphav) 
    title('Angle of Attack, \alpha')
    xlabel('time(sec)') 
    ylabel('(rad)')
    grid 
%%  Plot four subplots in one figure, the states 
figure(12)
plot3(x(:,1),x(:,2),x(:,3))
figure(4) 
subplot(2,2,1),plot(t,x(:,1),'k-',t,x(:,2),'k--',t,x(:,3),'k:','LineWidth',2) 
            set(gca,'LineWidth',1); 
    set(gca,'FontName','arial');  
            set(gca,'FontSize',11); 
        %xlabel('Time (sec)')
         ylabel('position (m)') 
         legend('x','y','z');  
            %axis tight 
             %Ylim([-2 16])
            grid
  
subplot(2,2,2),plot(t,x(:,4),'k-',t,x(:,5),'k--',t,x(:,6),'k:','LineWidth',2) 
         set(gca,'LineWidth',1); 
            set(gca,'FontName','arial');  
            set(gca,'FontSize',11); 
            %xlabel('Time (sec)')
         ylabel('velocity (m/sec)')
         %title('Velocity of CM')
         legend('x\prime','y\prime','z\prime');  
            %axis tight 
    %Ylim([-2 14])
            %set(gca,'YTickLabel',{}); 
    grid
     
subplot(2,2,3),plot(t,x(:,7),'k-',t,x(:,8),'k--','LineWidth',2) 
         set(gca,'LineWidth',1); 
            set(gca,'FontName','arial');  
            set(gca,'FontSize',11); 
            xlabel('time (sec)')
         ylabel('orientation (rad)')
            %title('Phi, Theta') 
%title('Disc plane orientation') 
         legend('\phi','\theta');  
            grid
  
subplot(2,2,4),plot(t,x(:,9),'k-',t,x(:,10),'k--',t,0.1*x(:,11),'k:','LineWidth',2) 
   set(gca,'LineWidth',1); 
            set(gca,'FontName','arial');  
            set(gca,'FontSize',11); 
            xlabel('time (sec)')
         ylabel('angular velocity (rad/sec)')
         %title('Angular velocities') 
         legend('\phi\prime','\theta\prime','0.1*\gamma\prime');  
            grid
            
            
            
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%  File:    discfltEOM.m
%%  By:      Sarah Hummel  
%%  Date:    July 2003 
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
function xdot=discfltEOM(~,x,CoefUsed) 
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
if    CoefUsed==1   % using short flights coefficients 
    CL  = CLo + CLa*alpha; 
    alphaeq = -CLo/CLa;    % this is angle of attack at zero lift 
    CD  = CDo + CDa*(alpha-alphaeq)*(alpha-alphaeq); 
    CM=CMo + CMa*alpha; 
     
    %CRr= CRr*d*omegaspinv/2./vmagv';
    %CRr= CRr*sqrt(d/g)*omegaspinv;    % this line produces NaN, so leave it in Mvp equation 
    %Mvp = Adp*d* (CRr*d*omegaspin/2/vmag  + CRp*omegavp)*uvp;   % expressed in N 
    Mvp = Adp*d* (sqrt(d/g)*CRr*omegaspin  + CRp*omegavp)*uvp;   % expressed in N 
end   % if    CoefUsed==1 % using short flights coefficients  
if     CoefUsed==2 % using potts coefficients 
%% interpolation of Potts and Crowther (2002) data 
    CL  = interp1(CL_data(:,1), CL_data(:,2), alpha,'spline');
    CD  = interp1(CD_data(:,1), CD_data(:,2), alpha,'spline');       
    CM  = interp1(CM_data(:,1), CM_data(:,2), alpha,'spline'); 
    CRr = interp2(CRr_rad,CRr_AdvR,CRr_data,alpha,AdvR,'spline');    
    Mvp = Adp*d* (CRr*  + CRp*omegavp)*uvp;   % Roll moment, expressed in N        
end   % if    CoefUsed==2 % using potts coefficients 
  
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
end