// C++ Std
#include <string>
#include <vector>
#include <sstream>
#include <iostream>

// ROS
#include "ros/ros.h"
// Refbox
#include "refbox.h"

int main(int argc, char **argv){
	ros::init(argc, argv, "refbox_client");
	ros::NodeHandle handle;
	refbox::Client client(handle);
	// Set to true for a silent execution
	client.mute = false;
	client.start();
	std::cout << "RefBox Client for ROS" << std::endl;
	ros::spin();
	return -1;
}