#include "Connector.h"
#include "Serializer.h"
using namespace refbox;
using boost::asio::ip::udp;

// const int Connector::inputBufferSize(81920);
// const int Connector::portIn(3001);
// const int Connector::portOut(3000);

Connector::Connector ()	: running_(false), mainThread_(NULL),
	socket_(NULL), inputBuffer_(NULL){
	this->inputBuffer_ = new char[inputBufferSize];
}

void Connector::beginReceive(){
	this->socket_->async_receive_from(
		boost::asio::buffer(inputBuffer_, inputBufferSize),
		this->senderEP_,
		boost::bind(
			&Connector::receiveCallback,
			this,
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred
		)
	);
}

void Connector::mainThreadTask(){
	while( this->running_ ){
		this->service_.run();
	}
}

void Connector::onMessageReceived(const std::string& message, const EndPoint& source){
	try{
		this->messageReceived(this, source, message);
	}catch(...){
#if DEBUG
		std::cout<< "Error!" << std::endl;
#endif
	}
}

void Connector::receiveCallback(const boost::system::error_code& error, size_t received){
	if(error || (received < 1))
		return;
	EndPoint clientEP(this->senderEP_);
	std::string sData(this->inputBuffer_, received);
	this->beginReceive();
	this->onMessageReceived(sData, clientEP);
}

void Connector::start(){
	if(this->running_)
		return;
	this->socket_ = new udp::socket(this->service_, EndPoint(udp::v4(), portIn));
	this->socket_->set_option(udp::socket::reuse_address(true));
	this->socket_->set_option(boost::asio::socket_base::broadcast(true));
	this->beginReceive();
	this->running_ = true;
	this->mainThread_ = new boost::thread(boost::bind(&Connector::mainThreadTask, this));
}

void Connector::stop(){
	if(!this->running_)
		return;
	this->running_ = false;
	this->mainThread_->join();
	// this->service.reset();
	this->service_.stop();
	delete this->socket_;
	this->socket_ = NULL;
	delete this->mainThread_;
	this->mainThread_ = NULL;
}

void Connector::broadcast(const Event& event) const{
	if(!this->running_ || (this->socket_ == NULL))
		return;
	// Serialize signal
	std::string serialized = Serializer::serialize (event);
	// Send the data
	this->socket_->send_to(
		boost::asio::buffer(serialized.c_str(), serialized.size()),
		EndPoint(udp::v4(), portOut)
	);
}