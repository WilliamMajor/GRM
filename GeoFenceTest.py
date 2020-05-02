import _thread
import paramiko
import select
import time
import GeoFencing


latitude = 00.000000
longitude = 00.00000



def getGPS():

    global latitude
    global longitude

    host = "192.168.1.59"

    ssh = paramiko.SSHClient()
    ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
    ssh.connect(host, username="root", password="sensthys1701")

    while True:
        stdin, stdout, stderr = ssh.exec_command("/riot/Socket/./socket GPS")
        while not stdout.channel.exit_status_ready():
            # Only print data if there is data to read in the channel
            if stdout.channel.recv_ready():
                rl, wl, xl = select.select([stdout.channel], [], [], 0.0)
                if len(rl) > 0:
                    # Print data from stdout
                    data = stdout.channel.recv(1024)
                    if(type(data) != 'str'):
                        data = data.decode("utf-8")
                    data = data.split('\n')
                    data = data[2]
                    data = data.split(':')
                    data = data[1]
                    data = data.strip()
                    data = data.split(',')
                    latitude = float(data[0])
                    longitude = float(data[1])
        time.sleep(0.2)


_thread.start_new_thread(getGPS, ())


if __name__ == '__main__':

    test = GeoFencing.Fence()

    while True:
        x = input("r to run t to check and e to exit boiiiiii: ")

        if x == 'r':
            count = 0;
            while count < 100:
                test.add_point((latitude, longitude))
                time.sleep(0.2)
                count += 1
                print(count)
            print(test.list_points())

        if x == 't':
            if test.check_point((latitude, longitude)):
                print("you in that mf fence bro!")
            else:
                print("na you outside that fence bro!")

        if x == 'e':
            break




