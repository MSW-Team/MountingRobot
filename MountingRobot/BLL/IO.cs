using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MountingRobot.BLL;
using MountingRobot.UI;
using MyLog;

namespace MountingRobot.BLL
{
    public class IO
    {
        #region IO读取
        /// <summary>
        /// PLC IO读取线程标志
        /// </summary>
        public bool PLCReceiveStart = false;
        /// <summary>
        /// PLC1轴状态读取线程标志(调宽轴)
        /// </summary>
        public bool AxisReceiveStart1 = false;
        /// <summary>
        /// PLC2轴状态读取线程标志(运行轴)
        /// </summary>
        public bool AxisReceiveStart2 = false;

        #region 循环读取PLC输入
        private bool[] listInput = new bool[30];
        public void ReadInput(int readInputCount)
        {
            listInput = new bool[readInputCount];
            Task.Run(() =>
            {
                while (PLCReceiveStart)
                {
                    listInput = FrmMain.smart200_1.ReadAllInput(readInputCount);
                    Thread.Sleep(100);
                }

            });
        }
        #endregion

        #region 循环读取PLC输出
        private bool[] listOutput = new bool[40];

        public void ReadOutput(int readOutputCount)
        {
            listOutput = new bool[readOutputCount];
            Task.Run(() =>
            {
                while (PLCReceiveStart)
                {
                    listOutput = FrmMain.smart200_1.ReadAllOutput(readOutputCount);
                    //buzzerClose = FrmMain.smart200_1.ReadBool("Q11.5");
                    //aAutoRun = FrmMain.smart200_1.ReadBool("Q10.0");
                    //bAutoRun = FrmMain.smart200_1.ReadBool("Q10.1");
                    //lackNFAlm1 = FrmMain.smart200_1.ReadBool("Q7.0");
                    //lackPFAlm1 = FrmMain.smart200_1.ReadBool("Q7.1");
                    //pickUpAPcbAlm = FrmMain.smart200_1.ReadBool("Q7.2");
                    //pickUpBPcbAlm = FrmMain.smart200_1.ReadBool("Q7.3");
                    //lackNFAlm2 = FrmMain.smart200_1.ReadBool("Q13.1");
                    //lackPFAlm2 = FrmMain.smart200_1.ReadBool("Q13.2");
                    //aIntoAlm = FrmMain.smart200_1.ReadBool("Q13.3");
                    //bIntoAlm = FrmMain.smart200_1.ReadBool("Q13.4");
                    //aStuckAlm = FrmMain.smart200_1.ReadBool("Q13.5");
                    //bStuckAlm = FrmMain.smart200_1.ReadBool("Q13.6");
                    Thread.Sleep(100);
                }
            });
        }
        #endregion

        #region 循环读取轴状态
        public void ReadPLC1Axis()
        {
            Task.Run(() =>
            {
                while(AxisReceiveStart1)
                {
                    aWidthStatus = FrmMain.smart200_1.ReadShort(Global.VW_AWidthStatus);
                    aWidthPositionActual = FrmMain.smart200_1.ReadFloat(Global.VD_AWidthPositionActual);
                    aWidthVelocityActual = FrmMain.smart200_1.ReadFloat(Global.VD_AWidthVelocityActual);
                    bWidthStatus = FrmMain.smart200_1.ReadShort(Global.VW_BWidthStatus);
                    bWidthPositionActual = FrmMain.smart200_1.ReadFloat(Global.VD_BWidthPositionActual);
                    bWidthVelocityActual = FrmMain.smart200_1.ReadFloat(Global.VD_BWidthVelocityActual);
                }
            });           
        }

        public void ReadPLC2Axis()
        {
            Task.Run(() =>
            {
                while (AxisReceiveStart2)
                {
                    aRunStatus = FrmMain.smart200_2.ReadShort(Global.VW_ARunStatus);
                    bRunStatus = FrmMain.smart200_2.ReadShort(Global.VW_BRunStatus);
                }
            });
        }

