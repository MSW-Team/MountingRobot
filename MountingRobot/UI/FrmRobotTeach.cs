using MountingRobot.BLL;
using RCAPINet;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLog;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace MountingRobot.UI
{
    public partial class FrmRobotTeach : Form
    {
        public FrmRobotTeach()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 退出手动、等待循环，true为退出
        /// </summary>
        private bool ScramManualExit;

        private void FrmRobotTeach_Load(object sender, EventArgs e)
        {
            #region 初始化界面
            //初始化表格
            IniPutPointDataTable(dgvAPointData);
            IniPutPointDataTable(dgvAVisonPoint);
            IniPointDataTable(dgvVisionPointData);
            IniPointDataTable(dgvTakePointData);
            IniPointDataTable(dgvThrowPointData);

            //初始化点数选择控件集合
            tlscomAPointCount.SelectedIndex = 0;
            tlscomAPointCountV.SelectedIndex = 0;
            tlscomVisionPointCount.SelectedIndex = 0;
            tlscomTakePointCount.SelectedIndex = 0;
            tlscomThrowPointCount.SelectedIndex = 0;

            //初始化手动选择控件
            tlscomAGripChoseV.SelectedIndex = 0;
            tlscomAPGChoseV.SelectedIndex = 0;

            //初始化点位文件默认加载
            LoadPointData("A", Global.AFileSavePath);
            LoadPointData("Vision", Global.VisionFileSavePath);
            LoadPointData("Take", Global.TakeFileSavePath);
            LoadPointData("Throw", Global.ThrowFileSavePath);
            if (Global.OrbitalCount == "双轨")
            {
                //双轨的话则将B轨的父容器设为tbcTeachPoint
                tbcTeachPoint.TabPages[3].Parent = tbcTeachPoint;
                tbcTeachPoint.TabPages[4].Parent = tbcTeachPoint;
                tbcTeachPoint.TabPages[5].Parent = tbcTeachPoint;

                //初始化表格
                IniPutPointDataTable(dgvBPointData);
                IniPutPointDataTable(dgvBVisonPoint);
                //初始化点数选择控件集合
                tlscomBPointCount.SelectedIndex = 0;
                tlscomBPointCountV.SelectedIndex = 0;
                //初始化点位文件默认加载
                LoadPointData("B", Global.BFileSavePath);
            }
            else
            {
                //单轨的话则将B轨的父容器设为null
                tbcTeachPoint.TabPages[3].Parent = null;
                tbcTeachPoint.TabPages[4].Parent = null;
                tbcTeachPoint.TabPages[5].Parent = null;
            }
            #endregion

            #region 连接机器人按钮显示更新
            if (Global.RobotReady)
            {
                tlsmitARobotConnect.Text = "断开机器人连接";
                tlsmitARobotConnectV.Text = "断开机器人连接";
                tlsmitBRobotConnect.Text = "断开机器人连接";
                tlsmitBRobotConnectV.Text = "断开机器人连接";
                tlsmitTakeRobotConnect.Text = "断开机器人连接";
                tlsmitThrowRobotConnect.Text = "断开机器人连接";
                tlsmitVisionRobotConnect.Text = "断开机器人连接";
            }
            else
            {
                tlsmitARobotConnect.Text = "连接机器人";
                tlsmitARobotConnectV.Text = "连接机器人";
                tlsmitBRobotConnect.Text = "连接机器人";
                tlsmitBRobotConnectV.Text = "连接机器人";
                tlsmitTakeRobotConnect.Text = "连接机器人";
                tlsmitThrowRobotConnect.Text = "连接机器人";
                tlsmitVisionRobotConnect.Text = "连接机器人";
            }
            #endregion

            //消息显示文本清除
            lblMsg.Text = "";
        }

        private void FrmRobotTeach_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScramManualExit = true;//退出手动等待线程
        }

        #region 消息显示
        /// <summary>
        /// 状态栏信息已显示秒数
        /// </summary>
        private int showSeconds;
        /// <summary>
        /// 消息显示函数
        /// </summary>
        /// <param name="msg">需显示消息</param>
        /// <param name="mode">0：正常消息 1：报警消息</param>
        private void showMsg(string msg, int mode)
        {
            Invoke(new Action(() =>
            {
                if (mode == 0)
                {
                    lblMsg.BackColor = Color.FromArgb(44, 44, 44);
                }
                else
                {
                    lblMsg.BackColor = Color.Red;
                }
                lblMsg.Text = msg;
                showSeconds = 0;
                if (!timerShowMsg.Enabled)//启动定时器
                {
                    timerShowMsg.Enabled = true;
                    timerShowMsg.Start();
                }
            }));
        }
        /// <summary>
        /// 消息显示5秒后消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerShowMsg_Tick(object sender, EventArgs e)
        {
            if (lblMsg.Text != "")
            {
                showSeconds++;
                if (showSeconds > 5)
                {
                    lblMsg.Text = "";
                    lblMsg.BackColor = Color.FromArgb(44, 44, 44);
                    showSeconds = 0;
                    timerShowMsg.Stop();
                    timerShowMsg.Enabled = false;
                }
            }
        }
        #endregion

        #region 检查机器人连接
        /// <summary>
        /// 检查机器人连接情况
        /// </summary>
        /// <returns></returns>
        private bool checkRobotConnect()
        {
            if (Global.RobotReady)
            {
                return true;
            }
            else
            {
                showMsg("机器人控制未就绪，稍后再试！", 1);
                return false;
            }
        }
        #endregion

        #region 限制数据表输入只能为数字
        private void dgvTeach_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (dataGridView.CurrentCell.ColumnIndex >= 2 && dataGridView.CurrentCell.ColumnIndex <= 5)
            {
                TextBox tx = e.Control as TextBox;
                tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress);
                tx.KeyPress += new KeyPressEventHandler(tx_KeyPress);
            }
        }

        void tx_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b' && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }
        #endregion

        #region 创建坐标文件程序
        /// <summary>
        /// 创建坐标文件程序
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        /// <returns></returns>
        private bool ShowSaveFileDialog(string str)
        {
            bool result = false;
            try
            {
                string title = string.Empty;
                string saveSection = string.Empty;

                SaveFileDialog sfd = new SaveFileDialog();

                switch (str)
                {
                    case "A":
                        title = "新建A轨点位文件";
                        saveSection = "A轨点位文件路径";
                        break;
                    case "B":
                        title = "新建B轨点位文件";
                        saveSection = "B轨点位文件路径";
                        break;
                    case "Vision":
                        title = "新建视觉点位文件";
                        saveSection = "视觉点位文件路径";
                        break;
                    case "Take":
                        title = "新建取料点位文件";
                        saveSection = "取料点位文件路径";
                        break;
                    case "Throw":
                        title = "新建抛料点位文件";
                        saveSection = "抛料点位文件路径";
                        break;

                    default:
                        break;
                }
                sfd.Title = title;
                //设置文件类型 
                sfd.Filter = "点位文件（*.ini）|*.ini";

                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;

                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;

                //点了保存按钮进入 
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    WriteReadINI createAPointData = new WriteReadINI(sfd.FileName.ToString());
                    string tempPath = sfd.FileName.ToString(); //获得文件路径 
                    string tempName = sfd.FileName.ToString().Substring(sfd.FileName.ToString().LastIndexOf("\\") + 1).Replace(".ini", ""); //获取文件名，不带路径
                    createAPointData.IniWriteValue("点位程序名", "name", tempName);  //新建该路径程序，将程序名写入

                    if (str == "A")//保存当前路径为默认路径至配置文件中
                    {
                        Global.AProductINI.IniWriteValue(saveSection, "path", sfd.FileName.ToString());
                    }
                    else if (str == "B")
                    {
                        Global.BProductINI.IniWriteValue(saveSection, "path", sfd.FileName.ToString());
                    }
                    else
                    {
                        Global.SystemINI.IniWriteValue(saveSection, "path", sfd.FileName.ToString());
                    }

                    if (str == "A" || str == "B")
                    {
                        //放料点增加工具坐标系下坐标点位文件
                        WriteReadINI createAVisionPiont = new WriteReadINI(sfd.FileName.ToString().Replace(".ini", Global.VisionPostfix + ".ini"));
                        createAVisionPiont.IniWriteValue("点位程序名", "name", tempName + Global.VisionPostfix);//新建该路径程序，将程序名写入
                    }
                    Common.myLog.writeOperateContent(title + ":" + tempName, Global.UserPermission, Global.UserPermission);
                    switch (str)
                    {
                        case "A":
                            Global.AFileSavePath = tempPath;
                            Global.AFileNameExt = tempName;
                            Global.AVisionFileSavePath = tempPath.Replace(".ini", Global.VisionPostfix + ".ini");
                            Global.AVisionFileNameExt = tempName + Global.VisionPostfix;
                            tlscomAPointCount.SelectedIndex = 0;
                            tlscomAPointCountV.SelectedIndex = 0;
                            break;
                        case "B":
                            Global.BFileSavePath = tempPath;
                            Global.BFileNameExt = tempName;
                            Global.BVisionFileSavePath = tempPath.Replace(".ini", Global.VisionPostfix + ".ini");
                            Global.BVisionFileNameExt = tempName + Global.VisionPostfix;
                            tlscomBPointCount.SelectedIndex = 0;
                            tlscomBPointCountV.SelectedIndex = 0;
                            break;
                        case "Vision":
                            Global.VisionFileSavePath = tempPath;
                            Global.VisionFileNameExt = tempName;
                            tlscomVisionPointCount.SelectedIndex = 0;
                            break;
                        case "Take":
                            Global.TakeFileSavePath = tempPath;
                            Global.TakeFileNameExt = tempName;
                            tlscomTakePointCount.SelectedIndex = 0;
                            break;
                        case "Throw":
                            Global.ThrowFileSavePath = tempPath;
                            Global.ThrowFileNameExt = tempName;
                            tlscomThrowPointCount.SelectedIndex = 0;
                            break;

                        default:
                            break;
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
        #endregion

        #region 选择坐标文件程序
        /// <summary>
        /// 选择坐标文件程序
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        private void SelectPointData(string str)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                string tempSection = string.Empty;
                string title = string.Empty;

                switch (str)
                {
                    case "A":
                        tempSection = "A轨点位文件路径";
                        title = "加载A轨点位程序";
                        break;
                    case "B":
                        tempSection = "B轨点位文件路径";
                        title = "加载B轨点位程序";
                        break;
                    case "Vision":
                        tempSection = "视觉点位文件路径";
                        title = "加载视觉点位程序";
                        break;
                    case "Take":
                        tempSection = "取料点位文件路径";
                        title = "加载取料点位程序";
                        break;
                    case "Throw":
                        tempSection = "抛料点位文件路径";
                        title = "加载抛料点位程序";
                        break;

                    default:
                        break;
                }

                openFileDialog.Title = title;
                openFileDialog.Filter = "点位文件（*.ini）|*.ini";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (str == "A")//保存当前路径为默认路径至系统配置文件中
                    {
                        Global.AProductINI.IniWriteValue(tempSection, "path", openFileDialog.FileName);
                    }
                    else if (str == "B")
                    {
                        Global.BProductINI.IniWriteValue(tempSection, "path", openFileDialog.FileName);
                    }
                    else
                    {
                        Global.SystemINI.IniWriteValue(tempSection, "path", openFileDialog.FileName);
                    }
                    LoadPointData(str, openFileDialog.FileName);
                    Common.myLog.writeOperateContent(title + ":" + openFileDialog.FileName.Substring(openFileDialog.FileName.ToString().LastIndexOf("\\") + 1).Replace(".ini", ""), Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 加载点位文件
        /// <summary>
        /// 加载点位文件方法
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        /// <param name="path">点位文件路径</param>
        private void LoadPointData(string str, string path)
        {
            try
            {
                int count;
                DataGridView dataGridView = new DataGridView();
                WriteReadINI loadPointIni = new WriteReadINI(path);
                ToolStripComboBox toolStripComboBox = new ToolStripComboBox();
                ToolStripLabel toolStripLabel = new ToolStripLabel();
                DataGridView dataGridViewVisin = new DataGridView();
                WriteReadINI loadPointIniVisin = new WriteReadINI(path.Replace(".ini", "-Vision.ini"));
                ToolStripComboBox toolStripComboBoxVison = new ToolStripComboBox();
                ToolStripLabel toolStripLabelVisin = new ToolStripLabel();
                string loadSection = string.Empty;
                string tempStr = string.Empty;

                switch (str)
                {
                    case "A":
                        dataGridView = dgvAPointData;
                        dataGridViewVisin = dgvAVisonPoint;
                        toolStripComboBox = tlscomAPointCount;
                        toolStripComboBoxVison = tlscomAPointCountV;
                        toolStripLabel = tlslblAPointName;
                        toolStripLabelVisin = tlslblAPointNameV;
                        Global.AFileSavePath = path;
                        Global.AVisionFileSavePath = path.Replace(".ini", Global.VisionPostfix + ".ini");
                        loadSection = "A贴装点";
                        break;
                    case "B":
                        dataGridView = dgvBPointData;
                        dataGridViewVisin = dgvBVisonPoint;
                        toolStripComboBox = tlscomBPointCount;
                        toolStripComboBoxVison = tlscomBPointCountV;
                        toolStripLabel = tlslblBPointName;
                        toolStripLabelVisin = tlslblBPointNameV;
                        Global.BFileSavePath = path;
                        Global.BVisionFileSavePath = path.Replace(".ini", Global.VisionPostfix + ".ini");
                        loadSection = "B贴装点";
                        break;
                    case "Vision":
                        dataGridView = dgvVisionPointData;
                        toolStripComboBox = tlscomVisionPointCount;
                        toolStripLabel = tlslblVisionPointName;
                        Global.VisionFileSavePath = path;
                        loadSection = "视觉点";
                        break;
                    case "Take":
                        dataGridView = dgvTakePointData;
                        toolStripComboBox = tlscomTakePointCount;
                        toolStripLabel = tlslblTakePointName;
                        Global.TakeFileSavePath = path;
                        loadSection = "取料点";
                        break;
                    case "Throw":
                        dataGridView = dgvThrowPointData;
                        toolStripComboBox = tlscomThrowPointCount;
                        toolStripLabel = tlslblThrowPointName;
                        Global.ThrowFileSavePath = path;
                        loadSection = "抛料点";
                        break;

                    default:
                        break;
                }

                if (!(str == "A" || str == "B"))//其他点位
                {
                    toolStripLabel.Text = loadPointIni.IniReadValue("点位程序名", "NULL", "name");  //获取程序名称 
                    toolStripComboBox.SelectedIndex = Convert.ToInt32(loadPointIni.IniReadValue(loadSection + "数量", "0", "点数"));  //获取点位文件中的点数                     
                    PointCountChange(str, toolStripComboBox.SelectedIndex);  //根据获取点位个数进行新增行 
                    count = toolStripComboBox.SelectedIndex;
                    for (int i = 0; i < count; i++)  //获取详细点坐标信息
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            switch (j)
                            {
                                case 0:
                                    tempStr = "点位描述";
                                    break;
                                case 1:
                                    tempStr = "X";
                                    break;
                                case 2:
                                    tempStr = "Y";
                                    break;
                                case 3:
                                    tempStr = "Z";
                                    break;
                                case 4:
                                    tempStr = "U";
                                    break;
                                case 5:
                                    tempStr = "Local";
                                    break;
                                case 6:
                                    tempStr = "手系统";
                                    break;
                                case 7:
                                    tempStr = "夹爪";
                                    break;
                                default:
                                    break;
                            }
                            dataGridView.Rows[i].Cells[j].Value = loadPointIni.IniReadValue(loadSection + (i + 1).ToString(), "", tempStr);
                        }
                    }
                }
                else//放料点
                {
                    toolStripLabel.Text = loadPointIni.IniReadValue("点位程序名", "NULL", "name");  //获取程序名称 
                    toolStripComboBox.SelectedIndex = Convert.ToInt32(loadPointIni.IniReadValue(loadSection + "数量", "0", "点数")) / 4;  //获取点位文件中的点数                    
                    toolStripComboBoxVison.SelectedIndex = Convert.ToInt32(loadPointIniVisin.IniReadValue(loadSection + "数量", "0", "点数")) / 4;//提前获取信息，加载生成表格行数
                    toolStripLabelVisin.Text = loadPointIniVisin.IniReadValue("点位程序名", "NULL", "name");
                    count = toolStripComboBox.SelectedIndex * 4;
                    PointCountChange(str, count);  //根据获取点位个数进行新增行 

                    //tool0坐标
                    for (int i = 0; i < count; i++)  //获取详细点坐标信息
                    {
                        for (int j = 1; j < 9; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    tempStr = "取料位";
                                    break;
                                case 2:
                                    tempStr = "X";
                                    break;
                                case 3:
                                    tempStr = "Y";
                                    break;
                                case 4:
                                    tempStr = "Z";
                                    break;
                                case 5:
                                    tempStr = "U";
                                    break;
                                case 6:
                                    tempStr = "Local";
                                    break;
                                case 7:
                                    tempStr = "手系统";
                                    break;
                                case 8:
                                    tempStr = "夹爪";
                                    break;
                                default:
                                    break;
                            }
                            dataGridView.Rows[i].Cells[j].Value = loadPointIni.IniReadValue(loadSection + (i + 1).ToString(), "", tempStr);
                        }
                    }


                    //加载工具坐标对应下的视觉校正点位

                    //PointCountChange(str, toolStripComboBoxV.SelectedIndex);  //根据获取点位个数进行新增行
                    if (toolStripLabelVisin.Text != "NULL")//若无点位文件，不加载点位，否则会报错
                    {
                        for (int i = 0; i < count + 2; i++)  //获取详细点坐标信息
                        {
                            for (int j = 1; j < 9; j++)
                            {
                                switch (j)
                                {
                                    case 1:
                                        tempStr = "取料位";
                                        break;
                                    case 2:
                                        tempStr = "X";
                                        break;
                                    case 3:
                                        tempStr = "Y";
                                        break;
                                    case 4:
                                        tempStr = "Z";
                                        break;
                                    case 5:
                                        tempStr = "U";
                                        break;
                                    case 6:
                                        tempStr = "Local";
                                        break;
                                    case 7:
                                        tempStr = "手系统";
                                        break;
                                    case 8:
                                        tempStr = "夹爪";
                                        break;
                                    default:
                                        break;
                                }
                                if (i <= 1)
                                {
                                    dataGridViewVisin.Rows[i].Cells[j].Value = loadPointIniVisin.IniReadValue(str + "本地坐标系点" + (i + 1).ToString(), "", tempStr);
                                }
                                else
                                {
                                    dataGridViewVisin.Rows[i].Cells[j].Value = loadPointIniVisin.IniReadValue(loadSection + (i - 1).ToString(), "", tempStr);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 保存点位文件
        /// <summary>
        /// 保存点位文件
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        /// <param name="count">点的数量</param>
        /// <param name="path">保存路径</param>
        private void SavePointData(string str, int count, string path)
        {
            try
            {
                DataGridView dataGridView = new DataGridView();
                WriteReadINI savePointIni = new WriteReadINI(path);
                string saveSection1 = string.Empty;
                string saveSection2 = string.Empty;
                string tempStr = string.Empty;
                ToolStripLabel toolStripLabel = new ToolStripLabel();

                switch (str)
                {
                    case "A":
                        dataGridView = dgvAPointData;
                        saveSection1 = "A贴装点";
                        toolStripLabel = tlslblAPointName;
                        break;
                    case "B":
                        dataGridView = dgvBPointData;
                        saveSection1 = "B贴装点";
                        toolStripLabel = tlslblBPointName;
                        break;
                    case "Vision":
                        dataGridView = dgvVisionPointData;
                        saveSection1 = "视觉点";
                        toolStripLabel = tlslblVisionPointName;
                        break;
                    case "Take":
                        dataGridView = dgvTakePointData;
                        saveSection1 = "取料点";
                        toolStripLabel = tlslblTakePointName;
                        break;
                    case "Throw":
                        dataGridView = dgvThrowPointData;
                        saveSection1 = "抛料点";
                        toolStripLabel = tlslblThrowPointName;
                        break;
                    default:
                        break;
                }
                string str1 = string.Empty;

                if (!(str == "A" || str == "B"))//普通点
                {
                    savePointIni.IniWriteValue(saveSection1 + "数量", "点数", count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            switch (j)
                            {
                                case 0:
                                    tempStr = "点位描述";
                                    break;
                                case 1:
                                    tempStr = "X";
                                    break;
                                case 2:
                                    tempStr = "Y";
                                    break;
                                case 3:
                                    tempStr = "Z";
                                    break;
                                case 4:
                                    tempStr = "U";
                                    break;
                                case 5:
                                    tempStr = "Local";
                                    break;
                                case 6:
                                    tempStr = "手系统";
                                    break;
                                case 7:
                                    tempStr = "夹爪";
                                    break;
                                default:
                                    break;
                            }
                            savePointIni.IniWriteValue(saveSection1 + (i + 1).ToString(), tempStr, dataGridView.Rows[i].Cells[j].Value.ToString());
                        }
                    }
                }
                else//放料点
                {
                    count = count * 4;//count为任务数，一个任务有4个爪，贴装点=count*4
                    savePointIni.IniWriteValue(saveSection1 + "数量", "点数", count.ToString());
                    for (int i = 0; i < count; i++)
                    {
                        for (int j = 1; j < 9; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    tempStr = "取料位";
                                    break;
                                case 2:
                                    tempStr = "X";
                                    break;
                                case 3:
                                    tempStr = "Y";
                                    break;
                                case 4:
                                    tempStr = "Z";
                                    break;
                                case 5:
                                    tempStr = "U";
                                    break;
                                case 6:
                                    tempStr = "Local";
                                    break;
                                case 7:
                                    tempStr = "手系统";
                                    break;
                                case 8:
                                    tempStr = "夹爪";
                                    break;
                                default:
                                    break;
                            }
                            savePointIni.IniWriteValue(saveSection1 + (i + 1).ToString(), tempStr, dataGridView.Rows[i].Cells[j].Value.ToString());
                        }
                    }

                    //保存视觉点位文件
                    switch (str)
                    {
                        case "A":
                            dataGridView = dgvAVisonPoint;
                            saveSection1 = "A贴装点";
                            saveSection2 = "A本地坐标系点";
                            toolStripLabel = tlslblAPointNameV;
                            break;

                        case "B":
                            dataGridView = dgvBVisonPoint;
                            saveSection1 = "B贴装点";
                            saveSection2 = "B本地坐标系点";
                            toolStripLabel = tlslblBPointNameV;
                            break;
                        default:
                            break;
                    }
                    WriteReadINI savePointIniV = new WriteReadINI(path.Replace(".ini", Global.VisionPostfix + ".ini"));//实例视觉点位INI类
                    savePointIniV.IniWriteValue(saveSection1 + "数量", "点数", count.ToString());
                    for (int i = 0; i < (count + 2); i++)//视觉位固定会多两个本地坐标系点位，所以count+2
                    {
                        for (int j = 1; j < 9; j++)
                        {
                            switch (j)
                            {
                                case 1:
                                    tempStr = "取料位";
                                    break;
                                case 2:
                                    tempStr = "X";
                                    break;
                                case 3:
                                    tempStr = "Y";
                                    break;
                                case 4:
                                    tempStr = "Z";
                                    break;
                                case 5:
                                    tempStr = "U";
                                    break;
                                case 6:
                                    tempStr = "Local";
                                    break;
                                case 7:
                                    tempStr = "手系统";
                                    break;
                                case 8:
                                    tempStr = "夹爪";
                                    break;
                                default:
                                    break;
                            }
                            if (i == 0 || i == 1)
                            {
                                savePointIniV.IniWriteValue(saveSection2 + (i + 1).ToString(), tempStr, dataGridView.Rows[i].Cells[j].Value.ToString());
                            }
                            else
                            {
                                savePointIniV.IniWriteValue(saveSection1 + (i - 1).ToString(), tempStr, dataGridView.Rows[i].Cells[j].Value.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 另存为点位程序文件
        /// <summary>
        /// 另存为点位程序文件
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        private void SaveAsPointData(string str)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                string title = string.Empty;  //标题
                int tempCount = 0;  //点个数

                switch (str)
                {
                    case "A":
                        title = "A轨程序另存为";
                        tempCount = tlscomAPointCount.SelectedIndex;
                        break;
                    case "B":
                        title = "B轨程序另存为";
                        tempCount = tlscomBPointCount.SelectedIndex;
                        break;
                    case "Vision":
                        title = "视觉程序另存为";
                        tempCount = tlscomVisionPointCount.SelectedIndex;
                        break;
                    case "Take":
                        title = "取料点程序另存为";
                        tempCount = tlscomTakePointCount.SelectedIndex;
                        break;
                    case "Throw":
                        title = "抛料点程序另存为";
                        tempCount = tlscomThrowPointCount.SelectedIndex;
                        break;
                    default:
                        break;
                }

                sfd.Title = title;
                sfd.Filter = "点位文件（*.ini）|*.ini";
                sfd.RestoreDirectory = true;
                sfd.FilterIndex = 1;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    WriteReadINI writeReadINI = new WriteReadINI(sfd.FileName);
                    string tempStr = sfd.FileName.ToString().Substring(sfd.FileName.ToString().LastIndexOf("\\") + 1).Replace(".ini", "");
                    writeReadINI.IniWriteValue("点位程序名", "name", tempStr);  //保存程序名称
                    WriteReadINI writeReadINIV = new WriteReadINI(sfd.FileName.Replace(".ini", "-Vision.ini"));
                    writeReadINIV.IniWriteValue("点位程序名", "name", tempStr + Global.VisionPostfix);
                    SavePointData(str, tempCount, sfd.FileName);  //调用保存程序，修改需要另存的路径
                    Common.myLog.writeOperateContent(title + ":" + tempStr, Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 点数选择
        /// <summary>
        /// 点数选择
        /// </summary>
        /// <param name="str">表格选择，如A轨填"A",B轨填"B",视觉填"Vision",取料填"Take"</param>
        /// <param name="count">点位个数</param>
        private void PointCountChange(string str, int count)
        {
            try
            {
                DataGridView dataGridView = new DataGridView();
                ToolStripLabel toolStripLabel = new ToolStripLabel();

                switch (str)
                {
                    case "A":
                        dataGridView = dgvAPointData;
                        toolStripLabel = tlslblAPointName;
                        break;
                    case "B":
                        dataGridView = dgvBPointData;
                        toolStripLabel = tlslblBPointName;
                        break;
                    case "Take":
                        dataGridView = dgvTakePointData;
                        toolStripLabel = tlslblTakePointName;
                        break;
                    case "Vision":
                        dataGridView = dgvVisionPointData;
                        toolStripLabel = tlslblVisionPointName;
                        break;
                    case "Throw":
                        dataGridView = dgvThrowPointData;
                        toolStripLabel = tlslblThrowPointName;
                        break;
                    default:
                        break;
                }

                if (toolStripLabel.Text != "NULL")
                {
                    int tempInt = 0;
                    tempInt = count - dataGridView.Rows.Count;  //选择点位数减去当前表格行数
                    if (tempInt >= 0)  //如果为正数代表点数较之前有增加
                    {
                        if (!(str == "A" || str == "B"))//其他点数据更新行
                        {
                            for (int i = dataGridView.Rows.Count; i < count; i++)  //增加行并默认赋值
                            {
                                DataGridViewRow row = new DataGridViewRow();
                                dataGridView.Rows.Add(row);
                                dataGridView.Rows[i].Cells[0].Value = "";
                                dataGridView.Rows[i].Cells[1].Value = "0.00";
                                dataGridView.Rows[i].Cells[2].Value = "0.00";
                                dataGridView.Rows[i].Cells[3].Value = "0.00";
                                dataGridView.Rows[i].Cells[4].Value = "0.00";
                                dataGridView.Rows[i].Cells[5].Value = "0";
                                dataGridView.Rows[i].Cells[6].Value = "左手姿态";
                                dataGridView.Rows[i].Cells[7].Value = "无";
                                dataGridView.Rows[i].Cells[8].Value = "示教";
                                dataGridView.Rows[i].Cells[9].Value = "移动";
                                dataGridView.Rows[i].Height = 35;
                            }
                        }
                        else
                        {
                            for (int i = dataGridView.Rows.Count; i < count; i++)  //增加行并默认赋值
                            {
                                DataGridViewRow row = new DataGridViewRow();
                                dataGridView.Rows.Add(row);
                                DataGridViewComboBoxCell stateCell = (DataGridViewComboBoxCell)(dataGridView.Rows[i].Cells[8]);//夹爪选择根据
                                stateCell.Items.Add("无");
                                switch (i % 4)
                                {
                                    case 0:
                                        stateCell.Items.Add("A爪");
                                        break;
                                    case 1:
                                        stateCell.Items.Add("B爪");
                                        break;
                                    case 2:
                                        stateCell.Items.Add("C爪");
                                        break;
                                    case 3:
                                        stateCell.Items.Add("D爪");
                                        break;
                                }
                                dataGridView.Rows[i].Cells[0].Value = "任务" + (i / 4 + 1).ToString();
                                dataGridView.Rows[i].Cells[1].Value = "0";
                                dataGridView.Rows[i].Cells[2].Value = "0.00";
                                dataGridView.Rows[i].Cells[3].Value = "0.00";
                                dataGridView.Rows[i].Cells[4].Value = "0.00";
                                dataGridView.Rows[i].Cells[5].Value = "0.00";
                                dataGridView.Rows[i].Cells[6].Value = "0";
                                dataGridView.Rows[i].Cells[7].Value = "左手姿态";
                                dataGridView.Rows[i].Cells[8].Value = "无";
                                dataGridView.Rows[i].Cells[9].Value = "示教";
                                dataGridView.Rows[i].Cells[10].Value = "移动";
                                dataGridView.Rows[i].Height = 35;
                            }
                        }
                    }
                    else  //如果为负数则代表点数较之前减少了，因此需要删除行
                    {
                        for (int i = dataGridView.Rows.Count - 1; i >= count; i--)
                        {
                            dataGridView.Rows.Remove(dataGridView.Rows[i]);
                        }
                    }
                    for (int i = 0; i < dataGridView.Rows.Count; i++)  //表格序号自动变化
                    {
                        dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                    }
                }

                if (str == "A" || str == "B")//若是A、B轨，还需操作视觉点位dgv
                {
                    //视觉点位
                    switch (str)
                    {
                        case "A":
                            dataGridView = dgvAVisonPoint;
                            toolStripLabel = tlslblAPointNameV;
                            break;
                        case "B":
                            dataGridView = dgvBVisonPoint;
                            toolStripLabel = tlslblBPointNameV;
                            break;
                        default:

                            break;
                    }
                    if (toolStripLabel.Text != "NULL")
                    {
                        int tempInt = 0;
                        tempInt = count - dataGridView.Rows.Count + 2;  //选择点位数减去当前表格行数
                        if (tempInt >= 0)  //如果为正数代表点数较之前有增加
                        {
                            for (int i = dataGridView.Rows.Count; i < count + 2; i++)  //增加行并默认赋值
                            {
                                DataGridViewRow row = new DataGridViewRow();
                                dataGridView.Rows.Add(row);
                                DataGridViewComboBoxCell stateCell = (DataGridViewComboBoxCell)(dataGridView.Rows[i].Cells[8]);//夹爪选择控件
                                stateCell.Items.Add("无");
                                if (i > 1)//排除本地坐标系点位
                                {
                                    switch ((i - 2) % 4)
                                    {
                                        case 0:
                                            stateCell.Items.Add("A爪");
                                            break;
                                        case 1:
                                            stateCell.Items.Add("B爪");
                                            break;
                                        case 2:
                                            stateCell.Items.Add("C爪");
                                            break;
                                        case 3:
                                            stateCell.Items.Add("D爪");
                                            break;
                                    }
                                }

                                if (i <= 1)//视觉点位页面两个本地坐标系点位默认备注
                                {
                                    dataGridView.Rows[i].Cells[0].Value = "本地坐标系点位" + (i + 1).ToString();
                                }
                                else
                                {
                                    dataGridView.Rows[i].Cells[0].Value = "任务" + ((i - 2) / 4 + 1).ToString();
                                }
                                dataGridView.Rows[i].Cells[1].Value = "0";
                                dataGridView.Rows[i].Cells[2].Value = "0.00";
                                dataGridView.Rows[i].Cells[3].Value = "0.00";
                                dataGridView.Rows[i].Cells[4].Value = "0.00";
                                dataGridView.Rows[i].Cells[5].Value = "0.00";
                                dataGridView.Rows[i].Cells[6].Value = "0";
                                dataGridView.Rows[i].Cells[7].Value = "左手姿态";
                                dataGridView.Rows[i].Cells[8].Value = "无";
                                dataGridView.Rows[i].Cells[9].Value = "获取";
                                dataGridView.Rows[i].Cells[10].Value = "测试";
                                dataGridView.Rows[i].Height = 35;
                            }
                        }
                        else  //如果为负数则代表点数较之前减少了，因此需要删除行
                        {
                            for (int i = dataGridView.Rows.Count - 1; i >= (count + 2); i--)
                            {
                                dataGridView.Rows.Remove(dataGridView.Rows[i]);
                            }
                        }
                        for (int i = 0; i < (dataGridView.Rows.Count); i++)  //表格序号自动变化
                        {
                            dataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 初始化表格函数
        /// <summary>
        /// 初始化点位表格
        /// </summary>
        /// <param name="dgv"></param>
        private void IniPointDataTable(DataGridView dgv)
        {
            try
            {
                dgv.Rows.Clear();  //清空表格


                DataGridViewTextBoxColumn PointDes = new DataGridViewTextBoxColumn();  //增加“点位描述”列
                PointDes.Name = "PointDes";
                PointDes.DataPropertyName = "PointDes";
                PointDes.HeaderText = "点位描述";
                dgv.Columns.Add(PointDes);
                PointDes.ReadOnly = true;//只读，不可编辑

                DataGridViewTextBoxColumn AxisX = new DataGridViewTextBoxColumn();  //增加“X”列
                AxisX.Name = "AxisX";
                AxisX.DataPropertyName = "X";
                AxisX.HeaderText = "X";
                dgv.Columns.Add(AxisX);

                DataGridViewTextBoxColumn AxisY = new DataGridViewTextBoxColumn();  //增加“Y”列
                AxisY.Name = "AxisY";
                AxisY.DataPropertyName = "Y";
                AxisY.HeaderText = "Y";
                dgv.Columns.Add(AxisY);

                DataGridViewTextBoxColumn AxisZ = new DataGridViewTextBoxColumn();  //增加“Z”列
                AxisZ.Name = "AxisZ";
                AxisZ.DataPropertyName = "Z";
                AxisZ.HeaderText = "Z";
                dgv.Columns.Add(AxisZ);

                DataGridViewTextBoxColumn AxisU = new DataGridViewTextBoxColumn();  //增加“U”列
                AxisU.Name = "AxisU";
                AxisU.DataPropertyName = "U";
                AxisU.HeaderText = "U";
                dgv.Columns.Add(AxisU);

                DataGridViewComboBoxColumn Local = new DataGridViewComboBoxColumn();  //增加“手系统”列
                Local.Name = "Local";
                Local.DataPropertyName = "Local";
                Local.HeaderText = "Local";
                Local.Items.Add("0");
                Local.Items.Add("1");
                Local.Items.Add("2");
                Local.Items.Add("3");
                Local.Items.Add("4");
                Local.Items.Add("5");
                Local.Items.Add("6");
                Local.Items.Add("7");
                Local.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Local);

                DataGridViewComboBoxColumn Hand = new DataGridViewComboBoxColumn();  //增加“手系统”列
                Hand.Name = "Hand";
                Hand.DataPropertyName = "Hand";
                Hand.HeaderText = "手系统";
                Hand.Items.Add("左手姿态");
                Hand.Items.Add("右手姿态");
                Hand.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Hand);

                DataGridViewComboBoxColumn Grip = new DataGridViewComboBoxColumn();  //增加“夹爪”列
                Grip.Name = "Grip";
                Grip.DataPropertyName = "Grip";
                Grip.HeaderText = "夹爪";
                if (dgv.Name == "dgvVisionPointData")
                {
                    Grip.Items.Add("无");
                    Grip.Items.Add("A爪");
                    Grip.Items.Add("B爪");
                    Grip.Items.Add("C爪");
                    Grip.Items.Add("D爪");
                    Grip.Items.Add("A轨Mark1");
                    Grip.Items.Add("A轨Mark2");
                    Grip.Items.Add("B轨Mark1");
                    Grip.Items.Add("B轨Mark2");
                }
                else if (dgv.Name == "dgvAPointData" || dgv.Name == "dgvBPointData")
                {
                    //夹爪每行不同，此处不添加，在创建行数时，再添加
                }
                else
                {
                    Grip.Items.Add("无");
                    Grip.Items.Add("A爪");
                    Grip.Items.Add("B爪");
                    Grip.Items.Add("C爪");
                    Grip.Items.Add("D爪");
                }
                Grip.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Grip);

                if (dgv.Name == "dgvAVisonPoint" || dgv.Name == "dgvBVisonPoint")
                {
                    DataGridViewButtonColumn Teach = new DataGridViewButtonColumn();  //增加“点位示教”列
                    Teach.Name = "Teach";
                    Teach.DataPropertyName = "Teach";
                    Teach.HeaderText = "获取坐标";
                    Teach.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Teach);

                    DataGridViewButtonColumn Move = new DataGridViewButtonColumn();  //增加“点位移动”列
                    Move.Name = "Move";
                    Move.DataPropertyName = "Move";
                    Move.HeaderText = "点位测试";
                    Move.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Move);
                }
                else
                {
                    DataGridViewButtonColumn Teach = new DataGridViewButtonColumn();  //增加“点位示教”列
                    Teach.Name = "Teach";
                    Teach.DataPropertyName = "Teach";
                    Teach.HeaderText = "点位示教";
                    Teach.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Teach);

                    DataGridViewButtonColumn Move = new DataGridViewButtonColumn();  //增加“点位移动”列
                    Move.Name = "Move";
                    Move.DataPropertyName = "Move";
                    Move.HeaderText = "点位移动";
                    Move.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Move);
                }

                //改变标题的高度;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgv.ColumnHeadersHeight = 40;

                //设置标题内容居中显示;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                //列宽禁止调节
                dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                //字体
                dgv.Font = new Font("宋体", 14, FontStyle.Regular);

                //行列头背景与字体颜色
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
                dgv.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.BackgroundColor = Color.FromArgb(44, 44, 44);

                //单元格背景与字体颜色
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(44, 44, 44);
                dgv.DefaultCellStyle.ForeColor = Color.White;

                //行标题宽度自适应
                dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                for (int i = 0; i < 10; i++)
                {
                    //设置列宽适应
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //禁止排序
                    dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //单元格居中
                    dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dgv.Columns[0].Width = 320;
                dgv.Columns[8].Width = 100;
                dgv.Columns[9].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 初始化放料点点位表格
        /// </summary>
        /// <param name="dgv"></param>
        private void IniPutPointDataTable(DataGridView dgv)
        {
            try
            {
                dgv.Rows.Clear();  //清空表格

                DataGridViewTextBoxColumn TaskNumber = new DataGridViewTextBoxColumn();//任务编号列
                TaskNumber.Name = "TaskNumber";
                TaskNumber.DataPropertyName = "TaskNumber";
                TaskNumber.HeaderText = "任务编号";
                dgv.Columns.Add(TaskNumber);

                DataGridViewComboBoxColumn GetNumber = new DataGridViewComboBoxColumn();  //取料位列
                GetNumber.Name = "GetNumber";
                GetNumber.DataPropertyName = "GetNumber";
                GetNumber.HeaderText = "取料位";
                GetNumber.Items.Add("0");
                GetNumber.Items.Add("1");
                GetNumber.Items.Add("2");
                GetNumber.Items.Add("3");
                GetNumber.Items.Add("4");
                GetNumber.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(GetNumber);

                DataGridViewTextBoxColumn AxisX = new DataGridViewTextBoxColumn();  //增加“X”列
                AxisX.Name = "AxisX";
                AxisX.DataPropertyName = "X";
                AxisX.HeaderText = "X";
                dgv.Columns.Add(AxisX);

                DataGridViewTextBoxColumn AxisY = new DataGridViewTextBoxColumn();  //增加“Y”列
                AxisY.Name = "AxisY";
                AxisY.DataPropertyName = "Y";
                AxisY.HeaderText = "Y";
                dgv.Columns.Add(AxisY);

                DataGridViewTextBoxColumn AxisZ = new DataGridViewTextBoxColumn();  //增加“Z”列
                AxisZ.Name = "AxisZ";
                AxisZ.DataPropertyName = "Z";
                AxisZ.HeaderText = "Z";
                dgv.Columns.Add(AxisZ);

                DataGridViewTextBoxColumn AxisU = new DataGridViewTextBoxColumn();  //增加“U”列
                AxisU.Name = "AxisU";
                AxisU.DataPropertyName = "U";
                AxisU.HeaderText = "U";
                dgv.Columns.Add(AxisU);

                DataGridViewComboBoxColumn Local = new DataGridViewComboBoxColumn();  //增加“手系统”列
                Local.Name = "Local";
                Local.DataPropertyName = "Local";
                Local.HeaderText = "Local";
                Local.Items.Add("0");
                Local.Items.Add("1");
                Local.Items.Add("2");
                Local.Items.Add("3");
                Local.Items.Add("4");
                Local.Items.Add("5");
                Local.Items.Add("6");
                Local.Items.Add("7");
                Local.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Local);

                DataGridViewComboBoxColumn Hand = new DataGridViewComboBoxColumn();  //增加“手系统”列
                Hand.Name = "Hand";
                Hand.DataPropertyName = "Hand";
                Hand.HeaderText = "手系统";
                Hand.Items.Add("左手姿态");
                Hand.Items.Add("右手姿态");
                Hand.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Hand);

                DataGridViewComboBoxColumn Grip = new DataGridViewComboBoxColumn();  //增加“夹爪”列
                Grip.Name = "Grip";
                Grip.DataPropertyName = "Grip";
                Grip.HeaderText = "夹爪";
                Grip.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dgv.Columns.Add(Grip);

                if (dgv.Name == "dgvAVisonPoint" || dgv.Name == "dgvBVisonPoint")
                {
                    DataGridViewButtonColumn Teach = new DataGridViewButtonColumn();  //增加“点位示教”列
                    Teach.Name = "Teach";
                    Teach.DataPropertyName = "Teach";
                    Teach.HeaderText = "获取坐标";
                    Teach.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Teach);

                    DataGridViewButtonColumn Move = new DataGridViewButtonColumn();  //增加“点位移动”列
                    Move.Name = "Move";
                    Move.DataPropertyName = "Move";
                    Move.HeaderText = "点位测试";
                    Move.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Move);
                }
                else
                {
                    DataGridViewButtonColumn Teach = new DataGridViewButtonColumn();  //增加“点位示教”列
                    Teach.Name = "Teach";
                    Teach.DataPropertyName = "Teach";
                    Teach.HeaderText = "点位示教";
                    Teach.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Teach);

                    DataGridViewButtonColumn Move = new DataGridViewButtonColumn();  //增加“点位移动”列
                    Move.Name = "Move";
                    Move.DataPropertyName = "Move";
                    Move.HeaderText = "点位移动";
                    Move.FlatStyle = FlatStyle.Flat;
                    dgv.Columns.Add(Move);
                }

                //列是否只读
                TaskNumber.ReadOnly = true;
                if (dgv.Name == "dgvAVisonPoint" || dgv.Name == "dgvBVisonPoint")
                {
                    GetNumber.ReadOnly = true;
                    Grip.ReadOnly = true;
                }
                else
                {
                    GetNumber.ReadOnly = false;
                    Grip.ReadOnly = false;
                }

                //改变标题的高度;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                dgv.ColumnHeadersHeight = 40;

                //设置标题内容居中显示;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                //列宽禁止调节
                dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

                //字体
                dgv.Font = new Font("宋体", 14, FontStyle.Regular);

                //行列头背景与字体颜色
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(22, 22, 22);
                dgv.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.BackgroundColor = Color.FromArgb(44, 44, 44);

                //单元格背景与字体颜色
                dgv.DefaultCellStyle.BackColor = Color.FromArgb(44, 44, 44);
                dgv.DefaultCellStyle.ForeColor = Color.White;

                //行标题宽度自适应
                dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

                for (int i = 0; i < 11; i++)
                {
                    //设置列宽适应
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //禁止排序
                    dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //单元格居中
                    dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dgv.Columns[0].Width = 200;
                dgv.Columns[9].Width = 100;
                dgv.Columns[10].Width = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 点位文件保存按钮
        private void tlsbtnASave_Click(object sender, EventArgs e)
        {
            if (tlslblAPointName.Text != "NULL")
            {
                if (tlscomAPointCount.SelectedIndex != 0)
                {
                    SavePointData("A", Convert.ToInt32(tlscomAPointCount.SelectedItem), Global.AFileSavePath);
                    Common.myLog.writeOperateContent("点击了A轨点位文件保存", Global.UserPermission, Global.UserPermission);
                    showMsg("A轨点位文件保存成功！", 0);
                }
                else
                {
                    showMsg("请选择贴装点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnBSave_Click(object sender, EventArgs e)
        {
            if (tlslblBPointName.Text != "NULL")
            {
                if (tlscomBPointCount.SelectedIndex != 0)
                {
                    SavePointData("B", Convert.ToInt32(tlscomBPointCount.SelectedItem), Global.BFileSavePath);
                    Common.myLog.writeOperateContent("点击了B轨点位文件保存", Global.UserPermission, Global.UserPermission);
                    showMsg("B轨点位文件保存成功！", 0);
                }
                else
                {
                    showMsg("请选择贴装点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnTakeSave_Click(object sender, EventArgs e)
        {
            if (tlslblTakePointName.Text != "NULL")
            {
                if (tlscomTakePointCount.SelectedIndex != 0)
                {
                    SavePointData("Take", Convert.ToInt32(tlscomTakePointCount.SelectedItem), Global.TakeFileSavePath);
                    Common.myLog.writeOperateContent("点击了取料点位文件保存", Global.UserPermission, Global.UserPermission);
                    showMsg("取料点位文件保存成功！", 0);
                }
                else
                {
                    showMsg("请选择取料点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnVisionSave_Click(object sender, EventArgs e)
        {
            if (tlslblVisionPointName.Text != "NULL")
            {
                if (tlscomVisionPointCount.SelectedIndex != 0)
                {
                    SavePointData("Vision", Convert.ToInt32(tlscomVisionPointCount.SelectedItem), Global.VisionFileSavePath);
                    Common.myLog.writeOperateContent("点击了视觉点位文件保存", Global.UserPermission, Global.UserPermission);
                    showMsg("视觉点位文件保存成功！", 0);
                }
                else
                {
                    showMsg("请选择视觉点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnThrowSave_Click(object sender, EventArgs e)
        {
            if (tlslblThrowPointName.Text != "NULL")
            {
                if (tlscomThrowPointCount.SelectedIndex != 0)
                {
                    SavePointData("Throw", Convert.ToInt32(tlscomThrowPointCount.SelectedItem), Global.ThrowFileSavePath);
                    Common.myLog.writeOperateContent("点击了抛料点位文件保存", Global.UserPermission, Global.UserPermission);
                    showMsg("抛料点位文件保存成功！", 0);
                }
                else
                {
                    showMsg("请选择抛料点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }
        #endregion

        #region 新建点位文件按钮
        private void tlsbtnABuild_Click(object sender, EventArgs e)  //A轨点位坐标新建
        {
            if (ShowSaveFileDialog("A"))
            {
                dgvAPointData.Rows.Clear();
                tlslblAPointName.Text = Global.AFileNameExt;
                dgvAVisonPoint.Rows.Clear();
                tlslblAPointNameV.Text = Global.AFileNameExt + "-Vision";
            }
        }

        private void tlsbtnBBulid_Click(object sender, EventArgs e)  //B轨点位坐标新建
        {
            if (ShowSaveFileDialog("B"))
            {
                dgvBPointData.Rows.Clear();
                tlslblBPointName.Text = Global.BFileNameExt;
                dgvBVisonPoint.Rows.Clear();
                tlslblBPointNameV.Text = Global.BFileNameExt + "-Vision";
            }
        }

        private void tlsbtnVisionBuild_Click(object sender, EventArgs e)  //视觉点位坐标新建
        {
            if (ShowSaveFileDialog("Vision"))
            {
                dgvBPointData.Rows.Clear();
                tlslblVisionPointName.Text = Global.VisionFileNameExt;
            }
        }

        private void tlsbtnTakeBuild_Click(object sender, EventArgs e)  //取料点位坐标新建
        {
            if (ShowSaveFileDialog("Take"))
            {
                dgvTakePointData.Rows.Clear();
                tlslblTakePointName.Text = Global.TakeFileNameExt;
            }
        }

        private void tlsbtnThrowBuild_Click(object sender, EventArgs e)  //抛料点位坐标新建
        {
            if (ShowSaveFileDialog("Throw"))
            {
                dgvThrowPointData.Rows.Clear();
                tlslblThrowPointName.Text = Global.ThrowFileNameExt;
            }
        }
        #endregion

        #region 点位数量选择控件
        /// <summary>
        /// 检查是否创建点位文件
        /// </summary>
        /// <param name="toolStripLabel"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        private bool CheckTlslblName(ToolStripLabel toolStripLabel)
        {
            if (toolStripLabel.Text != "NULL")
            {
                return true;
            }
            else
            {
                showMsg("未创建点位文件，无法增加点位！", 1);
                string str = toolStripLabel.Text;
                return false;
            }
        }

        private void tlscomAPointCount_SelectedIndexChanged(object sender, EventArgs e)  //A轨点位选择控件
        {
            if (tlscomAPointCount.SelectedIndex != 0)
            {
                if (CheckTlslblName(tlslblAPointName))
                {
                    PointCountChange("A", Convert.ToInt32(tlscomAPointCount.SelectedItem) * 4);
                    tlscomAPointCountV.SelectedIndex = tlscomAPointCount.SelectedIndex;
                }
                else
                {
                    tlscomAPointCount.SelectedIndex = 0;
                    tlscomAPointCountV.SelectedIndex = tlscomAPointCount.SelectedIndex;
                }
            }
            else
            {
                PointCountChange("A", 0);
            }
        }

        private void tlscomBPointCount_SelectedIndexChanged(object sender, EventArgs e)  //B轨点位选择控件
        {
            if (tlscomBPointCount.SelectedIndex != 0)
            {
                if (CheckTlslblName(tlslblBPointName))
                {
                    PointCountChange("B", Convert.ToInt32(tlscomBPointCount.SelectedItem) * 4);
                    tlscomBPointCountV.SelectedIndex = tlscomBPointCount.SelectedIndex;
                }
                else
                {
                    tlscomBPointCount.SelectedIndex = 0;
                    tlscomBPointCountV.SelectedIndex = tlscomBPointCount.SelectedIndex;
                }
            }
            else
            {
                PointCountChange("B", 0);
            }
        }

        private void tlscomTakePointCount_SelectedIndexChanged(object sender, EventArgs e)  //取料点位选择控件
        {
            if (tlscomTakePointCount.SelectedIndex != 0)
            {
                if (CheckTlslblName(tlslblTakePointName))
                {
                    PointCountChange("Take", Convert.ToInt32(tlscomTakePointCount.SelectedItem));
                }
                else
                {
                    tlscomTakePointCount.SelectedIndex = 0;
                }
            }
            else
            {
                PointCountChange("Take", 0);
            }
        }

        private void tlscomVisionPointCount_SelectedIndexChanged(object sender, EventArgs e)  //视觉点位选择控件
        {

            if (tlscomVisionPointCount.SelectedIndex != 0)
            {
                if (CheckTlslblName(tlslblVisionPointName))
                {
                    PointCountChange("Vision", Convert.ToInt32(tlscomVisionPointCount.SelectedItem));
                }
                else
                {
                    tlscomVisionPointCount.SelectedIndex = 0;
                }
            }
            else
            {
                PointCountChange("Vision", 0);
            }
        }

        private void tlscomThrowPointCount_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (tlscomThrowPointCount.SelectedIndex != 0)
            {
                if (CheckTlslblName(tlslblThrowPointName))
                {
                    PointCountChange("Throw", Convert.ToInt32(tlscomThrowPointCount.SelectedItem));
                }
                else
                {
                    tlscomThrowPointCount.SelectedIndex = 0;
                }
            }
            else
            {
                PointCountChange("Throw", 0);
            }
        }
        #endregion

        #region 加载点位文件按钮
        private void tlsbtnAOpen_Click(object sender, EventArgs e)
        {
            SelectPointData("A");
        }

        private void tlsbtnBOpen_Click(object sender, EventArgs e)
        {
            SelectPointData("B");
        }

        private void tlsbtnTakeOpen_Click(object sender, EventArgs e)
        {
            SelectPointData("Take");
        }

        private void tlsbtnVisionOpen_Click(object sender, EventArgs e)
        {
            SelectPointData("Vision");
        }

        private void tlsbtnThrowOpen_Click(object sender, EventArgs e)
        {
            SelectPointData("Throw");
        }
        #endregion

        #region 另存为按钮
        private void tlsbtnASaveAs_Click(object sender, EventArgs e)
        {
            if (tlslblAPointName.Text != "NULL")
            {
                if (tlscomAPointCount.SelectedIndex != 0)
                {
                    SaveAsPointData("A");
                }
                else
                {
                    showMsg("请选择贴装点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnBSaveAs_Click(object sender, EventArgs e)
        {
            if (tlslblBPointName.Text != "NULL")
            {
                if (tlscomBPointCount.SelectedIndex != 0)
                {
                    SaveAsPointData("B");
                }
                else
                {
                    showMsg("请选择贴装点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnTakeSaveAs_Click(object sender, EventArgs e)
        {
            if (tlslblTakePointName.Text != "NULL")
            {
                if (tlscomTakePointCount.SelectedIndex != 0)
                {
                    SaveAsPointData("Take");
                }
                else
                {
                    showMsg("请选择取料点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnVisionSaveAs_Click(object sender, EventArgs e)
        {
            if (tlslblVisionPointName.Text != "NULL")
            {
                if (tlscomVisionPointCount.SelectedIndex != 0)
                {
                    SaveAsPointData("Vision");
                }
                else
                {
                    showMsg("请选择视觉点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }

        private void tlsbtnThrowSaveAs_Click(object sender, EventArgs e)
        {
            if (tlslblThrowPointName.Text != "NULL")
            {
                if (tlscomThrowPointCount.SelectedIndex != 0)
                {
                    SaveAsPointData("Throw");
                }
                else
                {
                    showMsg("请选择抛料点数量！", 1);
                }
            }
            else
            {
                showMsg("请先新建点位程序！", 1);
            }
        }
        #endregion

        #region 示教及移动按钮
        private bool opening = false;
        private bool going = false;
        private void dgvAPointData_CellContentClick(object sender, DataGridViewCellEventArgs e)  //点位从P0-P29
        {
            try
            {
                if (e.ColumnIndex != -1)
                {
                    if (dgvAPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" || dgvAPointData.Columns[e.ColumnIndex].HeaderText == "点位移动")
                    {
                        RCAPINet.SpelPoint p;
                        if (checkRobotConnect())  //如果机器人准备OK,等待SDK初始化
                        {
                            if (dgvAPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" && !opening)  //示教
                            {
                                Task.Run(() =>
                                {
                                    Invoke(new Action(() =>
                                    {
                                        opening = true;  //防止重复开示教
                                        FrmIniRobot.RobotSpel.Tool(0);
                                        if (FrmIniRobot.RobotSpel.TeachPoint("robot1.pts", 999, "示教A轨贴装点" + (e.RowIndex + 1).ToString(), this))
                                        {
                                            p = FrmIniRobot.RobotSpel.GetPoint(999);
                                            dgvAPointData.Rows[e.RowIndex].Cells[2].Value = p.X;
                                            dgvAPointData.Rows[e.RowIndex].Cells[3].Value = p.Y;
                                            dgvAPointData.Rows[e.RowIndex].Cells[4].Value = p.Z;
                                            dgvAPointData.Rows[e.RowIndex].Cells[5].Value = p.U;
                                            dgvAPointData.Rows[e.RowIndex].Cells[6].Value = p.Local.ToString();
                                            if (p.Hand.ToString() == "Righty")
                                            {
                                                dgvAPointData.Rows[e.RowIndex].Cells[7].Value = "右手姿态";
                                            }
                                            else
                                            {
                                                dgvAPointData.Rows[e.RowIndex].Cells[7].Value = "左手姿态";
                                            }
                                            FrmIniRobot.RobotSpel.PDel(999);
                                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                                        }
                                        opening = false;
                                    }));
                                });
                            }

                            if (dgvAPointData.Columns[e.ColumnIndex].HeaderText == "点位移动" && !going)  //移动
                            {
                                if (dgvAPointData.Rows[e.RowIndex].Cells[8].Value.ToString() != "无")
                                {
                                    try
                                    {
                                        going = true;
                                        FrmIniRobot.RobotSpel.Tool(0);
                                        FrmIniRobot.RobotSpel.AsyncMode = true;
                                        FrmIniRobot.RobotSpel.Jump(200 + e.RowIndex);
                                        FrmIniRobot.RobotSpel.WaitCommandComplete();
                                        going = false;
                                    }
                                    catch (Exception ex)
                                    {
                                        going = false;
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                                else
                                {
                                    showMsg("请先选择夹爪", 1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvAVisonPoint_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvAVisonPoint.Columns[e.ColumnIndex].HeaderText == "获取坐标" || dgvAVisonPoint.Columns[e.ColumnIndex].HeaderText == "点位测试")
                {
                    RCAPINet.SpelPoint p;

                    if (dgvAVisonPoint.Columns[e.ColumnIndex].HeaderText == "获取坐标" && !opening)  //示教
                    {
                        if (checkRobotStatus())  //检查机器人连接情况与手自动状态
                        {
                            if (e.RowIndex == 0 || e.RowIndex == 1)//本地坐标系获取
                            {
                                if (MessageBox.Show("确定创建本地坐标？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    Task.Run(() =>
                                    {
                                        opening = true;  //防止重复开示教
                                        FrmIniRobot.RobotSpel.MemOn("ACreatLocal");

                                        while (!FrmMain.IO.ALocalCreatSuccess)//等待本地坐标创建成功标志
                                        {
                                            Application.DoEvents();
                                            if (FrmIniRobot.RobotSpel.MemSw("CamPCBA01NG"))//PCB板mark点1视觉NG
                                            {
                                                FrmIniRobot.RobotSpel.MemOff("CamPCBA01NG");
                                                showMsg("mark点1视觉NG", 1);
                                                goto end1;
                                            }

                                            if (FrmIniRobot.RobotSpel.MemSw("CamPCBA02NG"))//PCB板mark点2视觉NG
                                            {
                                                FrmIniRobot.RobotSpel.MemOff("CamPCBA02NG");
                                                showMsg("mark点2视觉NG", 1);
                                                goto end1;
                                            }
                                            if (ScramManualExit)//退出线程标志
                                            {
                                                FrmIniRobot.RobotSpel.Stop();
                                                goto end1;
                                            }
                                            Thread.Sleep(10);
                                        }

                                        FrmMain.IO.ALocalCreatSuccess = false;
                                        p = FrmIniRobot.RobotSpel.GetPoint(10);
                                        dgvAVisonPoint.Rows[0].Cells[2].Value = p.X;
                                        dgvAVisonPoint.Rows[0].Cells[3].Value = p.Y;
                                        dgvAVisonPoint.Rows[0].Cells[4].Value = p.Z;
                                        dgvAVisonPoint.Rows[0].Cells[5].Value = p.U;
                                        dgvAVisonPoint.Rows[0].Cells[6].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvAVisonPoint.Rows[0].Cells[7].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvAVisonPoint.Rows[0].Cells[7].Value = "左手姿态";
                                        }

                                        p = FrmIniRobot.RobotSpel.GetPoint(11);
                                        dgvAVisonPoint.Rows[1].Cells[2].Value = p.X;
                                        dgvAVisonPoint.Rows[1].Cells[3].Value = p.Y;
                                        dgvAVisonPoint.Rows[1].Cells[4].Value = p.Z;
                                        dgvAVisonPoint.Rows[1].Cells[5].Value = p.U;
                                        dgvAVisonPoint.Rows[1].Cells[6].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvAVisonPoint.Rows[1].Cells[7].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvAVisonPoint.Rows[1].Cells[7].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");

                                    end1:
                                        opening = false;
                                    });
                                }
                            }
                            else//放料点视觉点位获取
                            {
                                if (dgvAVisonPoint.Rows[e.RowIndex].Cells[8].Value.ToString() != "无")
                                {
                                    Task.Run(() =>
                                    {
                                        opening = true;  //防止重复开示教
                                        int grip;
                                        switch (dgvAVisonPoint.Rows[e.RowIndex].Cells[8].Value.ToString())//获取夹爪
                                        {
                                            case "A爪":
                                                grip = 1;
                                                break;
                                            case "B爪":
                                                grip = 2;
                                                break;
                                            case "C爪":
                                                grip = 3;
                                                break;
                                            case "D爪":
                                                grip = 4;
                                                break;
                                            default:
                                                grip = 0;
                                                break;
                                        }

                                        if (grip != 0)
                                        {
                                            FrmIniRobot.RobotSpel.SetVar("HandSelect", grip);//下发夹爪选择
                                            FrmIniRobot.RobotSpel.SetVar("CreatPointNumber", 300 + e.RowIndex - 2);//下发位置编号
                                            FrmIniRobot.RobotSpel.MemOn("ACreatPoint");

                                            while (!FrmMain.IO.PointCreatSuccess)//等待反馈成功标志
                                            {
                                                Thread.Sleep(10);
                                                Application.DoEvents();
                                                if (ScramManualExit)
                                                {
                                                    FrmIniRobot.RobotSpel.Stop();
                                                    goto end2;
                                                }
                                            }

                                            FrmMain.IO.PointCreatSuccess = false;
                                            p = FrmIniRobot.RobotSpel.GetPoint(300 + e.RowIndex - 2);//读取生成的点位，加载至dgv中
                                            dgvAVisonPoint.Rows[e.RowIndex].Cells[2].Value = p.X;
                                            dgvAVisonPoint.Rows[e.RowIndex].Cells[3].Value = p.Y;
                                            dgvAVisonPoint.Rows[e.RowIndex].Cells[4].Value = p.Z;
                                            dgvAVisonPoint.Rows[e.RowIndex].Cells[5].Value = p.U;
                                            dgvAVisonPoint.Rows[e.RowIndex].Cells[6].Value = p.Local.ToString();
                                            if (p.Hand.ToString() == "Righty")
                                            {
                                                dgvAVisonPoint.Rows[e.RowIndex].Cells[7].Value = "右手姿态";
                                            }
                                            else
                                            {
                                                dgvAVisonPoint.Rows[e.RowIndex].Cells[7].Value = "左手姿态";
                                            }

                                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");

                                        end2:
                                            opening = false;
                                        }
                                        else
                                        {
                                            opening = false;
                                        }
                                    });
                                }
                                else
                                {
                                    showMsg("请先选择夹爪", 1);
                                }
                            }
                        }
                    }

                    if (checkRobotConnect())  //检查机器人连接情况
                    {
                        if (e.ColumnIndex != 0 && e.ColumnIndex != 1)//本地坐标系点数据无法移动前往,排除一二行按钮
                        {
                            if (dgvAVisonPoint.Columns[e.ColumnIndex].HeaderText == "点位测试" && !going)  //移动
                            {
                                if (dgvAVisonPoint.Rows[e.RowIndex].Cells[8].Value.ToString() != "无")
                                {
                                    try
                                    {
                                        going = true;
                                        FrmIniRobot.RobotSpel.AsyncMode = true;
                                        switch (dgvAVisonPoint.Rows[e.RowIndex].Cells[8].Value)//根据夹爪切换工具坐标系
                                        {
                                            case "A爪":
                                                FrmIniRobot.RobotSpel.Tool(11);
                                                break;
                                            case "B爪":
                                                FrmIniRobot.RobotSpel.Tool(12);
                                                break;
                                            case "C爪":
                                                FrmIniRobot.RobotSpel.Tool(13);
                                                break;
                                            case "D爪":
                                                FrmIniRobot.RobotSpel.Tool(14);
                                                break;
                                        }
                                        FrmIniRobot.RobotSpel.Jump(300 + e.RowIndex - 2);//A轨视觉放料点坐标从P300开始，dgv视觉点位从第三位开始，故-2
                                        FrmIniRobot.RobotSpel.WaitCommandComplete();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                    finally
                                    {
                                        going = false;
                                    }
                                }
                                else
                                {
                                    showMsg("请先选择夹爪", 1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvBPointData_CellContentClick(object sender, DataGridViewCellEventArgs e)  //点位从P30-P59
        {
            try
            {
                if (dgvBPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" || dgvBPointData.Columns[e.ColumnIndex].HeaderText == "点位移动")
                {
                    RCAPINet.SpelPoint p;
                    if (checkRobotConnect())  //如果机器人准备OK,等待SDK初始化
                    {
                        if (dgvBPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" && !opening)  //示教
                        {
                            Task.Run(() =>
                            {
                                Invoke(new Action(() =>
                                {
                                    opening = true;  //防止重复开示教
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    if (FrmIniRobot.RobotSpel.TeachPoint("robot1.pts", 999, "示教B轨贴装点" + (e.RowIndex + 1).ToString(), this))
                                    {
                                        p = FrmIniRobot.RobotSpel.GetPoint(999);
                                        dgvBPointData.Rows[e.RowIndex].Cells[2].Value = p.X;
                                        dgvBPointData.Rows[e.RowIndex].Cells[3].Value = p.Y;
                                        dgvBPointData.Rows[e.RowIndex].Cells[4].Value = p.Z;
                                        dgvBPointData.Rows[e.RowIndex].Cells[5].Value = p.U;
                                        dgvBPointData.Rows[e.RowIndex].Cells[6].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvBPointData.Rows[e.RowIndex].Cells[7].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvBPointData.Rows[e.RowIndex].Cells[7].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.PDel(999);
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                                    }
                                    opening = false;
                                }));
                            });
                        }
                        if (dgvBPointData.Columns[e.ColumnIndex].HeaderText == "点位移动" && !going)  //移动
                        {
                            if (dgvBPointData.Rows[e.ColumnIndex].Cells[8].Value.ToString() != "无")
                            {
                                try
                                {
                                    going = true;
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    FrmIniRobot.RobotSpel.AsyncMode = true;
                                    FrmIniRobot.RobotSpel.Jump(e.RowIndex + 250);
                                    FrmIniRobot.RobotSpel.WaitCommandComplete();
                                    going = false;
                                }
                                catch (Exception ex)
                                {
                                    going = false;
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            else
                            {
                                showMsg("请先选择夹爪", 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvBVisonPoint_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvBVisonPoint.Columns[e.ColumnIndex].HeaderText == "获取坐标" || dgvBVisonPoint.Columns[e.ColumnIndex].HeaderText == "点位测试")
                {
                    RCAPINet.SpelPoint p;

                    if (dgvBVisonPoint.Columns[e.ColumnIndex].HeaderText == "获取坐标" && !opening)  //示教
                    {
                        if (checkRobotStatus())  //检查机器人连接情况与手自动情况
                        {
                            if (e.RowIndex == 0 || e.RowIndex == 1)//本地坐标系获取
                            {
                                if (MessageBox.Show("确定创建本地坐标？", "提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    Task.Run(() =>
                                    {
                                        opening = true;  //防止重复开示教
                                        FrmIniRobot.RobotSpel.MemOn("BCreatLocal");

                                        while (!FrmMain.IO.BLocalCreatSuccess)
                                        {
                                            Application.DoEvents();
                                            if (FrmIniRobot.RobotSpel.MemSw("CamPCBB01NG"))//PCB板mark点1视觉NG
                                            {
                                                FrmIniRobot.RobotSpel.MemOff("CamPCBB01NG");
                                                showMsg("mark点1视觉NG", 1);
                                                goto end1;
                                            }

                                            if (FrmIniRobot.RobotSpel.MemSw("CamPCBB02NG"))//PCB板mark点2视觉NG
                                            {
                                                FrmIniRobot.RobotSpel.MemOff("CamPCBB02NG");
                                                showMsg("mark点2视觉NG", 1);
                                                goto end1;
                                            }
                                            if (ScramManualExit)
                                            {
                                                FrmIniRobot.RobotSpel.Stop();
                                                goto end1;
                                            }
                                            Thread.Sleep(10);
                                        }

                                        FrmMain.IO.BLocalCreatSuccess = false;
                                        p = FrmIniRobot.RobotSpel.GetPoint(16);
                                        dgvBVisonPoint.Rows[0].Cells[2].Value = p.X;
                                        dgvBVisonPoint.Rows[0].Cells[3].Value = p.Y;
                                        dgvBVisonPoint.Rows[0].Cells[4].Value = p.Z;
                                        dgvBVisonPoint.Rows[0].Cells[5].Value = p.U;
                                        dgvBVisonPoint.Rows[0].Cells[6].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvBVisonPoint.Rows[0].Cells[7].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvBVisonPoint.Rows[0].Cells[7].Value = "左手姿态";
                                        }

                                        p = FrmIniRobot.RobotSpel.GetPoint(17);
                                        dgvBVisonPoint.Rows[1].Cells[2].Value = p.X;
                                        dgvBVisonPoint.Rows[1].Cells[3].Value = p.Y;
                                        dgvBVisonPoint.Rows[1].Cells[4].Value = p.Z;
                                        dgvBVisonPoint.Rows[1].Cells[5].Value = p.U;
                                        dgvBVisonPoint.Rows[1].Cells[6].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvBVisonPoint.Rows[1].Cells[7].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvBVisonPoint.Rows[1].Cells[7].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");

                                    end1:
                                        opening = false;
                                    });
                                }
                            }
                            else//放料点视觉点位获取
                            {
                                if (dgvBVisonPoint.Rows[e.ColumnIndex].Cells[8].Value.ToString() != "无")
                                {
                                    Task.Run(() =>
                                    {
                                        opening = true;  //防止重复开示教
                                        int grip;
                                        switch (dgvBVisonPoint.Rows[e.RowIndex].Cells[8].Value.ToString())//获取夹爪
                                        {
                                            case "A爪":
                                                grip = 1;
                                                break;
                                            case "B爪":
                                                grip = 2;
                                                break;
                                            case "C爪":
                                                grip = 3;
                                                break;
                                            case "D爪":
                                                grip = 4;
                                                break;
                                            default:
                                                grip = 0;
                                                break;
                                        }

                                        if (grip != 0)
                                        {
                                            FrmIniRobot.RobotSpel.SetVar("HandSelect", grip);//下发夹爪选择
                                            FrmIniRobot.RobotSpel.SetVar("CreatPointNumber", 300 + e.RowIndex - 2);//下发位置编号
                                            FrmIniRobot.RobotSpel.MemOn("BCreatPoint");

                                            while (!FrmIniRobot.RobotSpel.MemSw("PointCreatSuccess"))//等待反馈成功标志
                                            {
                                                Thread.Sleep(10);
                                                Application.DoEvents();
                                                if (ScramManualExit)
                                                {
                                                    FrmIniRobot.RobotSpel.Stop();
                                                    goto end2;
                                                }
                                            }

                                            FrmIniRobot.RobotSpel.MemOff("PointCreatSuccess");
                                            p = FrmIniRobot.RobotSpel.GetPoint(350 + e.RowIndex - 2);//读取生成的点位，加载至dgv中
                                            dgvBVisonPoint.Rows[e.RowIndex].Cells[2].Value = p.X;
                                            dgvBVisonPoint.Rows[e.RowIndex].Cells[3].Value = p.Y;
                                            dgvBVisonPoint.Rows[e.RowIndex].Cells[4].Value = p.Z;
                                            dgvBVisonPoint.Rows[e.RowIndex].Cells[5].Value = p.U;
                                            dgvBVisonPoint.Rows[e.RowIndex].Cells[6].Value = p.Local.ToString();
                                            if (p.Hand.ToString() == "Righty")
                                            {
                                                dgvBVisonPoint.Rows[e.RowIndex].Cells[7].Value = "右手姿态";
                                            }
                                            else
                                            {
                                                dgvBVisonPoint.Rows[e.RowIndex].Cells[7].Value = "左手姿态";
                                            }

                                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");

                                        end2:
                                            opening = false;
                                        }
                                    });
                                }
                                else
                                {
                                    showMsg("请先选择夹爪", 1);
                                }
                            }
                        }
                    }

                    if (checkRobotConnect())  //检查机器人连接情况
                    {
                        if (e.ColumnIndex != 0 && e.ColumnIndex != 1)//本地坐标系点数据无法移动前往
                        {
                            if (dgvBVisonPoint.Columns[e.ColumnIndex].HeaderText == "点位测试" && !going)  //移动
                            {
                                if (dgvBVisonPoint.Rows[e.ColumnIndex].Cells[8].Value.ToString() != "无")
                                {
                                    try
                                    {
                                        going = true;
                                        FrmIniRobot.RobotSpel.AsyncMode = true;
                                        switch (dgvBVisonPoint.Rows[e.RowIndex].Cells[8].Value)//根据夹爪切换工具坐标系
                                        {
                                            case "A爪":
                                                FrmIniRobot.RobotSpel.Tool(11);
                                                break;
                                            case "B爪":
                                                FrmIniRobot.RobotSpel.Tool(12);
                                                break;
                                            case "C爪":
                                                FrmIniRobot.RobotSpel.Tool(13);
                                                break;
                                            case "D爪":
                                                FrmIniRobot.RobotSpel.Tool(14);
                                                break;
                                        }
                                        FrmIniRobot.RobotSpel.Jump(350 + e.RowIndex - 2);
                                        FrmIniRobot.RobotSpel.WaitCommandComplete();
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                    finally
                                    {
                                        going = false;
                                    }
                                }
                                else
                                {
                                    showMsg("请先选择夹爪", 1);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvTakePointData_CellContentClick(object sender, DataGridViewCellEventArgs e)  //点位从P60-P89
        {
            try
            {
                if (dgvTakePointData.Columns[e.ColumnIndex].HeaderText == "点位示教" || dgvTakePointData.Columns[e.ColumnIndex].HeaderText == "点位移动")
                {
                    RCAPINet.SpelPoint p;
                    if (checkRobotConnect())  //如果机器人准备OK,等待SDK初始化
                    {
                        if (dgvTakePointData.Columns[e.ColumnIndex].HeaderText == "点位示教" && !opening)  //示教
                        {
                            Task.Run(() =>
                            {
                                Invoke(new Action(() =>
                                {
                                    opening = true;  //防止重复开示教
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    if (FrmIniRobot.RobotSpel.TeachPoint("robot1.pts", 999, "示教取料点" + (e.RowIndex + 1).ToString(), this))
                                    {
                                        p = FrmIniRobot.RobotSpel.GetPoint(999);
                                        dgvTakePointData.Rows[e.RowIndex].Cells[1].Value = p.X;
                                        dgvTakePointData.Rows[e.RowIndex].Cells[2].Value = p.Y;
                                        dgvTakePointData.Rows[e.RowIndex].Cells[3].Value = p.Z;
                                        dgvTakePointData.Rows[e.RowIndex].Cells[4].Value = p.U;
                                        dgvTakePointData.Rows[e.RowIndex].Cells[5].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvTakePointData.Rows[e.RowIndex].Cells[6].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvTakePointData.Rows[e.RowIndex].Cells[6].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.PDel(999);
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                                    }
                                    opening = false;
                                }));
                            });
                        }

                        if (dgvTakePointData.Columns[e.ColumnIndex].HeaderText == "点位移动" && !going)  //移动
                        {
                            if (dgvTakePointData.Rows[e.RowIndex].Cells[7].Value.ToString() != "无")
                            {
                                try
                                {
                                    going = true;
                                    int grip = 0;
                                    int num;
                                    for (int i = 0; i <= e.RowIndex; i++)
                                    {
                                        if (dgvTakePointData.Rows[i].Cells[7].Value.ToString() == dgvTakePointData.Rows[e.RowIndex].Cells[7].Value.ToString())
                                        {
                                            grip++;
                                        }
                                    }

                                    FrmIniRobot.RobotSpel.Tool(0);
                                    FrmIniRobot.RobotSpel.AsyncMode = true;
                                    switch (dgvTakePointData.Rows[e.RowIndex].Cells[7].Value.ToString())  //夹爪类型进行点位下载区分
                                    {
                                        case "A爪":
                                            num = 100;
                                            break;
                                        case "B爪":
                                            num = 104;
                                            break;
                                        case "C爪":
                                            num = 108;
                                            break;
                                        case "D爪":
                                            num = 112;
                                            break;
                                        default:
                                            num = 0;
                                            break;
                                    }
                                    if (num != 0)
                                    {
                                        FrmIniRobot.RobotSpel.Jump(num + grip - 1);//-1是因为从计数从第一个点开始，故计算时会多+1
                                        FrmIniRobot.RobotSpel.WaitCommandComplete();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    going = false;
                                }
                            }
                            else
                            {
                                showMsg("请先选择夹爪", 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvVisionPointData_CellContentClick(object sender, DataGridViewCellEventArgs e)  //点位从P120-P133
        {
            try
            {
                if (dgvVisionPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" || dgvVisionPointData.Columns[e.ColumnIndex].HeaderText == "点位移动")
                {
                    RCAPINet.SpelPoint p;
                    if (checkRobotConnect())  //如果机器人准备OK,等待SDK初始化
                    {
                        if (dgvVisionPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" && !opening)  //示教
                        {
                            Task.Run(() =>
                            {
                                Invoke(new Action(() =>
                                {
                                    opening = true;  //防止重复开示教
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    if (FrmIniRobot.RobotSpel.TeachPoint("robot1.pts", 999, "示教视觉拍摄点" + (e.RowIndex + 1).ToString(), this))
                                    {
                                        p = FrmIniRobot.RobotSpel.GetPoint(999);
                                        dgvVisionPointData.Rows[e.RowIndex].Cells[1].Value = p.X;
                                        dgvVisionPointData.Rows[e.RowIndex].Cells[2].Value = p.Y;
                                        dgvVisionPointData.Rows[e.RowIndex].Cells[3].Value = p.Z;
                                        dgvVisionPointData.Rows[e.RowIndex].Cells[4].Value = p.U;
                                        dgvVisionPointData.Rows[e.RowIndex].Cells[5].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvVisionPointData.Rows[e.RowIndex].Cells[6].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvVisionPointData.Rows[e.RowIndex].Cells[6].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.PDel(999);
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                                    }
                                    opening = false;
                                }));
                            });
                        }

                        if (dgvVisionPointData.Columns[e.ColumnIndex].HeaderText == "点位移动" && !going)  //移动
                        {
                            if (dgvVisionPointData.Rows[e.RowIndex].Cells[7].Value.ToString() != "无")
                            {
                                try
                                {
                                    going = true;
                                    int num;
                                    switch (dgvVisionPointData.Rows[e.RowIndex].Cells[7].Value.ToString())  //夹爪类型进行点位下载区分
                                    {
                                        case "A轨Mark1":
                                            num = 120;
                                            break;
                                        case "A轨Mark2":
                                            num = 121;
                                            break;
                                        case "B轨Mark1":
                                            num = 122;
                                            break;
                                        case "B轨Mark2":
                                            num = 123;
                                            break;
                                        case "A爪":
                                            num = 130;
                                            break;
                                        case "B爪":
                                            num = 131;
                                            break;
                                        case "C爪":
                                            num = 132;
                                            break;
                                        case "D爪":
                                            num = 133;
                                            break;
                                        default:
                                            num = 0;
                                            break;
                                    }
                                    if (num != 0)
                                    {
                                        FrmIniRobot.RobotSpel.Tool(0);
                                        FrmIniRobot.RobotSpel.AsyncMode = true;
                                        FrmIniRobot.RobotSpel.Jump(num);
                                        FrmIniRobot.RobotSpel.WaitCommandComplete();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    going = false;
                                }
                            }
                            else
                            {
                                showMsg("请先选择夹爪", 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvThrowPointData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dgvThrowPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" || dgvThrowPointData.Columns[e.ColumnIndex].HeaderText == "点位移动")
                {
                    RCAPINet.SpelPoint p;
                    if (checkRobotConnect())  //如果机器人准备OK,等待SDK初始化
                    {
                        if (dgvThrowPointData.Columns[e.ColumnIndex].HeaderText == "点位示教" && !opening)  //示教
                        {
                            Task.Run(() =>
                            {
                                Invoke(new Action(() =>
                                {
                                    opening = true;  //防止重复开示教
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    if (FrmIniRobot.RobotSpel.TeachPoint("robot1.pts", 999, "示教抛料点" + (e.RowIndex + 1).ToString(), this))
                                    {
                                        p = FrmIniRobot.RobotSpel.GetPoint(999);
                                        dgvThrowPointData.Rows[e.RowIndex].Cells[1].Value = p.X;
                                        dgvThrowPointData.Rows[e.RowIndex].Cells[2].Value = p.Y;
                                        dgvThrowPointData.Rows[e.RowIndex].Cells[3].Value = p.Z;
                                        dgvThrowPointData.Rows[e.RowIndex].Cells[4].Value = p.U;
                                        dgvThrowPointData.Rows[e.RowIndex].Cells[5].Value = p.Local.ToString();
                                        if (p.Hand.ToString() == "Righty")
                                        {
                                            dgvThrowPointData.Rows[e.RowIndex].Cells[6].Value = "右手姿态";
                                        }
                                        else
                                        {
                                            dgvThrowPointData.Rows[e.RowIndex].Cells[6].Value = "左手姿态";
                                        }
                                        FrmIniRobot.RobotSpel.PDel(999);
                                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                                    }
                                    opening = false;
                                }));
                            });
                        }
                        if (dgvThrowPointData.Columns[e.ColumnIndex].HeaderText == "点位移动" && !going)  //移动
                        {
                            if (dgvThrowPointData.Rows[e.RowIndex].Cells[7].Value.ToString() != "无")
                            {
                                try
                                {
                                    going = true;
                                    FrmIniRobot.RobotSpel.Tool(0);
                                    FrmIniRobot.RobotSpel.AsyncMode = true;
                                    FrmIniRobot.RobotSpel.Jump(e.RowIndex + 140);
                                    FrmIniRobot.RobotSpel.WaitCommandComplete();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    going = false;
                                }
                            }
                            else
                            {
                                showMsg("请先选择夹爪", 1);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 打开机器人工具
        #region RobotManager
        private void RobotManager_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.RunDialog(SpelDialogs.RobotManager, this);
                    Common.myLog.writeOperateContent("打开机器人管理器", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region IOMonitor
        private void IOMonitor_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.ShowWindow(SpelWindows.IOMonitor, this);
                    Common.myLog.writeOperateContent("打开机器人IO监控", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region TaskManager
        private void TaskManager_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.ShowWindow(SpelWindows.TaskManager, this);
                    Common.myLog.writeOperateContent("打开机器人任务管理器", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region Simulator
        private void Simulator_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.ShowWindow(SpelWindows.Simulator, this);
                    Common.myLog.writeOperateContent("打开机器人模拟器", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region ControllerTools
        private void ControllerTools_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.RunDialog(SpelDialogs.ControllerTools, this);
                    Common.myLog.writeOperateContent("打开机器人控制器", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region ProgramMode
        private void ProgramMode_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.OperationMode = SpelOperationMode.Program;
                    Common.myLog.writeOperateContent("打开机器人软件本体", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region VisionGuide
        private void VisionGuide_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    FrmIniRobot.RobotSpel.RunDialog(SpelDialogs.VisionGuide, this);
                    Common.myLog.writeOperateContent("打开视觉调试", Global.UserPermission, Global.UserPermission);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #endregion

        #region 点位下载
        private void tlsbtnADownload_Click(object sender, EventArgs e)//P200-P249
        {
            try
            {
                if (checkRobotConnect())
                {
                    dgvAPointData.EndEdit();
                    FrmIniRobot.RobotSpel.PDel(200, 249);  //下载点位前先删除P200-249点

                    //下载放料点点位
                    SpelPoint p = new SpelPoint();
                    for (int i = 0; i < Convert.ToInt32(tlscomAPointCount.SelectedIndex * 4); i++)
                    {
                        p.X = float.Parse(dgvAPointData.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvAPointData.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvAPointData.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvAPointData.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvAPointData.Rows[i].Cells[6].Value);
                        if (dgvAPointData.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }
                        FrmIniRobot.RobotSpel.SetPoint(200 + i, p);
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }

                    //下载任务信息
                    int[] ATask = new int[10];
                    int[] BTask = new int[10];
                    int[] CTask = new int[10];
                    int[] DTask = new int[10];

                    for (int i = 0; i < Convert.ToInt32(tlscomAPointCount.SelectedIndex); i++)//将表格数据载入数组中
                    {
                        int temp = 4 * i;//换算出表格行数

                        if (dgvAPointData.Rows[temp].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            ATask[i] = Int32.Parse(dgvAPointData.Rows[temp].Cells[1].Value.ToString()) * 1000 + temp + 1;//任务格式前三位为取料点，后三位为放料点;i从0开始计数，任务数从1开始计算
                        }
                        else
                        {
                            ATask[i] = 0;
                        }

                        if (dgvAPointData.Rows[temp + 1].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            BTask[i] = Int32.Parse(dgvAPointData.Rows[temp + 1].Cells[1].Value.ToString()) * 1000 + temp + 2;
                        }
                        else
                        {
                            BTask[i] = 0;
                        }

                        if (dgvAPointData.Rows[temp + 2].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            CTask[i] = Int32.Parse(dgvAPointData.Rows[temp + 2].Cells[1].Value.ToString()) * 1000 + temp + 3;
                        }
                        else
                        {
                            CTask[i] = 0;
                        }

                        if (dgvAPointData.Rows[temp + 3].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            DTask[i] = Int32.Parse(dgvAPointData.Rows[temp + 3].Cells[1].Value.ToString()) * 1000 + temp + 4;
                        }
                        else
                        {
                            DTask[i] = 0;
                        }
                    }
                    if (tlscomAPointCount.SelectedIndex < 10)
                    {
                        for (int i = Convert.ToInt32(tlscomAPointCount.SelectedIndex); i < 10; i++)//将剩余数组赋值0，否则null无法下发数据
                        {
                            ATask[i] = 0;
                            BTask[i] = 0;
                            CTask[i] = 0;
                            DTask[i] = 0;
                        }
                    }

                    for (int i = 0; i < ATask.Length; i++)//由于未知原因，API无法整个数组一次性下发，故分批单个发送
                    {
                        FrmIniRobot.RobotSpel.SetVar("A_a(" + i.ToString() + ")", ATask[i]);
                        FrmIniRobot.RobotSpel.SetVar("A_b(" + i.ToString() + ")", BTask[i]);
                        FrmIniRobot.RobotSpel.SetVar("A_c(" + i.ToString() + ")", CTask[i]);
                        FrmIniRobot.RobotSpel.SetVar("A_d(" + i.ToString() + ")", DTask[i]);
                    }

                    Common.myLog.writeOperateContent("下载A轨放料点位与任务信息至机器人", Global.UserPermission, Global.UserPermission);
                    showMsg("A轨点位与任务信息下载成功！", 0);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnADownloadV_Click(object sender, EventArgs e)//P300-P349
        {
            try
            {
                if (checkRobotConnect())
                {
                    dgvAVisonPoint.EndEdit();

                    //先下载视觉本地坐标系点位
                    FrmIniRobot.RobotSpel.PDel(10, 11);//A轨本地坐标系点位数据

                    SpelPoint p = new SpelPoint();
                    for (int i = 0; i < 2; i++)
                    {
                        p.X = float.Parse(dgvAVisonPoint.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvAVisonPoint.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvAVisonPoint.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvAVisonPoint.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvAVisonPoint.Rows[i].Cells[6].Value);
                        if (dgvAVisonPoint.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }
                        FrmIniRobot.RobotSpel.SetPoint(10 + i, p);
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }

                    //视觉放料点位坐标
                    FrmIniRobot.RobotSpel.PDel(300, 349);  //下载点位前先删除P300-349点

                    for (int i = 2; i < dgvAVisonPoint.RowCount; i++)//视觉dgv前两行是本地坐标系点位，故从2开始
                    {
                        p.X = float.Parse(dgvAVisonPoint.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvAVisonPoint.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvAVisonPoint.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvAVisonPoint.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvAVisonPoint.Rows[i].Cells[6].Value);
                        if (dgvAVisonPoint.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }
                        FrmIniRobot.RobotSpel.SetPoint(300 + i - 2, p);//视觉dgv前两行是本地坐标系点位，故从2开始，所以此处需要-2
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }
                    Common.myLog.writeOperateContent("下载A轨视觉点位至机器人", Global.UserPermission, Global.UserPermission);
                    showMsg("A轨视觉点位下载成功！", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnBDownload_Click(object sender, EventArgs e)//P250-P299
        {
            try
            {
                if (checkRobotConnect())
                {
                    dgvBPointData.EndEdit();
                    FrmIniRobot.RobotSpel.PDel(250, 299);  //下载点位前先删除P250-299点

                    SpelPoint p = new SpelPoint();
                    for (int i = 0; i < dgvVisionPointData.RowCount; i++)
                    {
                        p.X = float.Parse(dgvBPointData.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvBPointData.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvBPointData.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvBPointData.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvBPointData.Rows[i].Cells[6].Value);
                        if (dgvBPointData.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }

                        FrmIniRobot.RobotSpel.SetPoint(250 + i, p);
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }

                    //下载任务信息
                    int[] ATask = new int[10];
                    int[] BTask = new int[10];
                    int[] CTask = new int[10];
                    int[] DTask = new int[10];

                    for (int i = 0; i < Convert.ToInt32(tlscomBPointCount.SelectedIndex); i++)//将表格数据载入数组中
                    {
                        int temp = 4 * i;//换算出表格行数

                        if (dgvBPointData.Rows[temp].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            ATask[i] = Int32.Parse(dgvBPointData.Rows[temp].Cells[1].Value.ToString()) * 1000 + temp + 1;//任务格式前三位为取料点，后三位为放料点;i从0开始计数，任务数从1开始计算
                        }
                        else
                        {
                            ATask[i] = 0;
                        }

                        if (dgvBPointData.Rows[temp + 1].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            BTask[i] = Int32.Parse(dgvBPointData.Rows[temp + 1].Cells[1].Value.ToString()) * 1000 + temp + 2;
                        }
                        else
                        {
                            BTask[i] = 0;
                        }

                        if (dgvBPointData.Rows[temp + 2].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            CTask[i] = Int32.Parse(dgvBPointData.Rows[temp + 2].Cells[1].Value.ToString()) * 1000 + temp + 3;
                        }
                        else
                        {
                            CTask[i] = 0;
                        }

                        if (dgvBPointData.Rows[temp + 3].Cells[8].Value.ToString() != "无")//判断取料位有无设置，没有则认为当前任务轮次该夹爪无任务
                        {
                            DTask[i] = Int32.Parse(dgvBPointData.Rows[temp + 3].Cells[1].Value.ToString()) * 1000 + temp + 4;
                        }
                        else
                        {
                            DTask[i] = 0;
                        }
                    }
                    if (tlscomBPointCount.SelectedIndex < 10)
                    {
                        for (int i = Convert.ToInt32(tlscomBPointCount.SelectedIndex); i < 10; i++)//将剩余数组赋值0，否则null无法下发数据
                        {
                            ATask[i] = 0;
                            BTask[i] = 0;
                            CTask[i] = 0;
                            DTask[i] = 0;
                        }
                    }

                    for (int i = 0; i < ATask.Length; i++)//由于未知原因，API无法整个数组一次性下发，故分批单个发送
                    {
                        FrmIniRobot.RobotSpel.SetVar("B_a(" + i.ToString() + ")", ATask[i]);
                        FrmIniRobot.RobotSpel.SetVar("B_b(" + i.ToString() + ")", BTask[i]);
                        FrmIniRobot.RobotSpel.SetVar("B_c(" + i.ToString() + ")", CTask[i]);
                        FrmIniRobot.RobotSpel.SetVar("B_d(" + i.ToString() + ")", DTask[i]);
                    }

                    Common.myLog.writeOperateContent("下载B轨点位与任务信息至机器人", Global.UserPermission, Global.UserPermission);
                    showMsg("B轨点位与任务下载成功！", 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnBDowloadV_Click(object sender, EventArgs e)//P350-P399
        {
            try
            {
                if (checkRobotConnect())
                {
                    dgvBVisonPoint.EndEdit();
                    FrmIniRobot.RobotSpel.PDel(16, 17);
                    SpelPoint p = new SpelPoint();

                    for (int i = 0; i < 2; i++)
                    {
                        p.X = float.Parse(dgvBVisonPoint.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvBVisonPoint.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvBVisonPoint.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvBVisonPoint.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvBVisonPoint.Rows[i].Cells[6].Value);
                        if (dgvBVisonPoint.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }
                        FrmIniRobot.RobotSpel.SetPoint(16 + i, p);
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }

                    FrmIniRobot.RobotSpel.PDel(350, 399);  //下载点位前先删除P250-299点   

                    for (int i = 2; i < dgvBVisonPoint.RowCount; i++)
                    {
                        p.X = float.Parse(dgvBVisonPoint.Rows[i].Cells[2].Value.ToString());
                        p.Y = float.Parse(dgvBVisonPoint.Rows[i].Cells[3].Value.ToString());
                        p.Z = float.Parse(dgvBVisonPoint.Rows[i].Cells[4].Value.ToString());
                        p.U = float.Parse(dgvBVisonPoint.Rows[i].Cells[5].Value.ToString());
                        p.Local = Convert.ToInt32(dgvBVisonPoint.Rows[6].Cells[5].Value);
                        if (dgvBVisonPoint.Rows[i].Cells[7].Value.ToString() == "左手姿态")
                        {
                            p.Hand = SpelHand.Lefty;
                        }
                        else
                        {
                            p.Hand = SpelHand.Righty;
                        }
                        FrmIniRobot.RobotSpel.SetPoint(350 + i - 2, p);
                        FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                    }
                    Common.myLog.writeOperateContent("下载B轨视觉点位至机器人", Global.UserPermission, Global.UserPermission);
                    showMsg("B轨视觉点位下载成功！", 0);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnTakeDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    int gripError = 0;
                    int gripA = 0;
                    int gripB = 0;
                    int gripC = 0;
                    int gripD = 0;
                    for (int i = 0; i < dgvTakePointData.RowCount; i++)  //判断是否有夹爪未设置
                    {
                        if (dgvTakePointData.Rows[i].Cells[7].Value.ToString() == "无")
                        {
                            gripError++;
                        }
                    }
                    if (gripError > 0)
                    {
                        showMsg("有夹爪未设置！", 1);
                    }
                    else
                    {
                        dgvTakePointData.EndEdit();
                        FrmIniRobot.RobotSpel.PDel(100, 115);  //下载点位前先删除P60-89点

                        SpelPoint p = new SpelPoint();
                        for (int i = 0; i < Convert.ToInt32(tlscomTakePointCount.SelectedIndex); i++)
                        {
                            p.X = float.Parse(dgvTakePointData.Rows[i].Cells[1].Value.ToString());
                            p.Y = float.Parse(dgvTakePointData.Rows[i].Cells[2].Value.ToString());
                            p.Z = float.Parse(dgvTakePointData.Rows[i].Cells[3].Value.ToString());
                            p.U = float.Parse(dgvTakePointData.Rows[i].Cells[4].Value.ToString());
                            p.Local = Convert.ToInt32(dgvTakePointData.Rows[i].Cells[5].Value);
                            if (dgvTakePointData.Rows[i].Cells[6].Value.ToString() == "左手姿态")
                            {
                                p.Hand = SpelHand.Lefty;
                            }
                            else
                            {
                                p.Hand = SpelHand.Righty;
                            }

                            switch (dgvTakePointData.Rows[i].Cells[7].Value.ToString())  //夹爪类型进行点位下载区分
                            {
                                case "A爪":
                                    FrmIniRobot.RobotSpel.SetPoint(100 + gripA, p);
                                    gripA++;
                                    break;
                                case "B爪":
                                    FrmIniRobot.RobotSpel.SetPoint(104 + gripB, p);
                                    gripB++;
                                    break;
                                case "C爪":
                                    FrmIniRobot.RobotSpel.SetPoint(108 + gripC, p);
                                    gripC++;
                                    break;
                                case "D爪":
                                    FrmIniRobot.RobotSpel.SetPoint(112 + gripD, p);
                                    gripD++;
                                    break;
                                default:
                                    break;
                            }
                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                        }
                        Common.myLog.writeOperateContent("下载取料点位至机器人", Global.UserPermission, Global.UserPermission);
                        showMsg("取料点位下载成功！", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnVisionDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    int gripError = 0;
                    for (int i = 0; i < dgvVisionPointData.RowCount; i++)  //判断是否有夹爪未设置
                    {
                        if (dgvVisionPointData.Rows[i].Cells[7].Value.ToString() == "无")
                        {
                            gripError++;
                        }
                    }
                    if (gripError > 0)
                    {
                        showMsg("有夹爪未设置！", 1);
                    }
                    else
                    {
                        dgvVisionPointData.EndEdit();
                        FrmIniRobot.RobotSpel.PDel(120, 139);  //下载点位前先删除P90-119点

                        SpelPoint p = new SpelPoint();
                        for (int i = 0; i < Convert.ToInt32(tlscomVisionPointCount.SelectedIndex); i++)
                        {
                            p.X = float.Parse(dgvVisionPointData.Rows[i].Cells[1].Value.ToString());
                            p.Y = float.Parse(dgvVisionPointData.Rows[i].Cells[2].Value.ToString());
                            p.Z = float.Parse(dgvVisionPointData.Rows[i].Cells[3].Value.ToString());
                            p.U = float.Parse(dgvVisionPointData.Rows[i].Cells[4].Value.ToString());
                            p.Local = Convert.ToInt32(dgvVisionPointData.Rows[i].Cells[5].Value);
                            if (dgvVisionPointData.Rows[i].Cells[6].Value.ToString() == "左手姿态")
                            {
                                p.Hand = SpelHand.Lefty;
                            }
                            else
                            {
                                p.Hand = SpelHand.Righty;
                            }

                            switch (dgvVisionPointData.Rows[i].Cells[7].Value.ToString())  //夹爪类型进行点位下载区分
                            {
                                case "A轨Mark1":
                                    FrmIniRobot.RobotSpel.SetPoint(120, p);
                                    break;
                                case "A轨Mark2":
                                    FrmIniRobot.RobotSpel.SetPoint(121, p);
                                    break;
                                case "B轨Mark1":
                                    FrmIniRobot.RobotSpel.SetPoint(122, p);
                                    break;
                                case "B轨Mark2":
                                    FrmIniRobot.RobotSpel.SetPoint(123, p);
                                    break;
                                case "A爪":
                                    FrmIniRobot.RobotSpel.SetPoint(130, p);
                                    break;
                                case "B爪":
                                    FrmIniRobot.RobotSpel.SetPoint(131, p);
                                    break;
                                case "C爪":
                                    FrmIniRobot.RobotSpel.SetPoint(132, p);
                                    break;
                                case "D爪":
                                    FrmIniRobot.RobotSpel.SetPoint(132, p);
                                    break;
                                default:
                                    break;
                            }
                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                        }
                        Common.myLog.writeOperateContent("下载视觉点位至机器人", Global.UserPermission, Global.UserPermission);
                        showMsg("视觉点位下载成功！", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tlsbtnThrowDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkRobotConnect())
                {
                    int gripError = 0;
                    for (int i = 0; i < dgvThrowPointData.RowCount; i++)  //判断是否有夹爪未设置
                    {
                        if (dgvThrowPointData.Rows[i].Cells[7].Value.ToString() == "无")
                        {
                            gripError++;
                        }
                    }
                    if (gripError > 0)
                    {
                        showMsg("有夹爪未设置！", 1);
                    }
                    else
                    {
                        dgvThrowPointData.EndEdit();
                        FrmIniRobot.RobotSpel.PDel(140, 149);  //下载点位前先删除P70-79点

                        SpelPoint p = new SpelPoint();
                        for (int i = 0; i < Convert.ToInt32(tlscomThrowPointCount.SelectedIndex); i++)
                        {
                            p.X = float.Parse(dgvThrowPointData.Rows[i].Cells[1].Value.ToString());
                            p.Y = float.Parse(dgvThrowPointData.Rows[i].Cells[2].Value.ToString());
                            p.Z = float.Parse(dgvThrowPointData.Rows[i].Cells[3].Value.ToString());
                            p.U = float.Parse(dgvThrowPointData.Rows[i].Cells[4].Value.ToString());
                            p.Local = Convert.ToInt32(dgvThrowPointData.Rows[i].Cells[5].Value);
                            if (dgvThrowPointData.Rows[i].Cells[6].Value.ToString() == "左手姿态")
                            {
                                p.Hand = SpelHand.Lefty;
                            }
                            else
                            {
                                p.Hand = SpelHand.Righty;
                            }

                            switch (dgvThrowPointData.Rows[i].Cells[7].Value.ToString())  //夹爪类型进行点位下载区分
                            {
                                case "A爪":
                                    FrmIniRobot.RobotSpel.SetPoint(140, p);
                                    break;
                                case "B爪":
                                    FrmIniRobot.RobotSpel.SetPoint(141, p);
                                    break;
                                case "C爪":
                                    FrmIniRobot.RobotSpel.SetPoint(142, p);
                                    break;
                                case "D爪":
                                    FrmIniRobot.RobotSpel.SetPoint(143, p);
                                    break;
                                default:
                                    break;
                            }
                            FrmIniRobot.RobotSpel.SavePoints("robot1.pts");
                        }
                        Common.myLog.writeOperateContent("下载抛料点位至机器人", Global.UserPermission, Global.UserPermission);
                        showMsg("抛料点位下载成功！", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 夹爪示教点修改事件
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridView dataGridView = (DataGridView)sender;
                if (e.ColumnIndex >= 0)
                {
                    if (dataGridView.Columns[e.ColumnIndex].HeaderText == "夹爪")
                    {
                        int gripA = 0;
                        int gripB = 0;
                        int gripC = 0;
                        int gripD = 0;
                        int AMark1 = 0;
                        int AMark2 = 0;
                        int BMark1 = 0;
                        int BMark2 = 0;

                        for (int i = 0; i < dataGridView.RowCount; i++)
                        {
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "A爪")
                            {
                                gripA++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "B爪")
                            {
                                gripB++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "C爪")
                            {
                                gripC++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "D爪")
                            {
                                gripD++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "A轨Mark1")
                            {
                                AMark1++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "A轨Mark2")
                            {
                                AMark2++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "B轨Mark1")
                            {
                                BMark1++;
                            }
                            if (dataGridView.Rows[i].Cells[e.ColumnIndex].Value.ToString() == "B轨Mark2")
                            {
                                BMark2++;
                            }
                        }
                        if (dataGridView.Name == "dgvVisionPointData")
                        {
                            if (gripA > 1 || gripB > 1 || gripC > 1 || gripD > 1 || AMark1 > 1 || AMark2 > 1 /*|| BMark1 > 1 || BMark2 > 1*/)
                            {
                                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "无";
                                showMsg("超过该夹爪允许设置数，每个夹爪最多示教1个点！", 1);
                            }
                        }
                        else if (dataGridView.Name == "dgvTakePointData" || dataGridView.Name == "dgvThrowPointData")
                        {
                            if (gripA > 4 || gripB > 4 || gripC > 4 || gripD > 4)
                            {
                                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "无";
                                showMsg("超过该夹爪允许设置数，每个夹爪最多示教4个点！", 1);
                            }
                        }
                        else
                        {
                            if (gripA > 10 || gripB > 10 || gripC > 10 || gripD > 10)
                            {
                                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "无";
                                showMsg("超过该夹爪允许设置数，每个夹爪最多示教10个点！", 1);
                            }
                        }

                        switch (dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                        {
                            case "A爪":
                                if (dataGridView.Name == "dgvTakePointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PG1_" + gripA.ToString();
                                }
                                else if (dataGridView.Name == "dgvThrowPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PNG1";
                                }
                                else if (dataGridView.Name == "dgvVisionPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhoto1";
                                }
                                break;
                            case "B爪":
                                if (dataGridView.Name == "dgvTakePointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PG2_" + gripB.ToString();
                                }
                                else if (dataGridView.Name == "dgvThrowPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PNG2";
                                }
                                else if (dataGridView.Name == "dgvVisionPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhoto2";
                                }
                                break;
                            case "C爪":
                                if (dataGridView.Name == "dgvTakePointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PG3_" + gripC.ToString();
                                }
                                else if (dataGridView.Name == "dgvThrowPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PNG3";
                                }
                                else if (dataGridView.Name == "dgvVisionPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhoto3";
                                }
                                break;
                            case "D爪":
                                if (dataGridView.Name == "dgvTakePointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PG4_" + gripD.ToString();
                                }
                                else if (dataGridView.Name == "dgvThrowPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PNG4";
                                }
                                else if (dataGridView.Name == "dgvVisionPointData")
                                {
                                    dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhoto4";
                                }
                                break;
                            case "A轨Mark1":
                                dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhotoPCB1_A01";
                                break;
                            case "A轨Mark2":
                                dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhotoPCB1_A02";
                                break;
                            case "B轨Mark1":
                                dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhotoPCB1_B01";
                                break;
                            case "B轨Mark2":
                                dataGridView.Rows[e.RowIndex].Cells[0].Value = "PPhotoPCB1_B02";
                                break;
                            default:
                                dataGridView.Rows[e.RowIndex].Cells[0].Value = "";
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //放料点选择夹爪判断有无选择取料位
        private void dgvPointData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            if (e.ColumnIndex > 0)
            {
                if (dataGridView.Columns[e.ColumnIndex].HeaderText == "夹爪")
                {
                    if (dataGridView.Rows[e.RowIndex].Cells[8].Value.ToString() != "无")
                    {
                        if (dataGridView.Rows[e.RowIndex].Cells[1].Value.ToString() == "0")
                        {
                            dataGridView.Rows[e.RowIndex].Cells[8].Value = "无";
                            showMsg("请先选择取料位！", 1);
                        }
                    }
                }
            }
        }
        #endregion

        #region 手动按钮
        /// <summary>
        /// 检查机器人手自动状态,手动时返回true
        /// </summary>
        private bool checkRobotStatus()
        {
            if (checkRobotConnect())
            {
                if (!FrmIniRobot.RobotSpel.MemSw("AutoMode") && FrmIniRobot.RobotSpel.Oport(1))
                {
                    return true;
                }
                else
                {
                    showMsg("机器人未进入手动模式", 1);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 手动夹爪选择
        /// </summary>
        private int manualGripChose;
        /// <summary>
        /// 手动放料位选择
        /// </summary>
        private int manualPGNumber;
        /// <summary>
        /// 夹爪选择更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlscomGripChose_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox tlscomGripChose = (ToolStripComboBox)sender;
            //同步控件
            tlscomAGripChose.SelectedIndex = tlscomGripChose.SelectedIndex; 
            tlscomAGripChoseV.SelectedIndex = tlscomGripChose.SelectedIndex;
            tlscomBGripChose.SelectedIndex = tlscomGripChose.SelectedIndex;
            tlscomBGripChoseV.SelectedIndex = tlscomGripChose.SelectedIndex;
            tlscomTakeGripChose.SelectedIndex = tlscomGripChose.SelectedIndex;
            tlscomThrowGripChose.SelectedIndex = tlscomGripChose.SelectedIndex;
            tlscomVisionGripChose.SelectedIndex = tlscomGripChose.SelectedIndex;
            //更新数据
            manualGripChose = tlscomGripChose.SelectedIndex;
        }
        /// <summary>
        /// 取料点选择更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlscomPGChose_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolStripComboBox tlscomPGChose = (ToolStripComboBox)sender;
            //同步控件
            tlscomAPGChose.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomAPGChoseV.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomBPGChose.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomBPGChoseV.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomTakePGChose.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomThrowPGChose.SelectedIndex = tlscomPGChose.SelectedIndex;
            tlscomVisionPGChose.SelectedIndex = tlscomPGChose.SelectedIndex;
            //更新数据
            manualPGNumber = tlscomPGChose.SelectedIndex;
        }
        private void cobGripChose_SelectedIndexChanged(object sender, EventArgs e)
        {
            manualGripChose = cobGripChose.SelectedIndex;
        }

        private void cobPGChose_SelectedValueChanged(object sender, EventArgs e)
        {
            manualPGNumber = cobPGChose.SelectedIndex;
        }

        private void btnAGrip_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G1Grap"))
                {
                    FrmIniRobot.RobotSpel.Off("G1Grap");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G1Grap");
                }
            }
        }

        private void btnBGrip_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G2Grap"))
                {
                    FrmIniRobot.RobotSpel.Off("G2Grap");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G2Grap");
                }
            }
        }

        private void btnCGrip_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G3Grap"))
                {
                    FrmIniRobot.RobotSpel.Off("G3Grap");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G3Grap");
                }
            }
        }

        private void btnDGrip_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G4Grap"))
                {
                    FrmIniRobot.RobotSpel.Off("G4Grap");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G4Grap");
                }
            }
        }

        private void btnAPush_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G1Push"))
                {
                    FrmIniRobot.RobotSpel.Off("G1Push");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G1Push");
                }
            }
        }

        private void btnBPush_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G2Push"))
                {
                    FrmIniRobot.RobotSpel.Off("G2Push");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G2Push");
                }
            }
        }

        private void btnCPush_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G3Push"))
                {
                    FrmIniRobot.RobotSpel.Off("G3Push");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G3Push");
                }
            }
        }

        private void btnDPush_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("G4Push"))
                {
                    FrmIniRobot.RobotSpel.Off("G4Push");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("G4Push");
                }
            }
        }

        private void btnJ4Light_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("GLight"))
                {
                    FrmIniRobot.RobotSpel.Off("GLight");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("GLight");
                }
            }
        }

        private void btnGripLight_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("J4Light"))
                {
                    FrmIniRobot.RobotSpel.Off("J4Light");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("J4Light");
                }
            }
        }
        /// <summary>
        /// 手动夹爪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitGrip_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                switch (manualGripChose)
                {
                    case 1:
                        if (FrmIniRobot.RobotSpel.Oport("G1Grap"))
                        {
                            FrmIniRobot.RobotSpel.Off("G1Grap");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G1Grap");
                        }
                        break;
                    case 2:
                        if (FrmIniRobot.RobotSpel.Oport("G2Grap"))
                        {
                            FrmIniRobot.RobotSpel.Off("G2Grap");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G2Grap");
                        }
                        break;
                    case 3:
                        if (FrmIniRobot.RobotSpel.Oport("G3Grap"))
                        {
                            FrmIniRobot.RobotSpel.Off("G3Grap");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G3Grap");
                        }
                        break;
                    case 4:
                        if (FrmIniRobot.RobotSpel.Oport("G4Grap"))
                        {
                            FrmIniRobot.RobotSpel.Off("G4Grap");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G4Grap");
                        }
                        break;
                    default:
                        showMsg("请选择夹爪!", 1);
                        break;
                }
            }
        }
        /// <summary>
        /// 手动气缸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitPush_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                switch (manualGripChose)
                {
                    case 1:
                        if (FrmIniRobot.RobotSpel.Oport("G1Push"))
                        {
                            FrmIniRobot.RobotSpel.Off("G1Push");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G1Push");
                        }
                        break;
                    case 2:
                        if (FrmIniRobot.RobotSpel.Oport("G2Push"))
                        {
                            FrmIniRobot.RobotSpel.Off("G2Push");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G2Push");
                        }
                        break;
                    case 3:
                        if (FrmIniRobot.RobotSpel.Oport("G3Push"))
                        {
                            FrmIniRobot.RobotSpel.Off("G3Push");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G3Push");
                        }
                        break;
                    case 4:
                        if (FrmIniRobot.RobotSpel.Oport("G4Push"))
                        {
                            FrmIniRobot.RobotSpel.Off("G4Push");
                        }
                        else
                        {
                            FrmIniRobot.RobotSpel.On("G4Push");
                        }
                        break;
                    default:
                        showMsg("请选择夹爪!", 1);
                        break;
                }
            }
        }
        /// <summary>
        /// 手动夹爪光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitGripLight_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("GLight"))
                {
                    FrmIniRobot.RobotSpel.Off("GLight");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("GLight");
                }
            }
        }
        /// <summary>
        /// 手动轨道光源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitJ4Light_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                if (FrmIniRobot.RobotSpel.Oport("J4Light"))
                {
                    FrmIniRobot.RobotSpel.Off("J4Light");
                }
                else
                {
                    FrmIniRobot.RobotSpel.On("J4Light");
                }
            }
        }
        /// <summary>
        /// 一键取料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitOnceGet_Click(object sender, EventArgs e)
        {
            if (checkRobotStatus())
            {
                if (manualGripChose != 0 && manualPGNumber != 0)
                {
                    FrmIniRobot.RobotSpel.SetVar("HandSelect", manualGripChose);
                    FrmIniRobot.RobotSpel.SetVar("PGNumber", manualPGNumber);
                    FrmIniRobot.RobotSpel.MemOn("OnceGet");
                }
                else
                {
                    showMsg("夹爪或取料位未设置！", 1);
                }
            }
        }
        /// <summary>
        /// 一键获取工具坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitOnceTool_Click(object sender, EventArgs e)
        {
            if (checkRobotStatus())
            {
                if (manualGripChose != 0 && manualPGNumber != 0)
                {
                    Task.Run(() =>
                    {
                        FrmIniRobot.RobotSpel.SetVar("HandSelect", manualGripChose);
                        FrmIniRobot.RobotSpel.SetVar("MMSelect", manualPGNumber);
                        FrmIniRobot.RobotSpel.MemOn("OnceTool");
                        while (!FrmMain.IO.ToolCreatSuccess && !FrmMain.IO.ToolCreatFail)
                        {
                            Thread.Sleep(10);
                            Application.DoEvents();
                            if (ScramManualExit)
                            {
                                FrmIniRobot.RobotSpel.Stop();
                                goto end;
                            }
                        }

                        if (FrmMain.IO.ToolCreatSuccess)
                        {
                            FrmMain.IO.ToolCreatSuccess = false;
                            showMsg("工具坐标创建成功", 0);
                        }
                        else if (FrmMain.IO.ToolCreatFail)
                        {
                            FrmMain.IO.ToolCreatFail = false;
                            showMsg("工具坐标创建失败", 1);
                        }

                    end:;
                    });
                }
                else
                {
                    showMsg("夹爪或取料位未设置！", 1);
                }
            }
        }
        /// <summary>
        /// 进去手动模式按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitRobotManual_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                int temp = FrmIniRobot.RobotStar(false);
                if (temp == 0)
                {
                    showMsg("进入手动状态成功！", 0);
                }
                else
                {
                    showMsg("机器人报警:" + temp.ToString(), 1);
                }
            }
        }
        /// <summary>
        /// 停止任务按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitStopRobot_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                FrmIniRobot.RobotSpel.Stop();
            }
        }
        /// <summary>
        /// 报警复位按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitRobotReset_Click(object sender, EventArgs e)
        {
            if (checkRobotConnect())
            {
                FrmIniRobot.RobotSpel.Reset();
            }
        }
        /// <summary>
        /// 连接机器人按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tlsmitRobotConnect_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tlsmit = (ToolStripMenuItem)sender;

            if (tlsmit.Text == "连接机器人")
            {
                if (File.Exists(@Global.RobotProPath))
                {
                    try
                    {
                        FrmIniRobot.RobotSpel.Initialize();  //初始化机器人SDK
                        FrmIniRobot.RobotSpel.Project = @Global.RobotProPath;  //加载项目文件
                        FrmIniRobot.RobotSpel.ResetAbortEnabled = false;
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

                tlsmitARobotConnect.Text = "断开机器人连接";
                tlsmitARobotConnectV.Text = "断开机器人连接";
                tlsmitBRobotConnect.Text = "断开机器人连接";
                tlsmitBRobotConnectV.Text = "断开机器人连接";
                tlsmitTakeRobotConnect.Text = "断开机器人连接";
                tlsmitThrowRobotConnect.Text = "断开机器人连接";
                tlsmitVisionRobotConnect.Text = "断开机器人连接";
            }
            else
            {
                FrmIniRobot.RobotDisconnect();

                tlsmitARobotConnect.Text = "连接机器人";
                tlsmitARobotConnectV.Text = "连接机器人";
                tlsmitBRobotConnect.Text = "连接机器人";
                tlsmitBRobotConnectV.Text = "连接机器人";
                tlsmitTakeRobotConnect.Text = "连接机器人";
                tlsmitThrowRobotConnect.Text = "连接机器人";
                tlsmitVisionRobotConnect.Text = "连接机器人";
            }
        }
        #endregion

        #region 合并单元格重绘事件
        private void dgvAPointData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            // 对第1列相同单元格进行合并
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                using (Brush gridBrush = new SolidBrush(dgv.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        // 清除单元格
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        // 画 Grid 边线（仅画单元格的底边线和右边线）
                        //   如果下一行和当前行的数据不同，则在当前的单元格画一条底边线
                        if ((e.RowIndex < dgv.Rows.Count - 1 && dgv.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() != e.Value.ToString()) || e.RowIndex == dgv.Rows.Count - 1)
                        {
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        }

                        // 画右边线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);

                        // 画（填写）单元格内容，相同的内容的单元格只填写第一个
                        if (e.Value != null)
                        {
                            //if (e.RowIndex > 0 && 
                            //    dgvAPointData.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString()
                            //    &&dgvAPointData.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString())
                            if (e.RowIndex > 0 && (e.RowIndex % 4) == 1)
                            {
                                e.Graphics.DrawString((String)e.Value, e.CellStyle.Font, Brushes.White, e.CellBounds.X + (e.CellBounds.Width - (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Width) / 2, e.CellBounds.Y + 5, StringFormat.GenericDefault);
                            }
                            else
                            {

                            }
                        }
                        e.Handled = true;
                    }
                }
            }
        }

        private void dgvAVisonPoint_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            // 对第1列相同单元格进行合并
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                using (Brush gridBrush = new SolidBrush(dgv.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush))
                    {
                        // 清除单元格
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
                        // 画 Grid 边线（仅画单元格的底边线和右边线）
                        //   如果下一行和当前行的数据不同，则在当前的单元格画一条底边线
                        if ((e.RowIndex < dgv.Rows.Count - 1 && dgv.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() != e.Value.ToString()) || e.RowIndex == dgv.Rows.Count - 1)
                        {
                            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
                        }

                        // 画右边线
                        e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);

                        // 画（填写）单元格内容，相同的内容的单元格只填写第一个
                        if (e.Value != null)
                        {
                            //if (e.RowIndex > 0 && 
                            //    dgvAPointData.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString()
                            //    &&dgvAPointData.Rows[e.RowIndex + 1].Cells[e.ColumnIndex].Value.ToString() == e.Value.ToString())
                            if (e.RowIndex >= 0 && (((e.RowIndex - 2) % 4) == 1 || e.RowIndex < 2))
                            {
                                e.Graphics.DrawString((String)e.Value, e.CellStyle.Font, Brushes.White, e.CellBounds.X + (e.CellBounds.Width - (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Width) / 2, e.CellBounds.Y + 5, StringFormat.GenericDefault);
                            }
                            else
                            {

                            }
                        }
                        e.Handled = true;
                    }
                }
            }
        }
        #endregion

        #region TabControl重绘事件
        private void tbcTeachPoint_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl1 = (TabControl)sender;
            // 获取TabControl主控件的工作区域
            Rectangle rec = tabControl1.ClientRectangle;
            //获取背景图片，我的背景图片在项目资源文件中。
            //Image backImage = Resources.logo方形;

            //新建一个StringFormat对象，用于对标签文字的布局设置
            StringFormat StrFormat = new StringFormat();
            StrFormat.LineAlignment = StringAlignment.Center;// 设置文字垂直方向居中
            StrFormat.Alignment = StringAlignment.Center;// 设置文字水平方向居中           

            // 标签背景填充颜色，也可以是图片
            SolidBrush bru22 = new SolidBrush(Color.FromArgb(22, 22, 22));
            SolidBrush bru44 = new SolidBrush(Color.FromArgb(44, 44, 44));
            SolidBrush bruFont = new SolidBrush(Color.FromArgb(222, 222, 222));// 标签字体颜色
            Font font = new System.Drawing.Font("宋体", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold))));//设置标签字体样式 
                                                                                                                          //绘制主控件的背景图片
                                                                                                                          //e.Graphics.DrawImage(backImage, 0, 0, tabControl1.Width, tabControl1.Height);
                                                                                                                          //绘制主控件的背景颜色
            e.Graphics.FillRectangle(bru22, rec);
            //首次绘制标签样式
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                //获取标签头的工作区域
                Rectangle recChild = tabControl1.GetTabRect(i);
                //绘制标签头背景颜色
                e.Graphics.FillRectangle(bru22, recChild);
                //绘制标签头的文字                        
                e.Graphics.DrawString(tabControl1.TabPages[i].Text, font, bruFont, recChild, StrFormat);
            }
            Brush bshBack;

            if (e.Index == tabControl1.SelectedIndex)    //当前Tab页的样式
            {
                bshBack = bru44;
            }
            else    //其余Tab页的样式
            {
                bshBack = bru22;
            }
            //画样式
            string tabName = tabControl1.TabPages[e.Index].Text;
            e.Graphics.FillRectangle(bshBack, e.Bounds);
            Rectangle recTab = e.Bounds;
            recTab = new Rectangle(recTab.X, recTab.Y + 4, recTab.Width, recTab.Height - 4);
            e.Graphics.DrawString(tabName, font, bruFont, recTab, StrFormat);
        }
        #endregion

        #region ToolStripDropDownButton重绘事件
        private void tlsbtn_Paint(object sender, PaintEventArgs e)//打开下拉栏时，背景会变为白色，判断控件打开时，重绘控件
        {
            ToolStripDropDownButton tlsbtn = (ToolStripDropDownButton)sender;
            if (tlsbtn.Pressed)//判断有无打开
            {
                SolidBrush bruColor = new SolidBrush(tlsbtn.BackColor);//获取背景色
                SolidBrush bruFont = new SolidBrush(tlsbtn.ForeColor);// 标签字体颜色
                StringFormat StrFormat = new StringFormat();//字体格式
                StrFormat.LineAlignment = StringAlignment.Center;// 设置文字垂直方向居中
                StrFormat.Alignment = StringAlignment.Center;// 设置文字水平方向居中 
                string buttonName = tlsbtn.Text;//获取控件文本

                e.Graphics.FillRectangle(bruColor, e.ClipRectangle);//画背景矩形
                e.Graphics.DrawString(buttonName, tlsbtn.Font, bruFont, e.ClipRectangle, StrFormat);//填写名称
            }
        }
        #endregion

        #region GroupBox重绘事件
        private void grbRobotManual_Paint(object sender, PaintEventArgs e)
        {
            GroupBox gBox = (GroupBox)sender;

            Pen pen22 = new Pen(Color.FromArgb(22, 22, 22), 4);

            e.Graphics.Clear(gBox.BackColor);
            e.Graphics.DrawString(gBox.Text, gBox.Font, Brushes.White, 10, 1);
            var vSize = e.Graphics.MeasureString(gBox.Text, gBox.Font);
            e.Graphics.DrawLine(pen22, 1, gBox.Font.Height / 2, 8, gBox.Font.Height / 2);
            e.Graphics.DrawLine(pen22, vSize.Width + 8, gBox.Font.Height / 2, gBox.Width - 2, gBox.Font.Height / 2);
            e.Graphics.DrawLine(pen22, 1, gBox.Font.Height / 2, 1, gBox.Height - 2);
            e.Graphics.DrawLine(pen22, 1, gBox.Height - 2, gBox.Width - 2, gBox.Height - 2);
            e.Graphics.DrawLine(pen22, gBox.Width - 2, gBox.Font.Height / 2, gBox.Width - 2, gBox.Height - 2);
        }
        #endregion

        #region 同步放料点dgv
        //进入视觉page时，更新视觉点dgv
        private void tbcTeachPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tbcTeachPoint.SelectedTab.Name == "tbpAVisionPoint")
            {
                if (dgvAPointData.RowCount != 0)
                {
                    for (int i = 0; i < dgvAPointData.RowCount; i++)
                    {
                        dgvAVisonPoint.Rows[i + 2].Cells[1].Value = dgvAPointData.Rows[i].Cells[1].Value;
                        dgvAVisonPoint.Rows[i + 2].Cells[8].Value = dgvAPointData.Rows[i].Cells[8].Value;
                    }
                }
            }
            else if (tbcTeachPoint.SelectedTab.Name == "tbpBVisionPoint")
            {
                if (dgvBPointData.RowCount != 0)
                {
                    for (int i = 0; i < dgvBPointData.RowCount; i++)
                    {
                        dgvBVisonPoint.Rows[i + 2].Cells[1].Value = dgvBPointData.Rows[i].Cells[1].Value;
                        dgvBVisonPoint.Rows[i + 2].Cells[8].Value = dgvBPointData.Rows[i].Cells[8].Value;
                    }
                }
            }
        }


        #endregion

        
    }
}
