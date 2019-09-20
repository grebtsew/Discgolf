import matplotlib.pyplot as plt
import math

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
R = 0.1085 # in m

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
#above_AREA = math.pi * pow(R, 2)
above_AREA = 0.0568
side_AREA = R*2*Center_Height


def simulate(y0, vx0, vy0, alpha, deltaT):
    # Calculation of the lift coefficient using the relationship given by S.A.Hummel.

    # Calculate the drag coefficient for prantls relationship
    cl = CL0 + CLA *alpha *math.pi/180
    cd = CD0 + CDA *pow((alpha -ALPHA0)*math.pi/180, 2)

    x = 0
    y = y0
    vx = vx0
    vy = vy0

    loop_count = 0

    x_values = []
    y_values = []
    vx_values = []

    while (y > 0):
        #Thechangeinvelocityintheydirectionobtainedsettingthe//netforceequaltothesumofthegravitationalforceandthe//liftforceandsolvingfordeltav.double
        deltavy=(RHO*pow(vx,2)*above_AREA*cl/2/m+g)*deltaT
        #Thechangeinvelocityinthexdirection,obtainedby//solvingtheforceequationfordeltav.(Theonlyforce//presentisthedragforce).double
        deltavx=-RHO*pow(vx,2)*above_AREA*cd*deltaT;

        vx=vx+deltavx
        vy=vy+deltavy
        x=x+vx*deltaT
        y=y+vy*deltaT

        if (loop_count % 4 == 0): # skip some values to speed things up
            x_values.append(x)
            y_values.append(y)
            vx_values.append(vx)

        loop_count+=1

    plt.title('Flight Trajectory')
    plt.xlabel('distance [m]')
    plt.ylabel('height [m]')

    plt.plot(x_values, y_values, '-b', label= "x")
    plt.plot(vx_values,'-r', label= "vx")
    plt.legend(loc="upper right")
    plt.show()

if __name__ == "__main__":
    y0 = 1 # 1 m above ground
    vx0 = 14 # 20 m/s x axis
    vy0 = 0 # 10 m/s y axis
    alpha = 7.5 # alpha angle degrees
    deltaT = 0.1 # number of calculation intervalls

    simulate(y0, vx0, vy0, alpha, deltaT)
