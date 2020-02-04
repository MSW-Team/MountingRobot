using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLog;

namespace MountingRobot.UI
{
    public partial class FrmLog : Form
    {
        public FrmLog()
        {
            InitializeComponent();
        }

        private void FrmLog_Load(object sender, EventArgs e)
        {
            FrmAllLog frmAllLog = new FrmAllLog();
            frmAllLog.TopLevel = false;
            frmAllLog.FormBorderStyle = FormBorderStyle.None;
            frmAllLog.Dock = DockStyle.Fill;
            frmAllLog.Parent = panel1;
            frmAllLog.Show();

        }
    }
}
