#pragma once
#ifndef __CONNECTOR_H__
#define __CONNECTOR_H__

#include <cstdlib>
#include <iostream>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <boost/thread.hpp>
// #include <boost/interprocess/sync/interprocess_mutex.hpp>
// #include <boost/interprocess/sync/scoped_lock.hpp>

#include "Event.h"
#include "Signal.h"

namespace refbox{
	class Connector{
	public:
		/**
		* Alias for endpoint
		*/
		
		typedef boost::asio::ip::udp::endpoint EndPoint;
		/**
		* Input buffer size
		*/
		static const int inputBufferSize = 81920;

		/**
		* The input port.
		*/
		static const int portIn = 3001;

		/**
		* The output port.
		*/
		static const int portOut = 3000;

		/**
		* Occurs when a message is received.
		*/
		boost::signals2::signal<void(const Connector* caller, const EndPoint& source, const std::string& message)> messageReceived;

		/**
		* Initializes a new instance of the Connector class.
		*/
		Connector ();

		/**
		* Broadcast the specified event.
		* @param event The event to send.
		*/
		void broadcast(const Event& event) const;

		/**
		* Starts async reception of UDP messages
		*/
		void start();

		/**
		* Stops async reception of UDP messages
		*/
		void stop();

	protected:
		/**
		* Raises the message received event.
		* @param message The message's source.
		* @param message The message.
		*/
		void onMessageReceived(const std::string& message, const EndPoint& source);

	private:

		/**
		* Udp listener socket
		*/
		boost::asio::ip::udp::socket* socket_;
		/**
		* Async service for async calls
		*/
		boost::asio::io_service service_;
		/**
		* Required to keep the io_service.poll() working
		*/
		boost::thread* mainThread_;
		/**
		* Indicates wether the mainThread is running
		*/
		bool running_;
		/**
		* Stores the Endpoint of the sender
		*/
		EndPoint senderEP_;
		/**
		* Input buffer
		*/
		char *inputBuffer_;

		void beginReceive();
		void mainThreadTask();
		void receiveCallback(const boost::system::error_code& error, size_t received);
	};
}

#endif // __CONNECTOR_H__
