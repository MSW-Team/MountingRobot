using MountingRobot.BLL;
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
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            lblErrInfo.Visible = false;
        }

        #region 窗体拖动
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        protected override void OnMouseDown(MouseEventArgs e)               // 窗体上鼠标按下时可拖动
        {
            if (e.Button == MouseButtons.Left & this.WindowState == FormWindowState.Normal)
            {
                // 移动窗体
                this.Capture = false;
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (rdbOperator.Checked==true && txtPwd.Text=="123456")
            {
                this.DialogResult = DialogResult.OK;  //返回登录成功，并显示主界面
                Global.UserPermission = "操作员";
                Common.myLog.writeOperateContent("登录系统", "操作员", "操作员");
            }
            else if (rdbEngineer.Checked == true && txtPwd.Text == "6256530")
            {
                this.DialogResult = DialogResult.OK;  //返回登录成功，并显示主界面
                Global.UserPermission = "工程师";
                Common.myLog.writeOperateContent("登录系统", "工程师", "工程师");
            }
            else if (rdbAdimn.Checked == true && txtPwd.Text == "MSW6256530")
            {
                this.DialogResult = DialogResult.OK;  //返回登录成功，并显示主界面
                Global.UserPermission = "管理员";
                Common.myLog.writeOperateContent("登录系统", "管理员", "管理员");
            }
            else
            {
                lblErrInfo.Visible = true;
                lblErrInfo.Text = "密码错误，请输入正确的密码！";
                txtPwd.Text = string.Empty;
                txtPwd.Focus();
            }
        }

        #region 输入完密码信息后回车替代按登录按钮
        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnLogin_Click(null, null);
            }
        }
        #endregion

        #region 选择完登录身份后给密码框焦点
        private void SetTxtPwdFocus(object sender, EventArgs e)
        {
            txtPwd.Focus();
        }
        #endregion
    }
}
