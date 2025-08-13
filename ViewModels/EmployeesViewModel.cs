using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Caliburn.Micro;
using GORI.Models;
using GORI.Services.DataServices;

namespace GORI.ViewModels
{
    public class EmployeesViewModel : Screen
    {

        private string userName;
        public string Username
        {
            get => userName;
            set
            {
                userName = value;
                NotifyOfPropertyChange(() => Username);
                try
                {
                    DEmployees.Clear();
                    DEmployees = excelRW.FindProductByID(Username);
                    mDictinaryEmployees.EmployeesInfo = DEmployees;
                    if (DEmployees.Count != 0)
                    {
                        FullName = DEmployees["Name"];
                        IsIDFocused = false;
                        IsPOFocused = true;
                    }
                    else
                    {
                        IsIDFocused = true;
                        IsPOFocused = false;
                        FullName = "";
                        ErrorMessage = "* Invalid username or password";
                    }

                }
                catch (Exception)
                {
                }
            }
        }

        private string fullName;
        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                NotifyOfPropertyChange(() => FullName);
            }
        }

        private string po;
        public string PO
        {
            get => po;
            set
            {
                po = value;
                NotifyOfPropertyChange(() => PO);
                if(!string.IsNullOrWhiteSpace(PO) && PO.Length == 12)
                {
                    ErrorMessage = "";
                    sPO.Add(PO);
                    mDictinaryEmployees.EmployeesInfo["PO"] = PO;
                }
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        private bool _IsIDFocused = true;
        public bool IsIDFocused { get => _IsIDFocused; set { _IsIDFocused = value; NotifyOfPropertyChange(() => IsIDFocused); } }

        private bool _IsPOFocused;
        public bool IsPOFocused { get => _IsPOFocused; set { _IsPOFocused = value; NotifyOfPropertyChange(() => IsPOFocused); } }

        public Dictionary<string, string> DEmployees = new Dictionary<string, string>();
        public List<string> sPO = new List<string>();

        private ExcelRW excelRW;

        public EmployeesViewModel(ref ExcelRW _excelRW)
        { 
            excelRW = _excelRW;

        }
    }
}