        #endregion

        #region 沿函数
        /// <summary>
        /// 上升沿数组
        /// </summary>
        private bool[] listPosedgeFlag = new bool[100];
        /// <summary>
        /// 上升沿指令
        /// </summary>
        /// <param name="Signal"></param>
        /// <param name="PosedgeFlag"></param>
        /// <returns></returns>
        public bool Posedge(bool Signal, ref bool PosedgeFlag)
        {
            if (!PosedgeFlag && Signal)
            {
                PosedgeFlag = true;
                return true;
            }
            else if (PosedgeFlag && !Signal)
            {
                PosedgeFlag = false;
                return false;
            }
            return false;
        }
        /// <summary>
        /// 下降沿指令
        /// </summary>
        /// <param name="Signal">信号点</param>
        /// <param name="NegedgeFlag">上升沿标志</param>
        /// <returns></returns>
        public bool Negedge(bool Signal, ref bool NegedgeFlag)
        {
            if (!NegedgeFlag && Signal)
            {
                NegedgeFlag = true;
                return false;
            }
            else if (NegedgeFlag && !Signal)
            {
                NegedgeFlag = false;
                return true;
            }
            return false;
        }
        #endregion

        #region 存储PLC输入属性
        /// <summary>
        /// A轨调宽原点I0.0
        /// </summary>
        public bool I_AWidthAxisOrigin
        {
            get { return listInput[0]; }
        }
        /// <summary>
        /// A轨调宽正限I0.1
        /// </summary>
        public bool I_AWidthAxisForwardLimit
        {
            get { return listInput[1]; }
        }
        /// <summary>
        /// 备用I0.2
        /// </summary>
        public bool I_Spare1
        {
            get { return listInput[2]; }
        }
        /// <summary>
        /// B轨调宽原点I0.3
        /// </summary>
        public bool I_BWidthAxisOrigin
        {
            get { return listInput[3]; }
        }
        /// <summary>
        /// B轨调宽正限I0.4
        /// </summary>
        public bool I_BWidthAxisForwardLimit
        {
            get { return listInput[4]; }
        }
        /// <summary>
        /// 备用I0.5
        /// </summary>
        public bool I_Spare2
        {
            get { return listInput[5]; }
        }
        /// <summary>
        /// 急停开关I0.6
        /// </summary>
        public bool I_EStop
        {
            get { return listInput[6]; }
        }
        /// <summary>
        /// 启动按钮I0.7
        /// </summary>
        public bool I_StartBtn
        {
            get { return listInput[7]; }
        }
        /// <summary>
        /// 停止按钮I1.0
        /// </summary>
        public bool I_StopBtn
        {
            get { return listInput[8]; }
        }
        /// <summary>
        /// 复位按钮I1.1
        /// </summary>
        public bool I_ResetBtn
        {
            get { return listInput[9]; }
        }
        /// <summary>
        /// 气压报警I1.2
        /// </summary>
        public bool I_AirAlarm
        {
            get { return listInput[10]; }
        }
        /// <summary>
        /// 备用I1.3
        /// </summary>
        public bool I_Spare3
        {
            get { return listInput[11]; }
        }
        /// <summary>
        /// A轨门限开关I1.4
        /// </summary>
        public bool I_ADoorSwitch
        {
            get
            {
                return listInput[12];
            }
        }
        /// <summary>
        /// A轨入口光电I1.5
        /// </summary>
        public bool I_AInSensors
        {
            get { return listInput[13]; }
        }
        /// <summary>
        /// A轨到位光电I1.6
        /// </summary>
        public bool I_AHaveSensors
        {
            get { return listInput[14]; }
        }
        /// <summary>
        /// A轨出口光电I1.7
        /// </summary>
        public bool I_AOutSensors
        {
            get { return listInput[15]; }
        }
        /// <summary>
        /// A轨振动盘满料I2.0
        /// </summary>
        public bool I_AVibrationFull
        {
            get { return listInput[16]; }
        }
        /// <summary>
        /// A轨直振有料I2.1
        /// </summary>
        public bool I_AVibrationHave
        {
            get { return listInput[17]; }
        }
        /// <summary>
        /// A轨上位机有板I2.2
        /// </summary>
        public bool I_AUpperComputerHave
        {
            get { return listInput[18]; }
        }
        /// <summary>
        /// A轨下位机求板信号I2.3
        /// </summary>
        public bool I_ALowerComputerRequest
        {
            get { return listInput[19]; }
        }
        /// <summary>
        /// 备用I2.4
        /// </summary>
        public bool I_Spare4
        {
            get { return listInput[20]; }
        }
        /// <summary>
        /// 备用I2.5
        /// </summary>
        public bool I_Spare5
        {
            get { return listInput[21]; }
        }
        /// <summary>
        /// B轨门限开关I2.6
        /// </summary>
        public bool I_BDoorSwitch
        {
            get { return listInput[22]; }
        }
        /// <summary>
        /// B轨进板光电I2.7
        /// </summary>
        public bool I_BInSensors
        {
            get { return listInput[23]; }
        }
        /// <summary>
        /// B轨中间光电I3.0
        /// </summary>
        public bool I_BHaveSensors
        {
            get { return listInput[24]; }
        }
        /// <summary>
        /// B轨出板光电I3.1
        /// </summary>
        public bool I_BOutSensors
        {
            get { return listInput[25]; }
        }
        /// <summary>
        /// B轨振动盘满料I3.2
        /// </summary>
        public bool I_BVibrationFull
        {
            get { return listInput[26]; }
        }
        /// <summary>
        /// B轨直振有料I3.3
        /// </summary>
        public bool I_BVibrationHave
        {
            get { return listInput[27]; }
        }
        /// <summary>
        /// B轨上位机有板信号I3.4
        /// </summary>
        public bool I_BUpperComputerHave
        {
            get { return listInput[28]; }
        }
        /// <summary>
        /// B轨下位机求板信号I3.5
        /// </summary>
        public bool I_BLowerComputerRequest
        {
            get { return listInput[29]; }
        }
        /// <summary>
        /// 备用I3.6
        /// </summary>
        public bool I_Spare6
        {
            get { return listInput[30]; }
        }
        /// <summary>
        /// 机器人报警
        /// </summary>
        public bool I_RobotAlarm
        {
            get { return listInput[31]; }
        }
        #endregion

