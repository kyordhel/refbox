##############################################################################
#
# REFBOX SERVER
#
##############################################################################

RefBox Server README

RefBox Server is intended to be an utility for interacting with the robots during the RoboCup @Home competition. It operates by sending and receiving UDP messages (broadcast). Two UDP ports are used for this purpose: one the RefBox listens for receiving incomming messages from the robot (default is 3000), and a second one for RefBox robot communication. 

RefBox Server works with two type of messages: the SIGNAL message which is sent to the robot for triggering actions, and the EVENT message which is intended to be used by the robot to notify what is doing.

Port        Listener    Data flow            Message type
UDP3000     Refbox      Robot -> RefBox      Event
UDP3001     Robot       RefBox -> Robot      Signal

SIGNAL MESSAGE
Sent by RefBox to the robot to notify the remaining test time and trigger actions. Four types of signal messages are used.

    start       Sent by RefBox to indicate the test has started. Contains the START literal.
    stop        Sent by RefBox to indicate the test has ended. Contains the STOP literal.
    time        Sent by RefBox to indicate the remaining test time. Contains an integer number of seconds left.
    continue    Sent by RefBox as an implementation of the CONTINUE rule. It contains text which may be interpreted as an speech inpiut.

EVENT MESSAGE
Sent by the robot to notify an event to RefBox. All event messages are logged
	speech		RefBox listen to speech event to display the robot's speech on screen.


##############################################################################
#
# REFBOX ROS CLIENT
#
##############################################################################
RefBox Client for ROS README

Implements an interface to allow robots using ROS to interact with the arena and the referees during the RoboCup @Home competition. It operates by sending and receiving UDP messages (broadcast). Two UDP ports are used for this purpose: one the RefBox client listens for receiving incomming messages from the server (default is 3001), and a second one for RefBox server communication. 

RefBox ROS client works with two type of messages: the EVENT message which is sent to the RefBox server to notify robot's actions and the SIGNAL message, sent by the RefBoz server to trigger actions.

Port        Listener    Data flow            Message type    Action
UDP3001     Robot       RefBox -> Robot      Signal          Topic Write
UDP3000     Refbox      Robot -> RefBox      Event           Topic Read

TOPICS
The following topics are used by RefBox client
	
    Topic name        Type      Purpose
    refbox_test       String    Written by RefBox client to indicate the test has started or stopped.
                                It contains the START or STOP literal strings.
    refbox_time       String    Written by RefBox client to indicate the remaining test time in seconds.
    refbox_continue   String    Written by RefBox client to inject text as stated in the CONTINUE rule.
                                It contains text which may be interpreted as an speech inpiut.
	speech		      String    RefBox client subscribes to this topic, interpreting its content as text to be
                                synthetized as speech by the robot.

