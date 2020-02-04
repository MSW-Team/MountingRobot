using MountingRobot.BLL;
using MountingRobot.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MountingRobot.UI
{
    public partial class FrmFlash : Form
    {
        public FrmFlash()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 实例Flash窗体
        /// </summary>
        private static FrmFlash frmFlash = new FrmFlash();
        /// <summary>
        /// 判断窗口有无打开，无则实例窗体
        /// </summary>
        /// <returns></returns>
        public static FrmFlash GetFrm()
        {
            if(frmFlash.IsDisposed)
            {
                frmFlash = new FrmFlash();
                return frmFlash;
            }
            else
            {
                return frmFlash;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmFlash_Load(object sender, EventArgs e)
        {
            switch(Global.AlarmFlash[0])
            {
                case 0:
                    picFlash.Image = null;
                    lblAlarmText.Text = "设备正常";
                    break;
                case 1:
                    picFlash.Image = Resources.LOGO_黑;
                    lblAlarmText.Text = "报警1";
                    break;
            }
        }
    }
}