        #region 存储PLC输出属性
        /// <summary>
        /// A轨调宽脉冲Q0.0
        /// </summary>
        public bool Q_AWidthAxisPulse
        {
            get { return listOutput[0]; }
        }
        /// <summary>
        /// B轨调宽脉冲Q0.1
        /// </summary>
        public bool Q_BWidthAxisPulse
        {
            get { return listOutput[1]; }
        }
        /// <summary>
        /// A轨调宽方向Q0.2
        /// </summary>
        public bool Q_AWidthAxisDirection
        {
            get { return listOutput[2]; }
        }
        /// <summary>
        /// 备用Q0.3
        /// </summary>
        public bool Q_Spare1
        {
            get { return listOutput[3]; }
        }
        /// <summary>
        /// 启动按钮灯Q0.4
        /// </summary>
        public bool Q_StartBtnLamp
        {
            get { return listOutput[4]; }
        }
        /// <summary>
        /// 复位按钮灯Q0.5
        /// </summary>
        public bool Q_ResetBtnLamp
        {
            get { return listOutput[5]; }
        }
        /// <summary>
        /// 振动盘电源启动Q0.6
        /// </summary>
        public bool Q_VibrationPower
        {
            get { return listOutput[6]; }
        }
        /// <summary>
        /// B轨调宽方向Q0.7
        /// </summary>
        public bool Q_BWidthAxisDirection
        {
            get { return listOutput[7]; }
        }
        /// <summary>
        /// 备用1.0
        /// </summary>
        public bool Q_Spare2
        {
            get { return listOutput[8]; }
        }
        /// <summary>
        /// 三色灯绿Q1.1
        /// </summary>
        public bool Q_LampGreen
        {
            get { return listOutput[9]; }
        }
        /// <summary>
        /// 三色灯黄Q1.2
        /// </summary>
        public bool Q_LampYellow
        {
            get { return listOutput[10]; }
        }
        /// <summary>
        /// 三色灯红Q1.3
        /// </summary>
        public bool Q_LampRed
        {
            get { return listOutput[11]; }
        }
        /// <summary>
        /// 蜂鸣器Q1.4
        /// </summary>
        public bool Q_Buzzer
        {
            get { return listOutput[12]; }
        }
        /// <summary>
        /// A轨托板气缸Q1.5
        /// </summary>
        public bool Q_ARiseCylinder
        {
            get { return listOutput[13]; }
        }
        /// <summary>
        /// A轨阻挡气缸Q1.6
        /// </summary>
        public bool Q_AStopCylinder
        {
            get { return listOutput[14]; }
        }
        /// <summary>
        /// A轨向上位机求板Q1.7
        /// </summary>
        public bool Q_AUpperComputerRequest
        {
            get { return listOutput[15]; }
        }
        /// <summary>
        /// A轨给下位机有板Q2.0
        /// </summary>
        public bool Q_ALowerComputerHave
        {
            get { return listOutput[16]; }
        }
        /// <summary>
        /// B轨托板气缸Q2.1
        /// </summary>
        public bool Q_BRiseCylinder
        {
            get { return listOutput[17]; }
        }
        /// <summary>
        /// B轨阻挡气缸Q2.2
        /// </summary>
        public bool Q_BStopCylinder
        {
            get { return listOutput[18]; }
        }
        /// <summary>
        /// B轨向上位机求板Q2.3
        /// </summary>
        public bool Q_BUpperComputerRequest
        {
            get { return listOutput[19]; }
        }
        /// <summary>
        /// B轨给下位机有板Q2.4
        /// </summary>
        public bool Q_BLowerComputerHave
        {
            get { return listOutput[20]; }
        }
        /// <summary>
        /// 备用Q2.5
        /// </summary>
        public bool Q_Spare3
        {
            get { return listOutput[21]; }
        }
        /// <summary>
        /// 机器人暂停Q2.6
        /// </summary>
        public bool Q_RobotPause
        {
            get { return listOutput[22]; }
        }
        /// <summary>
        /// 机器人复位
        /// </summary>
        public bool Q_RobotReset
        {
            get { return listOutput[23]; }
        }




