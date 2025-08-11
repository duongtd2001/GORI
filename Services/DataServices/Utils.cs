using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GORI.Services.DataServices
{
    public class Utils
    {
        public static string DATAPATH = Path.Combine("C:\\Users\\duong\\Documents\\Misumi", "DefSystem", "System");
        public static string MACHINEDATA_FILENAME = "MachineData.ini";
        public static string SYSTEMDATA_FILENAME = "System.ini";
        public static string VISIONDATA_FILENAME = "VisionJob.ini";
        public static string TORQUEDATA_FILENAME = "TORQUE.ini";
        public static string FOLDER_LOG = Path.Combine("C:\\Users\\duong\\Documents\\D\\D_VISION_LIBRARY", "VISION_LIBRARY", "Log");
        public static string FOLDER_DATA = Path.Combine("C:\\Users\\duong\\Documents\\D\\D_VISION_LIBRARY", "VISION_LIBRARY", "Data");
    }
}
