using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MountingRobot.BLL;
using MyLog;
using RCAPINet;

namespace MountingRobot.UI
{
    public partial class FrmMain : Form
    {

        public static ErgodicAProductDelegate ErgodicAProductDel;
        public static ErgodicBProductDelegate ErgodicBProductDel;

        #region 全局实例
        public static ConnectionS7 smart200_1 = new ConnectionS7();  //实例S7协议全局对象Smart200
        public static ConnectionS7 smart200_2 = new ConnectionS7();
        public static IO IO = new IO();  //实例IO全局对象
        //public static Spel RobotSpel = new Spel();  //实例机器人控制SDK
        #endregion

        public FrmMain()
        {
            InitializeComponent();
            //界面打开时，强制界面最小尺寸为设置大小，解决界面最小限制
            Size newSize = new Size(1920, 1080);
            this.MaximumSize = this.MinimumSize = newSize;
            this.Size = newSize;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            #region 初始化主界面
            MainMenuBarBtnRGB(0);
            FrmAutoRun frmAutoRun = new FrmAutoRun();
            ErgodicAProductDel += frmAutoRun.ErgodicAProduct;
            ErgodicBProductDel += frmAutoRun.ErgodicBProduct;
            ChangeWindow(frmAutoRun, "FrmAutoRun", true);
            //禁用按钮
            SetControlEnabled(this.btnAgingTest, false);
            this.btnAgingTest.ForeColor = Color.Gray; 

            #endregion

            #region 连接PLC,成功则开始刷新IO
            ConnectToPLC();
            #endregion

            #region 初始化机器人SDK显示状态与事件绑定
            Task.Run(() =>
            {
                if (Global.RobotReady)
                {
                    FrmIniRobot.RobotSpel.EventReceived += new Spel.EventReceivedEventHandler(RobotSpel_EventReceived);  //机器人事件
                    Global.DownloadRobotPoint();
                    Invoke(new Action(() =>  //成功初始化后状态栏显示
                    {
                        tslbRobot.Text = "已连接";
                        tslbRobot.BackColor = Color.Lime;
                        //读取机器人交互信号
                        IO.RobotReceiveStart = true;
                        IO.ReadRobotStatus();

                    }));
                }
                else
                {
                    Invoke(new Action(() =>  //初始化失败更新状态栏
                    {
                        tslbRobot.Text = "未连接";
                        tslbRobot.BackColor = Color.Red;
                    }));
                }
            });
            #endregion

            #region 状态栏刷新
            StatusTools();
            #endregion
        }

        #region 接收机器人事件
        public void RobotSpel_EventReceived(object sender, SpelEventArgs e)
        {
            //Common.myLog.writeRunContent(e.ToString(), Global.UserPermission, Global.UserPermission);
        }
        #endregion

