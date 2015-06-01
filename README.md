# refbox
RoboCup @Home Referee Box project
./server        contains the RefBox project intended to be used during the competition
./rosclient	    contains a ROS compliant client

For detailed information about functionality check README.txt

Compiling and running the server (requires Mono)
    $ make run

Compiling and running the ROS client (requires ROS & BOOST)
1. Create a symbolic link (or copy the source) to your catkin's source directory.
    catkin_ws/src$ ln -s /path/to/refbox_rosclient refbox_rosclient
2. Build and run it
    $ catkin_make && rosrun refbox_rosclient refbox_roslient