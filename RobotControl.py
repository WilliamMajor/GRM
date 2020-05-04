import RPi.GPIO as GPIO
import threading
import paramiko
import select
import time
#import GeoFencing
from enum import Enum

# class SensorData():
#     def __init__(self):
#         self.FEcho = 6
#         self.LEcho = 19
#         self.REcho = 9
#         self.Trigger = 18
#         self.sample_time = .3
#         GPIO.setmode(GPIO.BCM)
#         GPIO.setup(self.Trigger, GPIO.OUT)
#         GPIO.setup(self.FEcho, GPIO.IN)
#         GPIO.setup(self.LEcho, GPIO.IN)
#         GPIO.setup(self.REcho, GPIO.IN)
#     def get_Fdist(self):
#         time.sleep(self.sample_time)
#         GPIO.output(self.Trigger, True)
#         time.sleep(0.00001)
#         GPIO.output(self.Trigger,False)
#         start = time.time()
#         stop = time.time()
#         while GPIO.input(self.FEcho) == 0:
#             start = time.time()
#         while GPIO.input(self.FEcho) == 1:
#             stop = time.time()
#         elapsed = stop - start
#         distance = elapsed * 17150 #speed of sound is 34300 cm/s
#         return distance
#     def get_Ldist(self):
#         time.sleep(self.sample_time)
#         GPIO.output(self.Trigger, True)
#         time.sleep(0.00001)
#         GPIO.output(self.Trigger,False)
#         start = time.time()
#         stop = time.time()
#         while GPIO.input(self.LEcho) == 0:
#             start = time.time()
#         while GPIO.input(self.LEcho) == 1:
#             stop = time.time()
#         elapsed = stop - start
#         distance = elapsed * 17150 #speed of sound is 34300 cm/s
#         return distance
#     def get_Rdist(self):
#         time.sleep(self.sample_time)
#         GPIO.output(self.Trigger, True)
#         time.sleep(0.00001)
#         GPIO.output(self.Trigger,False)
#         start = time.time()
#         stop = time.time()
#         while GPIO.input(self.REcho) == 0:
#             start = time.time()
#         while GPIO.input(self.REcho) == 1:
#             stop = time.time()
#         elapsed = stop - start
#         distance = elapsed * 17150 #speed of sound is 34300 cm/s
#         return distance
    


class Direction(Enum):
    Forward = 0
    Right = 1
    Left = 2
    Stop = 3
    StartMotors = 4
    Overdrive = 5

#bools
ending = False
turning = False
followWall = True

#Gobal values for the lat and lon that we will be using
startingLat = 0
startingLon = 0
leftUpperCornerLat = 0
leftUpperCornerLon = 0
rightUpperCornerLat = 0
rightUpperCornerLon = 0
rightLowerCornerLat = 0
rightLowerCornerLat = 0
currentLat = 0
currentLon = 0
dstFromWall = 200

#sensor values
LSDist = 0
RSDist = 0
FSDist = 0
sample_time = 0.3

#define all needed GPIO shit
# trigger pin number they need to be changed to the pin to be used do we just need one trigger pin for all of them???
Trigger = 5

# echo pin number they need to be changed to the pin to be used
FEcho = 6
LEcho = 19
REcho = 9

#RIGHT WHEEL GPIO
en1 = 25 #right back enable
in1 = 26 #right back wheel (+) input
in2 = 23 #right back wheel (-) input
en2 = 18 #right front enable
in3 = 16 #right front wheel (+) input
in4 = 17 #right front wheel (-) input
temp1=1
#LEFT WHEELS GPIO
en3 = 12
in5 = 14
in6 = 15
en4 = 13
in7 = 8
in8 = 7

start_dc = 75 #define the starting motorspeed

GPIO.setmode(GPIO.BCM)#set the gpio to be defined by gpio number not pin number

#setup for sensors
GPIO.setup(Trigger, GPIO.OUT)
GPIO.setup(FEcho, GPIO.IN)
GPIO.setup(LEcho, GPIO.IN)
GPIO.setup(REcho, GPIO.IN)



#setup for motors
#right wheel setup
GPIO.setup(in1,GPIO.OUT)
GPIO.setup(in2,GPIO.OUT)
GPIO.setup(in3,GPIO.OUT)
GPIO.setup(in4,GPIO.OUT)
GPIO.setup(en1,GPIO.OUT) #ena
GPIO.setup(en2,GPIO.OUT) #enb
#left wheel setup
GPIO.setup(in5,GPIO.OUT)
GPIO.setup(in6,GPIO.OUT)
GPIO.setup(in7,GPIO.OUT)
GPIO.setup(in8,GPIO.OUT)
GPIO.setup(en3,GPIO.OUT) #ena
GPIO.setup(en4,GPIO.OUT) #enb

