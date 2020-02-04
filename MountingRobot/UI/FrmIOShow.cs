using MountingRobot.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MountingRobot.UI
{
    public partial class FrmIOShow : Form
    {
        /// <summary>
        /// IO刷新线程标志
        /// </summary>
        private bool IOShowStart;

        public FrmIOShow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// IO显示刷新线程
        /// </summary>
        private void RefrshControlsThread()
        {
            if(Global.ConnectSuccess1)
            {
                IOShowStart = true;

                Task.Run(()=> 
                {
                    while(IOShowStart)
                    {
                        #region PLC输入点
                        //IB0
                        setLblColor(FrmMain.IO.I_AWidthAxisOrigin, lblAWidthAxisOrigin);
                        setLblColor(FrmMain.IO.I_AWidthAxisForwardLimit, lblAWidthAxisForwardLimit);
                        setLblColor(FrmMain.IO.I_Spare1, lblISpare1);
                        setLblColor(FrmMain.IO.I_BWidthAxisOrigin, lblBWidthAxisOrigin);
                        setLblColor(FrmMain.IO.I_BWidthAxisForwardLimit, lblBWidthAxisForwardLimit);
                        setLblColor(FrmMain.IO.I_Spare2, lblISpare2);
                        setLblColor(FrmMain.IO.I_EStop, lblEStop);
                        setLblColor(FrmMain.IO.I_StartBtn, lblStartBtn);
                        //IB1
                        setLblColor(FrmMain.IO.I_StopBtn, lblStopBtn);
                        setLblColor(FrmMain.IO.I_ResetBtn, lblResetBtn);
                        setLblColor(FrmMain.IO.I_AirAlarm, lblAirAlarm);
                        setLblColor(FrmMain.IO.I_Spare3, lblISpare3);
                        setLblColor(FrmMain.IO.I_ADoorSwitch, lblADoorSwitch);
                        setLblColor(FrmMain.IO.I_AInSensors, lblAInSensors);
                        setLblColor(FrmMain.IO.I_AHaveSensors, lblAHaveSensors);
                        setLblColor(FrmMain.IO.I_AOutSensors, lblAOutSensors);
                        //IB2
                        setLblColor(FrmMain.IO.I_AVibrationFull, lblAVibrationFull);
                        setLblColor(FrmMain.IO.I_AVibrationHave, lblAVibrationHave);
                        setLblColor(FrmMain.IO.I_AUpperComputerHave, lblAUpperComputerHave);
                        setLblColor(FrmMain.IO.I_ALowerComputerRequest, lblALowerComputerRequest);
                        setLblColor(FrmMain.IO.I_Spare4, lblISpare4);
                        setLblColor(FrmMain.IO.I_Spare5, lblISpare5);
                        setLblColor(FrmMain.IO.I_BDoorSwitch, lblBDoorSwitch);
                        setLblColor(FrmMain.IO.I_BInSensors, lblBInSensors);
                        //IB3
                        setLblColor(FrmMain.IO.I_BHaveSensors, lblBInSensors);
                        setLblColor(FrmMain.IO.I_BOutSensors, lblBOutSensors);
                        setLblColor(FrmMain.IO.I_BVibrationFull, lblBVibrationFull);
                        setLblColor(FrmMain.IO.I_BVibrationHave, lblBVivrationHave);
                        setLblColor(FrmMain.IO.I_BUpperComputerHave, lblBUpperComputerHave);
                        setLblColor(FrmMain.IO.I_BLowerComputerRequest, lblBLowerComputerRequest);
                        setLblColor(FrmMain.IO.I_Spare6, lblISpare6);
                        setLblColor(FrmMain.IO.I_RobotAlarm, lblRobotAlarm);
                        #endregion

                        #region PLC输出点
                        //QB0
                        setLblColor(FrmMain.IO.Q_AWidthAxisPulse, lblAWidthAxisPulse);
                        setLblColor(FrmMain.IO.Q_BWidthAxisPulse, lblBWidthAxisPulse);
                        setLblColor(FrmMain.IO.Q_AWidthAxisDirection, lblBWidthAxisDirection);
                        setLblColor(FrmMain.IO.Q_Spare1, lblQSpare1);
                        setLblColor(FrmMain.IO.Q_StartBtnLamp, lblStartBenLamp);
                        setLblColor(FrmMain.IO.Q_ResetBtnLamp, lblResetBtnLamp);
                        setLblColor(FrmMain.IO.Q_VibrationPower, lblVibrationPower);
                        setLblColor(FrmMain.IO.Q_BWidthAxisDirection, lblBWidthAxisDirection);
                        //QB1
                        setLblColor(FrmMain.IO.Q_Spare2, lblQSpare1);
                        setLblColor(FrmMain.IO.Q_LampGreen, lblLampGreen);
                        setLblColor(FrmMain.IO.Q_LampYellow, lblLampYellow);
                        setLblColor(FrmMain.IO.Q_LampRed, lblLampRed);
                        setLblColor(FrmMain.IO.Q_Buzzer, lblBuzzer);
                        setLblColor(FrmMain.IO.Q_ARiseCylinder, lblARiseCylinder);
                        setLblColor(FrmMain.IO.Q_AStopCylinder, lblAStopCylinder);
                        setLblColor(FrmMain.IO.Q_AUpperComputerRequest, lblAUpperComputerRequest);
                        //QB2
                        setLblColor(FrmMain.IO.Q_ALowerComputerHave, lblALowerComputerHave);
                        setLblColor(FrmMain.IO.Q_BRiseCylinder, lblBRiseCylinder);
                        setLblColor(FrmMain.IO.Q_BStopCylinder, lblBStopCylinder);
                        setLblColor(FrmMain.IO.Q_BUpperComputerRequest, lblBUpperComputerRequest);
                        setLblColor(FrmMain.IO.Q_BLowerComputerHave, lblBLowerComputerHave);
                        setLblColor(FrmMain.IO.Q_Spare3, lblQSpare3);
                        setLblColor(FrmMain.IO.Q_RobotPause, lblRobotPause);
                        setLblColor(FrmMain.IO.Q_RobotReset, lblRobotReset);
                        #endregion

                        #region 交互点

                        #endregion

                        Thread.Sleep(10);
                    }
                });
            }            
        }
        /// <summary>
        /// 设置lbl控件背景颜色函数
        /// </summary>
        /// <param name="Signal">信号</param>
        /// <param name="label">控件名称</param>
        private void setLblColor(bool Signal, Label label)
        {
            if(Signal)
            {
                label.BackColor = Color.Green;
            }
            else
            {
                label.BackColor = Color.Transparent;
            }
        }
    }
}
