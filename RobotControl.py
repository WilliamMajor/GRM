import RPi.GPIO as GPIO
import _thread
import paramiko
import select
import time
from enum import Enum

class Direction(Enum):
    Forward = 0
    Right = 1
    Left = 2
    Stop = 3
    StartMotors = 4
    Overdrive = 5

#bools
ending = False;
turning = False

#Gobal values for the lat and lon that we will be using
startingLat = 0
startingLon = 0
leftUpperCornerLat = 0;
rightUpperCornerLat = 0;
rightLowerCornerLat = 0;
currentLat = 0
currentLon = 0

#sensor values
LSFDist = 0;
LSBDist = 0;
RSFDist = 0;
RSBDist = 0;
FSDist = 0;

#define all needed GPIO shit
# trigger pin number they need to be changed to the pin to be used do we just need one trigger pin for all of them???
LSFTrigger = 18
LSBTrigger = 18
RSFTrigger = 18
RSBTrigger = 18
FSTrigger = 18
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
GPIO.setup(LSFTrigger, GPIO.OUT)
GPIO.setup(LSFEcho, GPIO.IN)

GPIO.setup(LSBTrigger, GPIO.OUT)
GPIO.setup(LSBEcho, GPIO.IN)

GPIO.setup(RSFTrigger, GPIO.OUT)
GPIO.setup(RSFEcho, GPIO.IN)

GPIO.setup(RSBTrigger, GPIO.OUT)
GPIO.setup(RSBEcho, GPIO.IN)

GPIO.setup(FSTrigger, GPIO.OUT)
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


#main thread for the program
try:
    try:
        _thread.start_new_thread(getGPSData,())
        _thread.start_new_thread(getSensorData, ())
        _thread.start_new_thread(getLSFSensorData, ())
        _thread.start_new_thread(getLSBSensorData, ())
        _thread.start_new_thread(getRSFSensorData, ())
        _thread.start_new_thread(getRSBSensorData, ())
        _thread.start_new_thread(getFSensorData, ())
        _thread.start_new_thread(minorMotorControl, ())
    except:
        print("Error unable to start thread")


    while not ending:
        majorMotorControl(Direction.Forward)
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
        print("Error Connecting to GPS Data")

    #This will pull in the initial GPS so we know when we have gotten back to the starting point.
    stdin, stdout, stderr = ssh.exec_command("/riot/Socket/./socket GPS")
    while not stdout.channel.exit_status_ready():
        # Only print data if there is data to read in the channel
        if stdout.channel.recv_ready():
            rl, wl, xl = select.select([stdout.channel], [], [], 0.0)
            if len(rl) > 0:
                # Print data from stdout
                print(stdout.channel.recv(1024))

    while not ending: #this is going to be pulling the gps data continually from the reader
        stdin, stdout, stderr = ssh.exec_command("/riot/Socket/./socket GPS")
        while not stdout.channel.exit_status_ready():
            # Only print data if there is data to read in the channel
            if stdout.channel.recv_ready():
                rl, wl, xl = select.select([stdout.channel], [], [], 0.0)
                if len(rl) > 0:
                    # Print data from stdout
                    print(stdout.channel.recv(1024))
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


def minorMotorControl(): #This thread will use the sensors values/ GPS on the reader to make minor adjustments if we end up needing it
    while not ending:
        print("filler")


def startMotors():
    p.start(start_dc)
    p2.start(start_dc)
    p3.start(start_dc)
    p4.start(start_dc)


def right_dc(num):
    p.ChangeDutyCycle(num)
    p2.ChangeDutyCycle(num)


def left_dc(num):
    p3.ChangeDutyCycle(num)
    p4.ChangeDutyCycle(num)


def half_dc():
    p.ChangeDutyCycle(15)
    p2.ChangeDutyCycle(15)
    p3.ChangeDutyCycle(15)
    p4.ChangeDutyCycle(15)


def change_dc(num):
    p.ChangeDutyCycle(num)
    p2.ChangeDutyCycle(num)
    p3.ChangeDutyCycle(num)
    p4.ChangeDutyCycle(num)