#initialize wheels to not move
GPIO.output(in1,GPIO.LOW)
GPIO.output(in2,GPIO.LOW)
GPIO.output(in3,GPIO.LOW)
GPIO.output(in4,GPIO.LOW)
GPIO.output(in5,GPIO.LOW)
GPIO.output(in6,GPIO.LOW)
GPIO.output(in7,GPIO.LOW)
GPIO.output(in8,GPIO.LOW)
#create PWM signals on enable pins
p = GPIO.PWM(en1,1000)#back right motor pwm signal
p2 = GPIO.PWM(en2, 1000)#front right motor pwm signal
p3 = GPIO.PWM(en3, 1000)#back left motor pwm signal
p4 = GPIO.PWM(en4, 1000)#front left motor pwm signal

p.start(start_dc) 
p2.start(start_dc)
p3.start(start_dc)
p4.start(start_dc)


def getGPSData():#The program to get the gps data goes here
    host = "169.254.20.224"
    try:
        ssh = paramiko.SSHClient()
        ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        ssh.connect(host, username="root", password="sensthys1701")
    except:
        print("Error Connecting to Reader")

    #This will pull in the initial GPS so we know when we have gotten back to the starting point.
    stdin, stdout, stderr = ssh.exec_command("/riot/Socket/./socket GPS")
    while not stdout.channel.exit_status_ready():
        # Only print data if there is data to read in the channel
        if stdout.channel.recv_ready():
            rl, wl, xl = select.select([stdout.channel], [], [], 0.0)
            if len(rl) > 0:
                # Print data from stdout
                data = stdout.channel.recv(1024)
                if(type(data) != str ):
                    data = data.decode("utf-8")
                data = stdout.channel.recv(1024)
                data = data.split('\n')
                data = data[2]
                data = data.split(':')
                data = data[1]
                data = data.strip()
                data = data.split(',')
                startingLat = data[0]
                startingLon = data[1]

    while not ending: #this is going to be pulling the gps data continually from the reader
        stdin, stdout, stderr = ssh.exec_command("/riot/Socket/./socket GPS")
        while not stdout.channel.exit_status_ready():
            # Only print data if there is data to read in the channel
            if stdout.channel.recv_ready():
                rl, wl, xl = select.select([stdout.channel], [], [], 0.0)
                if len(rl) > 0:
                    # Print data from stdout
                    data = stdout.channel.recv(1024)
                    data = data.split('\n')
                    data = data[2]
                    data = data.split(':')
                    data = data[1]
                    data = data.strip()
                    data = data.split(',')
                    currentLat = data[0]
                    currentLon = data[1]
        time.sleep(0.2)

#Motor Control Functions
def majorMotorControl(motorCommand):#This function will act as a deligator calling different controls based on the value passed to it from the master thread
    controls = {
        0: #Forward
            
            forward(),
        1: #TurnRight
            pright(),
        2: #TurnLeft
            pleft(),
        3: stop(),
        4: #StartMotors
            print("start"),
        5: #Overdrive
            overdrive(),
    }


def followingWall():
    time.sleep(4)
    change_dc(75)
    forward()
    global dstFromWall
    global followWall
    while(LSDist == 0):
        time.sleep(0.2)
    dstFromWall = LSDist
    print(dstFromWall)
    time.sleep(3)
    while followWall:
        if currentLat == startingLat and currentLon == startingLon and FSDist <= dstFromWall: # we will need to round the lat and lon to get  in the right ballpark
            followWall = False
            break

        if LSDist > 200: #this should be close to max range of the sensor so we know when basically there is nothing there
            print("calling left turn")
            majorMotorControl(Direction.Left)#turn left to continue following eh wall
        if FSDist <= dstFromWall:
            majorMotorControl(Direction.Right)
        if LSDist < dstFromWall - 10 and LSDist > (dstFromWall - 60): ## we are drifing in to the left
            print("need to turn slightly right")
        elif LSDist > dstFromWall + 10:
            print("need to turn slightly left ")




def getSensorData():
    global LSDist
    global RSDist
    global FSDist
    while 1:
        FSDist = get_Fdist()
        LSDist = get_Ldist()
        RSDist = get_Rdist()
        time.sleep(0.1)