        ///// <summary>
        ///// A皮带马达启动Q0.2
        ///// </summary>
        //public bool Q_AMotor
        //{
        //    get { return listOutput[2]; }
        //}

        ///// <summary>
        ///// B皮带马达启动Q0.3
        ///// </summary>
        //public bool Q_BMotor
        //{
        //    get { return listOutput[3]; }
        //}

        ///// <summary>
        ///// 电源座直震与吹气Q1.7
        ///// </summary>
        //public bool Q_PFBlow
        //{
        //    get { return listOutput[15]; }
        //}

        ///// <summary>
        ///// 排针座直震与吹气Q2.0
        ///// </summary>
        //public bool Q_NFBlow
        //{
        //    get { return listOutput[16]; }
        //}



        ///// <summary>
        ///// 日光灯Q2.2
        ///// </summary>
        //public bool Q_Lamp
        //{
        //    get { return listOutput[18]; }
        //}

        ///// <summary>
        ///// A进板信号给机器人Q2.3
        ///// </summary>
        //public bool Q_AInToRobot
        //{
        //    get { return listOutput[19]; }
        //}

        ///// <summary>
        ///// B进板信号给机器人Q2.4
        ///// </summary>
        //public bool Q_BInToRobot
        //{
        //    get { return listOutput[20]; }
        //}

