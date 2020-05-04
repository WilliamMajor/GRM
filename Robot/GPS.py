import paramiko
import select
import time


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
                data = data.split('\n')
                data = data[2]
                data = data.split(':')
                data = data[1]
                data = data.strip()
                data = data.split(',')
                latitude = data[0]
                longitude = data[1]
                print("latitude: " + latitude)
                print("longitude: " + longitude)
    time.sleep(0.2)

ssh.close()
