using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using FontAwesome.Sharp;
using GORI.Models;
using GORI.Services;
using GORI.Services.Communication;
using GORI.Services.DataServices;
using GORI.Services.Process;
using GORI.Views;

namespace GORI.ViewModels
{
    public class MainViewModel : Conductor<object> , IViewAware
    {

        //Fields
        private string _currentUserAccount;
        private string _CurrentUserAccountIMG;
        private string _caption;
        private IconChar _icon;

        // Properties
        public string CurrentUserAccountIMG { get => _CurrentUserAccountIMG; set { _CurrentUserAccountIMG = value; NotifyOfPropertyChange(() => CurrentUserAccountIMG); } }

        public string CurrentUserAccount { get => _currentUserAccount; set { _currentUserAccount = value; NotifyOfPropertyChange(() => CurrentUserAccount); } }

        public string Caption { get => _caption; set { _caption = value; NotifyOfPropertyChange(() => Caption); } }

        public IconChar Icon { get => _icon; set { _icon = value; NotifyOfPropertyChange(() => Icon); } }

        private bool _isCheckHomeView = false;
        public bool isCheckHomeView { get => _isCheckHomeView; set { _isCheckHomeView = value; NotifyOfPropertyChange(() => isCheckHomeView); } }
        

        private object _view;

        bool _expandAuto = false;

        public void AttachView(object view, object context = null)
        {
            _view = view;
        }

        public object GetView()
        {
            return _view;
        }

        private HomeViewModel homeViewModel;
        private ReportViewModel reportViewModel;
        private SettingViewModel settingViewModel;
        private HistoryViewModel historyViewModel;
        private Process_Torque _Torque;

        private MachineData machineData;
        private MasterTorque masterTorque;
        private ModbusComm PLCComm;
        private ExcelRW excelRW;

        private bool IsHomeView = false;

        public MainViewModel()
        {
            machineData = new MachineData();
            machineData.ReadData();

            excelRW = new ExcelRW(machineData);

            masterTorque = new MasterTorque();
            masterTorque.ReadData();

            PLCComm = new ModbusComm(machineData);

            _Torque = new Process_Torque(ref PLCComm, ref excelRW);

            homeViewModel = new HomeViewModel(masterTorque, ref excelRW, ref _Torque);
            reportViewModel = new ReportViewModel();
            settingViewModel = new SettingViewModel();
            historyViewModel = new HistoryViewModel();

            ActivateItemAsync(homeViewModel);
            IsHomeView = true;
            isCheckHomeView = true;
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }

        public void ShowHomeViewCommand()
        {
            if(!IsHomeView)
            {
                homeViewModel = null;
                homeViewModel = new HomeViewModel(masterTorque, ref excelRW, ref _Torque);
                IsHomeView = true;
                isCheckHomeView = true;
                ActivateItemAsync(homeViewModel);
                Caption = "Dashboard";
            }
            
        }

        public void ShowReportViewCommand()
        {
            if(!homeViewModel.isstart)
            {
                ActivateItemAsync(reportViewModel);
                IsHomeView = false;
                isCheckHomeView = false;
                Caption = "Report";
                Icon = IconChar.ChartPie;
            }
            else
            {
                MessageBox.Show("The program is running and cannot switch tabs. Please stop the progran.", 
                    "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                isCheckHomeView = true;
                return;
            }
        }

        public void ShowSettingViewCommand()
        {
            if (!homeViewModel.isstart)
            {
                ActivateItemAsync(settingViewModel);
                IsHomeView = false;
                isCheckHomeView = false;
                Caption = "Setting";
                Icon = IconChar.Tools;
            }
            else
            {
                MessageBox.Show("The program is running and cannot switch tabs. Please stop the progran.", 
                    "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                isCheckHomeView = true;
                return;
            }
        }

        public void ShowHistoryViewCommand()
        {
            if (!homeViewModel.isstart)
            {
                ActivateItemAsync(historyViewModel);
                IsHomeView = false;
                isCheckHomeView = false;
                Caption = "History";
                Icon = IconChar.History;
            }
            else
            {
                MessageBox.Show("The program is running and cannot switch tabs. Please stop the progran.", 
                    "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                isCheckHomeView = true;
                return;
            }
        }

        public void ExpandControlAuto()
        {
            _expandAuto = !_expandAuto;
            if (_expandAuto)
            {
                homeViewModel.ControlAutoView = 300;
            }
            else
                homeViewModel.ControlAutoView = 0;
        }

        public void AppClose()
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                if (_view is MainView mv)
                {
                    mv.ShutdownWithFade();
                }
            }
            else
                return;
        }

        public void AppMaximine()
        {
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (window != null)
            {
                if (window.WindowState == WindowState.Normal)
                {
                    window.WindowState = WindowState.Maximized;
                }
                else
                {
                    window.WindowState = WindowState.Normal;
                }
            }
        }

        public void AppMinimine()
        {
            if (_view is MainView mv)
            {
                mv.MinimizeWithFade();
            }
        }
    }
}
