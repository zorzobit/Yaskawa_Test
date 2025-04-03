
using PropertyChanged;
using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using YMConnect;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Yaskawa_Test
{
    public class MainViewModel : BaseViewModel
    {
        MainWindow mainWindow;
        Yaskawa_interface yaskawa_interface;
        BackgroundWorker DataCheck;
        ControllerStateData stateData = new ControllerStateData();
        internal void Loaded(MainWindow mWindow)
        {
            this.mainWindow = mWindow;
            yaskawa_interface = new Yaskawa_interface();
            DataCheck = new BackgroundWorker();
            DataCheck.DoWork += DataCheck_DoWork; ;
            DataCheck.RunWorkerCompleted += DataCheck_RunWorkerCompleted;
            mainWindow.OverrideSlider.PreviewMouseDown += OverrideSlider_MouseDown;
            mainWindow.OverrideSlider.PreviewMouseUp += OverrideSlider_MouseUp;
            RobotPositionVariableData robotPosition = new RobotPositionVariableData();
            PositionData positionData = new PositionData();
            positionData.AxisData.SetValue(1.025, 0);
            positionData.AxisData.SetValue(-22.04, 1);
            positionData.AxisData.SetValue(12.123, 2);
            positionData.AxisData.SetValue(-32.333, 3);
            positionData.AxisData.SetValue(132.22, 4);
            positionData.AxisData.SetValue(-12.120, 5);
            positionData.CoordinateType = CoordinateType.UserCoordinate;
            positionData.ToolNumber = 1;
            positionData.UserCoordinateNumber = 1;
            Figure figure = new Figure();
            figure.FlipOrNoFlip = FlipOrNoFlip.Flip;
            figure.FrontOrBack = FrontOrBack.Back;
            figure.UpperOrLower = UpperOrLower.Lower;
            figure.AxisAngles.SetValue(AxisAngle.GreaterThanOrEqual180, 0);
            figure.AxisAngles.SetValue(AxisAngle.LessThan180, 1);
            figure.AxisAngles.SetValue(AxisAngle.GreaterThanOrEqual180, 2);
            figure.AxisAngles.SetValue(AxisAngle.LessThan180, 3);
            figure.AxisAngles.SetValue(AxisAngle.GreaterThanOrEqual180, 4);
            figure.AxisAngles.SetValue(AxisAngle.LessThan180, 5);
            
        }
        bool overrideHold = false;
        private void OverrideSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (yaskawa_interface.IsConnected)
                yaskawa_interface.SetOverride(Override, OverrideGroup);
            overrideHold = false;
        }
        private void OverrideSlider_MouseDown(object sender, MouseButtonEventArgs e)
        {
            overrideHold = true;
        }

        private void DataCheck_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (yaskawa_interface.IsConnected)
            {

                OperationStatus = stateData.IsRunning ? "RUNNING" : "STOP";
                DataCheck.RunWorkerAsync();
            }
        }
        private void DataCheck_DoWork(object? sender, DoWorkEventArgs e)
        {
            stateData = yaskawa_interface.GetStateData();
            var posj = yaskawa_interface.GetPosj();
            var posw = yaskawa_interface.GetPosw();
            var posu = yaskawa_interface.GetPosu();
            if (posj.HasValue)
            {
                this.PosJ = new Pos()
                {
                    X = posj.Value.AxisData[0],
                    Y = posj.Value.AxisData[1],
                    Z = posj.Value.AxisData[2],
                    RX = posj.Value.AxisData[3],
                    RY = posj.Value.AxisData[4],
                    RZ = posj.Value.AxisData[5],
                };
            }
            if (posw.HasValue)
            {
                this.PosW = new Pos()
                {
                    X = posw.Value.AxisData[0],
                    Y = posw.Value.AxisData[1],
                    Z = posw.Value.AxisData[2],
                    RX = posw.Value.AxisData[3],
                    RY = posw.Value.AxisData[4],
                    RZ = posw.Value.AxisData[5],
                };
            }
            if (posu.HasValue)
            {
                this.PosU = new Pos()
                {
                    X = posu.Value.AxisData[0],
                    Y = posu.Value.AxisData[1],
                    Z = posu.Value.AxisData[2],
                    RX = posu.Value.AxisData[3],
                    RY = posu.Value.AxisData[4],
                    RZ = posu.Value.AxisData[5],
                };
            }
            if (!overrideHold)
                Override = yaskawa_interface.GetOverride(OverrideGroup);
            try
            {
                var alarms = yaskawa_interface.GetActiveAlarms();
                if (alarms.HasValue)
                {
                    ActiveAlarms = new ObservableCollection<AlarmData>(alarms.Value.Alarms.ToList());
                }
            }
            catch (Exception ex)
            {
                AlarmData alarm = new AlarmData();
                alarm.Name = ex.Message;
                ActiveAlarms = new ObservableCollection<AlarmData>();
                ActiveAlarms.Add(alarm);
            }

            if (SetNumValuesEnabled)
            {
                if(RegNum1.Num!=null&&RegNum1.Val!=null) 
                    yaskawa_interface.SetRegisterInt(ushort.Parse(RegNum1.Num), int.Parse(RegNum1.Val));
                if (RegNum2.Num != null && RegNum2.Val != null)
                    yaskawa_interface.SetRegisterInt(ushort.Parse(RegNum2.Num), int.Parse(RegNum2.Val));
            }
            else
            {
                if (RegNum1.Num != null)
                    RegNum1.Val = yaskawa_interface.GetRegisterInt(ushort.Parse(RegNum1.Num)).ToString();
                if (RegNum2.Num != null)
                    RegNum2.Val = yaskawa_interface.GetRegisterInt(ushort.Parse(RegNum2.Num)).ToString();
            }

            if (SetRealValuesEnabled)
            {
                if (RegReal1.Num != null && RegReal1.Val != null)
                    yaskawa_interface.SetRegisterFloat(ushort.Parse(RegReal1.Num), int.Parse(RegReal1.Val));
                if (RegReal2.Num != null && RegReal2.Val != null)
                    yaskawa_interface.SetRegisterFloat(ushort.Parse(RegReal2.Num), int.Parse(RegReal2.Val));
            }
            else
            {
                if (RegReal1.Num != null)
                    RegReal1.Val = yaskawa_interface.GetRegisterFloat(ushort.Parse(RegReal1.Num)).ToString();
                if (RegReal2.Num != null)
                    RegReal2.Val = yaskawa_interface.GetRegisterFloat(ushort.Parse(RegReal2.Num)).ToString();
            }
            if (SetPRValuesEnabled)
            {
                yaskawa_interface.SetPR(PosReg1.RobotPosition);
                yaskawa_interface.SetPR(PosReg2.RobotPosition);
            }
            else
            {
                if (PosReg1.Num != null && yaskawa_interface.GetPR(ushort.Parse(PosReg1.Num)).HasValue)
                {
                    PosReg1.RobotPosition = yaskawa_interface.GetPR(ushort.Parse(PosReg1.Num)).Value;
                }
                if (PosReg2.Num != null && yaskawa_interface.GetPR(ushort.Parse(PosReg2.Num)).HasValue)
                {
                    PosReg2.RobotPosition = yaskawa_interface.GetPR(ushort.Parse(PosReg2.Num)).Value;
                }
            }
            yaskawa_interface.ReadInputData(RDI1);
            yaskawa_interface.ReadInputData(RDI2);
            if (SetIOValuesEnabled)
            {
                yaskawa_interface.WriteOutputData(RDO1);
                yaskawa_interface.WriteOutputData(RDO2);
            }
            else
            {
                yaskawa_interface.ReadOutputData(RDO1);
                yaskawa_interface.ReadOutputData(RDO2);
            }
        }
        public int Override { get; set; }
        public string ConnectButtonContext { get; set; } = "Connect";
        public string IPTextBox { get; set; } = "192.168.255.1";
        public string OperationStatus { get; set; } = "No Connection";
        public string ActiveTask { get; set; } = "No Connection";
        public ObservableCollection<AlarmData> ActiveAlarms { get; set; }

        public string RegNumSetButtonName { get; set; } = "SET";
        public bool SetNumValuesEnabled { get; private set; }
        public bool SetNumNamesEnabled { get; private set; } = true;
        public string RegRealSetButtonName { get; set; } = "SET";
        public bool SetRealValuesEnabled { get; private set; }
        public bool SetRealNamesEnabled { get; private set; } = true;
        public bool SetIOValuesEnabled { get; private set; }
        public bool SetIONamesEnabled { get; private set; } = true;
        public string SetIOButtonName { get; set; } = "SET";

        public ushort OverrideGroup { get; set; } = 199;


        public string PosRegSetButtonName { get; set; } = "SET";
        public bool SetPRValuesEnabled { get; private set; }
        public bool SetPRNamesEnabled { get; private set; } = true;

        public RegItem RegNum1 { get; set; }
        public RegItem RegNum2 { get; set; }
        public RegItem RegReal1 { get; set; }
        public RegItem RegReal2 { get; set; }

        public RegItem PosReg1 { get; set; }
        public RegItem PosReg2 { get; set; }

        public RegItem RDI1 { get; set; }
        public RegItem RDI2 { get; set; }
        public RegItem RDO1 { get; set; }
        public RegItem RDO2 { get; set; }



        public Pos PosJ { get; set; } = new Pos();
        public Pos PosW { get; set; } = new Pos();
        public Pos PosU { get; set; } = new Pos();

        public ICommand RegNumSetButton
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (RegNumSetButtonName == "SET")
                    {
                        RegNumSetButtonName = "GET";
                        SetNumValuesEnabled = true;
                        SetNumNamesEnabled = false;
                    }
                    else
                    {
                        RegNumSetButtonName = "SET";
                        SetNumValuesEnabled = false;
                        SetNumNamesEnabled = true;
                    }
                }, o => true);
            }
        }
        public ICommand RegRealSetButton
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (RegRealSetButtonName == "SET")
                    {
                        RegRealSetButtonName = "GET";
                        SetRealValuesEnabled = true;
                        SetRealNamesEnabled = false;
                    }
                    else
                    {
                        RegRealSetButtonName = "SET";
                        SetRealValuesEnabled = false;
                        SetRealNamesEnabled = true;
                    }
                }, o => true);
            }
        }
        public ICommand SetIOValues
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (SetIOButtonName == "SET")
                    {
                        SetIOButtonName = "GET";
                        SetIOValuesEnabled = true;
                        SetIONamesEnabled = false;
                    }
                    else
                    {
                        SetIOButtonName = "SET";
                        SetIOValuesEnabled = false;
                        SetIONamesEnabled = true;
                    }
                }, o => true);
            }
        }
        private Brush _backgroundBrush = Brushes.White;
        private DispatcherTimer _animationTimer;
        public Brush BackgroundBrush { get; set; }
        public ICommand PosRegSetButton
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (PosRegSetButtonName == "SET")
                    {
                        PosRegSetButtonName = "GET";
                        SetPRValuesEnabled = true;
                        SetPRNamesEnabled = false;
                    }
                    else
                    {
                        PosRegSetButtonName = "SET";
                        SetPRValuesEnabled = false;
                        SetPRNamesEnabled = true;
                    }
                }, o => true);
            }
        }
        public ICommand ConnectButtonClick
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (ConnectButtonContext == "Connect")
                    {
                        Task.Run(() =>
                        {
                            OperationStatus = "Connecting...";
                            BackgroundBrush = Brushes.Yellow;
                            yaskawa_interface.Connect(IPTextBox);
                            if (yaskawa_interface.IsConnected)
                            {
                                DataCheck.RunWorkerAsync();
                                ConnectButtonContext = "DisConnect";
                                RegNum1 = new RegItem();
                                RegNum2 = new RegItem();
                                RegReal1 = new RegItem();
                                RegReal2 = new RegItem();
                                PosReg1 = new RegItem();
                                PosReg2 = new RegItem();
                            }
                            else
                            {
                                OperationStatus = "Connection failed";
                                BackgroundBrush = Brushes.LightPink;
                                Thread.Sleep(1500);
                                BackgroundBrush = Brushes.White;
                                OperationStatus = "No Connection";
                            }
                        });

                    }
                    else
                    {
                        yaskawa_interface.DisConnect();
                        ConnectButtonContext = "Connect";
                    }
                }, o => true);
            }
        }
        public ICommand Start
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (yaskawa_interface.IsConnected)
                        yaskawa_interface.Start();
                }, o => true);
            }
        }
        public ICommand Stop
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (yaskawa_interface.IsConnected)
                        yaskawa_interface.FeedHold();
                }, o => true);
            }
        }
        public ICommand Abort
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (yaskawa_interface.IsConnected)
                        yaskawa_interface.Abort();
                }, o => true);
            }
        }
        public ICommand Reset
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (yaskawa_interface.IsConnected)
                        yaskawa_interface.Reset();
                }, o => true);
            }
        }
    }
    public class Pos
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }

    }
    [AddINotifyPropertyChangedInterface] // Fody automatically implements INotifyPropertyChanged
    public class RegItem
    {
        public string Num { get; set; }
        public string Val { get; set; }
        public bool IsON { get; set; }
        private RobotPositionVariableData _robotPosition;
        public RobotPositionVariableData RobotPosition
        {
            get
            {
                return ValToPos();
            }
            set
            {
                this.Val = value.ToString();
            }
        }
        private RobotPositionVariableData ValToPos()
        {
            RobotPositionVariableData robotPosition = new RobotPositionVariableData();
            return robotPosition;
        }
        public bool Bit0 { get; set; }
        public bool Bit1 { get; set; }
        public bool Bit2 { get; set; }
        public bool Bit3 { get; set; }
        public bool Bit4 { get; set; }
        public bool Bit5 { get; set; }
        public bool Bit6 { get; set; }
        public bool Bit7 { get; set; }


    }
}
