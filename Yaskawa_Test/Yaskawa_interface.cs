﻿using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Controls;
using YMConnect;
using YMConnect.Interop;
namespace Yaskawa_Test
{
    internal class Yaskawa_interface
    {
        MotomanController controller;
        int SUCCESS = 0;
        public bool IsConnected { get; set; } = false;
        public Yaskawa_interface()
        {

        }
        public void Connect(string ip)
        {
            StatusInfo status = new StatusInfo();
            controller = MotomanController.OpenConnection(ip, out status);
            if (status != null && status.StatusCode == SUCCESS)
                this.IsConnected = true;
            else
                this.IsConnected = false;
        }
        public void DisConnect()
        {
            if (controller != null)
            {
                try
                {
                    controller.CloseConnection();
                }
                catch (Exception)
                {
                }
            }
            this.IsConnected = false;
        }
        public ControllerStateData GetStateData()
        {
            ControllerStateData controllerStateData = new ControllerStateData();
            controller.Status.ReadState(out controllerStateData);
            return controllerStateData;
        }
        public PositionData? GetPosj()
        {
            PositionData positionData = new PositionData();
            var status = controller?.ControlGroup?.ReadPositionData(ControlGroupId.R1, CoordinateType.BaseCoordinate, 1, 1, out positionData);
            if (status != null && status.StatusCode == SUCCESS)
                return positionData;
            else
                return null;
        }
        public PositionData? GetPosw()
        {
            PositionData positionData = new PositionData();
            var status = controller?.ControlGroup?.ReadPositionData(ControlGroupId.R1, CoordinateType.RobotCoordinate, 0, 0, out positionData);
            if (status != null && status.StatusCode == SUCCESS)
                return positionData;
            else
                return null;
        }
        public PositionData? GetPosu()
        {
            PositionData positionData = new PositionData();
            var status = controller?.ControlGroup?.ReadPositionData(ControlGroupId.R1, CoordinateType.UserCoordinate, 1, 1, out positionData);
            if (status != null && status.StatusCode == SUCCESS)
                return positionData;
            else
                return null;
        }
        public bool WriteIO(uint num, byte io)
        {
            var status = controller?.IO?.WriteByte(num, io);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public int? GetRegisterInt(ushort num)
        {
            IntegerVariableData integerVariableData = new IntegerVariableData();
            var status = controller?.Variables?.IntegerVariable?.Read(num, out integerVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return (int)integerVariableData.Value;
            else
                return null;
        }
        public bool SetRegisterInt(ushort num, int value)
        {
            IntegerVariableData integerVariableData = new IntegerVariableData();
            integerVariableData.VariableIndex = num;
            integerVariableData.Value = (short)value;
            var status = controller?.Variables?.IntegerVariable?.Write(integerVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public float? GetRegisterFloat(ushort num)
        {
            RealVariableData realVariableData = new RealVariableData();
            var status = controller?.Variables?.RealVariable?.Read(num, out realVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return realVariableData.Value;
            else
                return null;
        }
        public bool SetRegisterFloat(ushort num, float value)
        {
            RealVariableData realVariableData = new RealVariableData();
            realVariableData.VariableIndex = num;
            realVariableData.Value = value;
            var status = controller?.Variables?.RealVariable?.Write(realVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public RobotPositionVariableData? GetPR(ushort index)
        {
            RobotPositionVariableData robotPositionVariableData = new RobotPositionVariableData();

            var status = controller?.Variables?.RobotPositionVariable?.Read(index, out robotPositionVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return robotPositionVariableData;
            else
                return null;
        }
        public bool SetPR(RobotPositionVariableData robotPositionVariableData)
        {
            var status = controller?.Variables?.RobotPositionVariable?.Write(robotPositionVariableData);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public bool Start()
        {
            var status = controller?.ControlCommands?.StartJob();
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public bool FeedHold()
        {
            var status = controller?.ControlCommands.SetHold(SignalStatus.ON);
            Thread.Sleep(200);
            var status2 = controller?.ControlCommands.SetHold(SignalStatus.OFF);
            if (status != null && status.StatusCode == SUCCESS && status2 != null && status2.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public bool Abort()
        {
            var status = controller?.Job.SetActiveJob("NEWJOB1",1);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }
        public bool Reset()
        {
            //var status = c?.Job.SetActiveJob("Test", 0);

            var status2 = controller.ControlCommands.SetServos(SignalStatus.ON);
            if (status2 != null && status2.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }

        //How Speed Override Works in Yaskawa Robots:
        //According to the Yaskawa manual:
        //The Speed Override function allows modifying the playback speed of a robot job.
        //The override percentage is stored in specific parameters(S4C288 - S4C295) or controlled via external input signals.
        //The Universal Input signals (1-8) define the speed override percentage.If all override signals are 0, the speed is determined directly from the input signal status.
        //Parameter S2C701 must be set to 1 to enable external speed override control.
        //The priority of signals follows this order: Signal 1 > Signal 8. The highest priority signal determines the override speed.
        //
        //Enable Speed Override via Parameters:
        //Set S2C701 = 1 to enable external signal control.
        //Set S4C287 to define the Universal Input group(1-512).(Here I have used 199)
        //Configure S4C288 - S4C295 with speed values(1-255).
        //If S4C288 - S4C295 = 0, the input signal directly determines the speed percentage.
        //Confirm Override in the Robot Interface:
        //Navigate to { SETUP} → {SPEED OVERRIDE SETTING} to verify override values.
        //Ensure security mode allows modifications.
        //
        internal bool SetOverride(int @override, ushort group)
        {
            IOByteData val = new IOByteData(Convert.ToByte(@override));
            var status = controller.IO.WriteByte(IOType.NetworkInput, group, val);
            if (status != null && status.StatusCode == SUCCESS)
                return true;
            else
                return false;
        }

        internal int GetOverride(ushort group)
        {
            byte val = Convert.ToByte(0);
            var status = controller.IO.ReadByte(IOType.NetworkInput, group, out val);
            if (status != null && status.StatusCode == SUCCESS)
                return Convert.ToInt32(val);
            else
                return 100;
        }
        internal ActiveAlarms? GetActiveAlarms()
        {
            ActiveAlarms activeAlarms = new ActiveAlarms();
            var status = controller.Faults.GetActiveAlarms(out activeAlarms);
            if (status != null && status.StatusCode == SUCCESS)
                return activeAlarms;
            else
                return null;
        }

        internal void ReadInputData(RegItem RegInput)
        {
            if(RegInput != null && RegInput.Num != null)
            {
                IOByteData val = new IOByteData();
                var status = controller?.IO?.ReadByte(IOType.GeneralInput, Convert.ToUInt16(RegInput.Num), out val);
                if (status != null && status.StatusCode == SUCCESS)
                {
                    RegInput.Bit0 = val.Get(0);
                    RegInput.Bit1 = val.Get(1);
                    RegInput.Bit2 = val.Get(2);
                    RegInput.Bit3 = val.Get(3);
                    RegInput.Bit4 = val.Get(4);
                    RegInput.Bit5 = val.Get(5);
                    RegInput.Bit6 = val.Get(6);
                    RegInput.Bit7 = val.Get(7);
                }
            }
        }
        internal void ReadOutputData(RegItem RegOutput)
        {
            if (RegOutput != null && RegOutput.Num != null)
            {
                IOByteData val = new IOByteData();
                var status = controller?.IO?.ReadByte(IOType.GeneralOutput, Convert.ToUInt16(RegOutput.Num), out val);
                if (status != null && status.StatusCode == SUCCESS)
                {
                    RegOutput.Bit0 = val.Get(0);
                    RegOutput.Bit1 = val.Get(1);
                    RegOutput.Bit2 = val.Get(2);
                    RegOutput.Bit3 = val.Get(3);
                    RegOutput.Bit4 = val.Get(4);
                    RegOutput.Bit5 = val.Get(5);
                    RegOutput.Bit6 = val.Get(6);
                    RegOutput.Bit7 = val.Get(7);
                }
            }
        }

        internal void WriteOutputData(RegItem RegOutput)
        {
            if (RegOutput != null && RegOutput.Num != null)
            {
                IOByteData val = new IOByteData();
                val.Set(0, RegOutput.Bit0);
                val.Set(1, RegOutput.Bit1);
                val.Set(2, RegOutput.Bit2);
                val.Set(3, RegOutput.Bit3);
                val.Set(4, RegOutput.Bit4);
                val.Set(5, RegOutput.Bit5);
                val.Set(6, RegOutput.Bit6);
                val.Set(7, RegOutput.Bit7);
                var status = controller?.IO?.WriteByte(IOType.GeneralOutput, Convert.ToUInt16(RegOutput.Num), val);
            }
        }
    }
}