        ///// <summary>
        ///// 给机器人A到位信号Q2.5
        ///// </summary>
        //public bool Q_AArriveSignal
        //{
        //    get { return listOutput[21]; }
        //}

        ///// <summary>
        ///// 给机器人B到位信号Q2.6
        ///// </summary>
        //public bool Q_BArriveSignal
        //{
        //    get { return listOutput[22]; }
        //}

        ///// <summary>
        ///// 给机器人电源座有料Q2.7
        ///// </summary>
        //public bool Q_PFFullToRobot
        //{
        //    get { return listOutput[23]; }
        //}

        ///// <summary>
        ///// 给机器人排针座有料Q8.0
        ///// </summary>
        //public bool Q_NFFullToRobot
        //{
        //    get { return listOutput[24]; }
        //}

        ///// <summary>
        ///// 给机器人启动信号Q8.1
        ///// </summary>
        //public bool Q_RobotStart
        //{
        //    get { return listOutput[25]; }
        //}

        ///// <summary>
        ///// 给机器人复位信号Q8.2
        ///// </summary>
        //public bool Q_RobotReset
        //{
        //    get { return listOutput[26]; }
        //}

        ///// <summary>
        ///// 给机器人停止信号Q8.3
        ///// </summary>
        //public bool Q_RobotStop
        //{
        //    get { return listOutput[27]; }
        //}

        ///// <summary>
        ///// 蜂鸣器消声按钮Q11.5
        ///// </summary>
        //public bool Q_BuzzerClose
        //{
        //    get { return buzzerClose; }
        //}

        ///// <summary>
        ///// A轨自动运行与否Q10.0
        ///// </summary>
        //public bool Q_AAutoRun
        //{
        //    get { return aAutoRun; }
        //}

        ///// <summary>
        ///// B轨自动运行与否Q10.1
        ///// </summary>
        //public bool Q_BAutoRun
        //{
        //    get { return bAutoRun; }
        //}  

        #endregion

        #region 存储PLC轴状态
        /// <summary>
        /// A轨调宽轴状态
        /// </summary>
        private short aWidthStatus;
        /// <summary>
        /// A轨调宽轴状态
        /// </summary>
        public short AWidthStatus
        {
            get { return aWidthStatus; }
        }
        /// <summary>
        /// A轨调宽实际轴位置
        /// </summary>
        private float aWidthPositionActual;
        /// <summary>
        /// A轨调宽实际轴位置
        /// </summary>
        public float AWidthPositionActual
        {
            get { return aWidthPositionActual; }
        }
        /// <summary>
        /// A轨调宽实际速度
        /// </summary>
        private float aWidthVelocityActual;
        /// <summary>
        /// A轨调宽实际速度
        /// </summary>
        public float AWidthVelocityActual
        {
            get { return aWidthVelocityActual; }
        }
        /// <summary>
        /// B轨调宽轴状态
        /// </summary>
        private short bWidthStatus;
        /// <summary>
        /// A轨调宽轴状态
        /// </summary>
        public short BWidthStatus
        {
            get { return bWidthStatus; }
        }
        /// <summary>
        /// B轨调宽实际轴位置
        /// </summary>
        private float bWidthPositionActual;
        /// <summary>
        /// B轨调宽实际轴位置
        /// </summary>
        public float BWidthPositionActual
        {
            get { return bWidthPositionActual; }
        }
        /// <summary>
        /// B轨调宽实际速度
        /// </summary>
        private float bWidthVelocityActual;
        /// <summary>
        /// B轨调宽实际速度
        /// </summary>
        public float BWidthVelocityActual
        {
            get { return bWidthVelocityActual; }
        }
        /// <summary>
        /// A轨运行轴状态
        /// </summary>
        private short aRunStatus;
        /// <summary>
        /// A轨运行轴状态
        /// </summary>
        public short ARunStatus
        {
            get { return aRunStatus; }
        }
        /// <summary>
        /// B轨运行轴状态
        /// </summary>
        private short bRunStatus;
        /// <summary>
        /// B轨运行轴状态
        /// </summary>
        public short BRunStatus
        {
            get { return bRunStatus; }
        }
        #endregion

