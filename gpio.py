import RPi.GPIO as GPIO
import time
GPIO.setmode(GPIO.BCM)
trigger = 5 #trigger pin number
echo = 6 #echo pin number
echo2 = 19
echo3 = 9
sample_time = .3
GPIO.setup(trigger, GPIO.OUT)
GPIO.setup(echo, GPIO.IN)
GPIO.setup(echo2, GPIO.IN)
GPIO.setup(echo3, GPIO.IN)
def get_distance():
    time.sleep(sample_time)
    GPIO.output(trigger, True)
    time.sleep(0.00001)
    GPIO.output(trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(echo) == 0:
        start = time.time()
    
    
    while GPIO.input(echo) == 1:
        stop = time.time()
        
    
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return distance

def get_distance2():
    time.sleep(sample_time)
    GPIO.output(trigger, True)
    time.sleep(0.00001)
    GPIO.output(trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(echo2) == 0:
        start = time.time()
    
    
    while GPIO.input(echo2) == 1:
        stop = time.time()
        
    
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return distance

def get_distance3():
    time.sleep(sample_time)
    GPIO.output(trigger, True)
    time.sleep(0.00001)
    GPIO.output(trigger,False)
    start = time.time()
    stop = time.time()
    while GPIO.input(echo3) == 0:
        start = time.time()
    
    
    while GPIO.input(echo3) == 1:
        stop = time.time()
        
    
    elapsed = stop - start
    distance = elapsed * 17150 #speed of sound is 34300 cm/s
    return distance
if __name__ == '__main__':
    try:
        while(True):
            dist = get_distance()
            dist2 = get_distance2()
            dist3 = get_distance3()
            print(f"Distance: {round(dist,2)} cm")
            print(f"Distance2: {round(dist2,2)} cm")
            print(f"Distance3: {round(dist3,2)} cm")
            
    except KeyboardInterrupt:
        print("Measurement stopped by user")
        GPIO.cleanup()
    except:
        print("Other exit, probably by forced stop")
        GPIO.cleanup()
