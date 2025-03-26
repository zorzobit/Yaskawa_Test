using System;
using System.ComponentModel;
using System.Windows.Automation;
using YMConnect;
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

        internal void SetOverride(int @override)
        {
            //throw new NotImplementedException();
        }

        internal int GetOverride()
        {
            var jd = new JobData();
            var status2 = controller.Job.GetExecutingJobInformation(InformTaskNumber.Master, out jd);
            return (int)jd.SpeedOverride;
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
    }
}