        #region 界面切换方法
        /// <summary>
        /// 界面切换画面
        /// </summary>
        /// <param name="objFrm">传入窗体对象</param>
        /// <param name="FrmName">传入窗体对象名称</param>
        /// <param name="refresh">窗体是否重新刷新</param>
        public void ChangeWindow(Form objFrm, string FrmName, bool refresh)
        {
            try
            {
                foreach (Control item in splitContainer1.Panel1.Controls)
                {
                    if (item is Form)
                    {
                        Form frm = (Form)item;
                        {
                            if (refresh)
                            {
                                frm.Close();
                            }
                            else
                            {
                                if (frm.Name == FrmName)
                                {
                                    return;
                                }
                                else
                                {
                                    frm.Close();
                                }
                            }
                        }
                    }
                }

                objFrm.TopLevel = false;
                objFrm.FormBorderStyle = FormBorderStyle.None;
                objFrm.Parent = splitContainer1.Panel1;
                objFrm.Dock = DockStyle.Fill;
                objFrm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 按钮禁用后可改字体颜色
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int GWL_STYLE = -16;
        public const int WS_DISABLED = 0x8000000;

        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (enabled)
            { SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE)); }
            else
            { SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED | GetWindowLong(c.Handle, GWL_STYLE)); }
        }

        #endregion

        #region 窗体拖动
        //const int WM_NCLBUTTONDOWN = 0xA1;
        //const int HT_CAPTION = 0x2;
        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //private void toolStripMenu_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal)
        //    {
        //        // 移动窗体
        //        this.Capture = false;
        //        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        //    }
        //}

        [System.Runtime.InteropServices.DllImport("user32.dll")]//拖动无窗体的控件
        public static extern bool ReleaseCapture();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private void Start_MouseDown(object sender, MouseEventArgs e)
        {
            //拖动窗体
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }
        #endregion

        #region 主菜单栏按钮颜色变化方法
        private void MainMenuBarBtnRGB(int index)
        {
            //btnAutoRun.BackColor = Color.FromArgb(22, 22, 22);
            //btnParameSet.BackColor = Color.FromArgb(22, 22, 22);
            //btnWatch.BackColor = Color.FromArgb(22, 22, 22);
            //btnManual.BackColor = Color.FromArgb(22, 22, 22);
            //btnLog.BackColor = Color.FromArgb(22, 22, 22);

            //switch (index)
            //{
            //    case 0: btnAutoRun.BackColor = Color.FromArgb(0, 192, 0); break;
            //    case 1: btnParameSet.BackColor = Color.FromArgb(0, 192, 0); break;
            //    case 2: btnManual.BackColor = Color.FromArgb(0, 192, 0); break;
            //    case 3: btnManual.BackColor = Color.FromArgb(0, 192, 0); break;
            //    case 4: btnLog.BackColor = Color.FromArgb(0, 192, 0); break;
            //    default:
            //        break;
            //}

            btnAutoRun.BackgroundImage = Properties.Resources.主页面按钮图标_视觉调试灰;
            btnAutoRun.BackColor = Color.FromArgb(22,22,22);
            btnAutoRun.Tag = 0;
            btnLogin.BackgroundImage = Properties.Resources.主页面按钮图标_用户管理灰;
            btnLogin.BackColor = Color.FromArgb(22, 22, 22);
            btnLogin.Tag = 0;
            btnLog.BackgroundImage = Properties.Resources.主页面按钮图标_日志查询灰;
            btnLog.BackColor = Color.FromArgb(22, 22, 22);
            btnLog.Tag = 0;
            btnAddNew.BackgroundImage = Properties.Resources.主页面按钮图标_产品管理灰;
            btnAddNew.BackColor = Color.FromArgb(22, 22, 22);
            btnAddNew.Tag = 0;
            btnWatch.BackgroundImage = Properties.Resources.主页面按钮图标_IO监控灰;
            btnWatch.BackColor = Color.FromArgb(22, 22, 22);
            btnWatch.Tag = 0;
            btnManual.BackgroundImage = Properties.Resources.主页面按钮图标_手动调试灰;
            btnManual.BackColor = Color.FromArgb(22, 22, 22);
            btnManual.Tag = 0;
            btnParameSet.BackgroundImage = Properties.Resources.主页面按钮图标_系统设置灰;
            btnParameSet.BackColor = Color.FromArgb(22, 22, 22);
            btnParameSet.Tag = 0;
            btnAgingTest.BackgroundImage = Properties.Resources.主页面按钮图标_老化测试灰;
            btnAgingTest.BackColor = Color.FromArgb(22, 22, 22);
            btnAgingTest.Tag = 0;
            btnHelp.BackgroundImage = Properties.Resources.主页面按钮图标_设备帮助灰;
            btnHelp.BackColor = Color.FromArgb(22, 22, 22);
            btnHelp.Tag = 0;

            switch (index)
            {
                case 0: btnAutoRun.BackgroundImage = Properties.Resources.主页面按钮图标_视觉调试蓝;
                    btnAutoRun.BackColor = Color.FromArgb(44, 44, 44);
                    btnAutoRun.Tag = 1;//页面打开标志
                    break;
                case 1: btnLogin.BackgroundImage = Properties.Resources.主页面按钮图标_用户管理蓝;
                    btnLogin.BackColor = Color.FromArgb(44, 44, 44);
                    btnLogin.Tag = 1;
                    break;
                case 2: btnLog.BackgroundImage = Properties.Resources.主页面按钮图标_日志查询蓝;
                    btnLog.BackColor = Color.FromArgb(44, 44, 44);
                    btnLog.Tag = 1;
                    break;
                case 3: btnAddNew.BackgroundImage = Properties.Resources.主页面按钮图标_产品管理蓝;
                    btnAddNew.BackColor = Color.FromArgb(44, 44, 44);
                    btnAddNew.Tag = 1;
                    break;
                case 4: btnWatch.BackgroundImage = Properties.Resources.主页面按钮图标_IO监控蓝;
                    btnWatch.BackColor = Color.FromArgb(44, 44, 44);
                    btnWatch.Tag = 1;
                    break;
                case 5: btnManual.BackgroundImage = Properties.Resources.主页面按钮图标_手动调试蓝;
                    btnManual.BackColor = Color.FromArgb(44, 44, 44);
                    btnManual.Tag = 1;
                    break; 
                case 6: btnParameSet.BackgroundImage = Properties.Resources.主页面按钮图标_系统设置蓝;
                    btnParameSet.BackColor = Color.FromArgb(44, 44, 44);
                    btnParameSet.Tag = 1;
                    break;
                case 7: btnAgingTest.BackgroundImage = Properties.Resources.主页面按钮图标_老化测试蓝;
                    btnAgingTest.BackColor = Color.FromArgb(44, 44, 44);
                    btnAgingTest.Tag = 1;
                    break;
                case 8: btnHelp.BackgroundImage = Properties.Resources.主页面按钮图标_设备帮助蓝;
                    btnHelp.BackColor = Color.FromArgb(44, 44, 44);
                    btnHelp.Tag = 1; break;
            }
        }
        #endregion
        
        #region 菜单栏按钮事件
        private void tsbtnAutoRun_Click(object sender, EventArgs e)  //自动运行界面
        {
            Common.myLog.writeOperateContent("进入自动运行界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(0);
            FrmAutoRun frmAutoRun = new FrmAutoRun();
            ErgodicAProductDel += frmAutoRun.ErgodicAProduct;
            ErgodicBProductDel += frmAutoRun.ErgodicBProduct;
            ChangeWindow(frmAutoRun, "FrmAutoRun", false);
        }
        private void tsbtnParameSet_Click(object sender, EventArgs e)  //参数设置界面
        {
            Common.myLog.writeOperateContent("进入参数设置界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(6);
            ChangeWindow(new FrmParameSet(), "FrmParameSet", false);
        }

        
        private void btnAddNew_Click(object sender, EventArgs e)//产品管理界面
        {
            FrmProduct frmProduct = new FrmProduct();
            frmProduct.ShowDialog();
        }

        private void tsbtnRobotTeach_Click(object sender, EventArgs e)//打开手动下拉栏
        {
            this.ctmsManual.Show(btnManual, new Point(0, 88));
        }
        private void tsmiRobotTeach_Click(object sender, EventArgs e)//机器人示教界面
        {
            Common.myLog.writeOperateContent("进入机器人示教界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(5);
            ChangeWindow(new FrmRobotTeach(), "FrmRobotTeach", false);
        }

        private void tsmiPLCManual_Click(object sender, EventArgs e)//PLC手动界面
        {
            Common.myLog.writeOperateContent("进入PLC手动界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(5);
            ChangeWindow(new FrmManual(), "FrmManual", false);
        }

        private void btnWatch_Click(object sender, EventArgs e)//IO监控界面
        {
            Common.myLog.writeOperateContent("进入IO监控界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(4);
            ChangeWindow(new FrmIOShow(), "FrmIOShow", false);
        }

        private void tsbtnLogin_Click(object sender, EventArgs e)  //登录界面
        {
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
        }

        private void tsbtnLog_Click(object sender, EventArgs e)  //日志
        {
            Common.myLog.writeOperateContent("进入日志记录界面", Global.UserPermission, Global.UserPermission);
            MainMenuBarBtnRGB(2);
            ChangeWindow(new FrmLog(), "FrmLog", false);
        }

        private void tsbtnHelp_Click(object sender, EventArgs e)  //帮助按钮
        {
            FrmInfo frmInfo = new FrmInfo();
            frmInfo.ShowDialog();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void tsbtnExit_Click(object sender, EventArgs e)  //退出系统
        {
            if (MessageBox.Show("是否退出系统？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Common.myLog.writeOperateContent("退出系统", Global.UserPermission, Global.UserPermission);
                IO.PLCReceiveStart = false;  //关闭刷新IO线程                
                statusToolsStart = false;  //关闭状态栏刷新线程
                if(Global.RobotReady)
                {
                    FrmIniRobot.RobotDisconnect();
                }                
                FrmIniRobot.RobotSpel.Dispose();//释放机器人SKD资源
                if(Global.ConnectSuccess1)
                {
                    smart200_1.Write(Global.M_AutoMode, false);//切换手动模式
                    smart200_1.Write(Global.M_ManualMode, true);
                    smart200_1.ConnectClose();//关闭与PLC1的连接
                }
                
                if(Global.ConnectSuccess2)
                {
                    smart200_2.ConnectClose();//关闭与PLC2的连接
                }                
                Application.DoEvents();
                Application.Exit();
            }
        }

        private void btn_Enter(object sender,EventArgs e)
        {
            Button button = (Button)sender;

            button.FlatAppearance.BorderSize = 2;
            //button.FlatAppearance.BorderColor = Color.FromArgb(0, 185, 240);
            if (button.Tag.ToString() == "0")
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(22, 22, 22);
            }
            else
            {
                button.FlatAppearance.BorderColor = Color.FromArgb(44, 44, 44);
            }
            button.BackColor = Color.FromArgb(94, 94, 94);
        }

        private void btn_Leave(object sender, EventArgs e)
        {
           Button button = (Button)sender;

            button.FlatAppearance.BorderSize = 0;
            if(button.Tag.ToString()=="0")
            {
                button.BackColor = Color.FromArgb(22, 22, 22);
            }
            else
            {
                button.BackColor = Color.FromArgb(44,44,44);
            }
        }
        #endregion

        #region 连接PLC，成功后刷IO，写入配置参数至PLC
        private void ConnectToPLC()
        {
            Task.Run(() =>
            {
                Global.ConnectSuccess1 = smart200_1.Connect(Global.PLC1_IP);
                Global.ConnectSuccess2 = smart200_2.Connect(Global.PLC2_IP);
                if (Global.ConnectSuccess1)
                {
                    Common.myLog.writeRunContent("连接PLC成功", "系统", "系统");

                    #region 刷IO
                    IO.PLCReceiveStart = true;
                    IO.ReadInput(30);
                    IO.ReadOutput(40);
                    #endregion

                    #region  写入参数至PLC
                    //smart200_1.Write("Q11.6", Global.AByPassCk);
                    //smart200_1.Write("Q11.7", Global.BByPassCk);
                    //smart200_1.Write("Q9.2", Global.AConNext);
                    //smart200_1.Write("Q9.3", Global.BConNext);
                    //smart200_1.Write("V100", Convert.ToUInt16(Global.PFFullDelay));
                    //smart200_1.Write("V102", Convert.ToUInt16(Global.NFFullDelay));
                    #endregion
                }
                else
                {
                    Common.myLog.writeRunContent("连接PLC失败", "系统", "系统");
                }


            });
        }
        #endregion

        #region 状态栏
        private bool statusToolsStart = false;
        private void StatusTools()
        {
            tslbRunStatus.Text = "停止";
            tslbRunStatus.BackColor = Color.FromName("Control");

            statusToolsStart = true;
            Task.Run(() =>
            {
                while (statusToolsStart)
                {
                    Invoke(new Action(() =>
                    {
                        //系统时间
                        tslblTime.Text = DateTime.Now.ToString();

                        //PLC1连接状态
                        if (Global.ConnectSuccess1)
                        {
                            tslbPlcConStaus1.Text = "已连接";
                            tslbPlcConStaus1.BackColor = Color.Green;

                            if (smart200_1.ReadBool("M1.0") && smart200_1.ReadBool("M3.0"))  //M1.0为A轨启动，M3.0为B轨启动
                            {
                                tslbRunStatus.Text = "启动";
                                tslbRunStatus.BackColor = Color.Green;
                            }
                            else
                            {
                                tslbRunStatus.Text = "停止";
                                tslbRunStatus.BackColor = Color.FromName("Control");
                            }

                            //设备运行模式
                            if (smart200_1.ReadBool(Global.M_AutoMode) && smart200_1.ReadBool(Global.M_ManualMode))//若手动模式和自动模式同时出现，则有问题，强制PLC进入手动模式
                            {
                                tslbRunMode.Text = "错误";
                                smart200_1.Write(Global.M_AutoMode, false);
                                smart200_1.Write(Global.M_ManualMode, true);
                            }
                            else if (smart200_1.ReadBool(Global.M_AutoMode))
                            {
                                Global.RunMode = true;
                                tslbRunMode.Text = "自动模式";
                            }
                            else if (smart200_1.ReadBool(Global.M_ManualMode))
                            {
                                Global.RunMode = false;
                                tslbRunMode.Text = "手动模式";
                            }
                        }
                        else
                        {
                            tslbPlcConStaus1.Text = "未连接";
                            tslbPlcConStaus1.BackColor = Color.Red;
                        }

                        //PLC2连接状态
                        if (Global.ConnectSuccess2)
                        {
                            tslbPlcConStaus2.Text = "已连接";
                            tslbPlcConStaus2.BackColor = Color.Green;
                        }
                        else
                        {
                            tslbPlcConStaus2.Text = "未连接";
                            tslbPlcConStaus2.BackColor = Color.Red;
                        }

                        //机器人状态
                        if(Global.RobotReady)
                        {
                            if(!FrmIniRobot.RobotSpel.ErrorOn)//检查有无报警
                            {
                                if(FrmIniRobot.RobotSpel.PauseOn)//检查有无暂停
                                {
                                    tslbRobot.Text = "暂停";
                                    tslbRobot.BackColor = Color.DarkOrange;
                                }
                                else
                                {
                                    if(FrmIniRobot.RobotSpel.Oport(1))//检查有无运行状态
                                    {
                                        if(FrmIniRobot.RobotSpel.MemSw("AutoMode"))//判断手自动状态
                                        {
                                            tslbRobot.Text = "自动";
                                            tslbRobot.BackColor = Color.Green;
                                        }
                                        else
                                        {
                                            tslbRobot.Text = "手动";
                                            tslbRobot.BackColor = Color.Green;
                                        }
                                    }
                                    else if(FrmIniRobot.RobotSpel.Oport(0)&& !FrmIniRobot.RobotSpel.Oport(1))//是否有准备状态无运行
                                    {
                                        tslbRobot.Text = "待机";
                                        tslbRobot.BackColor = Color.DarkOrange;
                                    }
                                    else
                                    {
                                        tslbRobot.Text = "错误";
                                        tslbRobot.BackColor = Color.Red;
                                    }
                                }
                            }
                            else
                            {
                                tslbRobot.Text = "报警:"+FrmIniRobot.RobotSpel.ErrorCode.ToString();
                                tslbRobot.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            tslbRobot.Text = "未连接";
                            tslbRobot.BackColor = Color.Red;
                        }


                        //账号显示
                        tslbPermission.Text = Global.UserPermission;
                    }));

                    Thread.Sleep(200);
                }
            });
        }
        #endregion

        #region 超过90天删除图像
        private void deletePhoto()
        {
            try
            {
                if (Directory.Exists(Global.PhotoSavePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Global.PhotoSavePath);
                    FileInfo[] fileInfo = directoryInfo.GetFiles("*.bmp");
                    DateTime dateNow = DateTime.Now;
                    foreach (FileInfo file in fileInfo)
                    {
                        TimeSpan day = dateNow.Subtract(file.LastWriteTime);
                        if (day.TotalDays > 90)
                        {
                            file.Delete();
                        }
                    }
                }
            }
            catch
            {

            }
        }




        #endregion
               
        
    }    
}
