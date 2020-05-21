import matplotlib.pyplot as plt
import math

"""
Termology explained:

    x - axis are forward
    y - axis are height
    z - axis are sides

    *alpha is then the angle between x-axis and the outgoing direction
    *beta is the angle in z-axis

 Above AREA
[-----------]
  ________       _
 /|       |\     |
| |       | |    | Side AREA
 \|       |/     |
                 -

Main sources:
http://scripts.mit.edu/~womens-ult/frisbee_physics.pdf (good article with about 2dmodels)
https://scholarworks.moreheadstate.edu/cgi/viewcontent.cgi?article=1082&context=student_scholarship_posters
https://morleyfielddgc.files.wordpress.com/2009/04/hummelthesis.pdf (good article with about 3dmodels)
"""


# Variables
x = 0.0
y = 0.0
z = 0.0

vx = 0.0
vy = 0.0
vz = 0.0

g = -9.81 # gravity
RHO = 1.23 # density of air

# Frisbee specifics
m = 0.175 # in kg
R = 0.1085 # radie in m

# See these on discsport.se
Rim_Thickness = 0.012
Rim_depth = 0.013
Center_Height = 0.019 # max height

CL0 = 0.1 #The lift coefficient at alpha=0.
CLA = 1.4 #The lift coefficient dependent on alpha

CD0 = 0.08 #The drag coefficient at alpha = 0
CDA = 2.72 #The drag coefficient dependent on alpha

ALPHA0 = -4

# AREAs
above_AREA = math.pi * pow(R, 2)
side_AREA = R*2*Center_Height

"""
Physics functions
"""

def x_axis_physics(alpha, vx):
    # Calculate the drag coefficient for prantls relationship
    cd = CD0 + CDA *pow((alpha -ALPHA0)*math.pi/180, 2)
    # The change invelocity in the x direction , obtained by solving
    # the force equation for deltav. ( The only force present is the drag force)
    return -RHO*pow(vx,2)*above_AREA*cd*deltaT;

def y_axis_physics(alpha, vx):
    # Calculation of the lift coefficient using the relationship given by S.A.Hummel.
    cl = CL0 + CLA *alpha *math.pi/180
    # The change in velocity in the y direction obtained
    # setting the  net force equal to the sum of the gravitational force and the lift force and solving for deltav
    return (RHO*pow(vx,2)*above_AREA*cl/2/m+g)*deltaT


def z_axis_physics(beta, vx, vy, vz):

    # Aerodynamic physics
    """
    ROLL
    Crr, Crp = constants, z = Roll = p
    Formula:
    R = (Crr*r + Crp*p)*1/2*RHO*v^2*AREA*2*R
    """
    crr = 0.014
    crp = -0.0055
    roll = (crr*vy + crp*vz)* 1/2 * RHO * pow(vx,2) * above_AREA * 2*R

    """
    PITCH
    Cm0, Cma, Cmq = constants, x = Pitch  = q
    Formula:
    M = (CM0 + Cma*alpha + CMq*q)*1/2*RHO*v^2*AREA*d
    """
    CM0 = -0.08
    CMA = 0.43
    CMq = - 0.005
    pitch =(CM0 + CMA*beta*math.pi / 180  + CMq*vx) * 1/2 * RHO * pow(vx,2) * above_AREA * 2*R

    return roll+pitch

def rotation(rot, vx):
    """
    SPIN DOWN (might be negligible)
    y = spin down = r
    angular velocity drag
    Formula:
    N = (CNR*r)*1/2*RHO*v^2*AREA*d
    """
    CNR = -0.000034
    N = (CNR*rot) * 1/2 * RHO * pow(vx,2) * above_AREA * 2*R
    return N

def simulate(y0, vx0, vy0, vz0, alpha, beta, rot0, deltaT):

    print("Simulation Started:")
    print( "Velocity Vector (",vx0, vy0, vz0,") [m/s]" )
    print( "Velocity Vector (",3.6*vx0, 3.6*vy0, 3.6*vz0,") [km/h]" )
    print( "Rotation Vector (",alpha, 0, beta,")")
    print( "Rotation Speed ", rot0, "[rad/s]")

    x = 0
    y = y0
    z = 0
    vx = vx0
    vy = vy0
    vz = vz0
    rot = rot0

    loop_count = 0

    # These lists are used to save values for plot!
    x_values = []
    y_values = []
    z_values = []
    vx_values = []
    vz_values = []

    while (y > 0):

        deltavy =y_axis_physics(alpha, vx)
        deltavx =x_axis_physics(alpha, vx)
        deltavz =z_axis_physics(beta, vx, vy, vz)

        rot = rotation(rot, vx)*deltaT

        vx=vx+deltavx
        vy=vy+deltavy
        vz=vz+deltavz
        x=x+vx*deltaT
        y=y+vy*deltaT
        z=z+vz*deltaT

        if (loop_count % 4 == 0): # skip some values to speed things up
            x_values.append(x)
            y_values.append(y)
            z_values.append(z)
            vz_values.append(vz)
            vx_values.append(vx)

        loop_count+=1

    print("Done, Number of calculation iterations: ", loop_count)
    plot_graphs(x_values, y_values, z_values, vz_values, vx_values)

def plot_graphs(x_values, y_values, z_values, vz_values, vx_values):
    fig, axs = plt.subplots(2)

    axs[0].set_title('Frisbee Flight Trajectory')
    axs[0].set_xlabel('X distance [m] ')
    axs[0].set_ylabel('Y height [m]')
    axs[0].plot(x_values, y_values, '-b', label= "x")
    axs[0].plot(vx_values,'-r', label= "vx")

    axs[1].set_xlabel('Z displacement [m]')
    axs[1].set_ylabel('X distance [m]')
    axs[1].plot(z_values, x_values, '-b', label= "z")
    axs[1].plot(vz_values,'-r', label= "vz")

    axs[0].legend(loc="upper right")
    axs[1].legend(loc="upper right")

    plt.subplots_adjust(hspace = 0.5  )
    plt.show()

if __name__ == "__main__":
    y0 = 1 # m above ground
    vx0 = 27 # m/s x axis
    vy0 = 0 # m/s y axis
    vz0 = 0 # m/s in z axis
    rot0 = math.pi # rotation radian per second
    alpha = 7.5 # alpha angle degrees (z axis)
    beta = 0  # beta angle degrees
    deltaT = 0.1 # number of calculation intervalls

    simulate(y0, vx0, vy0, vz0, alpha, beta, rot0, deltaT)
