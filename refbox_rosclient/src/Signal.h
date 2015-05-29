#pragma once
#ifndef __SIGNAL_H__
#define __SIGNAL_H__

#include <boost/asio.hpp>

namespace refbox{
	class Signal{
	public:
		/**
		* Alias for endpoint
		*/
		typedef boost::asio::ip::udp::endpoint EndPoint;
		
		/** 
		* The type of the signal
		*/
		std::string type;

		/** 
		* The signal value.
		*/
		std::string value;

		/** 
		* The IPEndPoint where message comes from.
		*/
		EndPoint source;
		/** 
		* Initializes a new instance of Signal
		*/
		Signal();
		/** 
		* Initializes a new instance of Signal
		* @param other A signal object to copy data from
		*/
		Signal(const Signal& other);
		/** 
		* Initializes a new instance of Signal
		* @param type The type of the signal
		* @param value The signal value.
		* @param source The IPEndPoint where message comes from.
		*/
		Signal(	const std::string& type,
				const std::string& value,
				const EndPoint& source);
	};
}

#endif // __SIGNAL_H__