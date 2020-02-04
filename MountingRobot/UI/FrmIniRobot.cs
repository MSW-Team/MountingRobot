using MountingRobot.BLL;
using MyLog;
using RCAPINet;
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
using System.Threading;

namespace MountingRobot.UI
{
    public partial class FrmIniRobot : Form
    {
        #region 窗体拖动
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        private void FrmIniRobot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal)
            {
                // 移动窗体
                this.Capture = false;
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        public static Spel RobotSpel = new Spel();  //实例机器人控制SDK
        public FrmIniRobot()
        {
            InitializeComponent();
        }

        private void FrmIniRobot_Load(object sender, EventArgs e)
        {
            timLoad.Start();

            lblVersion.Text = Application.ProductVersion.ToString() + "\n";

            Task.Run(() =>
            {
                #region 读取INI参数文件
                Global.ReadSystemIni();
                Global.ReadProductINI();
                Global.ReadRobotPoint();
                #endregion

                if (File.Exists(@Global.RobotProPath))
                {
                    try
                    {
                        RobotSpel.Initialize();  //初始化机器人SDK
                        RobotSpel.Project = @Global.RobotProPath;  //加载项目文件
                        RobotSpel.ResetAbortEnabled = false;
                        Global.RobotReady = true;                       
                        Common.myLog.writeRunContent("初始化机器人控制器成功", "系统", "系统");
                    }
                    catch (Exception ex)
                    {
                        Global.RobotReady = false;
                        Common.myLog.writeRunContent("初始化机器人控制器失败，原因为：" + ex.Message, "系统", "系统");
                        MessageBox.Show(ex.Message, "提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    Common.myLog.writeRunContent("初始化机器人控制器失败，原因为：找不到机器人项目文件", "系统", "系统");
                    Global.RobotReady = false;
                    MessageBox.Show("找不到机器人项目文件！", "错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                DialogResult = DialogResult.OK;
            });

        }

        private void FrmIniRobot_FormClosing(object sender, FormClosingEventArgs e)
        {
            timLoad.Stop();
        }

        private int timAdd = 0;
        private void timLoad_Tick(object sender, EventArgs e)
        {
            switch (timAdd)
            {
                case 0: lblLoad.Text = "系统加载中"; break;
                case 1: lblLoad.Text = "系统加载中."; break;
                case 2: lblLoad.Text = "系统加载中.."; break;
                case 3: lblLoad.Text = "系统加载中..."; break;
                default:
                    break;
            }
            timAdd++;
            if (timAdd == 4)
            {
                timAdd = 0;
            }
        }

        /// <summary>
        /// 机器人开始方法
        /// </summary>
        /// <param name="mode">false:手动 ture:自动</param>
        /// <returns></returns>
        public static int RobotStar(bool mode)
        {
            if (!RobotSpel.ErrorOn)//检查机器人是否在报警状态
            {
                if (!RobotSpel.PauseOn && mode)//检查暂停状态
                {
                    if (RobotSpel.Oport(0) && !RobotSpel.Oport(1))//判断机器人是否已启动
                    {
                        RobotSpel.MemOn("AutoMode");
                        RobotSpel.Start(0);//无则开始任务
                    }
                    return 0;
                }
                else if (RobotSpel.PauseOn && mode)
                {
                    RobotSpel.Continue();//暂停则执行继续指令
                    return 0;
                }
                else if (RobotSpel.PauseOn && !mode)//若暂停状态则需要先停止任务，置位手动状态，重新开始任务
                {
                    RobotSpel.Stop();
                    RobotSpel.MemOff("AutoMode");
                    RobotSpel.Start(0);
                    return 0;
                }
                else
                {
                    if (!RobotSpel.Oport(1))//判断机器人是否已启动
                    {
                        RobotSpel.MemOff("AutoMode");
                        RobotSpel.Start(0);//无则开始任务
                        return 0;
                    }
                    else//当前已有任务，则需要先停止任务，置位手动状态，重新开始任务
                    {
                        RobotSpel.Stop();
                        RobotSpel.MemOff("AutoMode");
                        RobotSpel.Start(0);
                        return 0;
                    }
                }
            }
            else
            {
                return RobotSpel.ErrorCode;//返回报警代码
            }
        }
        /// <summary>
        /// 机器人断开连接
        /// </summary>
        /// <param name="mode">false：断开连接；true：关闭程序，释放资源</param>
        public static void RobotDisconnect()
        {
            Global.RobotReady = false;//复位机器人连接标志位
            FrmIniRobot.RobotSpel.Stop();//停止机器人任务
            FrmMain.IO.RobotReceiveStart = false;//关闭机器人状态读取线程
            Thread.Sleep(100);//等待线程结束
            RobotSpel.Disconnect();//断开机器人连接            
        }
        /// <summary>
        /// 实时更改机器人速度方法
        /// </summary>
        /// <param name="RunSpeed">运行速度</param>
        /// <param name="MoveSpeed">插补速度</param>
        public static void RobotSpeed(int RunSpeed, int MoveSpeed)
        {
            RobotSpel.Speed(RunSpeed);
            RobotSpel.Accel(RunSpeed, RunSpeed);
            RobotSpel.SpeedS(MoveSpeed);
            RobotSpel.AccelS(MoveSpeed, MoveSpeed);
        }
        /// <summary>
        /// 机器人下发SW内存点信号
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Var"></param>
        public static void RobotSendBool(string Name, bool Var)
        {
            if (Var)
            {
                RobotSpel.MemOn(Name);
            }
            else
            {
                RobotSpel.MemOff(Name);
            }
        }

    }
}