def get_Fdist():
    time.sleep(sample_time)
    GPIO.output(Trigger, True)
    time.sleep(0.00001)
    GPIO.output(Trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(FEcho) == 0:
        start = time.time()
    while GPIO.input(FEcho) == 1:
        stop = time.time()
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return round(distance, 2)

def get_Ldist():
    time.sleep(sample_time)
    GPIO.output(Trigger, True)
    time.sleep(0.00001)
    GPIO.output(Trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(LEcho) == 0:
        start = time.time()
    while GPIO.input(LEcho) == 1:
        stop = time.time()
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return round(distance, 2)

def get_Rdist():
    time.sleep(sample_time)
    GPIO.output(Trigger, True)
    time.sleep(0.00001)
    GPIO.output(Trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(REcho) == 0:
        start = time.time()
    while GPIO.input(REcho) == 1:
        stop = time.time()
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return round(distance, 2)

def right_dc(num):
    p3.ChangeDutyCycle(num)
    p4.ChangeDutyCycle(num)
    

def left_dc(num):
    p.ChangeDutyCycle(num)
    p2.ChangeDutyCycle(num)
    
    

def half_dc(): 
    p.ChangeDutyCycle(15)
    p2.ChangeDutyCycle(15)
    p3.ChangeDutyCycle(15)
    p4.ChangeDutyCycle(15)
def change_dc(num):
    print(f'changing DC to {num}')
    p.ChangeDutyCycle(num)
    p2.ChangeDutyCycle(num)
    p3.ChangeDutyCycle(num)
    p4.ChangeDutyCycle(num)

def start_dc1():
    p.ChangeDutyCycle(start_dc)
    p2.ChangeDutyCycle(start_dc)
    p3.ChangeDutyCycle(start_dc)
    p4.ChangeDutyCycle(start_dc)
def forward():
    print('foward')
    GPIO.output(in1,GPIO.LOW)
    GPIO.output(in2,GPIO.LOW)
    GPIO.output(in3,GPIO.HIGH)
    GPIO.output(in4,GPIO.HIGH)
    GPIO.output(in5,GPIO.HIGH)
    GPIO.output(in6,GPIO.HIGH)
    GPIO.output(in7,GPIO.LOW)
    GPIO.output(in8,GPIO.LOW)

def overdrive():
    print('overdrive mode')
    change_dc(100)

def backward():
    print("backward")
    GPIO.output(in1,GPIO.HIGH)
    GPIO.output(in2,GPIO.HIGH)
    GPIO.output(in3,GPIO.LOW)
    GPIO.output(in4,GPIO.LOW)
    GPIO.output(in5,GPIO.LOW)
    GPIO.output(in6,GPIO.LOW)
    GPIO.output(in7,GPIO.HIGH)
    GPIO.output(in8,GPIO.HIGH)
    temp1=0

def stop():
    print("stop")
    GPIO.output(in1,GPIO.LOW)
    GPIO.output(in2,GPIO.LOW)
    GPIO.output(in3,GPIO.LOW)
    GPIO.output(in4,GPIO.LOW)
    GPIO.output(in5,GPIO.LOW)
    GPIO.output(in6,GPIO.LOW)
    GPIO.output(in7,GPIO.LOW)
    GPIO.output(in8,GPIO.LOW)

def pleft():
    GPIO.output(in1,GPIO.HIGH)
    GPIO.output(in2,GPIO.HIGH)
    GPIO.output(in3,GPIO.LOW)
    GPIO.output(in4,GPIO.LOW)
    GPIO.output(in5,GPIO.HIGH)
    GPIO.output(in6,GPIO.HIGH)
    GPIO.output(in7,GPIO.LOW)
    GPIO.output(in8,GPIO.LOW)

def pright():
    GPIO.output(in1,GPIO.LOW)
    GPIO.output(in2,GPIO.LOW)
    GPIO.output(in3,GPIO.HIGH)
    GPIO.output(in4,GPIO.HIGH)
    GPIO.output(in5,GPIO.LOW)
    GPIO.output(in6,GPIO.LOW)
    GPIO.output(in7,GPIO.HIGH)
    GPIO.output(in8,GPIO.HIGH)


def dir_sr():
    left_dc(77)
    right_dc(75)


def dir_sl():
    left_dc(75)
    right_dc(77)


#main thread for the program
try:
    try:
        # _thread.start_new_thread(getGPSData,())
        # _thread.start_new_thread(followingWall, ())
        
        t1 = threading.Thread(target=getSensorData)
        t2 = threading.Thread(target=followingWall)
        t1.start()
        t2.start()
        
    except:
        print("Error unable to start thread")

    while 1:
        time.sleep(1)
        
except KeyboardInterrupt:
    GPIO.cleanup()
except:
    GPIO.cleanup()
