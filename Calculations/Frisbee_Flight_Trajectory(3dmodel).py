import matplotlib.pyplot as plt
import math
import numpy as np
"""
Termology explained:

    x - axis are forward/backward / roll
    y - axis are height / spin
    z - axis are sides / pitch

 Above AREA
[-----------]
  ________       _
 /|       |\     |
| |       | |    | Side AREA
 \|       |/     |
                 -

Main sources:
http://scripts.mit.edu/~womens-ult/frisbee_physics.pdf (good article about python 2dmodels, see script in the end of article)
https://scholarworks.moreheadstate.edu/cgi/viewcontent.cgi?article=1082&context=student_scholarship_posters
https://morleyfielddgc.files.wordpress.com/2009/04/hummelthesis.pdf (good article about 3dmodels, see figures on page 33 and 35!)
"""

class Vector():
    def __init__(self, x = 0.0, y= 0.0, z= 0.0):
        self.x = x
        self.y = y
        self.z = z
    def tuple(self):
        return (self.x,self.y,self.z)
    def mag(self):
        return math.sqrt(self.x**2+self.y**2+self.z**2)

class Frisbee():
    # Frisbee specifics
    m = 0.175 # in kg
    R = 0.1085 # radie in m
    # See these on discsport.se
    Rim_Thickness = 0.012
    Rim_depth = 0.013
    Center_Height = 0.019 # max height
    # Disc specifics
    CL0 = 0.33 #The lift coefficient at alpha=0.
    CLA = 1.91 #The lift coefficient dependent on alpha

    CD0 = 0.18 #The drag coefficient at alpha = 0
    CDA = 0.69 #The drag coefficient dependent on alpha

    ALPHA0 = -4

    Iz = 0.00122
    Ix = 0.00122
    Iy = 0.00235

    # AREAs
    above_AREA = math.pi * pow(R, 2)
    side_AREA = R*2*Center_Height

    # Rotational parameters
    CRR = 0.014
    CRP = -0.0055
    CNR = -0.0000071
    CM0 = -0.08
    CMA = 0.43
    CMq = - 0.005

# Global Variables!
g = -9.81 # gravity
RHO = 1.23 # density of air
f = Frisbee() # frisbee reference

"""
Physics functions
"""

def x_axis_velocity(r, v):
    # Calculate the drag coefficient for prantls relationship
    #cd = CD0 + CDA *pow((alpha -ALPHA0)*math.pi/180, 2)
    cd = f.CD0 + f.CDA *pow((r.z -f.ALPHA0)*math.pi/180, 2)
    # The change invelocity in the x direction , obtained by solving
    # the force equation for deltav. ( The only force present is the drag force)
    #deltavx=-RHO*pow(vx,2)*above_AREA*cd*deltaT
    return -RHO*pow(v.x,2)*f.side_AREA*cd*deltaT

def y_axis_velocity(r, v):
    # Calculation of the lift coefficient using the relationship given by S.A.Hummel.
    #cl = CL0 + CLA *alpha *math.pi/180
    cl = f.CL0 + f.CLA *r.z *math.pi/180
    # The change in velocity in the y direction obtained
    # setting the  net force equal to the sum of the gravitational force and the lift force and solving for deltav
    #deltavy=(RHO*pow(vx,2)*above_AREA*cl/2/m+g)*deltaT
    return (RHO*pow(v.x,2)*f.above_AREA*cl/2/f.m+g)*deltaT

def z_axis_velocity(r, v):
    # Same as y axis but sin(x) dependent
    cl = f.CL0 + f.CLA *r.z*math.pi/180
    return 0#(RHO*pow(v.mag(),2)*f.above_AREA*cl/2/f.m)*deltaT

def x_axis_rot(w, v):
    """
    ROLL
    Crr, Crp = constants, z = Roll = p
    Formula:
    R = (Crr*r + Crp*p)*1/2*RHO*v^2*AREA*2*R
    """
    roll = (f.CRR*w.y+ f.CRP*w.x)* 1/2 * RHO * pow(v.z,2) * f.above_AREA * 2*f.R

    #roll = (crr*yrot*math.pi / 180 + crp*xrot*math.pi / 180)* 1/2 * RHO * pow(vx,2) * above_AREA * 2*R
    return roll*deltaT

def y_axis_rot(w, v):
    """
    SPIN DOWN (might be negligible)
    y = spin down = r
    angular velocity drag
    Formula:
    N = (CNR*r)*1/2*RHO*v^2*AREA*d
    """
    spin = (f.CNR*w.y) * 1/2 * RHO * pow(v.z,2) * f.above_AREA * 2*f.R
    #spin = (CNR*yrot*math.pi / 180) * 1/2 * RHO * pow(vx,2) * above_AREA * 2*R
    return spin*deltaT


