using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using System.Threading;

namespace MountingRobot.BLL
{
    public class ConnectionS7
    {
        private SiemensS7Net siemensS7Net;
        /// <summary>
        /// S7协议连接PLC
        /// </summary>
        /// <param name="IPAdrr">PLC地址</param>
        /// <returns></returns>
        public bool Connect(string IPAdrr)
        {
            bool result = false;
            if (!System.Net.IPAddress.TryParse(IPAdrr, out System.Net.IPAddress address))
            {
                MessageBox.Show("IP地址错误！");
            }
            else
            {
                siemensS7Net = new SiemensS7Net(SiemensPLCS.S200Smart, IPAdrr)
                {
                    ConnectTimeOut = 1000
                };
                OperateResult connect = siemensS7Net.ConnectServer();
                if (connect.IsSuccess)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        #region 读取IO
        /// <summary>
        /// 读取西门子PLC整组输入状态
        /// </summary>
        /// <param name="readCount">需要读取输入个数</param>
        /// <returns></returns>
        public bool[] ReadAllInput(int readCount)
        {
            bool[] listInput = new bool[readCount];
            int H = 0;
            int L = 0;
            string IStr = string.Empty;
            for (int i = 0; i < readCount; i++)
            {
                H = i / 8;
                L = i % 8;
                IStr = "I" + H.ToString() + "." + L.ToString();
                if (siemensS7Net.ReadBool(IStr).Content)
                {
                    listInput[i]=true;
                }
                else
                {
                    listInput[i] = false; ;
                }
            }
            return listInput;
        }

        /// <summary>
        /// 读取西门子PLC整组输出状态
        /// </summary>
        /// <param name="readCount">需要读取输出个数</param>
        /// <returns></returns>
        public bool[] ReadAllOutput(int readCount)
        {
            bool[] listOutput = new bool[readCount];
            int H = 0;
            int L = 0;
            string OStr = string.Empty;
            for (int i = 0; i < readCount; i++)
            {
                if (i<24)  //Smart200 PLC Q0.0-Q2.7
                {
                    H = i / 8;
                    L = i % 8;
                    OStr = "Q" + H.ToString() + "." + L.ToString();
                    if (siemensS7Net.ReadBool(OStr).Content)
                    {
                        listOutput[i] = true;
                    }
                    else
                    {
                        listOutput[i] = false; ;
                    }
                }
                else  //Smart200 PLC Q8.0开始
                {
                    H = i / 8+5;
                    L = i % 8;
                    OStr = "Q" + H.ToString() + "." + L.ToString();
                    if (siemensS7Net.ReadBool(OStr).Content)
                    {
                        listOutput[i] = true;
                    }
                    else
                    {
                        listOutput[i] = false; ;
                    }
                }
            }
            return listOutput;
        }

        /// <summary>
        /// 读取PLC单个IO
        /// </summary>
        /// <param name="IO">IO点，如：I0.0、Q0.0</param>
        /// <returns></returns>
        public bool ReadBool(string IO)
        {
            return siemensS7Net.ReadBool(IO).Content;
        }
        #endregion

        #region 读PLC寄存器值
        /// <summary>
        /// 读取PLC双字节有符号值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public short ReadShort(string MemoryAddr)
        {
            return siemensS7Net.ReadInt16(MemoryAddr).Content;
        }

        /// <summary>
        /// 读取PLC双字节无符号值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public ushort ReaduShort(string MemoryAddr)
        {
            return siemensS7Net.ReadUInt16(MemoryAddr).Content;
        }

        /// <summary>
        /// 读取PLC双字有符号值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public int ReadInt(string MemoryAddr)
        {
            return siemensS7Net.ReadInt32(MemoryAddr).Content;
        }

        /// <summary>
        /// 读取PLC双字无符号值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public uint ReaduInt(string MemoryAddr)
        {
            return siemensS7Net.ReadUInt32(MemoryAddr).Content;
        }

        /// <summary>
        /// 读取PLC单精度值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public float ReadFloat(string MemoryAddr)
        {
            return siemensS7Net.ReadFloat(MemoryAddr).Content;
        }

        /// <summary>
        /// 读取PLC双精度值
        /// </summary>
        /// <param name="MemoryAddr">寄存器地址，如：V100（注意符号大写）</param>
        /// <returns></returns>
        public  double ReadDouble(string MemoryAddr)
        {
            return siemensS7Net.ReadDouble(MemoryAddr).Content;
        }
        #endregion

        #region 写值到PLC中
        /// <summary>
        /// 写IO状态到PLC
        /// </summary>
        /// <param name="IO">IO点，如：I0.0、Q0.0</param>
        /// <param name="Status">IO点状态</param>
        public void Write(string IO,bool Status)
        {
            siemensS7Net.Write(IO,Status);
        }

        /// <summary>
        /// 写寄存器双字节有符号数据
        /// </summary>
        /// <param name="MemoryAddr">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string MemoryAddr, short Data)
        {
            siemensS7Net.Write(MemoryAddr, Data);
        }

        /// <summary>
        /// 写寄存器双字节无符号数据
        /// </summary>
        /// <param name="MemoryAddr">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string MemoryAddr, ushort Data)
        {
            siemensS7Net.Write(MemoryAddr, Data);
        }

        /// <summary>
        /// 写寄存器双字有符号数据
        /// </summary>
        /// <param name="Memory">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string Memory, int Data)
        {
            siemensS7Net.Write(Memory, Data);
        }

        /// <summary>
        /// 写寄存器双字无符号数据
        /// </summary>
        /// <param name="MemoryAddr">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string MemoryAddr, uint Data)
        {
            siemensS7Net.Write(MemoryAddr, Data);
        }

        /// <summary>
        /// 写寄存器单精度数据
        /// </summary>
        /// <param name="MemoryAddr">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string MemoryAddr, float Data)
        {
            siemensS7Net.Write(MemoryAddr, Data);
        }

        /// <summary>
        /// 写寄存器双精度数据
        /// </summary>
        /// <param name="MemoryAddr">内存地址，如：V100（注意符号大写）</param>
        /// <param name="Data">数据值</param>
        public void Write(string MemoryAddr, double Data)
        {
            siemensS7Net.Write(MemoryAddr, Data);
        }
        #endregion

        #region 断开连接
        public void ConnectClose()
        {
            siemensS7Net.ConnectClose();
        }
        #endregion
    }
}
