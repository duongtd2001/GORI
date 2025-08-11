using System;
using System.Collections.Generic;
using System.Threading;
using GORI.Models;
using GORI.Services.Communication;
using GORI.Services.DataServices;

namespace GORI.Services.Process
{
    public class Process_Torque
    {

        private enum ProcessStatus
        {
            INITIALIZE,
            CHECK_PLC_SIGNAL,
            START_DRAWING,
            PREDICT_DATA,
            RESULT_OK,
            RESULT_NG,
            SAVE_DATA,
            WAIT_COM_ACK
        }

        private enum PredictResult
        {
            NONE, 
            CASE_A,
            CASE_B,
            CASE_C,
            CASE_D,
            CASE_E,
            CASE_F,
            CASE_G,
            CASE_H,
        }

        private ProcessStatus mProcessStatus = ProcessStatus.INITIALIZE;

        private Thread mMainThread;
        private Thread mMainThread2;
        private bool mThreadFlag = false;
        private bool mThreadFlag2 = false;

        public Queue<double> queue = new Queue<double>();
        private Queue<double> mData = new Queue<double>();

        private ModbusComm PLCComm;
        private ExcelRW excelRW;

        public Process_Torque(ref ModbusComm _PLCComm, ref ExcelRW excel)
        {
            PLCComm = _PLCComm;
            excelRW = excel;
        }

        public void ThreadRun()
        {
            if (mThreadFlag) return;

            if (mMainThread != null)
            {
                mMainThread.Join(500);
                mMainThread.Abort();
                mMainThread = null;
            }
            mMainThread = new Thread(() =>
            {
                DoRunProcess();
            });
            mMainThread.SetApartmentState(ApartmentState.STA);
            mMainThread.Priority = ThreadPriority.Highest;
            mMainThread.IsBackground = true;
            mThreadFlag = true;

            mMainThread.Start();
        }

        public void ThreadStop()
        {
            mThreadFlag = false;

            if (mMainThread != null)
            {
                mMainThread.Join(500);
                mMainThread.Abort();
                mMainThread = null;
            }
        }

        private void DoRunProcess()
        {
            mProcessStatus = ProcessStatus.INITIALIZE;
            List<int> triggerArr = new List<int>();
            List<double> templist = new List<double>();
            queue.Clear();
            mData.Clear();
            //sPlotView.isPlot = true;
            while (mThreadFlag)
            {
                //GC.Collect();
                //Thread.Sleep(10);
                try
                {
                    switch(mProcessStatus)
                    {
                        case ProcessStatus.INITIALIZE:
                            if (PLCComm.IsModbus())
                            {
                                mProcessStatus = ProcessStatus.CHECK_PLC_SIGNAL;
                            }
                            break;
                        case ProcessStatus.CHECK_PLC_SIGNAL:
                            triggerArr = PLCComm.ReadMutilHoldingInt(4, 3);

                            if (triggerArr[0] == 0 || triggerArr[2] == 0)
                            {
                                break;
                            }
                            mProcessStatus = ProcessStatus.START_DRAWING;
                            break;
                        case ProcessStatus.START_DRAWING:
                            templist = PLCComm.ReadMutilHoldingReal(0, 2);
                            queue.Enqueue(templist[0]);
                            mData.Enqueue(templist[0]);
                            if(mData.Count >= 1000)
                            {
                                mProcessStatus = ProcessStatus.SAVE_DATA;
                            }
                            break;
                        case ProcessStatus.PREDICT_DATA:
                            
                            break;
                        case ProcessStatus.RESULT_OK:
                            break;
                        case ProcessStatus.RESULT_NG:
                            break;
                        case ProcessStatus.SAVE_DATA:
                            excelRW.AddColumn(templist[0].ToString());
                            excelRW.SaveData(mData);
                            mProcessStatus = ProcessStatus.WAIT_COM_ACK;
                            break;
                        case ProcessStatus.WAIT_COM_ACK:
                            mProcessStatus = ProcessStatus.INITIALIZE;
                            break;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
