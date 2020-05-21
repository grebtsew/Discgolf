#Developers: Tom McClintock, Elizabeth Hannah
#Initialize frisbee object with appropriate coefficient values and initial conditions.
#Current parameter input values obtained from Hummel 2003 (pg. 82)
#Change to debug=False to supress printing
import frisbee_article_appendixA
import frisbee_article_appendixB

from mpl_toolkits import mplot3d
from scipy.integrate import odeint


import math
from matplotlib import *
import matplotlib.pyplot as plt

import numpy as np

#init_positions = [0.,0.,1.,20.,0.,0.,0.,-.87,0.,0.,0.,-50.]

test_fris=frisbee_article_appendixB.Frisbee(0.,0.,1.0,30.,0.,0.,0.,0.087,0.,0.,0.,50.)#We pass the model directly to the frisbee

# disc
model = frisbee_article_appendixA.Model(0.3331,1.9124,0.1769,0.685,0.4338,-0.0144,-0.0821,-0.0125,-0.00171,-0.0000341)
test_fris.model=model

#test_fris.initialize_model(0.331,1.9124,0.1769,0.685, 0.4338,0.0144,0.0821, 0.0125,0.00171,0.0000341)
#print("Initial derivatives: ",test_fris.derivatives_array())
#sys.exit()
#---------------------------------------------------------------------------------------------------#
#Define function to feed into ODE integrator. The function will compute all relevant #at time t, defined in main. All derivatives are explained and calculated in frisbee_#module, and an array of derivatives is called below.
def old_equations_of_motion(positions, t):
    #Update the positions of the frisbee
    (test_fris.x,test_fris.y,test_fris.z,
    test_fris.vx,test_fris.vy,test_fris.vz,
    test_fris.phi,test_fris.theta,test_fris.gamma,
    test_fris.phidot,test_fris.thetadot,test_fris.gammadot)=positions
    #If it is on the ground, turn all derivatives to 0.
    if test_fris.z <= 0.0:
        return np.zeros_like(positions)
    #Calculate all derivatives based on current positions. Return array of derivatives.
    positionsdot=test_fris.derivatives_array()
    return positionsdot

def equations_of_motion(positions, t, frisbee):
    #Update the positions of the frisbee
    (frisbee.x,frisbee.y,frisbee.z,
    frisbee.vx,frisbee.vy,frisbee.vz,
    frisbee.phi,frisbee.theta,frisbee.gamma,
    frisbee.phidot,frisbee.thetadot,frisbee.gammadot)=positions
    #If it is on the ground, turn all derivatives to 0.
    if frisbee.z <= 0.0:
        return np.zeros_like(positions)
    #Calculate all derivatives based on current positions. Return array of derivatives.
    positionsdot=frisbee.derivatives_array()
    #print positionsdot
    return positionsdot
def main():
#---------------------------------------------------------------------------------------------------#
    #Integration of ODEs that reflect equations of motion.
    #Common release conditions obtained from Hummel 2003 (pg. 83)
    #The starting positions are a copy of the test frisbee’s
    #positions at the start of this main() function.
    initial_positions=np.array(test_fris.get_positions()).copy()
    #Define initial and final times
    ti=0.0
    tf = 10.0
    #Define number of steps and calculate step size
    n = 1000
    dt=(tf-ti)/(n-1)

    #Create time array
    time=np.arange(ti,tf,dt)
    time=np.linspace(ti,tf, n)
    xlist = []
    ylist = []
    zlist = []
    poslist= []

    """
    THIS IS A SIMPLE RK4 ODE SOLVER WRITTEN BY TOM FOR DEBUGGING
    solution = np.zeros((n,12))
    positions = np.array(initial_positions).copy()
    for i in range(n):
        print ("\n\nAt t = %f"%time[i])
        print (test_fris)
        solution[i] = positions.copy()
        k1 = np.array(equations_of_motion(positions,time[i],test_fris))
        temp = positions + k1*dt/2.
        k2 = np.array(equations_of_motion(temp,time[i]+dt/2.,test_fris))
        temp = positions + k2*dt/2.
        k3 = np.array(equations_of_motion(temp,time[i]+dt/2.,test_fris))
        temp = positions + k3*dt
        k4 = np.array(equations_of_motion(temp,time[i]+dt,test_fris))
        positions = positions + dt/6.*(k1+2*k2+2*k3+k4)
        poslist.append(positions)
        xlist.append(positions[0])
        ylist.append(positions[1])
        zlist.append(positions[2])

        if positions[2] < 0: # below ground
            break
    #print [positions[:]]
    #print "positions:",test_fris.get_positions()
    #sys.exit()
    """

    solution=odeint(equations_of_motion, initial_positions, time, args=(test_fris,))
    np.savetxt("solution.txt",solution)

    fig=plt.figure()
    ax=fig.add_subplot(111, projection='3d')


    plt.plot(solution[:,0], solution[:,1],solution[:,2])#,)
    plt.draw()
    plt.show()
    #plt.pause(1)
    #plt.close(fig)
if __name__ == "__main__":
    main()
