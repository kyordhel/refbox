#pragma once
#ifndef __CLIENT_H__
#define __CLIENT_H__

// C++ Std
#include <map>
#include <string>
#include <sstream>
#include <iostream>

// ROS
#include "ros/ros.h"
#include "std_msgs/String.h"

// Project
#include "Connector.h"

namespace refbox{
	class Client{
	
	public:
		/**
		* Indicates wether incomming and outgoing messages will be printed or not
		*/
		bool mute;
		/**
		* Initializes a new instance of Client
		* @param handle The ros::NodeHandle object which will hold publishers and subscribers
		*/
		Client(ros::NodeHandle& handle);
		/**
		* Gets the ros::NodeHandle
		* @return The ros::NodeHandle object which holds publishers and subscribers
		*/
		ros::NodeHandle& getHandle() const;
		/**
		* Starts the client
		*/
		void start();
		/**
		* Stops the client
		*/
		void stop();

	private:
		Connector cnn_;
		ros::NodeHandle* handle_;
		std::map<std::string, ros::Publisher> publishers_;
		std::map<std::string, ros::Subscriber> subscribers_;

		Client();
		Client(const Client&);
		inline void initialize();
		void setupConnector();
		void setupPublishers();
		void setupSubscribers();
		void topicCallback(const std_msgs::String::ConstPtr &msg, const std::string &topic);
		void udpMessageReceived(const Connector*, const Connector::EndPoint&, const std::string&);
	};
}

#endif // __CLIENT_H__