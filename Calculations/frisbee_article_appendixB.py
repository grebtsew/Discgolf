
#Developers: Tom McClintock, Elizabeth Hannah
import numpy as np
import math
import frisbee_article_appendixA
rho=1.225 #kg/m^3, density of air
area=0.057 #m^2, area of disc used in Hummel 2003
m=0.175 #kg, mass of disc used in Hummel 2003
g=9.81 #m/s^2, gravitational acceleration
F_gravity = m*g*np.array([0.,0.,-1.]) #gravitational force
Izz=0.002352 #kg-m^2
Ixx=Iyy=Ixy=0.001219 #kg-m^2
d=2*(area/math.pi)**0.5 ##m; diameter of disc
#---------------------------------------------------------------------------------------------------#
#Create a Frisbee class, and assign Frisbee self values that correspond
#to initial conditions (positions) of the frisbee.
class Frisbee(object):
    model = None

    def __init__(self,x,y,z,vx,vy,vz,phi,theta,gamma, phidot,thetadot,gammadot,debug=False):
        self.debug=debug #Default value is false if debug isn’t specified.
        self.x=x
        self.y=y
        self.z=z
        self.vx=vx
        self.vy=vy
        self.vz=vz
        self.phi=phi
        self.theta=theta
        self.gamma=gamma
        self.phidot=phidot
        self.thetadot=thetadot
        self.gammadot=gammadot
        self.update_data_fields() #Calculate all vectors and matrices
    #Represent Frisbee object by printing instantaneous positions and velocities.
    def __str__(self):
        outstr = "Position: (%f,%f,%f)\n"%(self.x,self.y,self.z)
        outstr += "Velocity: (%f, %f, %f)\n"%(self.vx, self.vy, self.vz)
        outstr+= "Angles: (%f,%f,%f)\n"%(self.phi,self.theta,self.gamma)
        outstr+= "AngVelos: (%f,%f,%f)\n"%(self.phidot, self.thetadot, self.gammadot)
        return outstr

    def initialize_model(self,PL0, PLa, PD0, PDa, PTya, PTywy, PTy0, PTxwx, PTxwz, PTzwz):
        self.model=model_object.Model(PL0, PLa, PD0, PDa, PTya, PTywy, PTy0, PTxwx, PTxwz)

        #---------------------------------------------------------------------------------------------------#
        #A function that returns the positions of the frisbee as it currently is
    def get_positions(self):
        return [self.x,self.y,self.z,self.vx,self.vy,self.vz,self.phi,self.theta,self.gamma,self.phidot, self.thetadot, self.gammadot]
        #---------------------------------------------------------------------------------------------------#
        #Update the data fields in the frisbee that need to be used in the calculation,
        #namely the rotation matrix, the bhat vectors, the angle of attack, the velocity, etc.
    def update_data_fields(self):
        self.calculate_trig_functions()
        self.velocity = np.array([self.vx,self.vy,self.vz])
        self.angle_dots = np.array([self.phidot,self.thetadot,self.gammadot])
        self.vhat = self.velocity/np.linalg.norm(self.velocity)
        self.v2 = np.dot(self.velocity,self.velocity)
        self.rotation_matrix = self.calc_rotation_matrix()
        self.xbhat,self.ybhat,self.zbhat = self.calc_body_hat_vectors()
        self.angle_of_attack = self.calc_angle_of_attack()
        self.angular_velocity_frisframe = self.calc_angular_velocity_frisframe()
        self.angular_velocity_labframe = np.dot(self.angular_velocity_frisframe,self.rotation_matrix)
        self.wxb,self.wyb,self.wzb = self.calc_angular_velocity()
        return
        #Success
    #---------------------------------------------------------------------------------------------------#
    #Calculates the trigonometric functions of the Euler angles of our frisbee
    def calculate_trig_functions(self):
        self.sin_phi = np.sin(self.phi)
        self.cos_phi = np.cos(self.phi)
        self.sin_theta = np.sin(self.theta)
        self.cos_theta = np.cos(self.theta)
        return #Success
