using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using Modbus.Device;
using System.Net.Sockets;
using System.Windows.Shapes;
using OxyPlot.Annotations;
using OxyPlot.Wpf;
using GORI.Services.DataServices;
using GORI.Models;
using System.Threading;
using System.Windows;
using HorizontalAlignment = OxyPlot.HorizontalAlignment;
using GORI.Services.Process;
using GORI.Services.Communication;
using static System.Windows.Forms.LinkLabel;


namespace GORI.ViewModels
{
    public class HomeViewModel : Screen
    {

        private int controlAutoView = 0;
        public int ControlAutoView { get => controlAutoView; set { controlAutoView = value; NotifyOfPropertyChange(() => ControlAutoView); } }

        private object contentChildView;
        public object ContentChildView { get => contentChildView; set { contentChildView = value; NotifyOfPropertyChange(() => ContentChildView); } }

        private string _IsStart = "START";
        public string IsStart { get => _IsStart; set { _IsStart = value; NotifyOfPropertyChange(() => IsStart); } }

        private Brush _bgrIsStart = Brushes.Green;
        public Brush bgrIsStart { get => _bgrIsStart; set { _bgrIsStart = value; NotifyOfPropertyChange(() => bgrIsStart); } }

        public bool isstart = false;

        private PlotModel _plotModel = null;
        public PlotModel MyModel { get => _plotModel; set { _plotModel = value; NotifyOfPropertyChange(() => MyModel); } }

        TcpClient client;
        ModbusIpMaster master;
        private int signalCount = 0;
        private LineSeries _currentSeries;
        private int _currentX = 0;
        private Thread mThreadDrawing;
        private bool mThreadDrawingFlag = false;

        List<OxyColor> rainbowColor;

        private MasterTorque masterTorque;
        private ExcelRW excelRW;
        private Process_Torque process_Torque;


        public HomeViewModel(MasterTorque _masterTorque, ref ExcelRW _saveExcel, ref Process_Torque _process_Torque)
        {
            //excelRW = _saveExcel;
            masterTorque = _masterTorque;
            process_Torque = _process_Torque;
            DrawingChart();
            Start();
        }
        

        public void DrawingChart()
        {
            //rainbowColor = new List<OxyColor>()
            //{
            //    OxyColor.FromArgb(Opacity, 0, 255, 255),
            //    OxyColor.FromArgb(Opacity, 148, 0, 211),
            //    OxyColor.FromArgb(Opacity, 75, 0, 130),
            //    OxyColor.FromArgb(Opacity, 0, 0, 255),
            //    OxyColor.FromArgb(Opacity, 0, 255, 0),
            //    OxyColor.FromArgb(Opacity, 255, 255, 0),
            //    OxyColor.FromArgb(Opacity, 255, 127, 0),
            //    OxyColor.FromArgb(Opacity, 255, 0, 0),
            //};
            GC.Collect();

            MyModel = new PlotModel
            {
                Title = "TORQUE CHART",
                TitleColor = OxyColors.White,
                Background = OxyColors.Transparent,
                PlotAreaBackground = OxyColors.Transparent,
                PlotAreaBorderColor = OxyColors.White,
            };

            MyModel.Axes.Add(new LinearAxis
            {
                Title = "DATA POINT",
                Position = AxisPosition.Bottom,
                TextColor = OxyColors.White,
                TitleFontSize = 15,
                TitleFontWeight = OxyPlot.FontWeights.Bold,
                TicklineColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                TitleColor = OxyColors.White,
                Minimum = -30,
                Maximum = 1050,
            });
            MyModel.Axes.Add(new LinearAxis
            {
                Title = "TORQUE",
                Position = AxisPosition.Left,
                TextColor = OxyColors.White,
                TitleFontSize = 15,
                TitleFontWeight = OxyPlot.FontWeights.Bold,
                TicklineColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                TitleColor = OxyColors.White,
                Minimum = -0.001,
                Maximum = 0.021
            });

            foreach(var y in masterTorque.values)
            {
                var line = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = y.Lower,
                    Color = OxyColors.DeepPink,
                    LineStyle = LineStyle.Dash,
                };
                MyModel.Annotations.Add(line);
            }

