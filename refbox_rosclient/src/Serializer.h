#pragma once
#ifndef __SERIALIZER_H__
#define __SERIALIZER_H__

#include "Event.h"
#include "Signal.h"
#include <boost/property_tree/ptree.hpp>

namespace refbox{
	class Serializer{
	public:
		/**
		* Serializes the specified object into a string.
		* @param s The object to serialize.
		* @return A string containing the XML representation of the object.
		*/
		static std::string serialize(const Event& event);

		/**
		* Serializes the specified object into a string.
		* @param s The object to serialize.
		* @return A string containing the XML representation of the object.
		*/
		static std::string serialize(const Signal& signal);

		/**
		* Deserializes an event from the provided string s.
		* If deserialization fails, returns null
		* @param s A string containing an event
		* @param source The enpoint which the provided string comes from
		*/
		static Event* deserializeEvent(const std::string& s, const Event::EndPoint& source);

		/**
		* Deserializes a signal from the provided string s.
		* If deserialization fails, returns null
		* @param s A string containing an event
		* @param source The enpoint which the provided string comes from
		*/
		static Signal* deserializeSignal(const std::string& s, const Signal::EndPoint& source);

	private:
		static bool deserialize(const std::string&, boost::property_tree::ptree&);
		static bool readNode(boost::property_tree::ptree&,
			const std::string&, std::string&, std::string&);
		static std::string removeXmlHeader(const std::string &);
		static bool writeNode(boost::property_tree::ptree&,
			const std::string&, const std::string&, const std::string&);
	};
}

#endif // __SERIALIZER_H__