def z_axis_rot(w, r, v):
    """
    PITCH
    Cm0, Cma, Cmq = constants, x = Pitch = vx
    Formula:
    M = (CM0 + Cma*alpha + CMq*q)*1/2*RHO*v^2*AREA*d
    """
    pitch =(f.CM0 + f.CMA*(r.z)  + f.CMq* w.z) * 1/2 * RHO * pow(v.z,2) * f.above_AREA * 2*f.R
    #pitch =(CM0 + CMA*(-CL0/CLA)*math.pi / 180  + CMq* zrot*math.pi / 180) * 1/2 * RHO * pow(vx,2) * above_AREA * 2*R
    return pitch*deltaT

def simulate(p, v, r, w, deltaT):
    """
    @PARAMS:
    p - position Vector
    v - velocity Vector
    r - Rotation Vector
    w - Rotation velocity Vector
    """

    print("Simulation Started:")
    print( "Position Vector (",p.x, p.y, p.z,") [m]" )
    print( "Velocity Vector (",v.x, v.y, v.z,") [m/s]" )
    print( "Velocity Vector (",3.6*v.x, 3.6*v.y, 3.6*v.z,") [km/h]" )
    print( "Rotation Vector (",r.x, r.y, r.z,") [rad/s]")
    print( "Rotational Speed Vector (", w.x,w.y,w.z, ") [rad/s]")

    # These lists are used to save values for plot!
    rx_values = []
    ry_values = []
    rz_values = []
    x_values = []
    y_values = []
    z_values = []
    vx_values = []
    vz_values = []
    counter = 0
    data = [["Position [m]","Velocity [m/s]","Rotation [Degree]","Angular Velocity [rad/s]"]]
    while (p.y > 0):

        #w.x = y_axis_rot(w, v)#*deltaT # spin
        #w.y = x_axis_rot(w, v)#*deltaT # roll
        #w.z = z_axis_rot(w, r, v)#*deltaT # pitch

        #r.x += w.x*deltaT
        #r.y += w.y*deltaT
        #r.z += w.z*deltaT

        #ax = x_axis_velocity(r, v)
        #ay = y_axis_velocity(r, v)
        #az = z_axis_velocity(r, v)
        # Position change
        v.x+= ax
        v.y+= ay
        #v.z+= az

        p.x+=v.x*deltaT
        p.y+=v.y*deltaT
        #p.z+=v.z*deltaT

        if (counter % 4 == 0): # skip some values to speed things up
            x_values.append(p.x)
            y_values.append(p.y)
            z_values.append(p.z)
            vz_values.append(v.z)
            vx_values.append(v.x)
            rx_values.append(r.x)
            ry_values.append(r.y)
            rz_values.append(r.z)

        counter+=1

    print("Done, Number of calculation iterations: ", counter)

    plot_graphs(x_values, y_values, z_values, vz_values, vx_values,rx_values, ry_values, rz_values)

def get_all_pos(x,y, data):
    res = []
    c = 0
    for d in data:
        if c == 0:
            c+=1
            continue
        res.append(d[x].tuple()[y])
    return res

def get_index_name(i):
    if i == 0:
        return "x"
    if i == 1:
        return "y"
    if i == 2:
        return "z"
    else:
        return "Nan"

def plot_graphs(x_values, y_values, z_values, vz_values, vx_values,rx_values, ry_values, rz_values):
    fig, axs = plt.subplots(5, figsize=(8, 6), dpi=80, facecolor='w', edgecolor='k')

    axs[0].set_title('Frisbee Flight Trajectory')
    axs[0].set_xlabel('X distance [m] ')
    axs[0].set_ylabel('Y height [m]')
    axs[0].plot(x_values, y_values, '-b', label= "x")
    axs[0].plot(vx_values,'-r', label= "vx")

    axs[1].set_xlabel('Z displacement [m]')
    axs[1].set_ylabel('X distance [m]')
    axs[1].plot(z_values, x_values, '-b', label= "z")
    axs[1].plot(vz_values,'-r', label= "vz")

    axs[2].set_xlabel('X Rotation')
    axs[2].set_ylabel('X distance [m]')
    axs[2].plot(x_values, rx_values, '-b', label= "rx")

    axs[3].set_xlabel('Y Rotation [m]')
    axs[3].set_ylabel('X distance [m]')
    axs[3].plot(x_values, ry_values, '-b', label= "ry")

    axs[4].set_xlabel('Z Rotation')
    axs[4].set_ylabel('X distance [m]')
    axs[4].plot(x_values, rz_values, '-b', label= "rz")


    axs[0].legend(loc="upper right")
    axs[1].legend(loc="upper right")
    axs[2].legend(loc="upper right")
    axs[3].legend(loc="upper right")
    axs[4].legend(loc="upper right")

    plt.subplots_adjust(hspace = 1  )
    plt.show()

if __name__ == "__main__":
    position_vector = Vector(0,1,0) # [m]
    velocity_vector = Vector(30,0,0) # [m/s]
    rotation_vector = Vector(0,0,10) # [Degrees celsius]
    rotational_speed_vector = Vector(0,50,0) # [radians per second] roll,spin,pitch
    deltaT = 0.1 # time intervall

    simulate(position_vector, velocity_vector, rotation_vector, rotational_speed_vector, deltaT)
