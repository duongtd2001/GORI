using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GORI.Services.DataServices
{
    public class MachineData : FileRW
    {

        public string PLC_IP;
        public int _port;

        private string LinkSaveData;
        private string FileNameData;
        public string PathCombineData;
        public bool isSaveData;

        private string PathEmployees;
        private string FileNameEmployees;
        public string PathCombineEmployees;

        public string DataSource;
        public string InitialCatalog;
        public string PersistSecurityInfo;
        public string UserID;
        public string Password;
        public bool isSaveSQL;

        public MachineData() : base(Utils.DATAPATH, Utils.MACHINEDATA_FILENAME)
        {
        }

        public void ReadData(string _model = "")
        {
            string sVal;
            int iVal;
            bool bVal;
            var section = "Enet_Siemens";
            sVal = ReadValue(_model, section, "IP", "169.254.192.222");
            if (!string.IsNullOrEmpty(sVal)) PLC_IP = sVal;

            sVal = ReadValue(_model, section, "Port", "502");
            if(int.TryParse(sVal, out iVal)) _port = iVal;

            section = "PathData";
            sVal = ReadValue(_model, section, "Path", "C:\\Users\\duong\\Documents\\Misumi\\DefSystem\\Data");
            if(!string.IsNullOrEmpty(sVal)) LinkSaveData = sVal;

            sVal = ReadValue(_model, section, "FileName", "Data.xlsx");
            if (!string.IsNullOrEmpty(sVal)) FileNameData = sVal;

            sVal = ReadValue(_model, section, "SaveData", "false");
            if(bool.TryParse(sVal, out bVal)) isSaveData = bVal;
            PathCombineData = Path.Combine(LinkSaveData, FileNameData);

            section = "Employees";
            sVal = ReadValue(_model, section, "PathEmployees", "\\192.168.100.100\\05_OST_Program\\EMPLOYEES_OST");
            if(!string.IsNullOrEmpty(sVal)) PathEmployees = sVal;

            sVal = ReadValue(_model, section, "FileName", "Employees.xls");
            if(!string.IsNullOrEmpty(sVal)) FileNameEmployees = sVal;
            PathCombineEmployees = Path.Combine(PathEmployees, FileNameEmployees);

            section = "SQLServer";
            sVal = ReadValue(_model, section, "Data Source", "Data Source=192.168.100.100");
            if(!string.IsNullOrEmpty(sVal)) DataSource = $"Data Source={sVal}";

            sVal = ReadValue(_model, section, "Initial Catalog", "Initial Catalog=OST");
            if (!string.IsNullOrEmpty(sVal)) InitialCatalog = $"Initial Catalog={sVal}";

            sVal = ReadValue(_model, section, "Persist Security Info", "Persist Security Info=True");
            if (!string.IsNullOrEmpty(sVal)) PersistSecurityInfo = $"Persist Security Info={sVal}";

            sVal = ReadValue(_model, section, "UserID", "User ID=ost_pe");
            if (!string.IsNullOrEmpty(sVal)) UserID = $"User ID={sVal}";

            sVal = ReadValue(_model, section, "Password", "Password=ost_pe@spclt");
            if (!string.IsNullOrEmpty(sVal)) Password = $"Password={sVal}";

            sVal = ReadValue(_model, section, "SaveSQL", "false");
            if(bool.TryParse(sVal, out bVal)) isSaveSQL = bVal;
        }
    }
}
