#include "Event.h"
using namespace refbox;

Event::Event(){}

Event::Event(const Event& other):
	type(other.type), value(other.value), source(other.source){}

Event::Event(const std::string& type, const std::string& value):
	type(type), value(value){}

Event::Event(const std::string& type, const std::string& value, const EndPoint& source):
	type(type), value(value), source(source){}