#include "Signal.h"
using namespace refbox;

Signal::Signal(){}

Signal::Signal(const Signal& other):
	type(other.type), value(other.value), source(other.source){}

Signal::Signal(const std::string& type, const std::string& value, const EndPoint& source) :
	type(type), value(value), source(source){}