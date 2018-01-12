# Leftovers Alarm

[![Demo video](https://i.imgur.com/hY11wqW.png)](https://vimeo.com/250845661) [![laser etched board](https://i.imgur.com/6zxrqX1m.jpg)](https://imgur.com/6zxrqX1)

At [asynchrony](http://www.asynchrony.com/), when a sales meeting comes to a conclusion, the leftovers from lunch are set in the hall outside the meeting room. An email is sent out announcing what food is leftover and where.

### A ravenous problem

When the company was ~200 people, this was no big deal. One could check their email periodically, and lazily make their way to the meeting room with a reasonable expectation of a cheesy burrito or crisp salad.

Now that the company is more than double that size, the free lunch market is much more competitive. Each leftovers email is immediately followed by a mad dash of programmers, immediately followed by the disappointed return of all but the first dozen to make it in line.

In the spirit of giving my team a competitive edge, and "taking things too far", this project alerts coworkers in the immediate vicinity to the presence and location of leftovers.

### Working theory

People in the company are notified of leftovers by email. The fastest responders have email push notifications, and check their phones religiously. This gives them a response time of about 15 seconds (5 seconds for push notification to propagate, 5 seconds to take out their phone, 5 seconds to read and understand message). We can beat this response time by reducing or removing all three steps.

A Raspberry Pi running the Windows IOT Core can use Microsoft's own exchange libraries directly, reducing the push notification time below a second (mostly just network latency between the pi and exchange server). A super-bright emergency strobe could immediately and unmistakably announce the presence of leftovers. Finally, a laser-etched map of the building, complete with LEDs at known leftover locations could direct teammates to the correct hallway where they might find a savory brisket or pulled pork sandwich.

### Limitations

 - There is no indication of what type of leftovers might be available, just when and where.
 - Malicious actors could send false leftovers emails to the account associated with the device, degrading the trust in the system.

### Construction

As the strobe requires mains power and I want to protect the pi, I am using an 8 channel relay board to drive both the strobe and LEDs. Since I will be driving several relays simultaneously, I am going to run them [active-low](https://learn.sparkfun.com/tutorials/logic-levels/active-low-and-active-high) (except for the status LED, which is driven directly by the pi).

 - Connect the [relay board to the raspberry pi](https://www.raspberrypi.org/forums/viewtopic.php?t=36225)
 - Splice one of the relays into the power cable for the strobe (be careful with mains power, it can kill you)
 - Bridge the input of the rest of the relays to a 5v supply
 - Connect the [normally-open](https://www.theautomationstore.com/electrical-contacts-normally-open-and-normally-closed-contacts/) side of each relay to the positive side of an LED, and ground the negative side

### Configuring code

You'll need to follow the steps to setup the [Windows IOT Core](https://developer.microsoft.com/en-us/windows/iot/getstarted) on your pi. Once you can reliably deploy code, you can configure it to your rooms and email contents.

 - Update MainPage.xaml.cs with your exchange server and credentials
 - Update Rooms.cs with your list of rooms on your map. Make sure to leave Status and Beacon in place.
 - Update the PickRoom function in the MessageHelper with the logic needed to select a room based on the subject/message of an email
 - Update the LightController constructor with the GPIO pins that each room is connected.
 
