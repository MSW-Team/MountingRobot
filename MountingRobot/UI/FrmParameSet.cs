using MountingRobot.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MountingRobot.UI
{
    public partial class FrmParameSet : Form
    {
        public FrmParameSet()
        {
            InitializeComponent();
        }

        private void FrmParameSet_Load(object sender, EventArgs e)
        {
            loadControlValue();
            lblMessage.Text = "";
        }

        private void FrmParameSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerMessage.Enabled == true)
            {
                timerMessage.Stop();
                timerMessage.Enabled = false;
            }
        }

        #region 加载参数至页面显示
        private void loadControlValue()
        {
            //机器人
            nudRobotGrapDelay.Value = Global.RobotGripDelay;
            nudCamDelay.Value = Global.CamDelay;
            nudRobotAutoSpeed.Value = Convert.ToDecimal(Global.RobotAutoSpeed);
            nudRobotManualSpeed.Value = Convert.ToDecimal(Global.RobotManualSpeed);
            nudRobotMoveSpeed.Value = Convert.ToDecimal(Global.RobotMoveSpeed);
            nudRobotSafeHeight.Value = Convert.ToDecimal(Global.RobotSafeHeight);
            nudCamNGLimit.Value = Convert.ToDecimal(Global.CamNGlimit);
            chkACamDownEnable.Checked = Global.CamADownEnable;
            chkACamUpEnable.Checked = Global.CamAUpEnable;
            chkBCamDownEnable.Checked = Global.CamBDownEnable;
            chkBCamUpEnable.Checked = Global.CamBUpEnable;
            chkNGPhotoSaveEanble.Checked = Global.PhotoNGSaveEnable;
            chkPassPhotoSaveEnable.Checked = Global.PhotoPassSaveEnable;
            chkCamDownExceedEnable.Checked = Global.CamDownExceedEnable;
            txtCamProductSelect.Text = Global.CamProductSelect;
            txtPhotoSavePath.Text = Global.PhotoSavePath;
            txtRobotProPath.Text = Global.RobotProPath;

            //PLC
            txtPLC1_IP.Text = Global.PLC1_IP;
            txtPLC2_IP.Text = Global.PLC2_IP;

            if (Global.ConnectSuccess1 && Global.ConnectSuccess2)
            {
                lblConnectStatus1.BackColor = Color.Lime;
                lblConnectStatus2.BackColor = Color.Lime;
                btnOpenConnect.Text = "关闭通讯";
            }
            else if (Global.ConnectSuccess1 && !Global.ConnectSuccess2)
            {
                lblConnectStatus1.BackColor = Color.Lime;
                lblConnectStatus2.BackColor = Color.FromName("ScrollBar");
                btnOpenConnect.Text = "关闭通讯";
            }
            else if (!Global.ConnectSuccess1 && Global.ConnectSuccess2)
            {
                lblConnectStatus1.BackColor = Color.FromName("ScrollBar");
                lblConnectStatus2.BackColor = Color.Lime;
                btnOpenConnect.Text = "关闭通讯";
            }
            else
            {
                lblConnectStatus1.BackColor = Color.FromName("ScrollBar");
                lblConnectStatus2.BackColor = Color.FromName("ScrollBar");
                btnOpenConnect.Text = "打开通讯";
            }



            nudAInDelay.Value = Global.AInDelay;
            nudAArriveDelay.Value = Global.AArriveDelay;
            nudAOutDelay.Value = Global.AOutDelay;
            nudAOutCloseDelay.Value = Global.AOutCloseDelay;
            nudAEnterDelay.Value = Global.AEnterDelay;
            nudARiseDelay.Value = Global.ARiseDelay;
            nudAAnastoleDelay.Value = Global.AAnastolDelay;
            nudBInDelay.Value = Global.BInDelay;
            nudBArriveDelay.Value = Global.BArriveDelay;
            nudBOutDelay.Value = Global.BOutDelay;
            nudBOutCloseDelay.Value = Global.BOutCloseDelay;
            nudBEnterDelay.Value = Global.BEnterDelay;
            nudBRiseDelay.Value = Global.BRiseDelay;
            nudBAnastoleDelay.Value = Global.BAnastolDelay;
            nudPFFullDelay.Value = Global.PFFullDelay;
            nudNFFullDelay.Value = Global.NFFullDelay;

            if (Global.AByPassCk)
            {
                rdbANormalRun.Checked = true;
            }
            else
            {
                rdbAByPass.Checked = true;
            }

            if (Global.BByPassCk)
            {
                rdbBNormalRun.Checked = true;
            }
            else
            {
                rdbBByPass.Checked = true;
            }

            //if (Global.AConNext)
            //{
            //    ckbAConNext.Checked = true;
            //}
            //else
            //{
            //    ckbAConNext.Checked = false;
            //}

            //if (Global.BConNext)
            //{
            //    ckbBConNext.Checked = true;
            //}
            //else
            //{
            //    ckbBConNext.Checked = false;
            //}

            if (Global.OrbitalCount == "单轨")
            {
                cobOrbitalCount.SelectedIndex = 0;
            }
            else
            {
                cobOrbitalCount.SelectedIndex = 1;
            }

            chbAlarmFlashEnable.Checked = Global.AlarmFlashEnable;
        }
        #endregion        

        #region 保存plc参数
        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePLCParamer();
        }

        private void SavePLCParamer()
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + @"\SysParame"))  //判断是否存在SysParame文件夹，不存在则自行创建
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\SysParame");
                }
                

                Global.AProductINI.IniWriteValue("PLC参数", "电源座有料延时", nudPFFullDelay.Value.ToString());
                Global.PFFullDelay = nudPFFullDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "排针座有料延时", nudNFFullDelay.Value.ToString());
                Global.NFFullDelay = nudNFFullDelay.Value;

                Global.AProductINI.IniWriteValue("其它参数", "轨道数", cobOrbitalCount.Text);
                Global.OrbitalCount = cobOrbitalCount.Text;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨入口光电延时", nudAInDelay.Value.ToString());
                Global.AInDelay = nudAInDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨到位光电延时", nudAArriveDelay.Value.ToString());
                Global.AArriveDelay = nudAArriveDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨出口光电延时", nudAOutDelay.Value.ToString());
                Global.AOutDelay = nudAOutDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨出口光电关电延时", nudAOutCloseDelay.Value.ToString());
                Global.AOutCloseDelay = nudAOutCloseDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨进板延时", nudAEnterDelay.Value.ToString());
                Global.AEnterDelay = nudAEnterDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨顶升延时", nudARiseDelay.Value.ToString());
                Global.ARiseDelay = nudARiseDelay.Value;

                Global.AProductINI.IniWriteValue("PLC参数", "A轨缩回延时", nudAAnastoleDelay.Value.ToString());
                Global.AAnastolDelay = nudAAnastoleDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨入口光电延时", nudBInDelay.Value.ToString());
                Global.BInDelay = nudBInDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨到位光电延时", nudBArriveDelay.Value.ToString());
                Global.BArriveDelay = nudBArriveDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨出口光电延时", nudBOutDelay.Value.ToString());
                Global.BOutDelay = nudBOutDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨出口光电关电延时", nudBOutCloseDelay.Value.ToString());
                Global.BOutCloseDelay = nudBOutCloseDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨进板延时", nudBEnterDelay.Value.ToString());
                Global.BEnterDelay = nudBEnterDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨顶升延时", nudBRiseDelay.Value.ToString());
                Global.BRiseDelay = nudBRiseDelay.Value;

                Global.BProductINI.IniWriteValue("PLC参数", "B轨缩回延时", nudBAnastoleDelay.Value.ToString());
                Global.BAnastolDelay = nudBAnastoleDelay.Value;


                if (Global.ConnectSuccess1)  //参数写到PLC中
                {
                    FrmMain.smart200_1.Write(Global.M_AThroughMode, rdbAByPass.Checked);
                    FrmMain.smart200_1.Write(Global.M_BThroughMode, rdbBByPass.Checked);
                    //FrmMain.smart200_1.Write("Q9.2", ckbAConNext.Checked);
                    //FrmMain.smart200_1.Write("Q9.3", ckbBConNext.Checked);
                    //FrmMain.smart200_1.Write("V100", Convert.ToUInt16(Global.PFFullDelay));
                    //FrmMain.smart200_1.Write("V102", Convert.ToUInt16(Global.NFFullDelay));

                    FrmMain.smart200_1.Write(Global.VW_AInDelay, Convert.ToUInt16(Global.AInDelay));
                    FrmMain.smart200_1.Write(Global.VW_AArriveDelay, Convert.ToUInt16(Global.AArriveDelay));
                    FrmMain.smart200_1.Write(Global.VW_AOutDelay, Convert.ToUInt16(Global.AOutDelay));
                    FrmMain.smart200_1.Write(Global.VW_AOutCloseDelay, Convert.ToUInt16(Global.AOutCloseDelay));
                    FrmMain.smart200_1.Write(Global.VW_AEnterDelay, Convert.ToUInt16(Global.AEnterDelay));
                    FrmMain.smart200_1.Write(Global.VW_ARiseDelay, Convert.ToUInt16(Global.ARiseDelay));
                    FrmMain.smart200_1.Write(Global.VW_AAnastoleDelay, Convert.ToUInt16(Global.AAnastolDelay));
                    FrmMain.smart200_1.Write(Global.VW_BInDelay, Convert.ToUInt16(Global.BInDelay));
                    FrmMain.smart200_1.Write(Global.VW_BArriveDelay, Convert.ToUInt16(Global.BArriveDelay));
                    FrmMain.smart200_1.Write(Global.VW_BOutDelay, Convert.ToUInt16(Global.BOutDelay));
                    FrmMain.smart200_1.Write(Global.VW_BOutCloseDelay, Convert.ToUInt16(Global.BOutCloseDelay));
                    FrmMain.smart200_1.Write(Global.VW_BEnterDelay, Convert.ToUInt16(Global.BEnterDelay));
                    FrmMain.smart200_1.Write(Global.VW_BRiseDelay, Convert.ToUInt16(Global.BRiseDelay));
                    FrmMain.smart200_1.Write(Global.VW_BAnastoleDelay, Convert.ToUInt16(Global.BAnastolDelay));


                }
                showMeesage("PLC参数保存成功", 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                showMeesage("PLC参数保存失败", 1);
            }
        }
        #endregion            

        #region 通讯按钮事件
        private void btnOpenConnect_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Invoke(new Action(() =>
                {
                    btnOpenConnect.Enabled = false;
                }));
                if (btnOpenConnect.Text == "关闭通讯")  //关闭PLC通讯
                {
                    FrmMain.smart200_1.ConnectClose();
                    Invoke(new Action(() =>
                    {
                        FrmMain.IO.PLCReceiveStart = false;  //关闭IO刷新
                        lblConnectStatus1.BackColor = Color.FromName("ScrollBar");
                        Global.ConnectSuccess1 = false;
                        btnOpenConnect.Text = "打开通讯";
                    }));

                    FrmMain.smart200_2.ConnectClose();
                    Invoke(new Action(() =>
                    {

                        lblConnectStatus2.BackColor = Color.FromName("ScrollBar");
                        Global.ConnectSuccess2 = false;
                    }));
                }
                else  //打开PLC通讯
                {
                    if (FrmMain.smart200_1.Connect(txtPLC1_IP.Text))
                    {
                        Invoke(new Action(() =>
                        {
                            btnOpenConnect.Text = "关闭通讯";
                            lblConnectStatus1.BackColor = Color.Lime;
                            Global.ConnectSuccess1 = true;

                            #region 刷IO
                            FrmMain.IO.PLCReceiveStart = true;
                            FrmMain.IO.ReadInput(30);
                            FrmMain.IO.ReadOutput(40);
                            #endregion

                            #region  写入参数至PLC
                            //FrmMain.smart200_1.Write("Q11.6", Global.AByPassCk);
                            //FrmMain.smart200_1.Write("Q11.7", Global.BByPassCk);
                            //FrmMain.smart200_1.Write("Q9.2", Global.AConNext);
                            //FrmMain.smart200_1.Write("Q9.3", Global.BConNext);
                            //FrmMain.smart200_1.Write("V100", Convert.ToUInt16(Global.PFFullDelay));
                            //FrmMain.smart200_1.Write("V102", Convert.ToUInt16(Global.NFFullDelay));
                            #endregion
                        }));
                    }


                    if (FrmMain.smart200_2.Connect(txtPLC2_IP.Text))
                    {
                        Invoke(new Action(() =>
                        {
                            btnOpenConnect.Text = "关闭通讯";
                            lblConnectStatus2.BackColor = Color.Lime;
                            Global.ConnectSuccess2 = true;
                        }));
                    }

                    if (Global.ConnectSuccess1 && Global.ConnectSuccess2)
                    {
                        showMeesage("连接1号、2号PLC成功！", 0);
                    }
                    else if (Global.ConnectSuccess2 && !Global.ConnectSuccess1)
                    {
                        showMeesage("连接1号PLC失败！", 1);
                    }
                    else if (!Global.ConnectSuccess2 && Global.ConnectSuccess1)
                    {
                        showMeesage("连接2号PLC失败！", 1);
                    }
                    else
                    {
                        showMeesage("连接1号、2号PLC失败！", 1);
                    }
                }
                Invoke(new Action(() =>
                {
                    btnOpenConnect.Enabled = true;
                }));
            });
        }
        #endregion

        #region 浏览机器人项目文件路径与图像保存路径
        private void btnSelectPro_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择机器人项目路径";
            openFileDialog.Filter = "机器人项目文件（*.sprj）|*.sprj";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtRobotProPath.Text = openFileDialog.FileName;
            }
        }

        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Title = "选择图像保存文件夹路径";
            //openFileDialog.Filter = "所有文件(*.*)|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    txtPhotoSavePath.Text = openFileDialog.FileName;
            //}
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtPhotoSavePath.Text = dialog.SelectedPath;
            }

        }
        #endregion

        #region 机器人参数保存
        private void btnRobotSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否保存数据", "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (updataRobotValue())
                {
                    WriteRobotIni();

                }
            }
        }
        /// <summary>
        /// 更新机器人全局变量
        /// </summary>
        /// <returns></returns>
        private bool updataRobotValue()
        {
            try
            {
                Global.RobotGripDelay = nudRobotGrapDelay.Value;
                Global.CamDelay = nudCamDelay.Value;
                Global.RobotAutoSpeed = Convert.ToInt32(nudRobotAutoSpeed.Value);
                Global.RobotManualSpeed = Convert.ToInt32(nudRobotManualSpeed.Value);
                Global.RobotMoveSpeed = Convert.ToInt32(nudRobotMoveSpeed.Value);
                Global.RobotSafeHeight = nudRobotSafeHeight.Value;
                Global.CamNGlimit = Convert.ToInt32(nudCamNGLimit.Value);
                Global.CamADownEnable = chkACamDownEnable.Checked;
                Global.CamAUpEnable = chkACamUpEnable.Checked;
                Global.CamBDownEnable = chkBCamDownEnable.Checked;
                Global.CamBUpEnable = chkBCamUpEnable.Checked;
                Global.PhotoNGSaveEnable = chkNGPhotoSaveEanble.Checked;
                Global.CamDownExceedEnable = chkCamDownExceedEnable.Checked;
                Global.PhotoPassSaveEnable = chkPassPhotoSaveEnable.Checked;
                Global.CamProductSelect = txtCamProductSelect.Text;
                Global.PhotoSavePath = txtPhotoSavePath.Text;
                Global.RobotProPath = txtRobotProPath.Text;
                return true;
            }
            catch
            {
                showMeesage("保存失败！", 1);
                return false;
            }
        }
        /// <summary>
        /// 将机器人参数写入INI文件
        /// </summary>
        private void WriteRobotIni()
        {
            try
            {
                Global.AProductINI.IniWriteValue("机器人参数", "机器人夹爪延时", Global.RobotGripDelay.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "拍照延时", Global.CamDelay.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "机器人自动速度", Global.RobotAutoSpeed.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "机器人手动速度", Global.RobotManualSpeed.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "机器人插补速度", Global.RobotMoveSpeed.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "机器人安全高度", Global.RobotSafeHeight.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "视觉允许连续NG次数", Global.CamNGlimit.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "A轨向下相机启用", Global.CamADownEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "A轨向上相机启用", Global.CamAUpEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "B轨向下相机启用", Global.CamBDownEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "B轨向上相机启用", Global.CamBUpEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "视觉保存NG图像功能", Global.PhotoNGSaveEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "视觉保存正常图像功能", Global.PhotoPassSaveEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "向下相机超限报警功能", Global.CamDownExceedEnable.ToString());
                Global.AProductINI.IniWriteValue("机器人参数", "视觉调用子程序前缀", Global.CamProductSelect);
                Global.AProductINI.IniWriteValue("机器人参数", "图像保存路径", Global.PhotoSavePath);
                Global.SystemINI.IniWriteValue("机器人参数", "机器人项目文件路径", Global.RobotProPath);

                showMeesage("保存成功！", 0);

                downloadRobotVar();//参数保存成功后，下载至机器人
            }
            catch
            {
                showMeesage("保存失败！", 1);
            }
        }
        /// <summary>
        /// 下载至机器人
        /// </summary>
        private void downloadRobotVar()
        {
            try
            {
                if (Global.RobotReady)
                {
                    FrmIniRobot.RobotSpel.SetVar("GrapDelayTime", Global.RobotGripDelay / 1000);
                    FrmIniRobot.RobotSpel.SetVar("CamDelay", Global.CamDelay / 1000);
                    FrmIniRobot.RobotSpel.SetVar("AutoSpeed", Global.RobotAutoSpeed);
                    FrmIniRobot.RobotSpel.SetVar("ManualSpeed", Global.RobotManualSpeed);
                    FrmIniRobot.RobotSpel.SetVar("MoveSpeed", Global.RobotMoveSpeed);
                    FrmIniRobot.RobotSpel.SetVar("zh", Global.RobotSafeHeight);
                    FrmIniRobot.RobotSpel.SetVar("CamNGLimit", Global.CamNGlimit);
                    FrmIniRobot.RobotSpel.SetVar("ProductVNSelect$", Global.CamProductSelect);
                    FrmIniRobot.RobotSpel.SetVar("PhotoSavePath$", Global.PhotoSavePath);
                    FrmIniRobot.RobotSendBool("CamUpAEnable", Global.CamAUpEnable);
                    FrmIniRobot.RobotSendBool("CamDownAEnable", Global.CamADownEnable);
                    FrmIniRobot.RobotSendBool("CamUpBEnable", Global.CamBUpEnable);
                    FrmIniRobot.RobotSendBool("CamDownBEnable", Global.CamBDownEnable);
                    FrmIniRobot.RobotSendBool("PhotoPassSaveEnable", Global.PhotoPassSaveEnable);
                    FrmIniRobot.RobotSendBool("PhotoNGSaveEnable", Global.PhotoNGSaveEnable);
                    FrmIniRobot.RobotSendBool("CamDownExceedEnable", Global.CamDownExceedEnable);

                    showMeesage("下载至机器人成功", 0);
                }
                else
                {
                    showMeesage("参数已保存，机器人未连接，未下载！", 1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                showMeesage("下载至机器人失败", 1);
            }
        }

        /// <summary>
        /// 保存修改下发机器人速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveRobotSpeed_Click(object sender, EventArgs e)
        {
            //更新全局变量
            Global.RobotAutoSpeed = Convert.ToInt32(nudRobotAutoSpeed.Value);
            Global.RobotManualSpeed = Convert.ToInt32(nudRobotManualSpeed.Value);
            Global.RobotMoveSpeed = Convert.ToInt32(nudRobotMoveSpeed.Value);
            //保存至INI文件
            Global.AProductINI.IniWriteValue("机器人参数", "机器人自动速度", Global.RobotAutoSpeed.ToString());
            Global.AProductINI.IniWriteValue("机器人参数", "机器人手动速度", Global.RobotManualSpeed.ToString());
            Global.AProductINI.IniWriteValue("机器人参数", "机器人插补速度", Global.RobotMoveSpeed.ToString());
            //下发至机器人全局保存变量
            FrmIniRobot.RobotSpel.SetVar("AutoSpeed", Global.RobotAutoSpeed);
            FrmIniRobot.RobotSpel.SetVar("ManualSpeed", Global.RobotManualSpeed);
            FrmIniRobot.RobotSpel.SetVar("MoveSpeed", Global.RobotMoveSpeed);
            //实时修改机器人速度
            if (Global.RobotReady)
            {
                if (FrmIniRobot.RobotSpel.MemSw("AutoMode"))
                {
                    FrmIniRobot.RobotSpeed(Global.RobotAutoSpeed, Global.RobotMoveSpeed);
                }
                else
                {
                    FrmIniRobot.RobotSpeed(Global.RobotManualSpeed, Global.RobotMoveSpeed);
                }
            }
            showMeesage("机器人速度下发成功", 1);
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
            Invoke(new Action(() =>
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
            }));
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

        private void btnSystemSave_Click(object sender, EventArgs e)
        {
            Global.AProductINI.IniWriteValue("PLC参数", "PLC1_IP地址", txtPLC1_IP.Text);
            Global.PLC1_IP = txtPLC1_IP.Text;

            Global.AProductINI.IniWriteValue("PLC参数", "PLC2_IP地址", txtPLC2_IP.Text);
            Global.PLC2_IP = txtPLC2_IP.Text;

            Global.AProductINI.IniWriteValue("PLC参数", "A轨运行模式", rdbANormalRun.Checked.ToString());
            Global.AByPassCk = rdbANormalRun.Checked;

            Global.AProductINI.IniWriteValue("PLC参数", "B轨运行模式", rdbBNormalRun.Checked.ToString());
            Global.BByPassCk = rdbBNormalRun.Checked;

            Global.AProductINI.IniWriteValue("其它参数", "报警动画功能", chbAlarmFlashEnable.Checked.ToString());
            Global.AlarmFlashEnable = chbAlarmFlashEnable.Checked;

            //Global.parameINI.IniWriteValue("PLC参数", "A轨连接下位机", ckbAConNext.Checked.ToString());
            //Global.AConNext = ckbAConNext.Checked;

            //Global.parameINI.IniWriteValue("PLC参数", "B轨连接下位机", ckbBConNext.Checked.ToString());
            //Global.BConNext = ckbBConNext.Checked;
        }
    }
}
