using MountingRobot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MountingRobot
{
    static class Program
    {
        public static System.Threading.Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool ret;
            mutex = new Mutex(true, Application.ProductName, out ret);  //防止软件多开
            if (ret)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FrmLogin frmLogin = new FrmLogin();
                if (frmLogin.ShowDialog() == DialogResult.OK)
                {
                    FrmIniRobot frmIniRobot = new FrmIniRobot();
                    if (frmIniRobot.ShowDialog()==DialogResult.OK)
                    {
                        Application.Run(new FrmMain());  //登录成功后显示主界面
                    }
                }
            }
            else
            {
                MessageBox.Show("全自动贴装软件正在运行，请勿重复运行！", "提示！", MessageBoxButtons.OK);
            }
        }
    }
}