def start_dc1():
    p.ChangeDutyCycle(start_dc)
    p2.ChangeDutyCycle(start_dc)
    p3.ChangeDutyCycle(start_dc)
    p4.ChangeDutyCycle(start_dc)


def dir_sr():
    left_dc(69)
    right_dc(10)


def forward():
    change_dc(90) #high motor power to start
    GPIO.output(in1, GPIO.HIGH)
    GPIO.output(in2, GPIO.LOW)
    GPIO.output(in3, GPIO.HIGH)
    GPIO.output(in4, GPIO.LOW)
    GPIO.output(in5, GPIO.HIGH)
    GPIO.output(in6, GPIO.LOW)
    GPIO.output(in7, GPIO.HIGH)
    GPIO.output(in8, GPIO.LOW)
    sleep(.30)
    change_dc(55) # change motor power to a lower setting to allow robot to crawl across the roof


def stop():
    start_dc1()
    GPIO.output(in1, GPIO.LOW)
    GPIO.output(in2, GPIO.LOW)
    GPIO.output(in3, GPIO.LOW)
    GPIO.output(in4, GPIO.LOW)
    GPIO.output(in5, GPIO.LOW)
    GPIO.output(in6, GPIO.LOW)
    GPIO.output(in7, GPIO.LOW)
    GPIO.output(in8, GPIO.LOW)


def turnLeft():
    print("Filler")#Looks like this needs to have some timing done first before I implement this


def turnRight():
    change_dc(90)
    GPIO.output(in1, GPIO.HIGH)
    GPIO.output(in2, GPIO.LOW)
    GPIO.output(in3, GPIO.HIGH)
    GPIO.output(in4, GPIO.LOW)
    GPIO.output(in5, GPIO.HIGH)
    GPIO.output(in6, GPIO.LOW)
    GPIO.output(in7, GPIO.HIGH)
    GPIO.output(in8, GPIO.LOW)
    sleep(.3)
    dir_sr()
    sleep(1.5)
    change_dc(80)
    GPIO.output(in1, GPIO.LOW)
    GPIO.output(in2, GPIO.HIGH)
    GPIO.output(in3, GPIO.LOW)
    GPIO.output(in4, GPIO.HIGH)


def overdriveMode():
    p.ChangeDutyCycle(100)
    p2.ChangeDutyCycle(100)
    p3.ChangeDutyCycle(100)
    p4.ChangeDutyCycle(100)


def getLSFSensorData():
    while not ending:

        GPIO.output(LSFTrigger, True)
        time.sleep(0.00001)
        GPIO.output(LSFTrigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(LSFEcho) == 0:
            start = time.time()
        while GPIO.input(LSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        LSFDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getLSBSensorData():
    while not ending:

        GPIO.output(LSBTrigger, True)
        time.sleep(0.00001)
        GPIO.output(LSBTrigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(LSBEcho) == 0:
            start = time.time()
        while GPIO.input(LSBEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        LSBDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getRSFSensorData():
    while not ending:

        GPIO.output(RSFTrigger, True)
        time.sleep(0.00001)
        GPIO.output(RSFTrigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(RSFEcho) == 0:
            start = time.time()
        while GPIO.input(RSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        RSFDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getRSBSensorData():
    while not ending:

        GPIO.output(RSBTrigger, True)
        time.sleep(0.00001)
        GPIO.output(RSBTrigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(RSBEcho) == 0:
            start = time.time()
        while GPIO.input(RSFBcho) == 1:
            stop = time.time()
        elapsed = stop - start
        RSBDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def getFSensorData():
    while not ending:

        GPIO.output(FSTrigger, True)
        time.sleep(0.00001)
        GPIO.output(FSTrigger, False)
        start = time.time()
        stop = time.time()
        while GPIO.input(LSFEcho) == 0:
            start = time.time()
        while GPIO.input(LSFEcho) == 1:
            stop = time.time()
        elapsed = stop - start
        FSDist = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def createGEOFence(): #this is going to be the initial mode the robot starts in that simply circles the roof and returns to the starting point
    print("Filler")

