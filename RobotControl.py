import RPi.GPIO as GPIO
import _thread
import paramiko
import select
import time
from enum import Enum
import GeoFencing


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

#define all needed GPIO shit
# trigger pin number they need to be changed to the pin to be used do we just need one trigger pin for all of them???
Trigger = 18

# echo pin number they need to be changed to the pin to be used
LSFEcho = 24
LSBEcho = 24
RSFEcho = 24
RSBEcho = 24
FSEcho  = 24

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
GPIO.setup(LSFEcho, GPIO.IN)

GPIO.setup(LSBEcho, GPIO.IN)

GPIO.setup(RSFEcho, GPIO.IN)

GPIO.setup(RSBEcho, GPIO.IN)

GPIO.setup(FSEcho, GPIO.IN)

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

geofence = GeoFencing.Fence()


#main thread for the program
try:
    try:
        # _thread.start_new_thread(getGPSData,())
        _thread.start_new_thread(getLSSensorData, ())
        _thread.start_new_thread(getRSSensorData, ())
        _thread.start_new_thread(getFSensorData, ())
        _thread.start_new_thread(followingWall, ())
    except:
        print("Error unable to start thread")


    while not ending:
        time.sleep(1)
    GPIO.cleanup()
except KeyboardInterrupt:
    GPIO.cleanup()
except:
    GPIO.cleanup()



def getGPSData():#The program to get the gps data goes here
    host = "192.168.1.81"
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
            print("filler"),
        1: #TurnRight
            print("filler"),
        2: #TurnLeft
            print("filler"),
        3: #Stop
            print("filler"),
        4: #StartMotors
            print("filler"),
        5: #Overdrive
            print("filler"),
    }


def followingWall():
    global dstFromWall
    global followWall
    dstFromWall = LSDist;
    while followWall:
        if currentLat == startingLat and currentLon == startingLon and FSDist <= dstFromWall: # we will need to round the lat and lon to get  in the right ballpark
            followWall = False
            break

        if LSDist > 450: #this should be close to max range of the sensor so we know when basically there is nothing there
            majorMotorControl(Direction.Left)#turn left to continue following eh wall
        if FSDist <= dstFromWall:
            majorMotorControl(Direction.Right)
        if LSDist < dstFromWall - 10 and LSDist > (dstFromWall - 60): ## we are drifing in to the left
            print("need to turn slightly right")
        elif LSD > dstFromWall + 10:
            print("need to turn slightly left ")




def getLSSensorData():
    global LSDist
    while not ending:
        GPIO.output(Trigger, True)
        time.sleep(0.00001)
        GPIO.output(Trigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(LSFEcho) == 0:
            start = time.time()
        while GPIO.input(LSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        LSDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getRSSensorData():
    global RSDist
    while not ending:
        GPIO.output(Trigger, True)
        time.sleep(0.00001)
        GPIO.output(Trigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(RSFEcho) == 0:
            start = time.time()
        while GPIO.input(RSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        RSDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getFSensorData():
    global FSDist
    while not ending:
        GPIO.output(Trigger, True)
        time.sleep(0.00001)
        GPIO.output(Trigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(LSFEcho) == 0:
            start = time.time()
        while GPIO.input(LSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        FSDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)
