RefBox README

RefBox is intended to be an utility for interacting with the robots during the RoboCup @Home competition. It operates by sending and receiving UDP messages (broadcast). Two UDP ports are used for this purpose: one the RefBox listens for receiving incomming messages from the robot (default is 3000), and a second one for RefBox robot communication. 

RefBox works with two type of messages: the signal message which is sent to the robot to  and the event message.

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