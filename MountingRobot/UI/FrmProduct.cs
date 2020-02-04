using MountingRobot.BLL;
using MyLog;
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
    public partial class FrmProduct : Form
    {
        public FrmProduct()
        {
            InitializeComponent();
        }

        private void frmProduct_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            ErgodicAProduct();
            ErgodicBProduct();
        }
        /// <summary>
        /// 遍历A轨产品文件夹获取产品名称并显示
        /// </summary>
        private void ErgodicAProduct()
        {
            String path = Application.StartupPath + @"\Config\A";//产品参数保存路径
            var files = Directory.GetFiles(path, "*.ini");//获取产品路径下所有ini文件名称
            cobASourceName.Items.Clear();
            cobAProductDelete.Items.Clear();
            string temp;
            for (int i = 0; i < files.Length; i++)
            {
                temp = files[i].Substring(path.Length, files[i].Length - path.Length).Trim('\\');
                cobASourceName.Items.Add(temp.Substring(0, temp.Length - 4));
                cobAProductDelete.Items.Add(temp.Substring(0, temp.Length - 4));
            }
        }

        /// <summary>
        /// 遍历B轨产品文件夹获取产品名称并显示
        /// </summary>
        private void ErgodicBProduct()
        {
            String path = Application.StartupPath + @"\Config\B";//产品参数保存路径
            var files = Directory.GetFiles(path, "*.ini");//获取产品路径下所有ini文件名称
            cobBSourceName.Items.Clear();
            cobBProductDelete.Items.Clear();
            string temp;
            for (int i = 0; i < files.Length; i++)
            {
                temp = files[i].Substring(path.Length, files[i].Length - path.Length).Trim('\\');
                cobBSourceName.Items.Add(temp.Substring(0, temp.Length - 4));
                cobBProductDelete.Items.Add(temp.Substring(0, temp.Length - 4));
            }
        }
        #region 添加新产品按钮
        private void btnAAddNew_Click(object sender, EventArgs e)
        {
            if (cobASourceName.SelectedIndex != -1)
            {
                if (txtATargetName.Text != "")
                {
                    if (compareText(cobASourceName, txtATargetName))
                    {
                        File.Copy(Application.StartupPath + "\\Config\\A\\" + cobASourceName.Text.Trim() + ".ini", Application.StartupPath + "\\Config\\A\\" + txtATargetName.Text.Trim() + ".ini");//复制INI文件
                        ErgodicAProduct();
                        FrmMain.ErgodicAProductDel();
                        showMeesage("新增成功", 0);
                        Common.myLog.writeOperateContent("新增产品： " + cobASourceName.Text + " - ->> " + txtATargetName.Text, Global.UserPermission, Global.UserPermission);
                    }
                    else
                    {
                        showMeesage("新增产品已存在", 1);
                    }
                }
                else
                {
                    showMeesage("新增产品名称未输入", 1);
                }
            }
            else
            {
                showMeesage("源产品名称未选择", 1);
            }
        }
        private void btnBAddNew_Click(object sender, EventArgs e)
        {
            if (cobBSourceName.SelectedIndex != -1)
            {
                if (txtBTargetName.Text != "")
                {
                    if (compareText(cobBSourceName, txtBTargetName))
                    {
                        File.Copy(Application.StartupPath + "\\Config\\B\\" + cobBSourceName.Text.Trim() + ".ini", Application.StartupPath + "\\Config\\B\\" + txtBTargetName.Text.Trim() + ".ini");//复制INI文件
                        ErgodicBProduct();
                        FrmMain.ErgodicBProductDel();
                        showMeesage("新增成功", 0);
                        Common.myLog.writeOperateContent("新增产品： " + cobBSourceName.Text + " - ->> " + txtBTargetName.Text, Global.UserPermission, Global.UserPermission);
                    }
                    else
                    {
                        showMeesage("新增产品已存在", 1);
                    }
                }
                else
                {
                    showMeesage("新增产品名称未输入", 1);
                }
            }
            else
            {
                showMeesage("源产品名称未选择", 1);
            }
        }
        #endregion

        #region 删除产品按钮
        private void btnADelete_Click(object sender, EventArgs e)
        {
            if (cobAProductDelete.SelectedIndex != -1)
            {
                if (cobAProductDelete.Text != Global.AProductName)
                {
                    DialogResult result = MessageBox.Show("是否删除产品：" + cobAProductDelete.Text, "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(Application.StartupPath + "\\Config\\A\\" + cobAProductDelete.Text + ".ini");//删除INI文件
                        ErgodicAProduct();
                        FrmMain.ErgodicAProductDel();
                        showMeesage("产品删除成功", 0);
                        Common.myLog.writeOperateContent("删除产品：" + cobAProductDelete.Text, Global.UserPermission, Global.UserPermission);
                    }
                }
                else
                {
                    showMeesage("该产品参数正在使用，无法删除", 1);
                }
            }
            else
            {
                showMeesage("未选择删除产品名称", 1);
            }
        }
        private void btnBDelete_Click(object sender, EventArgs e)
        {
            if (cobBProductDelete.SelectedIndex != -1)
            {
                if (cobBProductDelete.Text != Global.BProductName)
                {
                    DialogResult result = MessageBox.Show("是否删除产品：" + cobBProductDelete.Text, "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(Application.StartupPath + "\\Config\\B\\" + cobBProductDelete.Text + ".ini");//删除INI文件
                        ErgodicBProduct();
                        FrmMain.ErgodicBProductDel();
                        showMeesage("产品删除成功", 0);
                        Common.myLog.writeOperateContent("删除产品：" + cobBProductDelete.Text, Global.UserPermission, Global.UserPermission);
                    }
                }
                else
                {
                    showMeesage("该产品参数正在使用，无法删除", 1);
                }
            }
            else
            {
                showMeesage("未选择删除产品名称", 1);
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 检查新增产品名称是否重复
        /// </summary>
        /// <returns></returns>
        private bool compareText(ComboBox comboBox,TextBox textBox)
        {
            bool result = true;
            foreach (object item in comboBox.Items)
            {
                if (item.ToString() == textBox.Text)
                {
                    result = false;
                    break;
                }

            }
            return result;
        }

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

       

        
    }
}