            var lastLine = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = masterTorque.values[masterTorque.values.Count - 1].Upper,
                Color = OxyColors.DeepPink,
                LineStyle = LineStyle.Dash,
            };
            MyModel.Annotations.Add(lastLine);

            foreach (var y in masterTorque.values)
            {
                var line = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = y.Upper,
                    Color = OxyColors.DeepPink,
                    LineStyle = LineStyle.Dash,
                };
                //MyModel.Annotations.Add(line);
            }
            int indexcolors = -1;
            foreach(var y in masterTorque.values)
            {
                indexcolors++;
                var rect = new RectangleAnnotation
                {
                    MinimumY = y.Lower,
                    MaximumY = y.Upper,
                    Fill = OxyColor.FromAColor(100, OxyColors.Gray),//rainbowColor[indexcolors],
                    Layer = AnnotationLayer.BelowSeries,
                };

                var text = new TextAnnotation
                {
                    Text = masterTorque.caseNames[indexcolors],
                    TextColor = OxyColors.White,
                    TextPosition = new DataPoint(-25, (y.Lower + y.Upper) / 2),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle,
                    FontWeight = OxyPlot.FontWeights.Bold,
                    Background = OxyColors.Transparent,
                    Stroke = OxyColors.Transparent,
                };

                MyModel.Annotations.Add(rect);
                MyModel.Annotations.Add(text);
            }
        }

        bool bThreadFlag = false;
        public void Start()
        {
            isstart = !isstart;
            if (isstart)
            {
                IsStart = "STOP";
                bgrIsStart = Brushes.Red;
                if(MyModel != null)
                {
                    MyModel.Series.Clear();
                }    
                StartNewSeries();
                bThreadFlag = true;
                process_Torque.ThreadRun();
                ThreadRun();
            }
            else
            {
                IsStart = "START";
                bgrIsStart = Brushes.Green;
                bThreadFlag = false;
                process_Torque.ThreadStop();
                ThreadStop();
            }
        }

        public void FitPlotView()
        {
            MyModel.ResetAllAxes();
            MyModel.InvalidatePlot(true);
        }

        public void AddPoint(double y)
        {
            if (_currentSeries == null)
            {
                _currentSeries = CreateNewSeries();
                MyModel.Series.Add(_currentSeries);
                _currentX = 0;
            }
            _currentSeries.Points.Add(new DataPoint(_currentX++, y));
            MyModel.InvalidatePlot(true);
        }

        private LineSeries CreateNewSeries()
        {
            var rnd = new Random();
            var color = OxyColor.FromRgb(
                (byte)rnd.Next(0, 255),
                (byte)rnd.Next(0, 255),
                (byte)rnd.Next(0, 255));
            var color2 = OxyColor.FromRgb(0, 255, 0);

            return new LineSeries
            {
                Title = $"Series  {signalCount++}",
                Color = color,
                StrokeThickness = 1.5,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle,
            };
        }

        public void StartNewSeries()
        {
            _currentSeries = CreateNewSeries();
            MyModel.Series.Add(_currentSeries);
            _currentX = 0;
            MyModel.InvalidatePlot(true);
        }

        public void ThreadRun()
        {
            if (mThreadDrawingFlag) return;

            if (mThreadDrawing != null)
            {
                mThreadDrawing.Join(500);
                mThreadDrawing.Abort();
                mThreadDrawing = null;
            }
            mThreadDrawing = new Thread(() =>
            {
                DoRunProcess();
            });
            mThreadDrawing.SetApartmentState(ApartmentState.STA);
            mThreadDrawing.Priority = ThreadPriority.Highest;
            mThreadDrawing.IsBackground = true;
            mThreadDrawingFlag = true;

            mThreadDrawing.Start();
        }

        public void ThreadStop()
        {
            mThreadDrawingFlag = false;

            if (mThreadDrawing != null)
            {
                mThreadDrawing.Join(500);
                mThreadDrawing.Abort();
                mThreadDrawing = null;
            }
        }

        private void DoRunProcess()
        {
            while (bThreadFlag)
            {
                if (process_Torque.queue.Count > 0)
                {
                    AddPoint(process_Torque.queue.Dequeue());
                }
            }
        }
    }
}
