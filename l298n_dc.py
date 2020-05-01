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

def dir_sr():
    left_dc(69)
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
        print("run")
        if(temp1==1):
         GPIO.output(in1,GPIO.HIGH)
         GPIO.output(in2,GPIO.LOW)
         GPIO.output(in3,GPIO.HIGH)
         GPIO.output(in4,GPIO.LOW)
         GPIO.output(in5,GPIO.HIGH)
         GPIO.output(in6,GPIO.LOW)
         GPIO.output(in7,GPIO.HIGH)
         GPIO.output(in8,GPIO.LOW)
         print("forward")
         
        else:
         GPIO.output(in1,GPIO.LOW)
         GPIO.output(in2,GPIO.HIGH)
         GPIO.output(in3,GPIO.LOW)
         GPIO.output(in4,GPIO.HIGH)
         GPIO.output(in5,GPIO.LOW)
         GPIO.output(in6,GPIO.HIGH)
         GPIO.output(in7,GPIO.LOW)
         GPIO.output(in8,GPIO.HIGH)
         print("backward")


    elif x=='s':
        print("stop")
        start_dc1()
        GPIO.output(in1,GPIO.LOW)
        GPIO.output(in2,GPIO.LOW)
        GPIO.output(in3,GPIO.LOW)
        GPIO.output(in4,GPIO.LOW)
        GPIO.output(in5,GPIO.LOW)
        GPIO.output(in6,GPIO.LOW)
        GPIO.output(in7,GPIO.LOW)
        GPIO.output(in8,GPIO.LOW)
        

    elif x=='f':
        print("forward")
        change_dc(90)
        GPIO.output(in1,GPIO.HIGH)
        GPIO.output(in2,GPIO.LOW)
        GPIO.output(in3,GPIO.HIGH)
        GPIO.output(in4,GPIO.LOW)
        GPIO.output(in5,GPIO.HIGH)
        GPIO.output(in6,GPIO.LOW)
        GPIO.output(in7,GPIO.HIGH)
        GPIO.output(in8,GPIO.LOW)
        sleep(.30)
        change_dc(55)
        temp1=1
    
    elif x == 'left':
        change_dc(95)
        GPIO.output(in1,GPIO.HIGH)
        GPIO.output(in2,GPIO.LOW)
        GPIO.output(in3,GPIO.HIGH)
        GPIO.output(in4,GPIO.LOW)
        GPIO.output(in5,GPIO.LOW)
        GPIO.output(in6,GPIO.HIGH)
        GPIO.output(in7,GPIO.LOW)
        GPIO.output(in8,GPIO.HIGH)
        
    elif x == 'sr':
        change_dc(90)
        GPIO.output(in1,GPIO.HIGH)
        GPIO.output(in2,GPIO.LOW)
        GPIO.output(in3,GPIO.HIGH)
        GPIO.output(in4,GPIO.LOW)
        GPIO.output(in5,GPIO.HIGH)
        GPIO.output(in6,GPIO.LOW)
        GPIO.output(in7,GPIO.HIGH)
        GPIO.output(in8,GPIO.LOW)
        sleep(.3)
        dir_sr()
        sleep(1.5)
        change_dc(80)
        GPIO.output(in1,GPIO.LOW)
        GPIO.output(in2,GPIO.HIGH)
        GPIO.output(in3,GPIO.LOW)
        GPIO.output(in4,GPIO.HIGH)
        
    elif x == 'sl':
        left_dc(50)
        right_dc(75)
        GPIO.output(in1,GPIO.HIGH)
        GPIO.output(in2,GPIO.LOW)
        GPIO.output(in3,GPIO.HIGH)
        GPIO.output(in4,GPIO.LOW)
        GPIO.output(in5,GPIO.HIGH)
        GPIO.output(in6,GPIO.LOW)
        GPIO.output(in7,GPIO.HIGH)
        GPIO.output(in8,GPIO.LOW)
        
    elif x=='b':
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

    elif x=='l':
        print("low")
        p.ChangeDutyCycle(25)
        p2.ChangeDutyCycle(25)
        p3.ChangeDutyCycle(25)
        p4.ChangeDutyCycle(25)
    elif x=='m':
        print("medium")
        p.ChangeDutyCycle(50)
        p2.ChangeDutyCycle(50)
        p3.ChangeDutyCycle(50)
        p4.ChangeDutyCycle(50)

    elif x=='h':
        print("high")
        p.ChangeDutyCycle(75)
        p2.ChangeDutyCycle(75)
        p3.ChangeDutyCycle(75)
        p4.ChangeDutyCycle(75)
    
    elif x == 'p':
        p.ChangeDutyCycle(90)
        p2.ChangeDutyCycle(90)
        p3.ChangeDutyCycle(90)
        p4.ChangeDutyCycle(90)
    
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