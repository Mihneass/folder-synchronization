# Folder synchronization:

## What it does:

This is a program that synchronizez two different folders unilateraly. It verifies whether or not the two folders are the same at a certain interval and upon discovering any inconsistencies it synchronizes them by emptying the target folder and copying over all the files from the source folder.

## How it works:

Upon running the program the user is requested to input four different pieces of information in sequence:
1. The full path of the source folder
2. The full path of the target folder
3. The full path of the log .txt file
4. The number of seconds between each check

Once this information has been provided, the program will perform a first-time synchronization of the folders. After that it will check to see if a) any difference exists between the two folders and b) there has been any change in the source folder. If any of these two is the case, it will synchronize the folders. Otherwise it will inform the user that no discrepancy has been found.

## How it fulfills the requirements of the task:

1. Synchronization must be one-way: after the synchronization content of the
 replica folder should be modified to exactly match content of the source
 folder;

Due to the logic explained in the previous section, the program maintains the one-way synchronization

2. Synchronization should be performed periodically;

The check for the need to synchronize takes place periodically and only makes the change if necessary

3. File creation/copying/removal operations should be logged to a file and to the
 console output;

All these operation are logged in the log file. Due to the fact that the application synchronizes by deleting and copying over all files from the source folder into the target folder, in the case of massive ammounts of data the console would get cluttered by the copy log, therefore I chose to exclude it - it can be found as a commentedline of code. As for creation and removal, both of these actions are logged in the console

4. Folder paths, synchronization interval and log file path should be provided
 using the command line arguments; 

All four are provided as command line arguments as explained above

5. It is undesirable to use third-party libraries that implement folder
 synchronization; 

The application does not make use of any third-party libraries
