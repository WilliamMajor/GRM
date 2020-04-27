import RPi.GPIO as GPIO          
from time import sleep
#RIGHT WHEEL GPIO 
en1 = 25 #right back enable
in1 = 26 #right back wheel (+) input
in2 = 23 #right back wheel (-) input
en2 = 27 #right front enable
in3 = 16 #right front wheel (+) input
in4 = 17 #right front wheel (-) input
temp1=1
#LEFT WHEEL GPIO

GPIO.setmode(GPIO.BCM)
GPIO.setup(in1,GPIO.OUT)
GPIO.setup(in2,GPIO.OUT)
GPIO.setup(in3,GPIO.OUT)
GPIO.setup(in4,GPIO.OUT)
GPIO.setup(en1,GPIO.OUT)
GPIO.output(in1,GPIO.LOW)
GPIO.output(in2,GPIO.LOW)
p=GPIO.PWM(en1,1000)

p.start(10)
print("\n")
print("r-run s-stop f-forward b-backward l-low m-medium h-high e-exit")
print("\n")    

while(1):

    x=input()
    
    if x=='r':
        print("run")
        if(temp1==1):
         GPIO.output(in1,GPIO.HIGH)
         GPIO.output(in2,GPIO.LOW)
         print("forward")
         
        else:
         GPIO.output(in1,GPIO.LOW)
         GPIO.output(in2,GPIO.HIGH)
         print("backward")


    elif x=='s':
        print("stop")
        GPIO.output(in1,GPIO.LOW)
        GPIO.output(in2,GPIO.LOW)

    elif x=='f':
        print("forward")
        GPIO.output(in1,GPIO.HIGH)
        GPIO.output(in2,GPIO.LOW)
        temp1=1

    elif x=='b':
        print("backward")
        GPIO.output(in1,GPIO.LOW)
        GPIO.output(in2,GPIO.HIGH)
        temp1=0

    elif x=='l':
        print("low")
        p.ChangeDutyCycle(25)

    elif x=='m':
        print("medium")
        p.ChangeDutyCycle(50)

    elif x=='h':
        print("high")
        p.ChangeDutyCycle(75)
     
    
    elif x=='e':
        GPIO.cleanup()
        print("GPIO Clean up")
        break
    
    else:
        print("<<<  wrong data  >>>")
        print("please enter the defined data to continue.....")