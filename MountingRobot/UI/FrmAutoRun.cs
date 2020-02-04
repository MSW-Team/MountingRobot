using MountingRobot.BLL;
using MyLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MountingRobot.UI
{
    public partial class FrmAutoRun : Form
    {
        private bool AnimationStart = false;
        public FrmAutoRun()
        {
            InitializeComponent();
        }

        private List<Image> listImage;  //创建图片储存集合
        private void FrmAutoRun_Load(object sender, EventArgs e)
        {
            #region 加载资源文件图片至动画（通过内存加载图片来加快控件中图片加载速度）
            listImage = new List<Image>();
            listImage.Add(global::MountingRobot.Properties.Resources.PCB2);  //PCB板图片0
            listImage.Add(global::MountingRobot.Properties.Resources.arrow);  //箭头图片1
            listImage.Add(global::MountingRobot.Properties.Resources.BuzzerOpen镂空);  //蜂鸣器开图片2
            listImage.Add(global::MountingRobot.Properties.Resources.BuzzerClose镂空);  //蜂鸣器关图片3
            listImage.Add(global::MountingRobot.Properties.Resources.AutoRun);  //自动模式图片4
            listImage.Add(global::MountingRobot.Properties.Resources.Manual);  //手动模式图片5
            listImage.Add(global::MountingRobot.Properties.Resources.CloseCam镂空);  //关闭相机图片6
            listImage.Add(global::MountingRobot.Properties.Resources.OpenCam镂空);  //打开相机图片7

            //PCB板图片
            picAInSensors.Image = listImage[0];
            picAMidSensors.Image = listImage[0];
            picAOutSensors.Image = listImage[0];
            picBInSensors.Image = listImage[0];
            picBMidSensors.Image = listImage[0];
            picBOutSensors.Image = listImage[0];

            //显示视觉观察
            btnVision1.BackgroundImage = listImage[6];
            #endregion  

            #region 初始化生产信息表格与动画
            if (Global.OrbitalCount == "单轨")
            {
                //加载动画
                panelAAnimation.Visible = true;
                panelBAnimation.Visible = false;
                Animation(1);
                //隐藏视觉窗体
            }
            else
            {
                //加载动画
                panelAAnimation.Visible = true;
                panelBAnimation.Visible = true;
                Animation(2);
            }
            #endregion
            //报警栏刷新显示线程
            EquipmentInfo();

            //加载产品名称至控件
            ErgodicAProduct();
            ErgodicBProduct();
            //显示信息统计
            ShowProductStatistics();

            //连接至spel实例
            FrmIniRobot.RobotSpel.SpelVideoControl = spelVideoViewDown;
            FrmIniRobot.RobotSpel.SpelVideoControl = spelVideoViewUp;
        }

        #region 轨道动画
        /// <summary>
        /// 轨道动画
        /// </summary>
        /// <param name="count">轨道数</param>
        private void Animation(int count)
        {
            AnimationStart = true;

            #region 隐藏动画PictrueBox控件
            picAInSensors.Visible = false;
            picAMidSensors.Visible = false;
            picAOutSensors.Visible = false;
            picBInSensors.Visible = false;
            picBMidSensors.Visible = false;
            picBOutSensors.Visible = false;
            #endregion

            btnBuzzer1.BackgroundImage = listImage[3];
            btnMode1.BackgroundImage = listImage[4];
            picAMidSensors.Visible = true;
            btnMode1.Visible = false;
            btnBuzzer1.Visible = false;
            btnVision1.Visible = false;

            Task.Run(() =>
            {
                while (AnimationStart)
                {
                    if (Global.ConnectSuccess1)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            #region 蜂鸣器按钮
                            if (FrmMain.smart200_1.ReadBool(Global.M_Buzzer))
                            {
                                btnBuzzer1.BackgroundImage = listImage[3];
                                btnBuzzer.Text = "蜂鸣器消音";
                            }
                            else
                            {
                                btnBuzzer1.BackgroundImage = listImage[2];
                                btnBuzzer.Text = "蜂鸣器启用";
                            }
                            #endregion

                            #region 手自动切换按钮
                            if (FrmMain.smart200_1.ReadBool(Global.M_AutoMode))
                            {
                                btnMode1.BackgroundImage = listImage[4];
                                btnMode.Text = "手动模式";
                            }
                            else
                            {
                                btnMode1.BackgroundImage = listImage[5];
                            }
                            #endregion

                            #region A轨动画
                            //A向上位机要板箭头闪烁动画
                            if (FrmMain.IO.Q_AUpperComputerRequest)
                            {
                                timASentPCBSignal.Start();
                            }
                            else
                            {
                                timASentPCBSignal.Stop();
                            }

                            //A进板光电感应
                            if (FrmMain.IO.I_AInSensors)
                            {
                                picAInSensors.Visible = true;
                            }
                            else
                            {
                                picAInSensors.Visible = false;
                            }

                            //A中间光电感应
                            if (FrmMain.IO.I_AHaveSensors)
                            {
                                picAMidSensors.Visible = true;
                            }
                            else
                            {
                                picAMidSensors.Visible = false;
                            }

                            //A出板光电感应
                            if (FrmMain.IO.I_AOutSensors)
                            {
                                picAOutSensors.Visible = true;
                            }
                            else
                            {
                                picAOutSensors.Visible = false;
                            }

                            //A阻挡气缸
                            if (FrmMain.IO.Q_AStopCylinder)
                            {
                                lblAStopCyLinder.BackColor = Color.Lime;
                            }
                            else
                            {
                                lblAStopCyLinder.BackColor = Color.Silver;
                            }

                            //A侧推气缸
                            if (FrmMain.IO.Q_ARiseCylinder)
                            {
                                lblAPushCylinder1.BackColor = Color.Lime;
                                lblAPushCylinder2.BackColor = Color.Lime;
                            }
                            else
                            {
                                lblAPushCylinder1.BackColor = Color.Silver;
                                lblAPushCylinder2.BackColor = Color.Silver;
                            }

                            //A电机
                            //if (FrmMain.IO.Q_AMotor)
                            //{
                            //    timAMotor.Start();
                            //}
                            //else
                            //{
                            //    timAMotor.Stop();
                            //    lblAMotor1.BackColor = Color.Silver;
                            //    lblAMotor2.BackColor = Color.Silver;
                            //}

                            //A下位机要板信号
                            if (FrmMain.IO.I_ALowerComputerRequest)
                            {
                                timAGetPCBSignal.Start();
                            }
                            else
                            {
                                timAGetPCBSignal.Stop();
                            }
                            #endregion

                            #region B轨动画
                            if (count > 1)
                            {
                                //B向上位机要板箭头闪烁动画
                                if (FrmMain.IO.Q_BUpperComputerRequest)
                                {
                                    timBSentPCBSignal.Start();
                                }
                                else
                                {
                                    timBSentPCBSignal.Stop();
                                }

                                //B进板光电感应
                                if (FrmMain.IO.I_BInSensors)
                                {
                                    picBInSensors.Visible = true;
                                }
                                else
                                {
                                    picBInSensors.Visible = false;
                                }

                                //B中间光电感应
                                if (FrmMain.IO.I_BHaveSensors)
                                {
                                    picBMidSensors.Visible = true;
                                }
                                else
                                {
                                    picBMidSensors.Visible = false;
                                }

                                //B出板光电感应
                                if (FrmMain.IO.I_BOutSensors)
                                {
                                    picBOutSensors.Visible = true;
                                }
                                else
                                {
                                    picBOutSensors.Visible = false;
                                }

                                //B阻挡气缸
                                if (FrmMain.IO.Q_BStopCylinder)
                                {
                                    lblBStopCyLinder.BackColor = Color.Lime;
                                }
                                else
                                {
                                    lblBStopCyLinder.BackColor = Color.Silver;
                                }

                                //B侧推气缸
                                if (FrmMain.IO.Q_BRiseCylinder)
                                {
                                    lblBPushCylinder1.BackColor = Color.Lime;
                                    lblBPushCylinder2.BackColor = Color.Lime;
                                }
                                else
                                {
                                    lblBPushCylinder1.BackColor = Color.Silver;
                                    lblBPushCylinder2.BackColor = Color.Silver;
                                }

                                //B电机
                                //if (FrmMain.IO.Q_BMotor)
                                //{
                                //    timBMotor.Start();
                                //}
                                //else
                                //{
                                //    timBMotor.Stop();
                                //    lblBMotor1.BackColor = Color.Silver;
                                //    lblBMotor2.BackColor = Color.Silver;
                                //}

                                //B下位机要板信号
                                if (FrmMain.IO.I_BLowerComputerRequest)
                                {
                                    timBGetPCBSignal.Start();
                                }
                                else
                                {
                                    timBGetPCBSignal.Stop();
                                }
                            }

                            #endregion
                        }));
                    }
                    Thread.Sleep(200);
                }
            });
        }

        #region A轨定时器闪烁动画
        //A向上位机要板箭头闪烁动画
        private void timASentPCBSignal_Tick(object sender, EventArgs e)
        {

        }

        //A电机动画
        private void timAMotor_Tick(object sender, EventArgs e)
        {
            if (lblAMotor1.BackColor == Color.Silver)
            {
                lblAMotor1.BackColor = Color.Lime;
            }
            else
            {
                lblAMotor1.BackColor = Color.Silver;
            }

            if (lblAMotor2.BackColor == Color.Silver)
            {
                lblAMotor2.BackColor = Color.Lime;
            }
            else
            {
                lblAMotor2.BackColor = Color.Silver;
            }
        }

        //A下位机求板信号动画
        private void timAGetPCBSignal_Tick(object sender, EventArgs e)
        {

        }
        #endregion

        #region B轨定时器动画
        //B向上位机求板信号动画
        private void timBSentPCBSignal_Tick(object sender, EventArgs e)
        {

        }

        //B电机
        private void timBMotor_Tick(object sender, EventArgs e)
        {
            if (lblBMotor1.BackColor == Color.Silver)
            {
                lblBMotor1.BackColor = Color.Lime;
            }
            else
            {
                lblBMotor1.BackColor = Color.Silver;
            }

            if (lblBMotor2.BackColor == Color.Silver)
            {
                lblBMotor2.BackColor = Color.Lime;
            }
            else
            {
                lblBMotor2.BackColor = Color.Silver;
            }
        }

        //B下位机要板信号
        private void timBGetPCBSignal_Tick(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion

        private void FrmAutoRun_FormClosing(object sender, FormClosingEventArgs e)
        {
            AnimationStart = false;  //退出时关闭动画线程
            EquipmentInfoStart = false;  //退出时关闭设备状态栏线程
            spelVideoViewDown.Dispose();  //释放spelVideoView内存
            spelVideoViewUp.Dispose();
            Application.DoEvents();
        }

        #region 蜂鸣器消声按钮
        private void btnBuzzer_Click(object sender, EventArgs e)
        {
            if (Global.ConnectSuccess1)
            {
                if (FrmMain.smart200_1.ReadBool(Global.M_Buzzer))
                {
                    Common.myLog.writeAlmContent("取消蜂鸣器消声", Global.UserPermission, Global.UserPermission);
                    FrmMain.smart200_1.Write(Global.M_Buzzer, false);
                }
                else
                {
                    Common.myLog.writeAlmContent("开启蜂鸣器消声", Global.UserPermission, Global.UserPermission);
                    FrmMain.smart200_1.Write(Global.M_Buzzer, true);
                }
            }
            else
            {
                MessageBox.Show("PLC已断开连接，请重新连接后再试！");
            }
        }
        private void btnBuzzer_Click_1(object sender, EventArgs e)
        {
            if (Global.ConnectSuccess1)
            {
                if (FrmMain.smart200_1.ReadBool(Global.M_Buzzer))
                {
                    Common.myLog.writeAlmContent("取消蜂鸣器消声", Global.UserPermission, Global.UserPermission);
                    FrmMain.smart200_1.Write(Global.M_Buzzer, false);
                }
                else
                {
                    Common.myLog.writeAlmContent("开启蜂鸣器消声", Global.UserPermission, Global.UserPermission);
                    FrmMain.smart200_1.Write(Global.M_Buzzer, true);
                }
            }
            else
            {
                MessageBox.Show("PLC已断开连接，请重新连接后再试！");
            }
        }
        #endregion

        #region 设备状态栏信息
        private bool EquipmentInfoStart = false;
        int almAdd = 0;
        private void EquipmentInfo()
        {

            EquipmentInfoStart = true;
            string tempStr = string.Empty;
            Task.Run(() =>
            {
                while (EquipmentInfoStart)
                {
                    Invoke(new Action(() =>
                    {
                        #region PLC报警
                        if (!Global.ConnectSuccess1)
                        {
                            if (!lbEquipmentInfo.Items.Contains("与PLC断开连接"))
                            {
                                lbEquipmentInfo.Items.Add("与PLC断开连接");
                            }
                            almAdd++;
                        }
                        else
                        {
                            if (lbEquipmentInfo.Items.Contains("与PLC断开连接"))
                            {
                                lbEquipmentInfo.Items.Remove("与PLC断开连接");
                            }

                            //急停按钮
                            if (!FrmMain.IO.I_EStop)
                            {
                                if (!lbEquipmentInfo.Items.Contains("急停按下"))
                                {
                                    lbEquipmentInfo.Items.Add("急停按下");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("急停按下"))
                                {
                                    lbEquipmentInfo.Items.Remove("急停按下");
                                }
                            }

                            //气压检测
                            if (!FrmMain.IO.I_AirAlarm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("气压过低"))
                                {
                                    lbEquipmentInfo.Items.Add("气压过低");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("气压过低"))
                                {
                                    lbEquipmentInfo.Items.Remove("气压过低");
                                }
                            }

                            //A门限检测
                            if (!FrmMain.IO.I_ADoorSwitch)
                            {
                                if (!lbEquipmentInfo.Items.Contains("A门打开，请注意安全"))
                                {
                                    lbEquipmentInfo.Items.Add("A门打开，请注意安全");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("A门打开，请注意安全"))
                                {
                                    lbEquipmentInfo.Items.Remove("A门打开，请注意安全");
                                }
                            }

                            //B门限检测
                            if (!FrmMain.IO.I_BDoorSwitch)
                            {
                                if (!lbEquipmentInfo.Items.Contains("B门打开，请注意安全"))
                                {
                                    lbEquipmentInfo.Items.Add("B门打开，请注意安全");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("B门打开，请注意安全"))
                                {
                                    lbEquipmentInfo.Items.Remove("B门打开，请注意安全");
                                }
                            }

                            //排针座震动盘缺料或卡料报警Q13.1
                            if (FrmMain.IO.Q_LackNFAlm2)
                            {
                                if (!lbEquipmentInfo.Items.Contains("排针座震动盘缺料或卡料"))
                                {
                                    lbEquipmentInfo.Items.Add("排针座震动盘缺料或卡料");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("排针座震动盘缺料或卡料"))
                                {
                                    lbEquipmentInfo.Items.Remove("排针座震动盘缺料或卡料");
                                }
                            }

                            //电源座震动盘缺料或卡料报警Q13.2
                            if (FrmMain.IO.Q_LackPFAlm2)
                            {
                                if (!lbEquipmentInfo.Items.Contains("电源座震动盘缺料或卡料"))
                                {
                                    lbEquipmentInfo.Items.Add("电源座震动盘缺料或卡料");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("电源座震动盘缺料或卡料"))
                                {
                                    lbEquipmentInfo.Items.Remove("电源座震动盘缺料或卡料");
                                }
                            }

                            //A轨进板异常Q13.3
                            if (FrmMain.IO.Q_AIntoAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("A轨进板异常"))
                                {
                                    lbEquipmentInfo.Items.Add("A轨进板异常");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("A轨进板异常"))
                                {
                                    lbEquipmentInfo.Items.Remove("A轨进板异常");
                                }
                            }

                            //B轨进板异常Q13.4
                            if (FrmMain.IO.Q_BIntoAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("B轨进板异常"))
                                {
                                    lbEquipmentInfo.Items.Add("B轨进板异常");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("B轨进板异常"))
                                {
                                    lbEquipmentInfo.Items.Remove("B轨进板异常");
                                }
                            }

                            //A轨卡板Q13.5
                            if (FrmMain.IO.Q_AStuckAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("A轨卡板"))
                                {
                                    lbEquipmentInfo.Items.Add("A轨卡板");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("A轨卡板"))
                                {
                                    lbEquipmentInfo.Items.Remove("A轨卡板");
                                }
                            }

                            //B轨卡板Q13.6
                            if (FrmMain.IO.Q_BStuckAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("B轨卡板"))
                                {
                                    lbEquipmentInfo.Items.Add("B轨卡板");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("B轨卡板"))
                                {
                                    lbEquipmentInfo.Items.Remove("B轨卡板");
                                }
                            }

                            //排针座直震卡料或缺料料报警Q7.0
                            if (FrmMain.IO.Q_LackNFAlm1)
                            {
                                if (!lbEquipmentInfo.Items.Contains("排针座直震卡料或缺料料报警"))
                                {
                                    lbEquipmentInfo.Items.Add("排针座直震卡料或缺料料报警");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("排针座直震卡料或缺料料报警"))
                                {
                                    lbEquipmentInfo.Items.Remove("排针座直震卡料或缺料料报警");
                                }
                            }

                            //电源座直震卡料或缺料料报警Q7.1
                            if (FrmMain.IO.Q_LackPFAlm1)
                            {
                                if (!lbEquipmentInfo.Items.Contains("电源座直震卡料或缺料料报警"))
                                {
                                    lbEquipmentInfo.Items.Add("电源座直震卡料或缺料料报警");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("电源座直震卡料或缺料料报警"))
                                {
                                    lbEquipmentInfo.Items.Remove("电源座直震卡料或缺料料报警");
                                }
                            }

                            //A轨贴装位PCB板被拿走Q7.2
                            if (FrmMain.IO.Q_PickUpAPCBAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("A轨贴装位PCB板被拿走"))
                                {
                                    lbEquipmentInfo.Items.Add("A轨贴装位PCB板被拿走");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("A轨贴装位PCB板被拿走"))
                                {
                                    lbEquipmentInfo.Items.Remove("A轨贴装位PCB板被拿走");
                                }
                            }

                            //B轨贴装位PCB板被拿走Q7.3
                            if (FrmMain.IO.Q_PickUpBPCBAlm)
                            {
                                if (!lbEquipmentInfo.Items.Contains("B轨贴装位PCB板被拿走"))
                                {
                                    lbEquipmentInfo.Items.Add("B轨贴装位PCB板被拿走");
                                }
                                almAdd++;
                            }
                            else
                            {
                                if (lbEquipmentInfo.Items.Contains("B轨贴装位PCB板被拿走"))
                                {
                                    lbEquipmentInfo.Items.Remove("B轨贴装位PCB板被拿走");
                                }
                            }
                        }
                        #endregion

                        #region 机器人报警

                        if (!showAlarmNC(Global.RobotReady, "与机器人断开连接"))//调用函数判断有无连接机器人，无则不进行下列判断，有才判断下列报警
                        {
                            showAlarmNO(FrmMain.IO.R_G1DLTimeOut, "机器人A爪下磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G2DLTimeOut, "机器人B爪下磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G3DLTimeOut, "机器人C爪下磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G4DLTimeOut, "机器人D爪下磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G1ULTimeOut, "机器人A爪上磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G2ULTimeOut, "机器人B爪上磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G3ULTimeOut, "机器人C爪上磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_G4ULTimeOut, "机器人D爪上磁感检测超时");
                            showAlarmNO(FrmMain.IO.R_CamPCBA01NG, "PCB相机A轨mark点1NG");
                            showAlarmNO(FrmMain.IO.R_CamPCBA02NG, "PCB相机A轨mark点2NG");
                            showAlarmNO(FrmMain.IO.R_CamPCBB01NG, "PCB相机B轨mark点1NG");
                            showAlarmNO(FrmMain.IO.R_CamPCBB02NG, "PCB相机B轨mark点2NG");
                            showAlarmNO(FrmMain.IO.R_CamPCBA01Out, "PCB相机A轨mark点1视觉超出范围");
                            showAlarmNO(FrmMain.IO.R_CamPCBA02Out, "PCB相机A轨mark点2视觉超出范围");
                            showAlarmNO(FrmMain.IO.R_CamPCBB01Out, "PCB相机B轨mark点1视觉超出范围");
                            showAlarmNO(FrmMain.IO.R_CamPCBB02Out, "PCB相机B轨mark点2视觉超出范围");
                            showAlarmNO(FrmMain.IO.R_CamMM1NG, "物料相机拍摄物料1NG");
                            showAlarmNO(FrmMain.IO.R_CamMM2NG, "物料相机拍摄物料2NG");
                            showAlarmNO(FrmMain.IO.R_CamMM3NG, "物料相机拍摄物料3NG");
                            showAlarmNO(FrmMain.IO.R_CamMM4NG, "物料相机拍摄物料4NG");
                        }

                        #endregion

                        if (almAdd > 0)
                        {
                            lbEquipmentInfo.ForeColor = Color.Red;
                        }
                        else
                        {
                            lbEquipmentInfo.Items.Add("正常");
                            lbEquipmentInfo.ForeColor = Color.Green;
                        }
                        almAdd = 0;
                    }));
                    Thread.Sleep(100);
                }
            });
        }
        /// <summary>
        /// 报警信息显示函数(常开)
        /// </summary>
        /// <param name="Var">报警变量</param>
        /// <param name="AlarmInfo">报警显示信息</param>
        private bool showAlarmNO(bool Var, string AlarmInfo)
        {
            if (Var)
            {
                if (!lbEquipmentInfo.Items.Contains(AlarmInfo))
                {
                    lbEquipmentInfo.Items.Add(AlarmInfo);
                }
                almAdd++;
                return true;
            }
            else
            {
                if (lbEquipmentInfo.Items.Contains(AlarmInfo))
                {
                    lbEquipmentInfo.Items.Remove(AlarmInfo);
                }
                return false;
            }
        }

        /// <summary>
        /// 报警信息显示函数(常闭)
        /// </summary>
        /// <param name="Var">报警变量</param>
        /// <param name="AlarmInfo">报警显示信息</param>
        private bool showAlarmNC(bool Var, string AlarmInfo)
        {
            if (!Var)
            {
                if (!lbEquipmentInfo.Items.Contains(AlarmInfo))
                {
                    lbEquipmentInfo.Items.Add(AlarmInfo);
                }
                almAdd++;
                return true;
            }
            else
            {
                if (lbEquipmentInfo.Items.Contains(AlarmInfo))
                {
                    lbEquipmentInfo.Items.Remove(AlarmInfo);
                }
                return false;
            }
        }
        #endregion

        #region 手自动切换按钮
        private void btnMode_Click(object sender, EventArgs e)
        {
            if (Global.ConnectSuccess1)
            {
                if (FrmMain.smart200_1.ReadBool(Global.M_AutoMode))
                {
                    if (MessageBox.Show("是否切换至手动模式？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MyLog.Common.myLog.writeAlmContent("切换至手动模式", Global.UserPermission, Global.UserPermission);
                        FrmMain.smart200_1.Write(Global.M_AutoMode, false);
                        //暂停机器人
                        FrmIniRobot.RobotSpel.Pause();

                        //手动运行时显示相机选择并设置视觉控件显示上相机
                        spelVideoViewDown.Camera = 1;
                    }
                }
                else
                {
                    if (MessageBox.Show("是否切换至自动模式？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MyLog.Common.myLog.writeAlmContent("切换至自动模式", Global.UserPermission, Global.UserPermission);
                        FrmMain.smart200_1.Write(Global.M_AutoMode, true);

                        //自动运行时屏蔽相机选择并设置视觉控件显示任一相机
                        spelVideoViewDown.Camera = 0;
                    }
                }
            }
            else
            {
                MessageBox.Show("PLC已断开连接，请重新连接后再试！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnMode_Click_1(object sender, EventArgs e)
        {
            if (Global.ConnectSuccess1)
            {
                if (FrmMain.smart200_1.ReadBool(Global.M_AutoMode))
                {
                    if (MessageBox.Show("是否切换至手动模式？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MyLog.Common.myLog.writeAlmContent("切换至手动模式", Global.UserPermission, Global.UserPermission);
                        FrmMain.smart200_1.Write(Global.M_AutoMode, false);
                        //暂停机器人
                        FrmIniRobot.RobotSpel.Pause();

                        //手动运行时显示相机选择并设置视觉控件显示上相机
                        spelVideoViewDown.Camera = 1;
                    }
                }
                else
                {
                    if (MessageBox.Show("是否切换至自动模式？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MyLog.Common.myLog.writeAlmContent("切换至自动模式", Global.UserPermission, Global.UserPermission);
                        FrmMain.smart200_1.Write(Global.M_AutoMode, true);

                        //自动运行时屏蔽相机选择并设置视觉控件显示任一相机
                        spelVideoViewDown.Camera = 0;
                    }
                }
            }
            else
            {
                MessageBox.Show("PLC已断开连接，请重新连接后再试！", "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region 视觉检测画面显示启用
        private void btnVision_Click(object sender, EventArgs e)
        {
            if (spelVideoViewDown.VideoEnabled == false)
            {
                spelVideoViewDown.VideoEnabled = true;
                spelVideoViewDown.GraphicsEnabled = true;
                spelVideoViewUp.VideoEnabled = true;
                spelVideoViewUp.GraphicsEnabled = true;
                btnVision.Text = "视觉启用";
            }
            else
            {
                spelVideoViewDown.VideoEnabled = false;
                spelVideoViewDown.GraphicsEnabled = false;
                spelVideoViewUp.VideoEnabled = false;
                spelVideoViewUp.GraphicsEnabled = false;
                btnVision.Text = "视觉关闭";
            }

        }

        private void btnVision_Click_1(object sender, EventArgs e)
        {
            if (spelVideoViewDown.VideoEnabled == false)
            {
                spelVideoViewDown.VideoEnabled = true;
                spelVideoViewDown.GraphicsEnabled = true;
                spelVideoViewUp.VideoEnabled = true;
                spelVideoViewUp.GraphicsEnabled = true;
                btnVision1.BackgroundImage = listImage[7];
            }
            else
            {
                spelVideoViewDown.VideoEnabled = false;
                spelVideoViewDown.GraphicsEnabled = false;
                spelVideoViewUp.VideoEnabled = false;
                spelVideoViewUp.GraphicsEnabled = false;
                btnVision1.BackgroundImage = listImage[6];
            }
        }
        #endregion

        #region 产品切换
        private void cobAProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (/*Global.ConnectSuccess1 && Global.ConnectSuccess2 &&*/ Global.RobotReady)//判断PLC与机器人连接状态
            {
                //if (FrmMain.smart200_1.ReadBool(Global.M_ManualMode))
                //{
                string cobName = cobAProductName.Text.Trim();
                if (Global.AProductName != cobName)
                {
                    Global.AProductName = cobName;
                    Global.ReadProductINI();
                    Global.DownloadRobotParameter();
                    Global.ReadRobotPoint();
                    Global.DownloadRobotPoint();
                }
                //}
                //else
                //{
                //    //自动运行不允许切换提示
                //}
            }
            else if (!Global.ConnectSuccess1)
            {
                //未连接提示
            }
            else if (!Global.ConnectSuccess2)
            {
                //未连接提示
            }
            else if (!Global.RobotReady)
            {
                //未连接提示
            }
        }

        /// <summary>
        /// 遍历A轨产品参数文件夹获取产品名称
        /// </summary>
        public void ErgodicAProduct()
        {
            if (cobAProductName.InvokeRequired)
            {
                ErgodicAProductDelegate fc = new ErgodicAProductDelegate(ErgodicAProduct);
                this.Invoke(fc);
            }
            else
            {
                string path = Application.StartupPath + @"\Config\A";//产品参数保存路径
                var files = Directory.GetFiles(path, "*.ini");//获取产品路径下所有ini文件名称
                cobAProductName.Items.Clear();
                cobAProductName.Items.Add("未选择产品");
                string temp;
                for (int i = 0; i < files.Length; i++)
                {
                    temp = files[i].Substring(path.Length, files[i].Length - path.Length).Trim('\\');
                    cobAProductName.Items.Add(temp.Substring(0, temp.Length - 4));
                }
                if (cobAProductName.Items.IndexOf(Global.AProductName) != -1)//判断集合中有无当前产品
                {
                    cobAProductName.SelectedIndex = cobAProductName.Items.IndexOf(Global.AProductName);
                }
                else
                {
                    cobAProductName.SelectedIndex = 0;
                }
            }
        }
        private void cobBProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (/*Global.ConnectSuccess1 && Global.ConnectSuccess2 &&*/ Global.RobotReady)//判断PLC与机器人连接状态
            {
                //if (FrmMain.smart200_1.ReadBool(Global.M_ManualMode))
                //{
                string cobName = cobBProductName.Text.Trim();
                if (Global.BProductName != cobName)
                {
                    Global.BProductName = cobName;
                    Global.ReadProductINI();
                    Global.DownloadRobotParameter();
                    Global.ReadRobotPoint();
                    Global.DownloadRobotPoint();
                }
                //}
                //else
                //{
                //    //自动运行不允许切换提示
                //}
            }
            else if (!Global.ConnectSuccess1)
            {
                //未连接提示
            }
            else if (!Global.ConnectSuccess2)
            {
                //未连接提示
            }
            else if (!Global.RobotReady)
            {
                //未连接提示
            }
        }
        /// <summary>
        /// 遍历B轨产品参数文件夹获取产品名称
        /// </summary>
        public void ErgodicBProduct()
        {
            if (cobBProductName.InvokeRequired)
            {
                ErgodicBProductDelegate fc = new ErgodicBProductDelegate(ErgodicBProduct);
                this.Invoke(fc);
            }
            else
            {
                string path = Application.StartupPath + @"\Config\B";//产品参数保存路径
                var files = Directory.GetFiles(path, "*.ini");//获取产品路径下所有ini文件名称
                cobBProductName.Items.Clear();
                cobBProductName.Items.Add("未选择产品");
                string temp;
                for (int i = 0; i < files.Length; i++)
                {
                    temp = files[i].Substring(path.Length, files[i].Length - path.Length).Trim('\\');
                    cobBProductName.Items.Add(temp.Substring(0, temp.Length - 4));
                }
                if (cobBProductName.Items.IndexOf(Global.BProductName) != -1)//判断集合中有无当前产品
                {
                    cobBProductName.SelectedIndex = cobBProductName.Items.IndexOf(Global.BProductName);
                }
                else
                {
                    cobBProductName.SelectedIndex = 0;
                }
            }
        }
        #endregion

        #region 产品信息统计
        /// <summary>
        /// A轨产品统计清零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAClear_Click(object sender, EventArgs e)
        {
            //记录当前时间
            string time = DateTime.Now.ToString();
            ShowAProductStartTime(time);
            Global.AProductStartTime = time;
            Global.AProductINI.IniWriteValue("统计信息", "开始时间", Global.AProductStartTime);
            //清零生产总量
            Global.AProductNums = 0;
            ShowAProductNums(Global.AProductNums);
            Global.AProductINI.IniWriteValue("统计信息", "生产数量", "0");
            //归零生产节拍
            ShowAProductBeat("0");
            Common.myLog.writeAlmContent("A轨生产信息数据清空", Global.UserPermission, Global.UserPermission);
        }
        /// <summary>
        /// B轨产品统计清零
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBClear_Click(object sender, EventArgs e)
        {
            //记录当前时间
            string time = DateTime.Now.ToString();
            ShowBProductStartTime(time);
            Global.BProductStartTime = time;
            Global.BProductINI.IniWriteValue("统计信息", "开始时间", Global.BProductStartTime);
            //清零生产总量
            Global.BProductNums = 0;
            ShowBProductNums(Global.BProductNums);
            Global.BProductINI.IniWriteValue("统计信息", "生产数量", "0");
            //归零生产节拍
            ShowBProductBeat("0");
            Common.myLog.writeAlmContent("B轨生产信息数据清空", Global.UserPermission, Global.UserPermission);
        }
        /// <summary>
        /// 加载统计信息至控件显示
        /// </summary>
        private void ShowProductStatistics()
        {
            ShowAProductStartTime(Global.AProductStartTime);
            ShowAProductNums(Global.AProductNums);
            ShowAProductBeat(Global.AProductBeat);
            ShowBProductStartTime(Global.BProductStartTime);
            ShowBProductNums(Global.BProductNums);
            ShowBProductBeat(Global.BProductBeat);
        }
        /// <summary>
        /// 刷新A轨产品开始时间
        /// </summary>
        /// <param name="time"></param>
        private void ShowAProductStartTime(string time)
        {
            lblAProductStartTime.Text = time + "  开始";
        }
        /// <summary>
        /// 刷新A轨产品生产数量
        /// </summary>
        /// <param name="num"></param>
        private void ShowAProductNums(ulong num)
        {
            lblAProductNums.Text = num.ToString() + "  块";
        }
        /// <summary>
        /// 刷新A轨产品生产节拍
        /// </summary>
        /// <param name="beat"></param>
        private void ShowAProductBeat(string beat)
        {
            lblAProductBeat.Text = beat + "  s/p";
        }
        /// <summary>
        /// 刷新B轨产品开始时间
        /// </summary>
        /// <param name="time"></param>
        private void ShowBProductStartTime(string time)
        {
            lblBProductStartTime.Text = time + "  开始";
        }
        /// <summary>
        /// 刷新B轨产品生产数量
        /// </summary>
        /// <param name="num"></param>
        private void ShowBProductNums(ulong num)
        {
            lblBProductNums.Text = num.ToString() + "  块";
        }
        /// <summary>
        /// 刷新B轨产品生产节拍
        /// </summary>
        /// <param name="beat"></param>
        private void ShowBProductBeat(string beat)
        {
            lblBProductBeat.Text = beat + "  s/p";
        }
        #endregion

        #region 无PLC，下发交互点给机器人按钮
        private void btnRobotStart_Click(object sender, EventArgs e)
        {
            FrmIniRobot.RobotStar(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmIniRobot.RobotSpel.MemOn("A_Arrive");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmIniRobot.RobotSpel.MemOn("L1_AllowGet");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmIniRobot.RobotSpel.MemOn("L2_AllowGet");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FrmIniRobot.RobotSpel.MemOn("L3_AllowGet");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Global.AProductNums++;
            Global.AProductINI.IniWriteValue("统计信息", "生产数量", Global.AProductNums.ToString());
            ShowAProductNums(Global.AProductNums);
        }
        public Stopwatch AProductBeatStopwatch = new Stopwatch();
        private void button9_Click(object sender, EventArgs e)
        {
            AProductBeatStopwatch.Restart();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowAProductBeat(AProductBeatStopwatch.Elapsed.TotalSeconds.ToString("0.00"));
            AProductBeatStopwatch.Reset();
        }

        #endregion

        
    }
}