        #region PLC报警变量声明
        private bool lackNFAlm1 = false;  //排针座直震卡料或缺料料报警V5029.2
        private bool aIntoAlm = false;  //A轨进板异常V5029.3
        private bool bIntoAlm = false;  //B轨进板异常Q13.4




        private bool lackPFAlm1 = false;  //电源座直震卡料或缺料料报警Q7.1
        private bool pickUpAPcbAlm = false;  //A轨贴装位PCB板被拿走Q7.2
        private bool pickUpBPcbAlm = false;  //B轨贴装位PCB板被拿走Q7.3
        private bool lackNFAlm2 = false;  //排针座震动盘缺料或卡料报警Q13.1
        private bool lackPFAlm2 = false;  //电源座震动盘缺料或卡料报警Q13.2


        private bool aStuckAlm = false;  //A轨卡板Q13.5
        private bool bStuckAlm = false;  //B轨卡板Q13.6

        /// <summary>
        /// 报警日志记录函数
        /// </summary>
        /// <param name="AlarmVar">报警变量</param>
        /// <param name="Flag">上升沿记录标志</param>
        /// <param name="AlarmText">记录信息</param>
        private void WriteAlarmLog(bool AlarmVar, ref bool Flag, string AlarmText)
        {
            if (Posedge(AlarmVar, ref Flag))
            {
                Common.myLog.writeAlmContent(AlarmText, Global.UserPermission, Global.UserPermission);
            }
        }

        /// <summary>
        /// 排针座震动盘缺料或卡料报警Q13.1
        /// </summary>
        public bool Q_LackNFAlm2
        {
            get
            {
                WriteAlarmLog(lackNFAlm2, ref listPosedgeFlag[0], "排针座震动盘缺料或卡料");
                return lackNFAlm2;
            }
        }
        /// <summary>
        /// 电源座震动盘缺料或卡料报警Q13.2
        /// </summary>
        public bool Q_LackPFAlm2
        {
            get { return lackPFAlm2; }
        }
        /// <summary>
        /// A轨进板异常
        /// </summary>
        public bool Q_AIntoAlm
        {
            get { return aIntoAlm; }
        }
        /// <summary>
        /// B轨进板异常
        /// </summary>
        public bool Q_BIntoAlm
        {
            get { return bIntoAlm; }
        }
        /// <summary>
        /// A轨卡板
        /// </summary>
        public bool Q_AStuckAlm
        {
            get { return aStuckAlm; }
        }
        /// <summary>
        /// B轨卡板
        /// </summary>
        public bool Q_BStuckAlm
        {
            get { return bStuckAlm; }
        }
        /// <summary>
        /// 排针座直震卡料或缺料料报警
        /// </summary>
        public bool Q_LackNFAlm1
        {
            get { return lackNFAlm1; }
        }
        /// <summary>
        /// 电源座直震卡料或缺料料报警
        /// </summary>
        public bool Q_LackPFAlm1
        {
            get { return lackPFAlm1; }
        }
        /// <summary>
        /// A轨贴装位PCB板被拿走
        /// </summary>
        public bool Q_PickUpAPCBAlm
        {
            get { return pickUpAPcbAlm; }
        }
        /// <summary>
        /// B轨贴装位PCB板被拿走
        /// </summary>
        public bool Q_PickUpBPCBAlm
        {
            get { return pickUpBPcbAlm; }
        }
        #endregion      

