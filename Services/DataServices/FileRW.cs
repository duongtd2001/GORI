using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GORI.Services.DataServices
{
    public class FileRW
    {
        private string path;

        private string fileName;

        private const int DEFAULT_SIZE = 255;

        private StringBuilder sbBuffer;

        private object obLock;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string strSection, string strKey, string strValue, string strFilePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string strSection, string strKey, string strDefault, StringBuilder retVal, int iSize, string strFilePath);

        public FileRW(string path, string fileName)
        {
            obLock = new object();
            this.path = path;
            this.fileName = fileName;
            sbBuffer = new StringBuilder(255);
        }

        public void WriteValue(string _model, string _section, string _key, object _iValue)
        {
            string path = ((!(_model == "")) ? Path.Combine(this.path, "Model") : this.path);
            if (!string.IsNullOrEmpty(_model))
            {
                path = Path.Combine(path, _model);
            }

            string strFilePath = Path.ChangeExtension(Path.Combine(path, fileName), "ini");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(strFilePath))
            {
                File.Create(strFilePath);
            }

            string strValue = _iValue?.ToString();
            lock (obLock)
            {
                WritePrivateProfileString(_section, _key, strValue, strFilePath);
            }
        }

        public string ReadValue(string _model, string _section, string _key, string _defaultValue)
        {
            string path = ((!(_model == "")) ? Path.Combine(this.path, "Model") : this.path);
            if (!string.IsNullOrEmpty(_model))
            {
                path = Path.Combine(path, _model);
            }

            string strFilePath = Path.ChangeExtension(Path.Combine(path, fileName), "ini");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(strFilePath))
            {
                File.Create(strFilePath);
            }

            sbBuffer.Clear();
            GetPrivateProfileString(_section, _key, _defaultValue, sbBuffer, 255, strFilePath);
            return sbBuffer.ToString();
        }

        public string ReadValue(string _model, string _section, string _key, string _defaultValue, string _dirPath, string _fileName)
        {
            string strFilePath = Path.ChangeExtension(Path.Combine(_dirPath, _fileName), "ini");
            if (!File.Exists(strFilePath))
            {
                File.Create(strFilePath);
            }

            sbBuffer.Clear();
            GetPrivateProfileString(_section, _key, _defaultValue, sbBuffer, 255, strFilePath);
            return sbBuffer.ToString();
        }
    }
}
