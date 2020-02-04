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
using MyLog;

namespace MountingRobot.UI
{
    public partial class FrmManual : Form
    {
        private bool start = false;
        public FrmManual()
        {
            InitializeComponent();
        }

        private void FrmManual_Load(object sender, EventArgs e)
        {
            #region 刷新IO控件
            RefrshControls();
            //开始调宽轴状态读取线程
            FrmMain.IO.AxisReceiveStart1 = true;
            FrmMain.IO.ReadPLC1Axis();
            //开始运行轴状态读取线程
            FrmMain.IO.AxisReceiveStart2 = true;
            FrmMain.IO.ReadPLC2Axis();
            #endregion
        }

        private void FrmManual_FormClosing(object sender, FormClosingEventArgs e)
        {
            start = false;//关闭控件显示刷新线程
            
            FrmMain.IO.AxisReceiveStart1 = false;//关闭调宽轴状态读取线程
            FrmMain.IO.AxisReceiveStart2 = false;//关闭运行轴状态读取线程
            timerShowIO.Stop();//停止IO控件刷新定时器
            timerShowIO.Enabled = false;            
            #region 下载覆盖PLC轴参数
            //关闭页面时，将保存的参数下载至PLC，使保存数据与PLC参数保持一致
            if (Global.ConnectSuccess1)
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthAutoSpeed, (float)Global.AWidthAutoSpeed);
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionLoosen, (float)Global.AWidthPositionLoosen);
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionPinch, (float)Global.AWidthPositionPinch);
                FrmMain.smart200_1.Write(Global.VD_BWidthAutoSpeed, (float)Global.BWidthAutoSpeed);
                FrmMain.smart200_1.Write(Global.VD_BWidthPositionLoosen, (float)Global.BWidthPositionLoosen);
                FrmMain.smart200_1.Write(Global.VD_BWidthPositionPinch, (float)Global.BWidthPositionPinch);
            }
            #endregion
        }

        #region IO刷新控件
        private void RefrshControls()
        {
            start = true;
            #region 参数加载
            nudAWidthAutoSpeed.Value = Global.AWidthAutoSpeed;
            txtAWidthPositionLoosen.Text = Global.AWidthPositionLoosen.ToString();
            txtAWidthPositionPinch.Text = Global.AWidthPositionPinch.ToString();
            nudBWidthAutoSpeed.Value = Global.BWidthAutoSpeed;
            txtBWidthPositionLoosen.Text = Global.BWidthPositionLoosen.ToString();
            txtBWidthPositionPinch.Text = Global.BWidthPositionPinch.ToString();
            #endregion

            if (Global.ConnectSuccess1)  //判断与PLC连接状态
            {
                #region 第一次刷新输出手动按钮状态
                readAndSetBtnColor(Global.M_AWidthHoming, btnAWidthHoming);
                readAndSetBtnColor(Global.M_ARiseCylinder, btnARiseCylinder);
                readAndSetBtnColor(Global.M_AStopCylinder, btnAStopCylinder);
                readAndSetBtnColor(Global.M_ABeltForward, btnABeltForward);
                readAndSetBtnColor(Global.M_ABeltBackward, btnABeltBackward);
                readAndSetBtnColor(Global.M_AUpperComputerRequest, btnAUpperComputerRequest);
                readAndSetBtnColor(Global.M_ALowerComputerHave, btnALowerComputerHave);

                readAndSetBtnColor(Global.M_BWidthHoming, btnBWidthHoming);
                readAndSetBtnColor(Global.M_BRiseCylinder, btnBRiseCylinder);
                readAndSetBtnColor(Global.M_BStopCylinder, btnBStopCylinder);
                readAndSetBtnColor(Global.M_BBeltForward, btnBBeltForward);
                readAndSetBtnColor(Global.M_BBeltBackward, btnBBeltBackward);
                readAndSetBtnColor(Global.M_BUpperComputerRequest, btnBUpperComputerRequest);
                readAndSetBtnColor(Global.M_BLowerComputerHave, btnBLowerComputerHave);

                readAndSetBtnColor(Global.M_AStartBtnLamp, btnAStartBtnLamp);
                readAndSetBtnColor(Global.M_AResetBtnLamp, btnAResetBtnLamp);
                readAndSetBtnColor(Global.M_BStartBtnLamp, btnBStartBtnLamp);
                readAndSetBtnColor(Global.M_BResetBtnLamp, btnBResetBtnLamp);
                readAndSetBtnColor(Global.M_RedLamp, btnRedLamp);
                readAndSetBtnColor(Global.M_GreenLamp, btnGreenLamp);
                readAndSetBtnColor(Global.M_YellowLamp, btnYellowLamp);
                readAndSetBtnColor(Global.M_Buzzer, btnBuzzer);
                readAndSetBtnColor(Global.M_VibrationPower, btnVibrationPower);
                #endregion

                timerShowIO.Enabled = true;
                timerShowIO.Start();

                #region 循环刷新IO
                //Task.Run(() =>
                //{
                //    while (start)
                //    {
                //        Invoke(new Action(() => 
                //        {
                //            #region 刷新输入点控件
                //            //IB0
                //            setLblColor(FrmMain.IO.I_AWidthAxisOrigin, lblAWidthAxisOrigin);
                //            setLblColor(FrmMain.IO.I_AWidthAxisForwardLimit, lblAWidthAxisForwardLimit);
                //            setLblColor(FrmMain.IO.I_BWidthAxisOrigin, lblBWidthAxisOrigin);
                //            setLblColor(FrmMain.IO.I_BWidthAxisForwardLimit, lblBWidthAxisForwardLimit);
                //            setLblColor(FrmMain.IO.I_EStop, lblEStop);
                //            setLblColor(FrmMain.IO.I_StartBtn, lblStartBtn);
                //            //IB1
                //            setLblColor(FrmMain.IO.I_StopBtn, lblStopBtn);
                //            setLblColor(FrmMain.IO.I_ResetBtn, lblResetBtn);
                //            setLblColor(FrmMain.IO.I_AirAlarm, lblAirAlarm);
                //            setLblColor(FrmMain.IO.I_ADoorSwitch, lblADoorSwitch);
                //            setLblColor(FrmMain.IO.I_AInSensors, lblAInSensors);
                //            setLblColor(FrmMain.IO.I_AHaveSensors, lblAHaveSensors);
                //            setLblColor(FrmMain.IO.I_AOutSensors, lblAOutSensors);
                //            //IB2
                //            setLblColor(FrmMain.IO.I_AVibrationFull, lblAVibrationFull);
                //            setLblColor(FrmMain.IO.I_AVibrationHave, lblAVibrationHave);
                //            setLblColor(FrmMain.IO.I_AUpperComputerHave, lblAUpperComputerHave);
                //            setLblColor(FrmMain.IO.I_ALowerComputerRequest, lblALowerComputerRequest);
                //            setLblColor(FrmMain.IO.I_BDoorSwitch, lblBDoorSwitch);
                //            setLblColor(FrmMain.IO.I_BInSensors, lblBInSensors);
                //            //IB3
                //            setLblColor(FrmMain.IO.I_BHaveSensors, lblBInSensors);
                //            setLblColor(FrmMain.IO.I_BOutSensors, lblBOutSensors);
                //            setLblColor(FrmMain.IO.I_BVibrationFull, lblBVibrationFull);
                //            setLblColor(FrmMain.IO.I_BVibrationHave, lblBVivrationHave);
                //            setLblColor(FrmMain.IO.I_BUpperComputerHave, lblBUpperComputerHave);
                //            setLblColor(FrmMain.IO.I_BLowerComputerRequest, lblBLowerComputerRequest);
                //            #endregion

                //            #region 刷新输出点控件
                //            //QB0
                //            setLblColor(FrmMain.IO.Q_AWidthAxisPulse, lblAWidthAxisPulse);
                //            setLblColor(FrmMain.IO.Q_BWidthAxisPulse, lblBWidthAxisPulse);
                //            setLblColor(FrmMain.IO.Q_AWidthAxisDirection, lblBWidthAxisDirection);
                //            setLblColor(FrmMain.IO.Q_StartBtnLamp, lblStartBenLamp);
                //            setLblColor(FrmMain.IO.Q_ResetBtnLamp, lblResetBtnLamp);
                //            setLblColor(FrmMain.IO.Q_VibrationPower, lblVibrationPower);
                //            setLblColor(FrmMain.IO.Q_BWidthAxisDirection, lblBWidthAxisDirection);
                //            //QB1
                //            setLblColor(FrmMain.IO.Q_LampGreen, lblLampGreen);
                //            setLblColor(FrmMain.IO.Q_LampYellow, lblLampYellow);
                //            setLblColor(FrmMain.IO.Q_LampRed, lblLampRed);
                //            setLblColor(FrmMain.IO.Q_Buzzer, lblBuzzer);
                //            setLblColor(FrmMain.IO.Q_ARiseCylinder, lblARiseCylinder);
                //            setLblColor(FrmMain.IO.Q_AStopCylinder, lblAStopCylinder);
                //            setLblColor(FrmMain.IO.Q_AUpperComputerRequest, lblAUpperComputerRequest);
                //            //QB2
                //            setLblColor(FrmMain.IO.Q_ALowerComputerHave, lblALowerComputerHave);
                //            setLblColor(FrmMain.IO.Q_BRiseCylinder, lblBRiseCylinder);
                //            setLblColor(FrmMain.IO.Q_BStopCylinder, lblBStopCylinder);
                //            setLblColor(FrmMain.IO.Q_BUpperComputerRequest, lblBUpperComputerRequest);
                //            setLblColor(FrmMain.IO.Q_BLowerComputerHave, lblBLowerComputerHave);
                //            #endregion

                //            #region 读取轴状态
                //            lblAWidthStatus.Text = FrmMain.smart200_1.ReaduInt(Global.VW_AWidthStatus).ToString();
                //            lblARunStatus.Text = FrmMain.smart200_2.ReaduInt(Global.VW_ARunStatus).ToString();
                //            lblAWidthPositionActual.Text = FrmMain.smart200_1.ReadFloat(Global.VD_AWidthPositionActual).ToString();
                //            lblAWidthVelocityActual.Text = FrmMain.smart200_1.ReadFloat(Global.VD_AWidthVelocityActual).ToString();

                //            lblBWidthStatus.Text = FrmMain.smart200_1.ReaduInt(Global.VW_BWidthStatus).ToString();
                //            lblBRunStatus.Text = FrmMain.smart200_2.ReaduInt(Global.VW_BRunStatus).ToString();
                //            lblBWidthPositionActual.Text = FrmMain.smart200_1.ReadFloat(Global.VD_BWidthPositionActual).ToString();
                //            lblBWidthVelocityActual.Text = FrmMain.smart200_1.ReadFloat(Global.VD_BWidthVelocityActual).ToString();
                //            #endregion
                //        }));
                //        Thread.Sleep(5);
                //    }
                //});
                #endregion
            }
        }
        #endregion

        #region 保存参数
        /// <summary>
        /// A轨参数保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定保存数据？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ASave();
                    ADownload();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// A轨保存
        /// </summary>
        private void ASave()
        {
            Global.AWidthAutoSpeed = nudAWidthAutoSpeed.Value;
            Global.AWidthPositionLoosen = Convert.ToDecimal(txtAWidthPositionLoosen.Text);
            Global.AWidthPositionPinch = Convert.ToDecimal(txtAWidthPositionPinch.Text);
            Global.AProductINI.IniWriteValue("PLC参数", "A轨调宽自动速度", Global.AWidthAutoSpeed.ToString());
            Global.AProductINI.IniWriteValue("PLC参数", "A轨调宽输送位", Global.AWidthPositionLoosen.ToString());
            Global.AProductINI.IniWriteValue("PLC参数", "A轨调宽夹紧位", Global.AWidthPositionPinch.ToString());
        }
        /// <summary>
        /// A轨下载
        /// </summary>
        private void ADownload()
        {
            if (Global.ConnectSuccess1)
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthAutoSpeed, (float)Global.AWidthAutoSpeed);
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionLoosen, (float)Global.AWidthPositionLoosen);
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionPinch, (float)Global.AWidthPositionPinch);
                showMeesage("A轨参数保存并下载成功", 0);
            }
            else
            {
                showMeesage("A轨参数保存成功，未下载", 0);
            }
        }
        /// <summary>
        /// B轨参数保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定保存数据？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BSave();
                    BDownload();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// B轨保存
        /// </summary>
        private void BSave()
        {
            Global.BWidthAutoSpeed = nudBWidthAutoSpeed.Value;
            Global.BWidthPositionLoosen = Convert.ToDecimal(txtBWidthPositionLoosen.Text);
            Global.BWidthPositionPinch = Convert.ToDecimal(txtBWidthPositionPinch.Text);
            Global.BProductINI.IniWriteValue("PLC参数", "B轨调宽自动速度", Global.BWidthAutoSpeed.ToString());
            Global.BProductINI.IniWriteValue("PLC参数", "B轨调宽输送位", Global.BWidthPositionLoosen.ToString());
            Global.BProductINI.IniWriteValue("PLC参数", "B轨调宽夹紧位", Global.BWidthPositionPinch.ToString());
        }
        /// <summary>
        /// B轨下载
        /// </summary>
        private void BDownload()
        {
            if (Global.ConnectSuccess1)
            {
                FrmMain.smart200_1.Write(Global.VD_BWidthAutoSpeed, (float)Global.BWidthAutoSpeed);
                FrmMain.smart200_1.Write(Global.VD_BWidthPositionLoosen, (float)Global.BWidthPositionLoosen);
                FrmMain.smart200_1.Write(Global.VD_BWidthPositionPinch, (float)Global.BWidthPositionPinch);
                showMeesage("B轨参数保存成功", 0);
            }
            else
            {
                showMeesage("B轨参数保存成功，未下载", 0);
            }
        }
        #endregion

        #region 输出控制按钮        
        /// <summary>
        /// A轨回原按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthHoming_Click(object sender, EventArgs e)
        {
            clickBtn(btnAWidthHoming, Global.M_AWidthHoming);
        }
        /// <summary>
        /// A轨顶升气缸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnARiseCylinder_Click(object sender, EventArgs e)
        {
            clickBtn(btnARiseCylinder, Global.M_ARiseCylinder);
        }
        /// <summary>
        /// A轨阻挡气缸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAStopCylinder_Click(object sender, EventArgs e)
        {
            clickBtn(btnAStopCylinder, Global.M_AStopCylinder);
        }
        /// <summary>
        /// A轨皮带正转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnABeltForward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(1))
            {
                if (btnABeltForward.BackColor == Color.Green)
                {
                    FrmMain.smart200_2.Write(Global.M_ABeltForward, false);
                    btnABeltForward.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    FrmMain.smart200_2.Write(Global.M_ABeltForward, true);
                    btnABeltForward.BackColor = Color.Green;
                }
            }

        }
        /// <summary>
        /// A轨皮带反转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnABeltBackward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(1))
            {
                if (btnABeltBackward.BackColor == Color.Green)
                {
                    FrmMain.smart200_2.Write(Global.M_ABeltBackward, false);
                    btnABeltBackward.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    FrmMain.smart200_2.Write(Global.M_ABeltBackward, true);
                    btnABeltBackward.BackColor = Color.Green;
                }
            }
        }
        /// <summary>
        /// A轨点动前进按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthJogForward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthJogDistance, (float)nudAWidthJogDistance.Value);//下发点动距离
                FrmMain.smart200_1.Write(Global.VD_AWidthManualSpeed, (float)nudAWidthManaulSpeed.Value);//下发手动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, true);
                FrmMain.smart200_1.Write(Global.M_AWidthJogBackward, false);
                FrmMain.smart200_1.Write(Global.M_AWidthJogForward, true);
            }
        }
        /// <summary>
        /// A轨点动后退按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthJogBackward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthJogDistance, (float)nudAWidthJogDistance.Value);//下发点动距离
                FrmMain.smart200_1.Write(Global.VD_AWidthManualSpeed, (float)nudAWidthManaulSpeed.Value);//下发手动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, true);
                FrmMain.smart200_1.Write(Global.M_AWidthJogForward, false);
                FrmMain.smart200_1.Write(Global.M_AWidthJogBackward, true);
            }
        }
        /// <summary>
        /// A轨调窄按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthForward_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthAutoSpeed, (float)nudAWidthAutoSpeed.Value);//下发自动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, false);
                FrmMain.smart200_1.Write(Global.M_AWidthForward, true);
            }
        }
        /// <summary>
        /// A轨调窄按钮松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthForward_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.M_AWidthForward, false);
            }
        }
        /// <summary>
        /// A轨调宽按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthBackward_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthAutoSpeed, (float)nudAWidthAutoSpeed.Value);//下发自动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, false);
                FrmMain.smart200_1.Write(Global.M_AWidthBackward, true);
            }
        }
        /// <summary>
        /// A轨调宽按钮松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthBackward_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.M_AWidthBackward, false);
            }
        }
        /// <summary>
        /// A轨向上位机要板按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAUpperComputerRequest_Click(object sender, EventArgs e)
        {
            clickBtn(btnAUpperComputerRequest, Global.M_AUpperComputerRequest);
        }
        /// <summary>
        /// A轨给下位机有板按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnALowerComputerHave_Click(object sender, EventArgs e)
        {
            clickBtn(btnALowerComputerHave, Global.M_ALowerComputerHave);
        }
        /// <summary>
        /// A轨调宽输送位SET按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthSetLoosen_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                txtAWidthPositionLoosen.Text = lblAWidthPositionActual.Text;
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionLoosen, (float)Convert.ToDecimal(txtAWidthPositionLoosen.Text));
            }
        }
        /// <summary>
        /// A轨调宽输送位GO按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthGoLoosen_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionLoosen, (float)Convert.ToDecimal(txtAWidthPositionLoosen.Text));//下发输送位位置值
                FrmMain.smart200_1.Write(Global.M_AWidthGoLoosen, true);
            }
        }
        /// <summary>
        /// A轨调宽夹紧位SET按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthSetPinch_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                txtAWidthPositionPinch.Text = lblAWidthPositionActual.Text;
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionPinch, (float)Convert.ToDecimal(txtAWidthPositionPinch.Text));
            }
        }
        /// <summary>
        /// A轨调宽夹紧位GO按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAWidthGoPinch_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthPositionPinch, (float)Convert.ToDecimal(txtAWidthPositionPinch.Text));//下发夹紧位位置值
                FrmMain.smart200_1.Write(Global.M_AWidthGoPinch, true);
            }
        }
        /// <summary>
        /// B轨回原按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthHoming_Click(object sender, EventArgs e)
        {
            clickBtn(btnBWidthHoming, Global.M_BWidthHoming);
        }
        /// <summary>
        /// B轨顶升气缸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBRiseCylinder_Click(object sender, EventArgs e)
        {
            clickBtn(btnBRiseCylinder, Global.M_BRiseCylinder);
        }
        /// <summary>
        /// B轨阻挡气缸按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBStopCylinder_Click(object sender, EventArgs e)
        {
            clickBtn(btnBStopCylinder, Global.M_BStopCylinder);
        }
        /// <summary>
        /// B轨皮带正转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBBeltForward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(1))
            {
                if (btnBBeltForward.BackColor == Color.Green)
                {
                    FrmMain.smart200_2.Write(Global.M_BBeltForward, false);
                    btnBBeltForward.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    FrmMain.smart200_2.Write(Global.M_BBeltForward, true);
                    btnBBeltForward.BackColor = Color.Green;
                }
            }

        }
        /// <summary>
        /// B轨皮带反转按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBBeltBackward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(1))
            {
                if (btnBBeltBackward.BackColor == Color.Green)
                {
                    FrmMain.smart200_2.Write(Global.M_BBeltBackward, false);
                    btnBBeltBackward.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    FrmMain.smart200_2.Write(Global.M_BBeltBackward, true);
                    btnBBeltBackward.BackColor = Color.Green;
                }
            }
        }
        /// <summary>
        /// B轨点动前进
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthJogForward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthJogDistance, (float)nudAWidthJogDistance.Value);//下发点动距离
                FrmMain.smart200_1.Write(Global.VD_AWidthManualSpeed, (float)nudAWidthManaulSpeed.Value);//下发手动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, true);
                FrmMain.smart200_1.Write(Global.M_AWidthJogBackward, false);
                FrmMain.smart200_1.Write(Global.M_AWidthJogForward, true);
            }
        }
        /// <summary>
        /// B轨点动后退
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthJogBackward_Click(object sender, EventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_AWidthJogDistance, (float)nudAWidthJogDistance.Value);//下发点动距离
                FrmMain.smart200_1.Write(Global.VD_AWidthManualSpeed, (float)nudAWidthManaulSpeed.Value);//下发手动速度
                FrmMain.smart200_1.Write(Global.M_AWidthJogMode, true);
                FrmMain.smart200_1.Write(Global.M_AWidthJogForward, false);
                FrmMain.smart200_1.Write(Global.M_AWidthJogBackward, true);
            }
        }
        /// <summary>
        /// B轨调窄按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthForward_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_BWidthAutoSpeed, (float)nudBWidthAutoSpeed.Value);//下发自动速度
                FrmMain.smart200_1.Write(Global.M_BWidthJogMode, false);
                FrmMain.smart200_1.Write(Global.M_BWidthForward, true);
            }
        }
        /// <summary>
        /// B轨调窄按钮松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthForward_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.M_BWidthForward, false);
            }
        }
        /// <summary>
        /// B轨调宽按钮按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthBackward_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.VD_BWidthAutoSpeed, (float)nudBWidthAutoSpeed.Value);//下发自动速度
                FrmMain.smart200_1.Write(Global.M_BWidthJogMode, false);
                FrmMain.smart200_1.Write(Global.M_BWidthBackward, true);
            }
        }
        /// <summary>
        /// B轨调宽按钮松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthBackward_MouseUp(object sender, MouseEventArgs e)
        {
            if (checkPLCConnect(0))
            {
                FrmMain.smart200_1.Write(Global.M_BWidthBackward, false);
            }
        }
        /// <summary>
        /// 向上位机要板按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBUpperComputerRequest_Click(object sender, EventArgs e)
        {
            clickBtn(btnBUpperComputerRequest, Global.M_BUpperComputerRequest);
        }
        /// <summary>
        /// 给下位机有板按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBLowerComputerHave_Click(object sender, EventArgs e)
        {
            clickBtn(btnBLowerComputerHave, Global.M_ALowerComputerHave);
        }
        /// <summary>
        /// B轨调宽输送位SET按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthSetLoosen_Click(object sender, EventArgs e)
        {
            txtBWidthPositionLoosen.Text = lblBWidthPositionActual.Text;
            FrmMain.smart200_1.Write(Global.VD_BWidthPositionLoosen, (float)Convert.ToDecimal(txtBWidthPositionLoosen.Text));
        }
        /// <summary>
        /// B轨调宽输送位GO按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthGoLoosen_Click(object sender, EventArgs e)
        {
            FrmMain.smart200_1.Write(Global.VD_BWidthPositionLoosen, (float)Convert.ToDecimal(txtBWidthPositionLoosen.Text));//下发输送位位置值
            FrmMain.smart200_1.Write(Global.M_BWidthGoLoosen, true);
        }
        /// <summary>
        /// B轨调宽夹紧位SET按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthSetPinch_Click(object sender, EventArgs e)
        {
            txtBWidthPositionPinch.Text = lblBWidthPositionActual.Text;
            FrmMain.smart200_1.Write(Global.VD_BWidthPositionPinch, (float)Convert.ToDecimal(txtBWidthPositionPinch.Text));
        }
        /// <summary>
        /// B轨调宽夹紧位GO按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBWidthGoPinch_Click(object sender, EventArgs e)
        {
            FrmMain.smart200_1.Write(Global.VD_BWidthPositionPinch, (float)Convert.ToDecimal(txtBWidthPositionPinch.Text));//下发夹紧位位置值
            FrmMain.smart200_1.Write(Global.M_BWidthGoPinch, true);
        }
        /// <summary>
        /// A轨启动按钮灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAStartBtnLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnAStartBtnLamp, Global.M_AStartBtnLamp);
        }
        /// <summary>
        /// A轨复位按钮灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAResetBtnLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnAResetBtnLamp, Global.M_AResetBtnLamp);
        }
        /// <summary>
        /// B轨启动按钮灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBStartBtnLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnBStartBtnLamp, Global.M_BStartBtnLamp);
        }
        /// <summary>
        /// B轨复位按钮灯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBResetBtnLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnBResetBtnLamp, Global.M_BResetBtnLamp);
        }
        /// <summary>
        /// 三色灯红
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRedLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnRedLamp, Global.M_RedLamp);
        }
        /// <summary>
        /// 三色灯绿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGreenLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnGreenLamp, Global.M_GreenLamp);
        }
        /// <summary>
        /// 三色灯黄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnYellowLamp_Click(object sender, EventArgs e)
        {
            clickBtn(btnYellowLamp, Global.M_YellowLamp);
        }
        /// <summary>
        /// 蜂鸣器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBuzzer_Click(object sender, EventArgs e)
        {
            clickBtn(btnBuzzer, Global.M_Buzzer);
        }
        /// <summary>
        /// 振动盘启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVibrationPower_Click(object sender, EventArgs e)
        {
            clickBtn(btnVibrationPower, Global.M_VibrationPower);
        }
        #endregion

        #region 控件操作函数
        /// <summary>
        /// 设置lbl控件背景颜色函数
        /// </summary>
        /// <param name="Signal">信号</param>
        /// <param name="label">lbl控件</param>
        private void setLblColor(bool Signal, Label label)
        {
            if (Signal)
            {
                label.BackColor = Color.Green;
            }
            else
            {
                label.BackColor = Color.Transparent;
            }
        }
        /// <summary>
        /// 读取设置按钮颜色
        /// </summary>
        /// <param name="Pointer">PLC指针</param>
        /// <param name="button">btn控件</param>
        private void readAndSetBtnColor(string Pointer, Button button)
        {
            if (FrmMain.smart200_1.ReadBool(Pointer))
            {
                button.BackColor = Color.Green;
            }
            else
            {
                button.BackColor = Color.FromArgb(44, 44, 44);
            }
        }
        /// <summary>
        /// 设置按钮颜色
        /// </summary>
        /// <param name="button">btn控件</param>
        /// <param name="value">值</param>
        private void setBtnColor(Button button, bool value)
        {
            if (value)
            {
                button.BackColor = Color.Green;
            }
            else
            {
                button.BackColor = Color.FromArgb(44, 44, 44);
            }
        }
        /// <summary>
        /// 点击按钮函数
        /// </summary>
        /// <param name="button">btn控件</param>
        /// <param name="pointer">PLC指针</param>
        private void clickBtn(Button button, String pointer)
        {
            if (checkPLCConnect(0))
            {
                if (button.BackColor == Color.Green)
                {
                    FrmMain.smart200_1.Write(pointer, false);
                    button.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    FrmMain.smart200_1.Write(pointer, true);
                    button.BackColor = Color.Green;
                }
            }
        }

        #endregion        

        #region 消息显示
        /// <summary>
        /// 信息已显示秒数
        /// </summary>
        private int showSeconds;
        /// <summary>
        /// 消息显示函数
        /// </summary>
        /// <param name="Msg">需显示信息</param>
        /// <param name="mode">0：正常消息  1：报警消息</param>
        private void showMeesage(string Msg, int mode)
        {
            timerMessage.Enabled = true;
            timerMessage.Start();
            showSeconds = 0;
            switch (mode)
            {
                case 0:
                    lblMessage.BackColor = Color.FromArgb(44, 44, 44);
                    break;
                case 1:
                    lblMessage.BackColor = Color.Red;
                    break;
            }
            lblMessage.Text = Msg;
        }
        /// <summary>
        /// 消息显示5秒定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerMessage_Tick(object sender, EventArgs e)
        {
            if (lblMessage.Text != "")
            {
                showSeconds++;
                if (showSeconds >= 5)
                {
                    showSeconds = 0;
                    lblMessage.Text = "";
                    timerMessage.Stop();
                    timerMessage.Enabled = false;
                }
            }
        }
        #endregion

        #region 检查PLC连接
        /// <summary>
        /// 检查PLC连接情况,无则报警
        /// </summary>
        /// <param name="node"></param>
        private bool checkPLCConnect(int node)
        {
            if (node == 0)
            {
                if (Global.ConnectSuccess1)
                {
                    return true;
                }
                else
                {
                    showMeesage("1号PLC未连接", 1);
                    return false;
                }
            }
            else
            {
                if (Global.ConnectSuccess2)
                {
                    return true;
                }
                else
                {
                    showMeesage("2号PLC未连接", 1);
                    return false;
                }
            }
        }

        #endregion

        private void timerShowIO_Tick(object sender, EventArgs e)
        {
            #region 刷新输入点控件
            //IB0
            setLblColor(FrmMain.IO.I_AWidthAxisOrigin, lblAWidthAxisOrigin);
            setLblColor(FrmMain.IO.I_AWidthAxisForwardLimit, lblAWidthAxisForwardLimit);
            setLblColor(FrmMain.IO.I_BWidthAxisOrigin, lblBWidthAxisOrigin);
            setLblColor(FrmMain.IO.I_BWidthAxisForwardLimit, lblBWidthAxisForwardLimit);
            setLblColor(FrmMain.IO.I_EStop, lblEStop);
            setLblColor(FrmMain.IO.I_StartBtn, lblStartBtn);
            //IB1
            setLblColor(FrmMain.IO.I_StopBtn, lblStopBtn);
            setLblColor(FrmMain.IO.I_ResetBtn, lblResetBtn);
            setLblColor(FrmMain.IO.I_AirAlarm, lblAirAlarm);
            setLblColor(FrmMain.IO.I_ADoorSwitch, lblADoorSwitch);
            setLblColor(FrmMain.IO.I_AInSensors, lblAInSensors);
            setLblColor(FrmMain.IO.I_AHaveSensors, lblAHaveSensors);
            setLblColor(FrmMain.IO.I_AOutSensors, lblAOutSensors);
            //IB2
            setLblColor(FrmMain.IO.I_AVibrationFull, lblAVibrationFull);
            setLblColor(FrmMain.IO.I_AVibrationHave, lblAVibrationHave);
            setLblColor(FrmMain.IO.I_AUpperComputerHave, lblAUpperComputerHave);
            setLblColor(FrmMain.IO.I_ALowerComputerRequest, lblALowerComputerRequest);
            setLblColor(FrmMain.IO.I_BDoorSwitch, lblBDoorSwitch);
            setLblColor(FrmMain.IO.I_BInSensors, lblBInSensors);
            //IB3
            setLblColor(FrmMain.IO.I_BHaveSensors, lblBInSensors);
            setLblColor(FrmMain.IO.I_BOutSensors, lblBOutSensors);
            setLblColor(FrmMain.IO.I_BVibrationFull, lblBVibrationFull);
            setLblColor(FrmMain.IO.I_BVibrationHave, lblBVivrationHave);
            setLblColor(FrmMain.IO.I_BUpperComputerHave, lblBUpperComputerHave);
            setLblColor(FrmMain.IO.I_BLowerComputerRequest, lblBLowerComputerRequest);
            #endregion

            #region 刷新输出点控件
            //QB0
            setLblColor(FrmMain.IO.Q_AWidthAxisPulse, lblAWidthAxisPulse);
            setLblColor(FrmMain.IO.Q_BWidthAxisPulse, lblBWidthAxisPulse);
            setLblColor(FrmMain.IO.Q_AWidthAxisDirection, lblBWidthAxisDirection);
            setLblColor(FrmMain.IO.Q_StartBtnLamp, lblStartBenLamp);
            setLblColor(FrmMain.IO.Q_ResetBtnLamp, lblResetBtnLamp);
            setLblColor(FrmMain.IO.Q_VibrationPower, lblVibrationPower);
            setLblColor(FrmMain.IO.Q_BWidthAxisDirection, lblBWidthAxisDirection);
            //QB1
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
            #endregion

            #region 读取轴状态
            lblAWidthStatus.Text = FrmMain.IO.AWidthStatus.ToString();
            lblAWidthPositionActual.Text = FrmMain.IO.AWidthPositionActual.ToString();
            lblAWidthVelocityActual.Text = FrmMain.IO.AWidthVelocityActual.ToString();

            lblBWidthStatus.Text = FrmMain.IO.BWidthStatus.ToString();
            lblBWidthPositionActual.Text = FrmMain.IO.BWidthPositionActual.ToString();
            lblBWidthVelocityActual.Text = FrmMain.IO.BWidthVelocityActual.ToString();

            lblARunStatus.Text = FrmMain.IO.ARunStatus.ToString();
            lblBRunStatus.Text = FrmMain.IO.BRunStatus.ToString();
            #endregion
        }
    }
}