#---------------------------------------------------------------------------------------------------#
#Calculate rotation matrix. Rotation matrix is the product Ry(theta)*Rx(phi) of "Euler #Matrices", found at https://en.wikipedia.org/wiki/Davenport_chained_rotations.
    def calc_rotation_matrix(self):
        sp,cp = self.sin_phi, self.cos_phi
        st,ct = self.sin_theta,self.cos_theta
        return np.array([[ct, sp*st, -st*cp],
        [0, cp, sp],
        [st, -sp*ct, cp*ct]])
#---------------------------------------------------------------------------------------------------#
#Calculate the body hat vectors.
    def calc_body_hat_vectors(self):
        v = self.velocity
        #The z-body hat vectory.
        zbhat = self.rotation_matrix[2] #The lower row
        #Calculate the unit vector in the x-body direction. Corresponds to the velocity
        #unit vector in the plane of the disc.
        zcomponent = np.dot(v,zbhat) #This is the lab-frame velocity vector
        vplane = v - (zbhat*zcomponent) #Velocity in the plane of the disc’s motion
        xbhat = vplane/np.linalg.norm(vplane) #Unit vector int the plane of the disc
        #Calculate the unit vector in the y-body direction.
        ybhat = np.cross(zbhat,xbhat)
        return [xbhat,ybhat,zbhat]
#---------------------------------------------------------------------------------------------------#
#Calculate angle of attack, defined as angle between plane of disc and velocity vector
#of the frisbee’s motion. First step is to calculate scalar component of the velocity
#that is not in the plane of the disc (dot product of z-body unit vector and velocity
#vector). We then subtract the scalar component calculated above times the z-body unit
#vector itself from the velocity vector. This gives us the velocity in the plane of #disc. The angle of attack is the angle formed between the z-component of the velocity #in the plane of the disc, which we calculate using an arctan function.
    def calc_angle_of_attack(self):
        v = self.velocity
        zbhat = self.zbhat
        zcomponent = np.dot(v,zbhat)
        v_plane = v - zbhat*zcomponent
        return -math.atan(zcomponent/np.linalg.norm(v_plane))
#---------------------------------------------------------------------------------------------------#
#Calculate the angular velocity in the frisbee frame. This is the \vec{w} quantity
#Note that this is not \vec{w}_F.
#Details found on page 34 in Hummell’s thesis
    def calc_angular_velocity_frisframe(self):
        st,ct = self.sin_theta,self.cos_theta
        return np.array([self.phidot*ct, self.thetadot, self.phidot*st + self.gammadot])
#---------------------------------------------------------------------------------------------------#
#Calculate the angular velocities rotated into the lab frame but dotted into
#the body-hat vectors as expressed in the lab frame (i.e. xbhat).
    def calc_angular_velocity(self):
        av_labframe = self.angular_velocity_labframe
        xbhat,ybhat,zbhat = self.xbhat,self.ybhat,self.zbhat
        wxb = np.dot(av_labframe,xbhat)
        wyb = np.dot(av_labframe,ybhat)
        wzb = np.dot(av_labframe,zbhat)
        return [wxb,wyb,wzb]
#---------------------------------------------------------------------------------------------------#
#Calculate forces acting on Frisbee
    def get_force(self):
        alpha, v2 = self.angle_of_attack, self.v2
        vhat,ybhat = self.vhat,self.ybhat
        force_amplitude = 0.5*rho*area*v2
        F_lift = self.model.C_lift(alpha)*force_amplitude*np.cross(vhat,ybhat)
        F_drag = self.model.C_drag(alpha)*force_amplitude*(-vhat)
        total_force=F_lift+F_drag+F_gravity
        if self.debug:
            print( "In get_forces")
            print ("\tCL:",self.model.C_lift(alpha))
            print ("\tCD:",self.model.C_drag(alpha))
            print ("\tAmplitude:",force_amplitude)
            print ("\tF_lift/m:",F_lift/m)
            print ("\tF_drag/m:",F_drag/m)
            print ("\tF_grav/m:",F_gravity/m)
        return total_force
