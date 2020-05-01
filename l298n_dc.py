import RPi.GPIO as GPIO          
from time import sleep
#RIGHT WHEEL GPIO 
en1 = 25 #right back enable
in1 = 26 #right back wheel (+) input
in2 = 23 #right back wheel (-) input
en2 = 18 #right front enable
in3 = 16 #right front wheel (+) input
in4 = 17 #right front wheel (-) input
temp1=1
#LEFT WHEEL GPIO
en3 = 12
in5 = 14
in6 = 15
en4 = 13
in7 = 8
in8 = 7

start_dc = 75

GPIO.setmode(GPIO.BCM)
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
p=GPIO.PWM(en1,1000)#back right motor pwm signal
p2 = GPIO.PWM(en2, 1000)#front right motor pwm signal
p3 = GPIO.PWM(en3, 1000)#back left motor pwm signal
p4 = GPIO.PWM(en4, 1000)#front left motor pwm signal

#start PWM with given duty cycle
p.start(start_dc) 
p2.start(start_dc)
p3.start(start_dc)
p4.start(start_dc)

print("\n")
print("r-run s-stop f-forward b-backward l-low m-medium h-high e-exit")
print("\n")    

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
def forward():
    GPIO.output(in1,GPIO.HIGH)
    GPIO.output(in2,GPIO.LOW)
    GPIO.output(in3,GPIO.HIGH)
    GPIO.output(in4,GPIO.LOW)
    GPIO.output(in5,GPIO.HIGH)
    GPIO.output(in6,GPIO.LOW)
    GPIO.output(in7,GPIO.HIGH)
    GPIO.output(in8,GPIO.LOW)

def backward():
    print("backward")
    GPIO.output(in1,GPIO.LOW)
    GPIO.output(in2,GPIO.HIGH)
    GPIO.output(in3,GPIO.LOW)
    GPIO.output(in4,GPIO.HIGH)
    GPIO.output(in5,GPIO.LOW)
    GPIO.output(in6,GPIO.HIGH)
    GPIO.output(in7,GPIO.LOW)
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
    GPIO.output(in2,GPIO.LOW)
    GPIO.output(in3,GPIO.HIGH)
    GPIO.output(in4,GPIO.LOW)
    GPIO.output(in5,GPIO.LOW)
    GPIO.output(in6,GPIO.HIGH)
    GPIO.output(in7,GPIO.LOW)
    GPIO.output(in8,GPIO.HIGH)

def dir_sr():
    left_dc(70)
    right_dc(10)

while(1):
    print('---Right Wheels---')
    print(GPIO.input(en1))
    print(GPIO.input(in1))
    print(GPIO.input(in2))
    print(GPIO.input(in3))
    print(GPIO.input(in4))
    print(GPIO.input(en2))
    print('---Left Wheels---')
    print(GPIO.input(en3))
    print(GPIO.input(in5))
    print(GPIO.input(in6))
    print(GPIO.input(in7))
    print(GPIO.input(in8))
    print(GPIO.input(en4))
    

    
    
    x=input()
    
    if x=='r':
        forward()


    elif x=='s':
        start_dc1()
        stop()
    
    elif x == 'f':
        print('forward')
        forward()

    elif x=='w':
        print("walk")
        change_dc(90)
        forward()
        sleep(.30)
        change_dc(40)
        temp1=1
    elif x== 'wb':
        print("walk backward")
        change_dc(90)
        backward()
        sleep(.30)
        change_dc(55)
        temp1=0
        
    
    elif x == 'pleft':
        change_dc(100)
        pleft()
        

    elif x == 'no hoots':
        change_dc(100)
        forward()


    elif x == 'sr': #slight right
        change_dc(90)
        forward()
        sleep(.3)
        dir_sr()
        
        
        
    elif x == 'sl': #slight left, they're different right now because im' experimenting with things
        left_dc(50)
        right_dc(75)
        forward()
        
    elif x=='b':
        backward()

    elif x=='l':
        print("low")
        change_dc(25)
    elif x=='m':
        print("medium")
        change_dc(50)

    elif x=='h':
        print("high")
        change_dc(80)
    

    
    elif x == 'o':
        print('overdrive mode: DC 100%')
        p.ChangeDutyCycle(100)
        p2.ChangeDutyCycle(100)
        p3.ChangeDutyCycle(100)
        p4.ChangeDutyCycle(100)
    
        
    
    elif x=='e':
        GPIO.cleanup()
        p.stop()
        p2.stop()
        p3.stop()
        p4.stop()
        print("GPIO Clean up")
        break
    
    
    else:
        print("<<<  wrong data  >>>")
        print("please enter the defined data to continue.....")