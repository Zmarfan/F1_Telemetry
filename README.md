# Dream Commentator
A tool made for commentators for the game F1 2020 by codemasters. Runs over the game and is controlled by keyboard inputs to display certain information. Windows OS only. Made in Unity.

##### Includes features such as:
* Customizable timingscreen
* Custom driver names
* Custom driver portraits
* Mid race lap time comparisions between drivers
* Halo hud
* Wear & Damage information per driver
* Weather information
* Qualifying timing comparisions
* Penalty events
* And much, much more!

### Installation
Not available for download as of now

### Screenshots
![pic-1](https://github.com/Zmarfan/F1_Telemetry/blob/main/readmePictures/readmePic1.jpg?raw=true)
![pic-2](https://github.com/Zmarfan/F1_Telemetry/blob/main/readmePictures/readmePic2.jpg?raw=true)
![pic-3](https://github.com/Zmarfan/F1_Telemetry/blob/main/readmePictures/readmePic3.jpg?raw=true)

### How it works (roughly)
1. UDP Receiver listen on F1 2020 specified port and collects data packets framewise sent from the game itself 
1. Interpret and store this data correctly through [codemasters packet documentation](https://forums.codemasters.com/topic/50942-f1-2020-udp-specification/)
1. Create borderless fullscreen overlay without screen borders through specific OS functions
1. Events are invoked on event-based data which tells the application which state the game currently is in
1. Low level keyobard hook based input system controls what information is displayed 