#---------------------------------------------------------------------------------------------------#
#Calculate torques acting on Frisbee
    def get_torque(self):
        #Calculate alpha and velocity^2
        alpha=self.angle_of_attack
        v2=self.v2
        #Get x,y,z components of angular velocity
        wxb,wyb,wzb = self.wxb,self.wyb,self.wzb

        #X-body torque; Note: in the frisbee frame
        torque_amplitude = 0.5*rho*d*area*v2
        roll_moment = self.model.C_x(wxb,wzb)*torque_amplitude*self.xbhat
        #Y-body torque; Note: in the frisbee frame
        pitch_moment = self.model.C_y(alpha,wyb)*torque_amplitude*self.ybhat
        #Z-body torque; Note: already in the lab frame
        spin_moment = self.model.C_z(wzb)*torque_amplitude*np.array([0,0,1.])
        #Total torque - We rotate from the frisbee frame into the lab frame
        rotation_matrix = self.rotation_matrix
        total_torque=np.dot(rotation_matrix,roll_moment+pitch_moment)+spin_moment
        #Use this to shut off the torques if you want
        #total_torque *= 0
        if self.debug:
            print ("In get_torque")
            print ("\tRoll amp:",self.model.C_x(wxb,wzb)*torque_amplitude)
            print ("\tPitch amp:", self.model.C_y(alpha,wyb)*torque_amplitude)
            print ("\tSpin amp:",self.model.C_z(wzb)*torque_amplitude)
            print ("\tRaw moments:",roll_moment,pitch_moment,spin_moment)
            #print ("\tLab moments:",np.dot(rotation_matrix,roll_moment),np.dot(rotation_matrix))
            print ("\ttotal_torque:",total_torque)
        return total_torque
#---------------------------------------------------------------------------------------------------#
#Calculate derivatives of phidot, thetadot, and gammadot, which correspond to angular
#values for phi, theta, and gamma. Units are radians/sec^2. Equations can be found
    def ang_acceleration(self):
        total_torque = self.get_torque()
        st,ct = self.sin_theta,self.cos_theta
        phidot,thetadot,gammadot = self.phidot, self.thetadot, self.gammadot
        phi_dd = (total_torque[0] + 2*Ixy*thetadot*phidot*st - Izz*thetadot*(phidot*st))
        theta_dd = (total_torque[1] + Izz*phidot*ct*(phidot*st+gammadot) - Ixy*phidot*phidot)
        gamma_dd = (total_torque[2] - Izz*(phidot*thetadot*ct + phi_dd*st))/Izz

        if self.debug:
            print ("In ang_acceleration:")
            print ("\tphi_dd parts:",total_torque[0],2*Ixy*thetadot*phidot*st,Izz*thetadot)
            print ("\ttheta_dd parts:",total_torque[1],Izz*phidot*ct*(phidot*st+gammadot),Ixy)
            print ("\tgamma_dd parts:",total_torque[2],-Izz*(phidot*thetadot*ct + phi_dd*st))
        return np.array([phi_dd, theta_dd, gamma_dd])
#---------------------------------------------------------------------------------------------------#
#Create array of derivatives to feed into numerical integrator
#variable_array=[x-velocity, y-velocity, z velocity
#x-accelration, y-acceleration, z-acceleration,
#phi ang. velocity, theta ang. velocity, gamma ang. velocity
#phi ang. acceleration, theta ang. acceleration, gamma ang. acceleration]
    def derivatives_array(self):
        if self.debug:
            print ("") #print a blank line
        self.update_data_fields()
        derivatives = np.zeros(12)
        derivatives[0:3] = self.velocity
        derivatives[3:6] = self.get_force()/m
        derivatives[6:9] = self.angle_dots
        derivatives[9:12]= self.ang_acceleration()
        if self.debug:
            print ("In derivatives_array:")
            print ("\tvelocities: ",derivatives[0:3])
            print ("\tforces/m: ",derivatives[3:6])
            print ("\tangle dots: ",derivatives[6:9])
            print ("\tang_accs: ",derivatives[9:12])
        return derivatives
