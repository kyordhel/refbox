#include <sstream>
#include "Serializer.h"

// http://www.boost.org/doc/libs/1_42_0/doc/html/boost_propertytree/tutorial.html
#include <boost/property_tree/ptree.hpp>
#include <boost/property_tree/xml_parser.hpp>
using namespace refbox;
using boost::property_tree::ptree;

std::string Serializer::serialize(const Event& event) {
	ptree pt;
	std::stringstream ss;
	writeNode(pt, "event", event.type, event.value);
	write_xml(ss, pt);
	return removeXmlHeader(ss.str());
}

std::string Serializer::serialize(const Signal& signal) {
	ptree pt;
	std::stringstream ss;
	writeNode(pt, "signal", signal.type, signal.value);
	write_xml(ss, pt);
	return removeXmlHeader(ss.str());
}

Event* Serializer::deserializeEvent(const std::string& s, const Event::EndPoint& source) {
	ptree pt;
	std::string type;
	std::string value;

	if(!deserialize(s, pt) || !readNode(pt, "event", type, value))
		return NULL;
	
	return new Event(type, value, source);
}

Signal* Serializer::deserializeSignal(const std::string& s, const Signal::EndPoint& source) {
	ptree pt;
	std::string type;
	std::string value;

	if(!deserialize(s, pt) || !readNode(pt, "signal", type, value))
		return NULL;
	
	return new Signal(type, value, source);
}

bool Serializer::deserialize(const std::string& s, ptree& tree) {
	std::stringstream ss(s);
	try{
		// Load the XML file into the property tree. If reading fails
		// (cannot open file, parse error), an exception is thrown.
		boost::property_tree::xml_parser::read_xml(ss, tree);
	}
	catch(...){ return false; }
	return true;
}

bool Serializer::readNode(ptree& pt, const std::string& nodeName,
	std::string& typeAttr, std::string& value){
	try{
		value = std::string(pt.get<std::string>(nodeName));
		typeAttr = std::string(pt.get<std::string>(nodeName + ".<xmlattr>.type"));
	}
	catch(...){ return false; }
	return true;
}

std::string Serializer::removeXmlHeader(const std::string& str){
	std::string::size_type pos = str.find( "?>", 0 );
	if(pos == std::string::npos) return str;
	pos+=2;
	while(pos < str.length()){
		char c = str[pos];
		if((c == '\n') || (c == '\r') || (c == '\t') || (c == '\t'))
			++pos;
		else
			break;
	}
	return str.substr(pos);
}

bool Serializer::writeNode(ptree& pt, const std::string& nodeName,
	const std::string& typeAttr, const std::string& value){
	pt.put(nodeName + ".<xmlattr>.type", typeAttr);
	pt.put(nodeName, value);
}
