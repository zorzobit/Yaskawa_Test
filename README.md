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
   Open the project in Visual Studio or your preferred C# IDE.

2. Ensure the YMConnect library is correctly referenced in your project.

3. Build the project to restore any dependencies.

## Usage
Connecting to the Robot
To establish a connection to the robot, create an instance of Yaskawa_interface and call the Connect method with the robot's IP address:
```csharp
   Yaskawa_interface robot = new Yaskawa_interface();
   robot.Connect("192.168.0.1");