        #region 机器人交互点声明
        /// <summary>
        /// A轨贴装完成
        /// </summary>
        public bool R_AFinnish;
        /// <summary>
        /// B轨贴装完成
        /// </summary>
        public bool R_BFinnish;
        /// <summary>
        /// A轨本地坐标生成成功
        /// </summary>
        public bool ALocalCreatSuccess;
        /// <summary>
        /// B轨本地坐标生成成功
        /// </summary>
        public bool BLocalCreatSuccess;
        /// <summary>
        /// 视觉放料点生成成功
        /// </summary>
        public bool PointCreatSuccess;
        /// <summary>
        /// 工具坐标生成成功
        /// </summary>
        public bool ToolCreatSuccess;
        /// <summary>
        /// 工具坐标生成失败
        /// </summary>
        public bool ToolCreatFail;
        /// <summary>
        /// A爪下限位检测超时
        /// </summary>
        public bool R_G1DLTimeOut;
        /// <summary>
        /// B爪下限位检测超时
        /// </summary>
        public bool R_G2DLTimeOut;
        /// <summary>
        /// C爪下限位检测超时
        /// </summary>
        public bool R_G3DLTimeOut;
        /// <summary>
        /// D爪下限位检测超时
        /// </summary>
        public bool R_G4DLTimeOut;
        /// <summary>
        /// A爪上限位检测超时
        /// </summary>
        public bool R_G1ULTimeOut;
        /// <summary>
        /// B爪上限位检测超时
        /// </summary>
        public bool R_G2ULTimeOut;
        /// <summary>
        /// C爪上限位检测超时
        /// </summary>
        public bool R_G3ULTimeOut;
        /// <summary>
        /// D爪上限位检测超时
        /// </summary>
        public bool R_G4ULTimeOut;
        /// <summary>
        /// PCB相机A轨mark点1NG
        /// </summary>
        public bool R_CamPCBA01NG;
        /// <summary>
        /// PCB相机A轨mark点2NG
        /// </summary>
        public bool R_CamPCBA02NG;
        /// <summary>
        /// PCB相机B轨mark点1NG
        /// </summary>
        public bool R_CamPCBB01NG;
        /// <summary>
        /// PCB相机B轨mark点2NG
        /// </summary>
        public bool R_CamPCBB02NG;
        /// <summary>
        /// PCB相机A轨mark点1视觉超出范围
        /// </summary>
        public bool R_CamPCBA01Out;
        /// <summary>
        /// PCB相机A轨mark点2视觉超出范围
        /// </summary>
        public bool R_CamPCBA02Out;
        /// <summary>
        /// PCB相机B轨mark点1视觉超出范围
        /// </summary>
        public bool R_CamPCBB01Out;
        /// <summary>
        /// PCB相机B轨mark点2视觉超出范围
        /// </summary>
        public bool R_CamPCBB02Out;
        /// <summary>
        /// 夹爪相机物料1NG
        /// </summary>
        public bool R_CamMM1NG;
        /// <summary>
        /// 夹爪相机物料1NG
        /// </summary>
        public bool R_CamMM2NG;
        /// <summary>
        /// 夹爪相机物料1NG
        /// </summary>
        public bool R_CamMM3NG;
        /// <summary>
        /// 夹爪相机物料1NG
        /// </summary>
        public bool R_CamMM4NG;
        #endregion

