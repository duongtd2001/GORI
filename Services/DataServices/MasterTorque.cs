using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GORI.Services.DataServices
{
    public class MasterTorque : FileRW
    {
        public double LowerA, LowerB, LowerC, LowerD, LowerE, LowerF, LowerG, LowerH;
        public double UpperA, UpperB, UpperC, UpperD, UpperE, UpperF, UpperG, UpperH;

        public List<(double Lower, double Upper)> values = new List<(double Lower, double Upper)> ();
        public List<string> caseNames = new List<string> ();

        public MasterTorque() : base(Utils.DATAPATH, Utils.TORQUEDATA_FILENAME)
        {
        }

        public void ReadData(string _model = "")
        {
            string sVal;
            double dVal;

            var section = "CASE_A";
            sVal = ReadValue(_model, section, "Lower", "0.01");
            if (double.TryParse(sVal, out dVal)) LowerA = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.03");
            if (double.TryParse(sVal, out dVal)) UpperA = dVal;
            values.Add((LowerA, UpperA));
            caseNames.Add(section);

            section = "CASE_B";
            sVal = ReadValue(_model, section, "Lower", "0.03");
            if (double.TryParse(sVal, out dVal)) LowerB = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.05");
            if (double.TryParse(sVal, out dVal)) UpperB = dVal;
            values.Add((LowerB, UpperB));
            caseNames.Add(section);

            section = "CASE_C";
            sVal = ReadValue(_model, section, "Lower", "0.05");
            if (double.TryParse(sVal, out dVal)) LowerC = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.07");
            if (double.TryParse(sVal, out dVal)) UpperC = dVal;
            values.Add((LowerC, UpperC));
            caseNames.Add(section);

            section = "CASE_D";
            sVal = ReadValue(_model, section, "Lower", "0.07");
            if (double.TryParse(sVal, out dVal)) LowerD = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.09");
            if (double.TryParse(sVal, out dVal)) UpperD = dVal;
            values.Add((LowerD, UpperD));
            caseNames.Add(section);

            section = "CASE_E";
            sVal = ReadValue(_model, section, "Lower", "0.09");
            if (double.TryParse(sVal, out dVal)) LowerE = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.11");
            if (double.TryParse(sVal, out dVal)) UpperE = dVal;
            values.Add((LowerE, UpperE));
            caseNames.Add(section);

            section = "CASE_F";
            sVal = ReadValue(_model, section, "Lower", "0.11");
            if (double.TryParse(sVal, out dVal)) LowerF = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.13");
            if (double.TryParse(sVal, out dVal)) UpperF = dVal;
            values.Add((LowerF, UpperF));
            caseNames.Add(section);

            section = "CASE_G";
            sVal = ReadValue(_model, section, "Lower", "0.13");
            if (double.TryParse(sVal, out dVal)) LowerG = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.15");
            if (double.TryParse(sVal, out dVal)) UpperG = dVal;
            values.Add((LowerG, UpperG));
            caseNames.Add(section);

            section = "CASE_H";
            sVal = ReadValue(_model, section, "Lower", "0.15");
            if (double.TryParse(sVal, out dVal)) LowerH = dVal;

            sVal = ReadValue(_model, section, "Upper", "0.17");
            if (double.TryParse(sVal, out dVal)) UpperH = dVal;
            values.Add((LowerH, UpperH));
            caseNames.Add(section);
        }
    }
}
