#include "Client.h"

// C++ Std
#include <map>
#include <string>
#include <sstream>
#include <iostream>

// Boost
#include <boost/algorithm/string.hpp>

// Project
#include "Serializer.h"

using namespace refbox;

Client::Client() : handle_(NULL){}

Client::Client(const Client&) : handle_(NULL){}

Client::Client(ros::NodeHandle& h) : mute(true), handle_(&h) {
	initialize();
}

ros::NodeHandle& Client::getHandle() const{
	return *(this->handle_);
}

inline void Client::initialize(){
	this->setupConnector();
	this->setupPublishers();
	this->setupSubscribers();
}

void Client::setupConnector(){
	using boost::bind;
	this->cnn_.messageReceived.connect(
		bind(&Client::udpMessageReceived, this, _1, _2, _3)
	);
}

void Client::setupPublishers(){
	using std_msgs::String;
	ros::NodeHandle& h = *(this->handle_);
	std::map<std::string, ros::Publisher>& p = this->publishers_;

	ros::Publisher test = h.advertise<String>("refbox_test", 1);
	p["start"] = test;
	p["stop"] = test;
	p["time"] = h.advertise<String>("refbox_time", 3);
	p["continue"] = h.advertise<String>("refbox_continue", 10);
}

void Client::setupSubscribers(){
	using boost::bind;
	using std_msgs::String;
	ros::NodeHandle& h = *(this->handle_);
	std::map<std::string, ros::Subscriber>& s = this->subscribers_;

	s["speech"] = h.subscribe<String>(
		"speech",
		1,
		bind(&Client::topicCallback, this, _1, "speech")
	);
}

void Client::start(){
	this->cnn_.start();
}

void Client::stop(){
	this->cnn_.stop();
}

void Client::topicCallback(const std_msgs::String::ConstPtr &msg, const std::string &topic) {
	Event event(topic, msg->data);
	this->cnn_.broadcast(event);
	if(!this->mute)
		ROS_INFO("Robot:  %s %s", topic.c_str(), msg->data.c_str());
}

void Client::udpMessageReceived(const Connector* caller,
								const Connector::EndPoint& source,
								const std::string& message){
	// std::cout << "Message " << message << std::endl
	Signal* s = Serializer::deserializeSignal(message, source);
	if(!s || (this->publishers_.count(s->type) < 1)) return;
	std_msgs::String msg;
	msg.data = s->value;
	if(ros::ok())
		this->publishers_[s->type].publish(msg);
	if(!this->mute){
		if(boost::iequals(s->type, "stop") || boost::iequals(s->type, "start"))
			ROS_INFO("RefBox: %s", s->type.c_str());
		else
			ROS_INFO("RefBox: %s %s", s->type.c_str(), s->value.c_str());
	}
	delete s;
}