        #region 循环读取机器人交互点
        /// <summary>
        /// 机器人读取线程启用标志
        /// </summary>
        public bool RobotReceiveStart = false;
        /// <summary>
        /// 机器人状态读取线程
        /// </summary>
        public void ReadRobotStatus()
        {
            Task.Run(() =>
            {
                while (RobotReceiveStart)
                {
                    //交互标志位
                    readRobotMemSW(ref R_AFinnish, "A_Finnish");
                    if (!RobotReceiveStart) { break; }//标志位清除后，退出循环避免API多次报错

                    readRobotMemSW(ref R_BFinnish, "B_Finnish");
                    if (!RobotReceiveStart) { break; }

                    readRobotMemSW(ref ALocalCreatSuccess, "ALocalCreatSuccess");
                    if (!RobotReceiveStart) { break; }

                    readRobotMemSW(ref BLocalCreatSuccess, "BLocalCreatSuccess");
                    if (!RobotReceiveStart) { break; }

                    readRobotMemSW(ref PointCreatSuccess, "PointCreatSuccess");
                    if (!RobotReceiveStart) { break; }

                    readRobotMemSW(ref ToolCreatSuccess, "ToolCreatSuccess");
                    if (!RobotReceiveStart) { break; }

                    readRobotMemSW(ref ToolCreatFail, "ToolCreatFail");
                    if (!RobotReceiveStart) { break; }

                    //报警
                    if (readRobotMemSW(ref R_G1DLTimeOut, "G1DLTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人A爪下磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G2DLTimeOut, "G2DLTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人B爪下磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G3DLTimeOut, "G3DLTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人C爪下磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G4DLTimeOut, "G4DLTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人D爪下磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G1ULTimeOut, "G1ULTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人A爪上磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G2ULTimeOut, "G2ULTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人B爪上磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G3ULTimeOut, "G3ULTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人C爪上磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_G4ULTimeOut, "G4ULTimeOut"))
                    {
                        Common.myLog.writeAlmContent("机器人D爪上磁感检测超时", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBA01NG, "CamPCBA01NG"))
                    {
                        Common.myLog.writeAlmContent("PCB相机A轨mark点1NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBA02NG, "CamPCBA02NG"))
                    {
                        Common.myLog.writeAlmContent("PCB相机A轨mark点2NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBB01NG, "CamPCBB01NG"))
                    {
                        Common.myLog.writeAlmContent("PCB相机B轨mark点1NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBB02NG, "CamPCBB02NG"))
                    {
                        Common.myLog.writeAlmContent("PCB相机B轨mark点2NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBA01Out, "CamPCBA01Out"))
                    {
                        Common.myLog.writeAlmContent("PCB相机A轨mark点1视觉超出范围", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBA02Out, "CamPCBA02Out"))
                    {
                        Common.myLog.writeAlmContent("PCB相机A轨mark点2视觉超出范围", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBB01Out, "CamPCBB01Out"))
                    {
                        Common.myLog.writeAlmContent("PCB相机B轨mark点1视觉超出范围", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamPCBB02Out, "CamPCBB02Out"))
                    {
                        Common.myLog.writeAlmContent("PCB相机B轨mark点2视觉超出范围", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamMM1NG, "CamMM1NG"))
                    {
                        Common.myLog.writeAlmContent("物料相机拍摄物料1NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamMM2NG, "CamMM2NG"))
                    {
                        Common.myLog.writeAlmContent("物料相机拍摄物料2NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamMM3NG, "CamMM3NG"))
                    {
                        Common.myLog.writeAlmContent("物料相机拍摄物料3NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }

                    if (readRobotMemSW(ref R_CamMM4NG, "CamMM4NG"))
                    {
                        Common.myLog.writeAlmContent("物料相机拍摄物料4NG", Global.UserPermission, Global.UserPermission);
                    }
                    if (!RobotReceiveStart) { break; }
                    Thread.Sleep(50);

                }
            });
        }
        /// <summary>
        /// 读取机器人内存位状态函数
        /// </summary>
        /// <param name="Var"></param>
        /// <param name="LabelName"></param>
        private bool readRobotMemSW(ref bool Var, string LabelName)
        {
            if (FrmIniRobot.RobotSpel.MemSw(LabelName))//读取机器人内存点位情况
            {
                Var = true;
                FrmIniRobot.RobotSpel.MemOff(LabelName);//读取完毕后，清除机器人内存点位
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #endregion
    }
}
