import RPi.GPIO as GPIO
import _thread
import paramiko
import select
import time
from enum import Enum

class Direction(Enum):
    Forward = 1
    Right = 2
    Left = 3
    Stop = 4

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
# trigger pin number they need to be changed to the pin to be used
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

GPIO.setmode(GPIO.BCM)
GPIO.setup(trigger, GPIO.OUT)
GPIO.setup(echo, GPIO.IN)

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
    except:
        print("Error unable to start thread")


    while not ending:
        majorMotorControl(Direction.Forward)
        time.sleep(1)
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


def majorMotorControl(motorCommand):#This is where the functions that the tell the robot how to turn left and right
    print(motorCommand)


def minorMotorControl(): #This thread will use the sensors values on the reader to make minor adjustments if we end up needing it
    print("filler")

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
        distance = elapsed * 17150  # speed of sound is 34300 cm/s
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
        distance = elapsed * 17150  # speed of sound is 34300 cm/s
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
        distance = elapsed * 17150  # speed of sound is 34300 cm/s
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
        distance = elapsed * 17150  # speed of sound is 34300 cm/s
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
        distance = elapsed * 17150  # speed of sound is 34300 cm/s
        time.sleep(0.2)


def findRoofingEdges(): #this is going to be the initial mode the robot starts in that simply circles the roof and returns to the starting point
    print("Filler")

