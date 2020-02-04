using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MountingRobot.UI;
using MyLog;
using RCAPINet;

namespace MountingRobot.BLL
{
    class Global
    {
        #region 状态栏
        /// <summary>
        /// 是否与PLC1通讯成功
        /// </summary>
        public static bool ConnectSuccess1 { get; set; }
        /// <summary>
        /// 是否与PLC2通讯成功
        /// </summary>
        public static bool ConnectSuccess2 { get; set; }
        /// <summary>
        /// 手自动模式
        /// </summary>
        public static bool RunMode { get; set; }

        /// <summary>
        /// 设备运行状态
        /// </summary>
        public static int RunStatus { get; set; }

        /// <summary>
        /// 账号身份
        /// </summary>
        public static string UserPermission { get; set; }
        #endregion

        #region"本类日志操作"

        /// <summary>
        /// 根据内容、字符串返回修改信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="oldInt"></param>
        /// <param name="newInt"></param>
        /// <returns></returns>
        private static string operateStrToStr(string content, string oldInt, string newInt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(content + " ");
            stringBuilder.Append(oldInt + " --> ");
            stringBuilder.Append(newInt);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 根据内容、整数返回修改信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="oldInt"></param>
        /// <param name="newInt"></param>
        /// <returns></returns>
        private static string operateIntToStr(string content, int oldInt, int newInt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(content + " ");
            stringBuilder.Append(oldInt.ToString() + " --> ");
            stringBuilder.Append(newInt.ToString());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 根据内容、双精度浮点返回修改信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="oldInt"></param>
        /// <param name="newInt"></param>
        /// <returns></returns>
        private static string operateDoubleToStr(string content, double oldInt, double newInt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(content + " ");
            stringBuilder.Append(oldInt.ToString() + " --> ");
            stringBuilder.Append(newInt.ToString());
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 根据内容、高精度浮点返回修改信息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="oldInt"></param>
        /// <param name="newInt"></param>
        /// <returns></returns>
        private static string operateDecimalToStr(string content, decimal oldInt, decimal newInt)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(content + " ");
            stringBuilder.Append(oldInt.ToString() + " --> ");
            stringBuilder.Append(newInt.ToString());
            return stringBuilder.ToString();
        }
        /// <summary>
        /// 私有写入操作消息
        /// </summary>
        /// <param name="msg">写入的操作消息</param>
        private static void writeOperate(string msg)
        {
            Common.myLog.writeOperateContent(/*productName + "  " +*/ msg, UserPermission, UserPermission);
        }

        #endregion

        #region PLC参数
        private static string plc1_Ip;
        /// <summary>
        /// PLC1的IP地址
        /// </summary>
        public static string PLC1_IP
        {
            get
            {
                return plc1_Ip;
            }
            set
            {
                if (plc1_Ip != value)
                {
                    writeOperate(operateStrToStr("1号PLC地址", plc1_Ip, value));
                }
                plc1_Ip = value;
            }
        }

        private static string plc2_Ip;
        /// <summary>
        /// PLC2的IP地址
        /// </summary>
        public static string PLC2_IP
        {
            get
            {
                return plc2_Ip;
            }
            set
            {
                if (plc2_Ip != value)
                {
                    writeOperate(operateStrToStr("2号PLC地址", plc2_Ip, value));
                }
                plc2_Ip = value;
            }
        }

        private static decimal pfFullDelay;
        /// <summary>
        /// 电源座有料延时
        /// </summary>
        public static decimal PFFullDelay
        {
            get
            {
                return pfFullDelay;
            }
            set
            {
                if (pfFullDelay != value)
                {
                    writeOperate(operateDecimalToStr("电源座有料延时", pfFullDelay, value));
                }
                pfFullDelay = value;
            }
        }

        private static decimal nfFullDelay;
        /// <summary>
        /// 排针座有料延时
        /// </summary>
        public static decimal NFFullDelay
        {
            get
            {
                return nfFullDelay;
            }
            set
            {
                if (nfFullDelay != value)
                {
                    writeOperate(operateDecimalToStr("排针座有料延时由", nfFullDelay, value));

                }
                nfFullDelay = value;
            }

        }

        private static bool aByPassCk;
        /// <summary>
        /// A轨是否直通
        /// </summary>
        public static bool AByPassCk
        {
            get
            {
                return aByPassCk;
            }
            set
            {
                if (aByPassCk != value)
                {
                    Common.myLog.writeOperateContent("A轨是否直通由" + aByPassCk + "->" + value, UserPermission, UserPermission);
                }
                aByPassCk = value;
            }
        }

        private static bool bByPassCk;
        /// <summary>
        /// B轨是否直通
        /// </summary>
        public static bool BByPassCk
        {
            get
            {
                return bByPassCk;
            }
            set
            {
                if (bByPassCk != value)
                {
                    Common.myLog.writeOperateContent("B轨是否直通由" + bByPassCk + "->" + value, UserPermission, UserPermission);
                }
                bByPassCk = value;
            }
        }

        private static bool aConNext;
        /// <summary>
        /// A轨是否连接下位机
        /// </summary>
        public static bool AConNext
        {
            get
            {
                return aConNext;
            }
            set
            {
                if (aConNext != value)
                {
                    Common.myLog.writeOperateContent("A轨是否连接下位机由" + aConNext + "->" + value, UserPermission, UserPermission);
                }
                aConNext = value;
            }
        }

        private static bool bConNext;
        /// <summary>
        /// B轨是否连接下位机
        /// </summary>
        public static bool BConNext
        {
            get
            {
                return bConNext;
            }
            set
            {
                if (bConNext != value)
                {
                    Common.myLog.writeOperateContent("B轨是否连接下位机由" + bConNext + "->" + value, UserPermission, UserPermission);
                }
                bConNext = value;
            }
        }


        /// <summary>
        /// A轨入口光电延时
        /// </summary>
        private static decimal aInDelay;
        /// <summary>
        /// A轨入口光电延时
        /// </summary>
        public static decimal AInDelay
        {
            get
            {
                return aInDelay;
            }
            set
            {
                if (aInDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨入口光电延时", aInDelay, value));
                }
                aInDelay = value;
            }
        }
        /// <summary>
        /// A轨到位延时光电
        /// </summary>
        private static decimal aArriveDelay;
        /// <summary>
        /// A轨到位延时光电
        /// </summary>
        public static decimal AArriveDelay
        {
            get
            {
                return aArriveDelay;
            }
            set
            {
                if (aArriveDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨到位光电延时", aArriveDelay, value));
                }
                aArriveDelay = value;
            }
        }
        /// <summary>
        /// A轨出口光电延时
        /// </summary>
        private static decimal aOutDelay;
        /// <summary>
        /// A轨出口光电延时
        /// </summary>
        public static decimal AOutDelay
        {
            get
            {
                return aOutDelay;
            }
            set
            {
                if (aOutDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨出口光电延时", aOutDelay, value));
                }
                aOutDelay = value;
            }
        }
        /// <summary>
        /// A轨出口光电关电延时
        /// </summary>
        private static decimal aOutCloseDelay;
        /// <summary>
        /// A轨出口光电关电延时
        /// </summary>
        public static decimal AOutCloseDelay
        {
            get
            {
                return aOutCloseDelay;
            }
            set
            {
                if (aOutCloseDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨出口光电关电延时", aOutCloseDelay, value));
                }
                aOutCloseDelay = value;
            }
        }
        /// <summary>
        /// A轨进板延时
        /// </summary>
        private static decimal aEnterDelay;
        /// <summary>
        /// A轨进板延时
        /// </summary>
        public static decimal AEnterDelay
        {
            get
            {
                return aEnterDelay;
            }
            set
            {
                if (aEnterDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨进板延时", aEnterDelay, value));
                }
                aEnterDelay = value;
            }
        }
        /// <summary>
        /// A轨顶升延时
        /// </summary>
        private static decimal aRiseDelay;
        /// <summary>
        /// A轨顶升延时
        /// </summary>
        public static decimal ARiseDelay
        {
            get
            {
                return aRiseDelay;
            }
            set
            {
                if (aRiseDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨顶升延时", aRiseDelay, value));
                }
                aRiseDelay = value;
            }
        }
        /// <summary>
        /// A轨缩回延时
        /// </summary>
        private static decimal aAnastolDelay;
        /// <summary>
        /// A轨缩回延时
        /// </summary>
        public static decimal AAnastolDelay
        {
            get
            {
                return aAnastolDelay;
            }
            set
            {
                if (aAnastolDelay != value)
                {
                    writeOperate(operateDecimalToStr("A轨缩回延时", aAnastolDelay, value));
                }
                aAnastolDelay = value;
            }
        }
        /// <summary>
        /// B轨入口光电延时
        /// </summary>
        private static decimal bInDelay;
        /// <summary>
        /// B轨入口光电延时
        /// </summary>
        public static decimal BInDelay
        {
            get
            {
                return bInDelay;
            }
            set
            {
                if (bInDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨入口光电延时", bInDelay, value));
                }
                bInDelay = value;
            }
        }
        /// <summary>
        /// B轨到位延时光电
        /// </summary>
        private static decimal bArriveDelay;
        /// <summary>
        /// B轨到位延时光电
        /// </summary>
        public static decimal BArriveDelay
        {
            get
            {
                return bArriveDelay;
            }
            set
            {
                if (bArriveDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨到位光电延时", bArriveDelay, value));
                }
                bArriveDelay = value;
            }
        }
        /// <summary>
        /// B轨出口光电延时
        /// </summary>
        private static decimal bOutDelay;
        /// <summary>
        /// B轨出口光电延时
        /// </summary>
        public static decimal BOutDelay
        {
            get
            {
                return bOutDelay;
            }
            set
            {
                if (bOutDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨出口光电延时", bOutDelay, value));
                }
                bOutDelay = value;
            }
        }
        /// <summary>
        /// B轨出口光电关电延时
        /// </summary>
        private static decimal bOutCloseDelay;
        /// <summary>
        /// B轨出口光电关电延时
        /// </summary>
        public static decimal BOutCloseDelay
        {
            get
            {
                return bOutCloseDelay;
            }
            set
            {
                if (bOutCloseDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨出口光电关电延时", bOutCloseDelay, value));
                }
                bOutCloseDelay = value;
            }
        }
        /// <summary>
        /// B轨进板延时
        /// </summary>
        private static decimal bEnterDelay;
        /// <summary>
        /// B轨进板延时
        /// </summary>
        public static decimal BEnterDelay
        {
            get
            {
                return bEnterDelay;
            }
            set
            {
                if (bEnterDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨进板延时", bEnterDelay, value));
                }
                bEnterDelay = value;
            }
        }
        /// <summary>
        /// B轨顶升延时
        /// </summary>
        private static decimal bRiseDelay;
        /// <summary>
        /// B轨顶升延时
        /// </summary>
        public static decimal BRiseDelay
        {
            get
            {
                return bRiseDelay;
            }
            set
            {
                if (bRiseDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨顶升延时", bRiseDelay, value));
                }
                bRiseDelay = value;
            }
        }
        /// <summary>
        /// B轨缩回延时
        /// </summary>
        private static decimal bAnastolDelay;
        /// <summary>
        /// B轨缩回延时
        /// </summary>
        public static decimal BAnastolDelay
        {
            get
            {
                return bAnastolDelay;
            }
            set
            {
                if (bAnastolDelay != value)
                {
                    writeOperate(operateDecimalToStr("B轨缩回延时", bAnastolDelay, value));
                }
                bAnastolDelay = value;
            }
        }
        /// <summary>
        /// A轨调宽自动速度
        /// </summary>
        private static decimal aWidthAutoSpeed;
        /// <summary>
        /// A轨调宽自动速度
        /// </summary>
        public static decimal AWidthAutoSpeed
        {
            get
            {
                return aWidthAutoSpeed;
            }
            set
            {
                if (aWidthAutoSpeed != value)
                {
                    writeOperate(operateDecimalToStr("A轨调宽自动速度", aWidthAutoSpeed, value));
                }
                aWidthAutoSpeed = value;
            }
        }
        /// <summary>
        /// A轨输送位位置
        /// </summary>
        private static decimal aWidthPositionLoosen;
        /// <summary>
        /// A轨输送位位置
        /// </summary>
        public static decimal AWidthPositionLoosen
        {
            get
            {
                return aWidthPositionLoosen;
            }
            set
            {
                if (aWidthPositionLoosen != value)
                {
                    writeOperate(operateDecimalToStr("A轨输送位", aWidthPositionLoosen, value));
                }
                aWidthPositionLoosen = value;
            }
        }
        /// <summary>
        /// A轨夹紧位位置
        /// </summary>
        private static decimal aWidthPositionPinch;
        /// <summary>
        /// A轨夹紧位位置
        /// </summary>
        public static decimal AWidthPositionPinch
        {
            get
            {
                return aWidthPositionPinch;
            }
            set
            {
                if (aWidthPositionPinch != value)
                {
                    writeOperate(operateDecimalToStr("A轨夹紧位", aWidthPositionPinch, value));
                }
                aWidthPositionPinch = value;
            }
        }
        /// <summary>
        /// B轨调宽自动速度
        /// </summary>
        private static decimal bWidthAutoSpeed;
        /// <summary>
        /// B轨调宽自动速度
        /// </summary>
        public static decimal BWidthAutoSpeed
        {
            get
            {
                return bWidthAutoSpeed;
            }
            set
            {
                if (bWidthAutoSpeed != value)
                {
                    writeOperate(operateDecimalToStr("BA轨调宽自动速度", bWidthAutoSpeed, value));
                }
                bWidthAutoSpeed = value;
            }
        }
        /// <summary>
        /// B轨输送位位置
        /// </summary>
        private static decimal bWidthPositionLoosen;
        /// <summary>
        /// B轨输送位位置
        /// </summary>
        public static decimal BWidthPositionLoosen
        {
            get
            {
                return bWidthPositionLoosen;
            }
            set
            {
                if (bWidthPositionLoosen != value)
                {
                    writeOperate(operateDecimalToStr("B轨输送位", bWidthPositionLoosen, value));
                }
                bWidthPositionLoosen = value;
            }
        }
        /// <summary>
        /// B轨夹紧位位置
        /// </summary>
        private static decimal bWidthPositionPinch;
        /// <summary>
        /// B轨夹紧位位置
        /// </summary>
        public static decimal BWidthPositionPinch
        {
            get
            {
                return bWidthPositionPinch;
            }
            set
            {
                if (bWidthPositionPinch != value)
                {
                    writeOperate(operateDecimalToStr("B轨夹紧位", bWidthPositionPinch, value));
                }
                bWidthPositionPinch = value;
            }
        }
        #endregion

        #region 产品信息
        /// <summary>
        /// A轨产品名称
        /// </summary>
        private static string aProductName;
        /// <summary>
        /// A轨产品名称
        /// </summary>
        public static string AProductName
        {
            get
            {
                return aProductName;
            }
            set
            {
                if (aProductName != value && value != "")
                {
                    writeOperate(operateStrToStr("A轨生产产品", aProductName, value));
                }
                aProductName = value;
                SystemINI.IniWriteValue("其它参数", "A轨上次产品名称", aProductName);//记录当前产品名称至INI文件，下次运行时读取
                AProductINIPath = Application.StartupPath + @"\Config\A\" + aProductName + ".ini";//更新产品参数ini路径
                AProductINI = new WriteReadINI(AProductINIPath);
            }
        }

        /// <summary>
        /// B轨产品名称
        /// </summary>
        private static string bProductName;
        /// <summary>
        /// B轨产品名称
        /// </summary>
        public static string BProductName
        {
            get
            {
                return bProductName;
            }
            set
            {
                if (bProductName != value && value != "")
                {
                    writeOperate(operateStrToStr("B轨生产产品", bProductName, value));
                }
                bProductName = value;
                SystemINI.IniWriteValue("其它参数", "B轨上次产品名称", bProductName);//记录当前产品名称至INI文件，下次运行时读取
                BProductINIPath = Application.StartupPath + @"\Config\B\" + bProductName + ".ini";//更新产品参数ini路径
                BProductINI = new WriteReadINI(BProductINIPath);
            }
        }
        /// <summary>
        /// A轨产品开始时间
        /// </summary>
        private static string aProductStartTime;
        /// <summary>
        /// A轨产品开始时间
        /// </summary>
        public static string AProductStartTime
        {
            get
            {
                return aProductStartTime;
            }
            set
            {
                aProductStartTime = value;
            }
        }
        /// <summary>
        /// A轨产品生产数量
        /// </summary>
        private static ulong aProductNums;
        /// <summary>
        /// A轨产品生产数量
        /// </summary>
        public static ulong AProductNums
        {
            get
            {
                return aProductNums;
            }
            set
            {
                aProductNums = value;
            }
        }
        /// <summary>
        /// A轨产品生产节拍
        /// </summary>
        public static string AProductBeat { get; set; }
        /// <summary>
        /// B轨产品开始时间
        /// </summary>
        private static string bProductStartTime;
        /// <summary>
        /// B轨产品开始时间
        /// </summary>
        public static string BProductStartTime
        {
            get
            {
                return bProductStartTime;
            }
            set
            {
                bProductStartTime = value;
            }
        }
        /// <summary>
        /// B轨产品生产数量
        /// </summary>
        private static ulong bProductNums;
        /// <summary>
        /// B轨产品生产数量
        /// </summary>
        public static ulong BProductNums
        {
            get
            {
                return bProductNums;
            }
            set
            {
                bProductNums = value;
            }
        }
        /// <summary>
        /// B轨产品生产节拍
        /// </summary>
        public static string BProductBeat { get; set; }
        #endregion

        #region 其它参数
        /// <summary>
        /// A轨产品参数INI路径
        /// </summary>
        public static string AProductINIPath;
        /// <summary>
        /// B轨产品参数INI路径
        /// </summary>
        public static string BProductINIPath;
        /// <summary>
        /// 设备编号
        /// </summary>
        private static string machineNumber;
        /// <summary>
        /// 设备编号
        /// </summary>
        public static string MachineNumber
        {
            get
            {
                return machineNumber;
            }
            set
            {
                if (machineNumber != value)
                {
                    writeOperate(operateStrToStr("设备编号", machineNumber, value));
                }
                machineNumber = value;
            }
        }
        /// <summary>
        /// 选择单双轨数
        /// </summary>
        private static string orbitalCount;
        /// <summary>
        /// 选择单双轨数
        /// </summary>
        public static string OrbitalCount
        {
            get
            {
                return orbitalCount;
            }
            set
            {
                if (orbitalCount != value)
                {
                    Common.myLog.writeOperateContent("选择单双轨数由" + orbitalCount + "->" + value, UserPermission, UserPermission);
                }
                orbitalCount = value;
            }
        }
        #endregion

        #region 机器人参数
        /// <summary>
        /// 视觉调用子程序前缀
        /// </summary>
        private static string camProductSelect;
        /// <summary>
        /// 视觉调用子程序前缀
        /// </summary>
        public static string CamProductSelect
        {
            get
            {
                return camProductSelect;
            }
            set
            {
                if (camProductSelect != value)
                {
                    writeOperate(operateStrToStr("视觉调用子程序前缀", camProductSelect, value));
                }
                camProductSelect = value;
            }
        }
        /// <summary>
        /// 图像保存路径
        /// </summary>
        private static string photoSavePath;
        /// <summary>
        /// 图像保存路径
        /// </summary>
        public static string PhotoSavePath
        {
            get
            {
                return photoSavePath;
            }
            set
            {
                if (photoSavePath != value)
                {
                    writeOperate(operateStrToStr("图像保存路径", photoSavePath, value));
                }
                photoSavePath = value;
            }
        }
        /// <summary>
        /// 机器人项目路径
        /// </summary>
        private static string robotProPath;
        /// <summary>
        /// 机器人项目路径
        /// </summary>
        public static string RobotProPath
        {
            get
            {
                return robotProPath;
            }
            set
            {
                if (robotProPath != value)
                {
                    writeOperate(operateStrToStr("机器人项目路径", robotProPath, value));
                }
                robotProPath = value;
            }
        }
        /// <summary>
        /// 机器人夹爪延时
        /// </summary>
        private static decimal robotGripDelay;
        /// <summary>
        /// 机器人夹爪延时
        /// </summary>
        public static decimal RobotGripDelay
        {
            get
            {
                return robotGripDelay;
            }
            set
            {
                if (robotGripDelay != value)
                {
                    writeOperate(operateDecimalToStr("机器人夹爪延时", robotGripDelay, value));
                }
                robotGripDelay = value;
            }
        }
        /// <summary>
        /// 拍照延时
        /// </summary>
        private static decimal camDelay;
        /// <summary>
        /// 拍照延时
        /// </summary>
        public static decimal CamDelay
        {
            get
            {
                return camDelay;
            }
            set
            {
                if (camDelay != value)
                {
                    writeOperate(operateDecimalToStr("拍照延时", camDelay, value));
                }
                camDelay = value;
            }
        }
        /// <summary>
        /// 机器人自动速度
        /// </summary>
        private static int robotAutoSpeed;
        /// <summary>
        /// 机器人自动速度
        /// </summary>
        public static int RobotAutoSpeed
        {
            get
            {
                return robotAutoSpeed;
            }
            set
            {
                if (robotAutoSpeed != value)
                {
                    writeOperate(operateIntToStr("机器人自动速度", robotAutoSpeed, value));
                }
                robotAutoSpeed = value;
            }
        }
        /// <summary>
        /// 机器人手动速度
        /// </summary>
        private static int robotManualSpeed;
        /// <summary>
        /// 机器人手动速度
        /// </summary>
        public static int RobotManualSpeed
        {
            get
            {
                return robotManualSpeed;
            }
            set
            {
                if (robotManualSpeed != value)
                {
                    writeOperate(operateIntToStr("机器人手动速度", robotManualSpeed, value));
                }
                robotManualSpeed = value;
            }
        }
        /// <summary>
        /// 机器人插补速度
        /// </summary>
        private static int robotMoveSpeed;
        /// <summary>
        /// 机器人插补速度
        /// </summary>
        public static int RobotMoveSpeed
        {
            get
            {
                return robotMoveSpeed;
            }
            set
            {
                if (robotMoveSpeed != value)
                {
                    writeOperate(operateIntToStr("机器人插补速度", robotMoveSpeed, value));
                }
                robotMoveSpeed = value;
            }
        }
        /// <summary>
        /// 机器人安全高度
        /// </summary>
        private static decimal robotSafeHeight;
        /// <summary>
        /// 机器人安全高度
        /// </summary>
        public static decimal RobotSafeHeight
        {
            get
            {
                return robotSafeHeight;
            }
            set
            {
                if (robotSafeHeight != value)
                {
                    writeOperate(operateDecimalToStr("机器人安全高度", robotSafeHeight, value));
                }
                robotSafeHeight = value;
            }
        }
        /// <summary>
        /// 视觉允许连续NG次数
        /// </summary>
        private static int camNGLimit;
        /// <summary>
        /// 视觉允许连续NG次数
        /// </summary>
        public static int CamNGlimit
        {
            get
            {
                return camNGLimit;
            }
            set
            {
                if (camNGLimit != value)
                {
                    writeOperate(operateIntToStr("视觉允许连续NG次数", camNGLimit, value));
                }
                camNGLimit = value;
            }
        }
        /// <summary>
        /// 向下相机超限报警功能
        /// </summary>
        private static bool camDownExceedEnable;
        /// <summary>
        /// 向下相机超限报警功能
        /// </summary>
        public static bool CamDownExceedEnable
        {
            get
            {
                return camDownExceedEnable;
            }
            set
            {
                if (camDownExceedEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用向下相机超限报警功能");
                    }
                    else
                    {
                        writeOperate("禁用向下相机超限报警功能");
                    }
                }
                camDownExceedEnable = value;
            }
        }
        /// <summary>
        /// A轨向下相机启用
        /// </summary>
        private static bool camADownEnable;
        /// <summary>
        /// A轨向下相机启用
        /// </summary>
        public static bool CamADownEnable
        {
            get
            {
                return camADownEnable;
            }
            set
            {
                if (camADownEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用A轨向下相机");
                    }
                    else
                    {
                        writeOperate("禁用A轨向下相机");
                    }
                    camADownEnable = value;
                }
            }
        }
        /// <summary>
        /// A轨向上相机启用
        /// </summary>
        private static bool camAUpEnable;
        /// <summary>
        /// A轨向上相机启用
        /// </summary>
        public static bool CamAUpEnable
        {
            get
            {
                return camAUpEnable;
            }
            set
            {
                if (camAUpEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用A轨向上相机");
                    }
                    else
                    {
                        writeOperate("禁用A轨向上相机");
                    }
                    camAUpEnable = value;
                }
            }
        }
        /// <summary>
        /// B轨向下相机启用
        /// </summary>
        private static bool camBDownEnable;
        /// <summary>
        /// B轨向下相机启用
        /// </summary>
        public static bool CamBDownEnable
        {
            get
            {
                return camBDownEnable;
            }
            set
            {
                if (camBDownEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用B轨向下相机");
                    }
                    else
                    {
                        writeOperate("禁用B轨向下相机");
                    }
                    camBDownEnable = value;
                }
            }
        }
        /// <summary>
        /// B轨向上相机启用
        /// </summary>
        private static bool camBUpEnable;
        /// <summary>
        /// B轨向上相机启用
        /// </summary>
        public static bool CamBUpEnable
        {
            get
            {
                return camBUpEnable;
            }
            set
            {
                if (camBUpEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用B轨向上相机");
                    }
                    else
                    {
                        writeOperate("禁用B轨向上相机");
                    }
                    camBUpEnable = value;
                }
            }
        }
        /// <summary>
        /// 视觉正常图像保存功能
        /// </summary>
        private static bool photoPassSaveEnable;
        /// <summary>
        /// 视觉正常图像保存功能
        /// </summary>
        public static bool PhotoPassSaveEnable
        {
            get
            {
                return photoPassSaveEnable;
            }
            set
            {
                if (photoPassSaveEnable != value)
                {
                    if (value == true)
                    {
                        writeOperate("启用视觉保存正常图像功能");
                    }
                    else
                    {
                        writeOperate("禁用视觉保存正常图像功能");
                    }
                    photoPassSaveEnable = value;
                }
            }
        }
        /// <summary>
        /// 视觉保存NG图像功能
        /// </summary>
        private static bool photoNGSaveEnable;
        /// <summary>
        /// 视觉保存NG图像功能
        /// </summary>
        public static bool PhotoNGSaveEnable
        {
            get
            {
                return photoNGSaveEnable;
            }
            set
            {
                if (value == true)
                {
                    writeOperate("启用视觉保存NG图像功能");
                }
                else
                {
                    writeOperate("禁用视觉保存NG图像功能");
                }
                photoNGSaveEnable = value;
            }
        }

        #endregion

        #region 读取INI参数配置文件
        public static WriteReadINI SystemINI = new WriteReadINI(Application.StartupPath + @"\SysConfig.INI");//实例读取设备参数配置文件全局对象
        public static WriteReadINI AProductINI; /*= new WriteReadINI(Application.StartupPath + @"\SysParame\SysConfig.INI");  //实例参数配置文件全局对象*/
        public static WriteReadINI BProductINI;

        /// <summary>
        /// 读取系统参数
        /// </summary>
        public static void ReadSystemIni()
        {
            if (!Directory.Exists(Application.StartupPath + @"\SysParame"))  //判断是否存在SysParame文件夹，不存在则自行创建
            {
                Directory.CreateDirectory(Application.StartupPath + @"\SysParame");
            }
            //PLC参数
            plc1_Ip = SystemINI.IniReadValue("PLC参数", "192.168.2.1", "PLC1_IP地址");
            plc2_Ip = SystemINI.IniReadValue("PLC参数", "192.168.2.2", "PLC2_IP地址");
            pfFullDelay = Convert.ToDecimal(SystemINI.IniReadValue("PLC参数", "200", "电源座有料延时"));
            nfFullDelay = Convert.ToDecimal(SystemINI.IniReadValue("PLC参数", "200", "排针座有料延时"));            

            orbitalCount = SystemINI.IniReadValue("其它参数", "双轨", "轨道数");
            machineNumber = SystemINI.IniReadValue("其它参数", "", "设备编号");
            AProductName = SystemINI.IniReadValue("其它参数", "", "A轨上次产品名称");//读取A轨上一次产品名称
            BProductName = SystemINI.IniReadValue("其它参数", "", "B轨上次产品名称");//读取B轨上一次产品名称
            alarmFlashEnable = Convert.ToBoolean(SystemINI.IniReadValue("其它参数", "false", "报警动画功能"));

            //机器人
            robotProPath = SystemINI.IniReadValue("机器人参数", "", "机器人项目文件路径");
            robotGripDelay = Convert.ToDecimal(SystemINI.IniReadValue("机器人参数", "200", "机器人夹爪延时"));
            camDelay = Convert.ToDecimal(SystemINI.IniReadValue("机器人参数", "200", "拍照延时"));
            robotAutoSpeed = Convert.ToInt32(SystemINI.IniReadValue("机器人参数", "30", "机器人自动速度"));
            robotManualSpeed = Convert.ToInt32(SystemINI.IniReadValue("机器人参数", "10", "机器人手动速度"));
            robotMoveSpeed = Convert.ToInt32(SystemINI.IniReadValue("机器人参数", "500", "机器人插补速度"));
            robotSafeHeight = Convert.ToDecimal(SystemINI.IniReadValue("机器人参数", "0", "机器人安全高度"));
            camNGLimit = Convert.ToInt32(SystemINI.IniReadValue("机器人参数", "0", "视觉允许连续NG次数"));
            camADownEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "A轨向下相机启用"));
            camAUpEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "A轨向上相机启用"));
            camBDownEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "B轨向下相机启用"));
            camBUpEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "B轨向上相机启用"));
            photoNGSaveEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "视觉保存NG图像功能"));
            photoPassSaveEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "视觉保存正常图像功能"));
            camDownExceedEnable = Convert.ToBoolean(SystemINI.IniReadValue("机器人参数", "false", "下相机超限报警功能"));
            camProductSelect = SystemINI.IniReadValue("机器人参数", "", "视觉调用子程序前缀");
            photoSavePath = SystemINI.IniReadValue("机器人参数", "", "图像保存路径");

            //机器人点位路径
            VisionFileSavePath = SystemINI.IniReadValue("视觉点位文件路径", "", "path");
            TakeFileSavePath = SystemINI.IniReadValue("取料点位文件路径", "", "path");
            ThrowFileSavePath = SystemINI.IniReadValue("抛料点位文件路径", "", "path");
        }
        /// <summary>
        /// 读取产品参数
        /// </summary>
        public static void ReadProductINI()
        {
            try
            {
                aInDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨入口光电延时"));
                aArriveDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨到位光电延时"));
                aOutDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨出口光电延时"));
                aOutCloseDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨出口光电关电延时"));
                aEnterDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨进板延时"));
                aRiseDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨顶升延时"));
                aAnastolDelay = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨缩回延时"));

                aWidthAutoSpeed = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨调宽自动速度"));
                aWidthPositionLoosen = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨调宽输送位"));
                aWidthPositionPinch = Convert.ToDecimal(AProductINI.IniReadValue("PLC参数", "200", "A轨调宽夹紧位"));

                bInDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨入口光电延时"));
                bArriveDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨到位光电延时"));
                bOutDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨出口光电延时"));
                bOutCloseDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨出口光电关电延时"));
                bEnterDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨进板延时"));
                bRiseDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨顶升延时"));
                bAnastolDelay = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨缩回延时"));

                bWidthAutoSpeed = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨调宽自动速度"));
                bWidthPositionLoosen = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨调宽输送位"));
                bWidthPositionPinch = Convert.ToDecimal(BProductINI.IniReadValue("PLC参数", "200", "B轨调宽夹紧位"));                

                //系统设置
                AFileSavePath = AProductINI.IniReadValue("A轨点位文件路径", "", "path");
                BFileSavePath = BProductINI.IniReadValue("B轨点位文件路径", "", "path");

                //统计信息
                aProductStartTime = AProductINI.IniReadValue("统计信息", "0", "开始时间");
                aProductNums= Convert.ToUInt64(AProductINI.IniReadValue("统计信息", "0", "生产数量"));
                AProductBeat = "0";

                bProductStartTime = BProductINI.IniReadValue("统计信息", "0", "开始时间");
                bProductNums = Convert.ToUInt64(BProductINI.IniReadValue("统计信息", "0", "生产数量"));
                BProductBeat = "0";

                MyLog.Common.myLog.writeRunContent("参数配置文件加载完成", "系统", "系统");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        #endregion

        #region 读取下载机器人点位文件与参数
        /// <summary>
        /// 下载机器人配置参数
        /// </summary>
        /// <returns></returns>
        public static bool DownloadRobotParameter()
        {
            try
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
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }



        /// <summary>
        /// 放料视觉点位文件后缀（创建放料位置点位时，同时创建工具坐标系下点位文件，名称为默认坐标系点位文件名称加本后缀）
        /// </summary>
        public static string VisionPostfix = "-Vision";

        //实例机器人点位数据DataTable
        public static DataTable RobotAPointDataTable = new DataTable();
        public static DataTable RobotAVisionDataTable = new DataTable();
        public static DataTable RobotBPointDataTable = new DataTable();
        public static DataTable RobotBVisionDataTable = new DataTable();
        public static DataTable RobotTakeDataTable = new DataTable();
        public static DataTable RobotThrowDataTable = new DataTable();
        public static DataTable RobotVisionDataTable = new DataTable();
        /// <summary>
        /// 读取机器人点位数据
        /// </summary>
        public static bool ReadRobotPoint()
        {
            try
            {
                getRobotPoint(RobotAPointDataTable, "A", AFileSavePath);
                getRobotPoint(RobotAVisionDataTable, "AV", AFileSavePath.Replace(".ini", Global.VisionPostfix + ".ini"));
                getRobotPoint(RobotBPointDataTable, "B", BFileSavePath);
                getRobotPoint(RobotBVisionDataTable, "BV", BFileSavePath.Replace(".ini", Global.VisionPostfix + ".ini"));
                getRobotPoint(RobotVisionDataTable, "Vision", VisionFileSavePath);
                getRobotPoint(RobotTakeDataTable, "Take", TakeFileSavePath);
                getRobotPoint(RobotThrowDataTable, "Throw", ThrowFileSavePath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }
        /// <summary>
        /// 下载机器人点位数据
        /// </summary>
        public static bool DownloadRobotPoint()
        {
            try
            {
                DownloadRobotPutPoint(RobotAPointDataTable, 200, 249);
                DownloadRobotPutVisionPoint(RobotAVisionDataTable, 300, 349, 10);
                DownloadRobotPutPoint(RobotBPointDataTable, 250, 299);
                DownloadRobotPutVisionPoint(RobotBVisionDataTable, 350, 399, 16);
                DownloadRobotTakePoint(RobotTakeDataTable);
                DownloadRobotVisionPoint(RobotVisionDataTable);
                DownloadRobotThrowPoint(RobotThrowDataTable);
                FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }

        }
        /// <summary>
        /// 读取机器点位数据函数
        /// </summary>
        /// <param name="dataTable">所需编辑表格</param>
        /// <param name="str">INI节点名称</param>
        /// <param name="path">INI路径</param>
        private static void getRobotPoint(DataTable dataTable, string str, string path)
        {
            int count;

            dataTable.Clear();//清除表格数据
            dataTable.Columns.Clear();//清除表格列结构            

            WriteReadINI loadPointIni = new WriteReadINI(path);//关联数据点位INI路径
            DataRow row;//创建DataTable行数据

            string loadSection1 = string.Empty;
            string loadSection2 = string.Empty;
            string tempStr = string.Empty;
            string tempColumns = string.Empty;

            dataTable.Columns.Add("PointDes", typeof(string));//添加描述或取料位列
            dataTable.Columns.Add("X", typeof(string));//添加X列
            dataTable.Columns.Add("Y", typeof(string));//添加Y列
            dataTable.Columns.Add("Z", typeof(string));//添加Z列
            dataTable.Columns.Add("U", typeof(string));//添加U列
            dataTable.Columns.Add("Local", typeof(string));//添加本地坐标列
            dataTable.Columns.Add("Hand", typeof(string));//添加手系统列
            dataTable.Columns.Add("Grip", typeof(string));//添加夹爪列

            switch (str)//选择读取节点名称
            {
                case "A":
                    loadSection1 = "A贴装点";
                    break;
                case "B":
                    loadSection1 = "B贴装点";
                    break;
                case "Vision":
                    loadSection1 = "视觉点";
                    break;
                case "Take":
                    loadSection1 = "取料点";
                    break;
                case "Throw":
                    loadSection1 = "抛料点";
                    break;
                case "AV":
                    loadSection1 = "A贴装点";
                    loadSection2 = "A本地坐标系点";
                    break;
                case "BV":
                    loadSection1 = "B贴装点";
                    loadSection2 = "B本地坐标系点";
                    break;
                default:
                    break;
            }

            if (str == "AV" || str == "BV")//视觉放料点会多出两个本地坐标系点
            {
                int temp = Convert.ToInt32(loadPointIni.IniReadValue(loadSection1 + "数量", "0", "点数"));//读取点数
                if (temp != 0)
                {
                    count = temp + 2;
                }
                else
                {
                    count = 0;
                }
            }
            else
            {
                count = Convert.ToInt32(loadPointIni.IniReadValue(loadSection1 + "数量", "0", "点数"));//读取点数
            }

            for (int i = 0; i < count; i++)  //遍历获取详细点坐标信息
            {
                row = dataTable.NewRow();//创建一个与DataTable相同架构的行

                for (int j = 0; j < 8; j++)
                {
                    switch (j)//选择读取键名称
                    {
                        case 0:
                            if (str == "AV" || str == "BV" || str == "A" || str == "B")
                            {
                                tempStr = "取料位";
                                tempColumns = "PointDes";
                            }
                            else
                            {
                                tempStr = "点位描述";
                                tempColumns = "PointDes";
                            }
                            break;
                        case 1:
                            tempStr = "X";
                            tempColumns = "X";
                            break;
                        case 2:
                            tempStr = "Y";
                            tempColumns = "Y";
                            break;
                        case 3:
                            tempStr = "Z";
                            tempColumns = "Z";
                            break;
                        case 4:
                            tempStr = "U";
                            tempColumns = "U";

                            break;
                        case 5:
                            tempStr = "Local";
                            tempColumns = "Local";
                            break;
                        case 6:
                            tempStr = "手系统";
                            tempColumns = "Hand";
                            break;
                        case 7:
                            tempStr = "夹爪";
                            tempColumns = "Grip";
                            break;
                        default:
                            break;
                    }
                    if (str == "AV" || str == "BV")//视觉放料点要多加载两个本地坐标系点位
                    {
                        if (i < 2)
                        {
                            row[tempColumns] = loadPointIni.IniReadValue(loadSection2 + (i + 1).ToString(), "", tempStr);//在新添行中添加列与具体数据
                        }
                        else
                        {
                            row[tempColumns] = loadPointIni.IniReadValue(loadSection1 + (i - 1).ToString(), "", tempStr);//在新添行中添加列与具体数据
                        }
                    }
                    else
                    {
                        row[tempColumns] = loadPointIni.IniReadValue(loadSection1 + (i + 1).ToString(), "", tempStr);//在新添行中添加列与具体数据
                    }
                }
                dataTable.Rows.Add(row);//将处理好的行添加至DataTable中
            }
        }

        /// <summary>
        /// 下载放料点位数据
        /// </summary>
        /// <param name="dataTable">下载表格</param>
        /// <param name="starPoint">机器人放料点开始点编号</param>
        /// <param name="endPoint">机器人放料点结束点编号</param>
        private static void DownloadRobotPutPoint(DataTable dataTable, int starPoint, int endPoint)
        {
            SpelPoint p = new SpelPoint();

            FrmIniRobot.RobotSpel.PDel(starPoint, endPoint);//下载前删除目标点位区域
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                p.X = float.Parse(dataTable.Rows[i][1].ToString());
                p.Y = float.Parse(dataTable.Rows[i][2].ToString());
                p.Z = float.Parse(dataTable.Rows[i][3].ToString());
                p.U = float.Parse(dataTable.Rows[i][4].ToString());
                p.Local = Convert.ToInt32(dataTable.Rows[i][5].ToString());
                if (dataTable.Rows[i][6].ToString() == "左手姿态")
                {
                    p.Hand = SpelHand.Lefty;
                }
                else
                {
                    p.Hand = SpelHand.Righty;
                }
                FrmIniRobot.RobotSpel.SetPoint(starPoint + i, p);//下载单个点位
            }
            //FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
        }
        /// <summary>
        /// 下载机器人视觉放料点位数据
        /// </summary>
        /// <param name="dataTable">下载表格</param>
        /// <param name="starPoint">机器人放料点开始编号</param>
        /// <param name="endPoint">机器人放料点结束编号</param>
        /// <param name="localStarPoint">本地坐标系开始编号</param>
        private static void DownloadRobotPutVisionPoint(DataTable dataTable, int starPoint, int endPoint, int localStarPoint)
        {
            SpelPoint p = new SpelPoint();

            FrmIniRobot.RobotSpel.PDel(starPoint, endPoint);//下载前删除目标点位区域
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {

                p.X = float.Parse(dataTable.Rows[i][1].ToString());
                p.Y = float.Parse(dataTable.Rows[i][2].ToString());
                p.Z = float.Parse(dataTable.Rows[i][3].ToString());
                p.U = float.Parse(dataTable.Rows[i][4].ToString());
                p.Local = Convert.ToInt32(dataTable.Rows[i][5].ToString());
                if (dataTable.Rows[i][6].ToString() == "左手姿态")
                {
                    p.Hand = SpelHand.Lefty;
                }
                else
                {
                    p.Hand = SpelHand.Righty;
                }
                if (i < 2)//区分本地坐标系点和视觉放料点
                {
                    FrmIniRobot.RobotSpel.SetPoint(localStarPoint + i, p);//下载单个点位
                }
                else
                {
                    FrmIniRobot.RobotSpel.SetPoint(starPoint + i - 2, p);//下载单个点位
                }
            }
            //FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
        }
        /// <summary>
        /// 下载机器人取料点位数据
        /// </summary>
        /// <param name="dataTable"></param>
        private static void DownloadRobotTakePoint(DataTable dataTable)
        {
            SpelPoint p = new SpelPoint();
            int gripA = 0, gripB = 0, gripC = 0, gripD = 0;

            FrmIniRobot.RobotSpel.PDel(100, 119);//下载前删除目标点位区域
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                p.X = float.Parse(dataTable.Rows[i][1].ToString());
                p.Y = float.Parse(dataTable.Rows[i][2].ToString());
                p.Z = float.Parse(dataTable.Rows[i][3].ToString());
                p.U = float.Parse(dataTable.Rows[i][4].ToString());
                p.Local = Convert.ToInt32(dataTable.Rows[i][5].ToString());
                if (dataTable.Rows[i][6].ToString() == "左手姿态")
                {
                    p.Hand = SpelHand.Lefty;
                }
                else
                {
                    p.Hand = SpelHand.Righty;
                }
                switch (dataTable.Rows[i][7].ToString())//判断夹爪类型，区分下载
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
            }
            //FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
        }
        /// <summary>
        /// 下载机器人视觉点位数据
        /// </summary>
        /// <param name="dataTable"></param>
        private static void DownloadRobotVisionPoint(DataTable dataTable)
        {
            SpelPoint p = new SpelPoint();

            FrmIniRobot.RobotSpel.PDel(120, 139);//下载前删除目标点位区域
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                p.X = float.Parse(dataTable.Rows[i][1].ToString());
                p.Y = float.Parse(dataTable.Rows[i][2].ToString());
                p.Z = float.Parse(dataTable.Rows[i][3].ToString());
                p.U = float.Parse(dataTable.Rows[i][4].ToString());
                p.Local = Convert.ToInt32(dataTable.Rows[i][5].ToString());
                if (dataTable.Rows[i][6].ToString() == "左手姿态")
                {
                    p.Hand = SpelHand.Lefty;
                }
                else
                {
                    p.Hand = SpelHand.Righty;
                }
                switch (dataTable.Rows[i][7].ToString())//判断夹爪类型，区分下载
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
            }
            //FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
        }
        /// <summary>
        /// 下载机器人抛料点位数据
        /// </summary>
        /// <param name="dataTable"></param>
        private static void DownloadRobotThrowPoint(DataTable dataTable)
        {
            SpelPoint p = new SpelPoint();

            FrmIniRobot.RobotSpel.PDel(140, 149);//下载前删除目标点位区域
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                p.X = float.Parse(dataTable.Rows[i][1].ToString());
                p.Y = float.Parse(dataTable.Rows[i][2].ToString());
                p.Z = float.Parse(dataTable.Rows[i][3].ToString());
                p.U = float.Parse(dataTable.Rows[i][4].ToString());
                p.Local = Convert.ToInt32(dataTable.Rows[i][5].ToString());
                if (dataTable.Rows[i][6].ToString() == "左手姿态")
                {
                    p.Hand = SpelHand.Lefty;
                }
                else
                {
                    p.Hand = SpelHand.Righty;
                }
                switch (dataTable.Rows[i][7].ToString())//判断夹爪类型，区分下载
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
            }
            //FrmIniRobot.RobotSpel.SavePoints("robot1.pts");//将机器人存储点位保存至点位文件中
        }

        #endregion

        #region 点位示教
        /// <summary>
        /// 机器人SDK加载完毕
        /// </summary>
        public static bool RobotReady { get; set; }

        /// <summary>
        /// A轨点位保存路径
        /// </summary>
        public static string AFileSavePath { get; set; }

        /// <summary>
        /// A轨点位保存程序名称
        /// </summary>
        public static string AFileNameExt { get; set; }

        /// <summary>
        /// A轨视觉点位保存路径
        /// </summary>
        public static string AVisionFileSavePath { get; set; }

        /// <summary>
        /// A轨视觉点位保存程序名称
        /// </summary>
        public static string AVisionFileNameExt { get; set; }

        /// <summary>
        /// B轨点位保存路径
        /// </summary>
        public static string BFileSavePath { get; set; }

        /// <summary>
        /// B轨点位保存程序名称
        /// </summary>
        public static string BFileNameExt = string.Empty;

        /// <summary>
        /// B轨视觉点位保存路径
        /// </summary>
        public static string BVisionFileSavePath { get; set; }

        /// <summary>
        /// B轨视觉点位保存程序名称
        /// </summary>
        public static string BVisionFileNameExt { get; set; }

        /// <summary>
        /// 视觉点位保存路径
        /// </summary>
        public static string VisionFileSavePath { get; set; }

        /// <summary>
        /// 视觉点位保存程序名称
        /// </summary>
        public static string VisionFileNameExt { get; set; }

        /// <summary>
        /// 取料点位保存路径
        /// </summary>
        public static string TakeFileSavePath { get; set; }

        /// <summary>
        /// 取料点位保存程序名称
        /// </summary>
        public static string TakeFileNameExt { get; set; }

        /// <summary>
        /// 抛料点位保存路径
        /// </summary>
        public static string ThrowFileSavePath { get; set; }

        /// <summary>
        /// 抛料点位保存程序名称
        /// </summary>
        public static string ThrowFileNameExt { get; set; }
        #endregion

        #region PLC点位地址指针
        #region M区指针
        /// <summary>
        /// A轨启动按钮灯按键
        /// </summary>
        public static string M_AStartBtnLamp
        {
            get { return "M1.0"; }
        }
        /// <summary>
        /// B轨启动按钮灯按键
        /// </summary>
        public static string M_BStartBtnLamp
        {
            get { return "M1.1"; }
        }
        /// <summary>
        /// A轨复位按钮灯按键
        /// </summary>
        public static string M_AResetBtnLamp
        {
            get { return "M1.2"; }
        }
        /// <summary>
        /// B轨复位按钮灯按键
        /// </summary>
        public static string M_BResetBtnLamp
        {
            get { return "M1.3"; }
        }
        /// <summary>
        /// 振动盘启动按键
        /// </summary>
        public static string M_VibrationPower
        {
            get { return "M1.4"; }
        }
        /// <summary>
        /// 三色灯红灯按键
        /// </summary>
        public static string M_RedLamp
        {
            get { return "M1.5"; }
        }
        /// <summary>
        /// 三色灯黄灯按键
        /// </summary>
        public static string M_YellowLamp
        {
            get { return "M1.6"; }
        }
        /// <summary>
        /// 三色灯绿灯按键
        /// </summary>
        public static string M_GreenLamp
        {
            get { return "M1.7"; }
        }
        /// <summary>
        /// 蜂鸣器按键
        /// </summary>
        public static string M_Buzzer
        {
            get { return "M2.0"; }
        }
        /// <summary>
        /// A轨顶升气缸按键
        /// </summary>
        public static string M_ARiseCylinder
        {
            get { return "M2.1"; }
        }
        /// <summary>
        /// A轨阻挡气缸按键
        /// </summary>
        public static string M_AStopCylinder
        {
            get { return "M2.2"; }
        }
        /// <summary>
        /// A轨向上位机要板信号按键
        /// </summary>
        public static string M_AUpperComputerRequest
        {
            get { return "M2.3"; }
        }
        /// <summary>
        /// A轨给下位机有板信号按键
        /// </summary>
        public static string M_ALowerComputerHave
        {
            get { return "M2.4"; }
        }
        /// <summary>
        /// B轨顶升气缸按键
        /// </summary>
        public static string M_BRiseCylinder
        {
            get { return "M2.5"; }
        }
        /// <summary>
        /// B轨阻挡气缸按键
        /// </summary>
        public static string M_BStopCylinder
        {
            get { return "M2.6"; }
        }
        /// <summary>
        /// B轨向上位机要板信号按键
        /// </summary>
        public static string M_BUpperComputerRequest
        {
            get { return "M2.7"; }
        }
        /// <summary>
        /// B轨给下位机有板信号按键
        /// </summary>
        public static string M_BLowerComputerHave
        {
            get { return "M3.0"; }
        }
        /// <summary>
        /// 自动按键
        /// </summary>
        public static string M_AutoMode
        {
            get { return "M3.1"; }
        }
        /// <summary>
        /// 手动按键
        /// </summary>
        public static string M_ManualMode
        {
            get { return "M3.3"; }
        }
        /// <summary>
        /// A轨直通
        /// </summary>
        public static string M_AThroughMode
        {
            get { return "M3.5"; }
        }
        /// <summary>
        /// B轨直通
        /// </summary>
        public static string M_BThroughMode
        {
            get { return "M3.6"; }
        }
        /// <summary>
        /// A轨皮带正转
        /// </summary>
        public static string M_ABeltForward
        {
            get { return "M3.7"; }
        }
        /// <summary>
        /// A轨皮带反转
        /// </summary>
        public static string M_ABeltBackward
        {
            get { return "M4.0"; }
        }
        /// <summary>
        /// B轨皮带正转
        /// </summary>
        public static string M_BBeltForward
        {
            get { return "M4.1"; }
        }
        /// <summary>
        /// B轨皮带反转
        /// </summary>
        public static string M_BBeltBackward
        {
            get { return "M4.2"; }
        }
        /// <summary>
        /// A轨点动模式
        /// </summary>
        public static string M_AWidthJogMode
        {
            get { return "M4.3"; }
        }
        /// <summary>
        /// B轨点动模式
        /// </summary>
        public static string M_BWidthJogMode
        {
            get { return "M4.4"; }
        }
        /// <summary>
        /// A轨调宽回原
        /// </summary>
        public static string M_AWidthHoming
        {
            get { return "M5.0"; }
        }
        /// <summary>
        /// A轨调宽正转按键
        /// </summary>
        public static string M_AWidthForward
        {
            get { return "M5.1"; }
        }
        /// <summary>
        /// A轨调宽反转按键
        /// </summary>
        public static string M_AWidthBackward
        {
            get { return "M5.2"; }
        }
        /// <summary>
        /// A轨调宽点动加按键
        /// </summary>
        public static string M_AWidthJogForward
        {
            get { return "M5.3"; }
        }
        /// <summary>
        /// A轨调宽点动减按键
        /// </summary>
        public static string M_AWidthJogBackward
        {
            get { return "M5.4"; }
        }
        /// <summary>
        /// A轨调宽前往调宽位
        /// </summary>
        public static string M_AWidthGoLoosen
        {
            get { return "M5.5"; }
        }
        /// <summary>
        /// A轨调宽前往夹紧位
        /// </summary>
        public static string M_AWidthGoPinch
        {
            get { return "M5.6"; }
        }
        /// <summary>
        /// A轨调宽设置调宽位
        /// </summary>
        public static string M_AWidthSetLoosen
        {
            get { return "M5.7"; }
        }
        /// <summary>
        /// A轨调宽设置夹紧位
        /// </summary>
        public static string M_AWidthSetPinch
        {
            get { return "M6.0"; }
        }

        /// <summary>
        /// B轨调宽回原
        /// </summary>
        public static string M_BWidthHoming
        {
            get { return "M6.1"; }
        }
        /// <summary>
        /// B轨调宽正转按键
        /// </summary>
        public static string M_BWidthForward
        {
            get { return "M6.2"; }
        }
        /// <summary>
        /// B轨调宽反转按键
        /// </summary>
        public static string M_BWidthBackward
        {
            get { return "M6.3"; }
        }
        /// <summary>
        /// B轨调宽点动加按键
        /// </summary>
        public static string M_BWidthJogForward
        {
            get { return "M6.4"; }
        }
        /// <summary>
        /// B轨调宽点动减按键
        /// </summary>
        public static string M_BWidthJogBackward
        {
            get { return "M6.5"; }
        }
        /// <summary>
        /// B轨调宽前往调宽位
        /// </summary>
        public static string M_BWidthGoLoosen
        {
            get { return "M6.6"; }
        }
        /// <summary>
        /// B轨调宽前往夹紧位
        /// </summary>
        public static string M_BWidthGoPinch
        {
            get { return "M6.7"; }
        }
        /// <summary>
        /// B轨调宽设置调宽位
        /// </summary>
        public static string M_BWidthSetLoosen
        {
            get { return "M7.0"; }
        }
        /// <summary>
        /// B轨调宽设置夹紧位
        /// </summary>
        public static string M_BWidthSetPinch
        {
            get { return "M7.1"; }
        }
        /// <summary>
        /// A轨回流按键
        /// </summary>
        public static string M_ABackfolw
        {
            get { return "M7.2"; }
        }
        /// <summary>
        /// B轨回流按键
        /// </summary>
        public static string M_BBackfolw
        {
            get { return "M7.3"; }
        }
        /// <summary>
        /// A轨振动盘报警屏蔽
        /// </summary>
        public static string M_ADisableVibrationAlarm
        {
            get { return "M7.4"; }
        }

        public static string M_BDisableVibrationAlarm
        {
            get { return "M7.5"; }
        }
        #endregion

        #region VW指针
        /// <summary>
        /// A轨入口光电延时
        /// </summary>
        public static string VW_AInDelay
        {
            get { return "V3000"; }
        }
        /// <summary>
        /// A轨到位光电延时
        /// </summary>
        public static string VW_AArriveDelay
        {
            get { return "V3002"; }
        }
        /// <summary>
        /// A轨出口光电延时
        /// </summary>
        public static string VW_AOutDelay
        {
            get { return "V3004"; }
        }
        /// <summary>
        /// A轨出口光电关电延时
        /// </summary>
        public static string VW_AOutCloseDelay
        {
            get { return "V3006"; }
        }
        /// <summary>
        /// A轨进板延时
        /// </summary>
        public static string VW_AEnterDelay
        {
            get { return "V3008"; }
        }
        /// <summary>
        /// A轨顶升延时
        /// </summary>
        public static string VW_ARiseDelay
        {
            get { return "V3010"; }
        }
        /// <summary>
        /// A轨缩回延时
        /// </summary>
        public static string VW_AAnastoleDelay
        {
            get { return "V3012"; }
        }


        /// <summary>
        /// B轨入口光电延时
        /// </summary>
        public static string VW_BInDelay
        {
            get { return "V3014"; }
        }
        /// <summary>
        /// B轨到位光电延时
        /// </summary>
        public static string VW_BArriveDelay
        {
            get { return "V3016"; }
        }
        /// <summary>
        /// B轨出口光电延时
        /// </summary>
        public static string VW_BOutDelay
        {
            get { return "V3018"; }
        }
        /// <summary>
        /// B轨出口光电关电延时
        /// </summary>
        public static string VW_BOutCloseDelay
        {
            get { return "V3020"; }
        }
        /// <summary>
        /// B轨进板延时
        /// </summary>
        public static string VW_BEnterDelay
        {
            get { return "V3022"; }
        }
        /// <summary>
        /// B轨顶升延时
        /// </summary>
        public static string VW_BRiseDelay
        {
            get { return "V3024"; }
        }
        /// <summary>
        /// B轨缩回延时
        /// </summary>
        public static string VW_BAnastoleDelay
        {
            get { return "V3026"; }
        }
        /// <summary>
        /// A轨运行状态显示
        /// </summary>
        public static string VW_ARunStatus
        {
            get { return "V3100"; }
        }
        /// <summary>
        /// A轨调宽状态显示
        /// </summary>
        public static string VW_AWidthStatus
        {
            get { return "V3102"; }
        }
        /// <summary>
        /// B轨运行状态显示
        /// </summary>
        public static string VW_BRunStatus
        {
            get { return "V3104"; }
        }
        /// <summary>
        /// B轨调宽状态显示
        /// </summary>
        public static string VW_BWidthStatus
        {
            get { return "V3106"; }
        }
        #endregion

        #region VD指针
        /// <summary>
        /// A轨调宽当前位置
        /// </summary>
        public static string VD_AWidthPositionActual
        {
            get { return "V350"; }
        }
        /// <summary>
        /// A轨调宽当前速度
        /// </summary>
        public static string VD_AWidthVelocityActual
        {
            get { return "V354"; }
        }
        /// <summary>
        /// A轨调宽位位置
        /// </summary>
        public static string VD_AWidthPositionLoosen
        {
            get { return "V374"; }
        }
        /// <summary>
        /// A轨夹紧位位置
        /// </summary>
        public static string VD_AWidthPositionPinch
        {
            get { return "V378"; }
        }
        /// <summary>
        /// A轨手动速度
        /// </summary>
        public static string VD_AWidthManualSpeed
        {
            get { return "V382"; }
        }
        /// <summary>
        /// A轨自动速度
        /// </summary>
        public static string VD_AWidthAutoSpeed
        {
            get { return "V386"; }
        }
        /// <summary>
        /// A轨点动距离
        /// </summary>
        public static string VD_AWidthJogDistance
        {
            get { return "V390"; }
        }
        /// <summary>
        /// B轨调宽当前位置
        /// </summary>
        public static string VD_BWidthPositionActual
        {
            get { return "V650"; }
        }
        /// <summary>
        /// B轨调宽当前速度
        /// </summary>
        public static string VD_BWidthVelocityActual
        {
            get { return "V654"; }
        }
        /// <summary>
        /// B轨调宽位位置
        /// </summary>
        public static string VD_BWidthPositionLoosen
        {
            get { return "V674"; }
        }
        /// <summary>
        /// B轨夹紧位位置
        /// </summary>
        public static string VD_BWidthPositionPinch
        {
            get { return "V678"; }
        }
        /// <summary>
        /// B轨手动速度
        /// </summary>
        public static string VD_BWidthManualSpeed
        {
            get { return "V682"; }
        }
        /// <summary>
        /// B轨自动速度
        /// </summary>
        public static string VD_BWidthAutoSpeed
        {
            get { return "V686"; }
        }
        /// <summary>
        /// B轨点动距离
        /// </summary>
        public static string VD_BWidthJogDistance
        {
            get { return "V690"; }
        }
        #endregion

        #endregion

        #region Flash报警
        public static int[] AlarmFlash = new int[100];
        #endregion

        #region 系统功能
        /// <summary>
        /// 报警动画功能
        /// </summary>
        private static bool alarmFlashEnable;
        /// <summary>
        /// 报警动画功能
        /// </summary>
        public static bool AlarmFlashEnable
        {
            get
            {
                return alarmFlashEnable;
            }
            set
            {
                if (alarmFlashEnable != value)
                {
                    Common.myLog.writeOperateContent("B轨是否连接下位机由" + alarmFlashEnable + "->" + value, UserPermission, UserPermission);
                }
                alarmFlashEnable = value;
            }
        }
        #endregion
    }
}
