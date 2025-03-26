# Yaskawa Test - Robot Communication Example


This project demonstrates how to establish communication with a Yaskawa robot using the `YMConnect` library within a WPF application. The code provides various methods for controlling and monitoring the robot, such as connecting, retrieving positions, controlling I/O, and reading/writing registers.

## Features

- **Connection Management:** Connect and disconnect from the Yaskawa robot controller.
- **Robot State Retrieval:** Fetch robot state data, including positions in different coordinate systems (joint, world, user).
- **I/O Control:** Write to and read from digital I/O.
- **Register Manipulation:** Get and set integer and floating-point values in the robot controller’s registers.
- **Job Control:** Start, hold, and reset robot jobs.
- **Alarms:** Fetch active alarms from the robot controller.

## Prerequisites

- **YMConnect Library:** This project requires the `YMConnect` library, which is used to communicate with the Yaskawa robot. Ensure that the library is referenced in your project.
- **.NET Framework 4.7.2+ or .NET Core (for newer versions)**
- **Yaskawa Robot:** A Yaskawa robot controller with network connectivity to your machine.

## Installation

1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/zorzobit/Yaskawa_Test.git
   ```
2. Open the project in Visual Studio or your preferred C# IDE.

3. Ensure the YMConnect library is correctly referenced in your project.

4. Build the project to restore any dependencies.

## Usage
Connecting to the Robot
To establish a connection to the robot, create an instance of Yaskawa_interface and call the Connect method with the robot's IP address:
```csharp
   Yaskawa_interface robot = new Yaskawa_interface();
   robot.Connect("192.168.0.1");
```

      
## Fetching Robot State
You can get the robot's current state by calling GetStateData:
```csharp
   var stateData = robot.GetStateData();
```
## Getting Robot Position
The following methods can be used to fetch the robot's position in different coordinate systems:
```csharp
   var posJ = robot.GetPosj(); // Joint Coordinates
   var posW = robot.GetPosw(); // World Coordinates
   var posU = robot.GetPosu(); // User Coordinates
```
## Writing and Reading I/O
You can write a digital value to an I/O point using the WriteIO method:
```csharp
   robot.WriteIO(1, 1); // Write value 1 to I/O point 1
```
To read integer or float values from the robot's registers, use GetRegisterInt and GetRegisterFloat:
```csharp
   var registerInt = robot.GetRegisterInt(10);
   var registerFloat = robot.GetRegisterFloat(20);
```

## Job Control
Control robot jobs (start, hold, reset) using the following methods:
```csharp
   robot.Start();      // Start a job
   robot.FeedHold();   // Feed hold the job
   robot.Reset();      // Reset the job
```

## Active Alarms
To get the active alarms, you can use:
```csharp
   var alarms = robot.GetActiveAlarms();
```
## Contributing
Contributions to this project are welcome! Feel free to fork the repository, make changes, and create pull requests.

1. Fork the repository.

2. Create a new branch (git checkout -b feature-branch).

3. Commit your changes (git commit -am 'Add new feature').

4. Push to the branch (git push origin feature-branch).
   
5. Create a new Pull Request.

## License
This project is open-source and available under the MIT License.

## Acknowledgements
- <a href="https://github.com/Yaskawa-Global/YMConnect" target="_blank">YMConnect</a> library for robot communication.

- <a href="https://www.yaskawa.com" target="_blank">Yaskawa Electric Corporation</a> for their robot controllers.

<i>This ReadMe created with ChatGPT</i>
