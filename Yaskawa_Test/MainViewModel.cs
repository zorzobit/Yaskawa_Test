
using System.ComponentModel;
using System.Data;
using System.Windows.Input;
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
        }
        bool overrideHold = false;
        private void OverrideSlider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (yaskawa_interface.IsConnected)
                yaskawa_interface.SetOverride(Override);
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
                Override = yaskawa_interface.GetOverride();
        }
        public int Override { get; set; }
        public string ConnectButtonContext { get; set; } = "Connect";
        public string IPTextBox { get; set; } = "10.0.0.2";
        public string OperationStatus { get; set; } = "No Connection";
        public string ActiveTask { get; set; } = "No Connection";

        public Pos PosJ { get; set; } = new Pos();
        public Pos PosW { get; set; } = new Pos();
        public Pos PosU { get; set; } = new Pos();
        public ICommand ConnectButtonClick
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (ConnectButtonContext == "Connect")
                    {
                        yaskawa_interface.Connect(IPTextBox);
                        if (yaskawa_interface.IsConnected)
                        {
                            DataCheck.RunWorkerAsync();
                            ConnectButtonContext = "DisConnect";
                        }
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
                        yaskawa_interface.FeedHold();
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
}
