using Microsoft.VisualBasic;
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

namespace MountingRobot.UI
{
    public partial class FrmInfo : Form
    {
        public FrmInfo()
        {
            InitializeComponent();
        }

        private void FrmInfo_Load(object sender, EventArgs e)
        {
            lblMachineNumber.Text = Global.MachineNumber;
            lblVersion.Text = Application.ProductVersion.ToString() + "\n";  //获取版本号
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblMachineNumber_DoubleClick(object sender, EventArgs e)
        {
            string result = Interaction.InputBox("请输入修改设备编号密码：", "密码");
            if (result == "MSW6256530")
            {
                string number = Interaction.InputBox("请输入新的设备编号", "修改设备编号");
                if (number != "")
                {
                    if (number.Length == 8)
                    {
                        ASCIIEncoding ascii = new ASCIIEncoding();//new ASCIIEncoding 的实例
                        byte[] bytestr = ascii.GetBytes(number);         //把string类型的参数保存到数组里

                        bool isDigit = true;
                        foreach (byte c in bytestr)                   //遍历这个数组里的内容
                        {
                            if (c < 48 || c > 57)                          //判断是否为数字
                            {
                                isDigit = false;
                                break;
                            }
                        }

                        if (isDigit)
                        {
                            Global.SystemINI.IniWriteValue("其它参数", "设备编号", number);
                            Global.MachineNumber = Global.SystemINI.IniReadValue("其它参数", "", "设备编号");
                            lblMachineNumber.Text = Global.MachineNumber;
                        }
                        else
                        {
                            MessageBox.Show("输入字符包含非数字字符");
                        }
                    }
                    else
                    {
                        MessageBox.Show("输入字符长度错误");
                    }
                }
            }
            else if(result != "")
            {
                MessageBox.Show("密码错误");
            }
        }
    }
}
