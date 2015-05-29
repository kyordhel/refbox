#pragma once
#ifndef __EVENT_H__
#define __EVENT_H__

#include <boost/asio.hpp>

namespace refbox{
	class Event{
	public:
		/**
		* Alias for endpoint
		*/
		typedef boost::asio::ip::udp::endpoint EndPoint;
		
		/** 
		* Gets or sets the type of the event
		*/
		std::string type;

		/** 
		* Gets the event value.
		*/
		std::string value;

		/** 
		* Gets or sets the IPEndPoint where message comes from.
		*/
		EndPoint source;
		/** 
		* Initializes a new instance of Event
		*/
		Event();
		/** 
		* Initializes a new instance of Event
		* @param other A event object to copy data from
		*/
		Event(const Event& other);
		/** 
		* Initializes a new instance of Event
		* @param type The type of the event
		* @param value The event value.
		*/
		Event(const std::string& type, const std::string& value);
		/** 
		* Initializes a new instance of Event
		* @param type The type of the event
		* @param value The event value.
		* @param source The IPEndPoint where message comes from.
		*/
		Event(const std::string& type, const std::string& value, const EndPoint& source);
	};
}

#endif // __EVENT_H__