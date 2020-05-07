# GRM
Repository for the GPS Guided RFID Moisture Sensing Robot

The main project is broken into four main parts. The first folder "CommandLine" Contains code for a C# based command line terminal program
that can be used to read RFID Moisture sensing tags, processing them and placing the final informatoin into a CSV file that can be used by
various other programs. The second folder "Console_updated" contains the a GUI program based in C# that allows the user to change reader
settings and scan for different types of tags and takes all of the information and displays it on a live updating table. This GUI also
generates a CSV file output that can be used by later projects. The third folder "MobileApp" is a basic Android app that allows the user
to display collected information from the readers generated CSV files. The fourth folder "Robot" contains the main code base for the
GPS guided robot.

The breakdown of member contribution is as follows

William Major: CommandLine, Console_updated, Robot, MobileApp, Documentation, and Reports

Austin Tran: Robot, Documentation, and Reports

Sero Nazarian: Documentation, and Reports

Rohan Patel:


How to run programs:

Command Line Program -- In order to run the command line program you will need to navigate to CommandLine/GRM/bin/Debug and run GRM.exe
however without a RFID reader on the network the program will simply sit and wait for a connection to be established. Please refer to
the demo video provided in this folder to view operation of the application.

RFID Console -- In order to run the RFID console you will need to navigate to Console_updated/MSRC/bin/Debug and run MSRC.exe
however without a RFID reader on the network the user will be stuck on the select reader screen as there is no reader to connect with.
Please refer to the demo video provided in this folder to view operation of the application.

Robot -- Code can run by navigating to Robot/ and running RobotControl.py. However, like before without the robot or RFID reader this code
will not operate as intended. Please refer to the demo video provided for operation of the robot.

Mobile Application -- In order to run the mobile application you will need to download and install Android Studio. Then load the MobileApp
folder and download the code onto an android phone. For android studio to upload an app to your phone you must have developer options
enabled and allow for USB debugging. There is a demo program that will load sample data for users to see how tags will be displayed in the 
app without input from the reader.

The link to the Github repository is https://github.com/WilliamMajor/GRM for the protection custom written code the repository has
been left private. To request access please email william.a.major@sjsu.edu, and we will be happy to add you to the repository.



