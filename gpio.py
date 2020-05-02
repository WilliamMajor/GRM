import RPi.GPIO as GPIO
import time
GPIO.setmode(GPIO.BCM)
trigger = 18 #trigger pin number
echo = 24 #echo pin number
GPIO.setup(trigger, GPIO.OUT)
GPIO.setup(echo, GPIO.IN)
def get_distance():
    time.sleep(1)
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
    
if __name__ == '__main__':
    try:
        while(True):
            dist = get_distance()
            print(f"Distance: {round(dist,2)} cm")
            
    except KeyboardInterrupt:
        print("Measurement stopped by user")
        GPIO.cleanup()
    except:
        print("Other exit, probably by forced stop")
        GPIO.cleanup()
