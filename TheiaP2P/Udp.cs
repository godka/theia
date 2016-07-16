using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
namespace System.Net.Udp
{
    /// <summary> 
    /// ----名称:UDP通讯类 
    /// ----建立:niefei 
    /// ----建立时间:2004-12-6 
    /// </summary> 
    /// <remarks> 
    /// ----使用说明与定义: 
    /// ----接到字符串 "NeedDownCards" 表示需要调用卡下载功能 
    /// </remarks> 
    public class UDPSocket
    {
        private ArrayList m_computers;

        /// <summary> 
        /// 发送命令文本常量 
        /// </summary> 
        private string m_sendText;

        /// <summary> 
        /// 默认发送的字符串 
        /// </summary> 
        private const string m_sendStr = "NeedDownCards";

        /// <summary> 
        /// Udp对象 
        /// </summary> 
        private UdpClient m_Client;



        /// <summary> 
        /// 本地通讯端口(默认8888) 
        /// </summary> 
        private int m_LocalPort;


        /// <summary> 
        /// 跟踪是否退出程序 
        /// </summary> 
        private bool m_Done;



        /// <summary> 
        /// 设置是否要发送 
        /// </summary> 
        private bool m_flag;



        /// <summary> 
        /// 定义一个接受线程 
        /// </summary> 
        public Thread recvThread;



        /// <summary> 
        /// 定义委托 
        /// </summary> 
        public delegate void SOCKETDelegateArrive(IPEndPoint endpoint,string str);



        /// <summary> 
        /// 定义一个消息接收事件 
        /// </summary> 
        public event SOCKETDelegateArrive SOCKETEventArrive;



        /// <summary> 
        /// 下载标志 
        /// </summary> 
        public bool flag
        {
            set { this.m_flag = value; }
            get { return this.m_flag; }
        }



        /// <summary> 
        /// 设置通讯端口 
        /// </summary> 
        public int LocalPort
        {
            set { m_LocalPort = value; }
            get { return m_LocalPort; }
        }



        /// <summary> 
        /// 设置要发送的岗位对象 
        /// </summary> 
        public ArrayList computers
        {
            set { this.m_computers = value; }
            get { return this.m_computers; }
        }



        /// <summary> 
        /// 断开接收  
        /// </summary> 
        public bool Done
        {
            set { m_Done = value; }
            get { return m_Done; }
        }


        
        public UDPSocket(int LocalPort = 0)
        {
            m_sendText = string.Empty;
            m_computers = new ArrayList();
            m_Done = false;
            m_flag = false;
            m_LocalPort = LocalPort;
            this.SOCKETEventArrive = null;
            Init();
        }
        /// <summary> 
        /// 析构函数 
        /// </summary> 
        ~UDPSocket() { Dispose(); }



        /// <summary> 
        /// 关闭对象 
        /// </summary> 
        public void Dispose()
        {
            DisConnection();
            m_computers = null;
        }



        /// <summary> 
        /// 初始化 
        /// </summary> 
        public void Init()
        {
            //初始化UDP对象 
            try
            {
                if (m_LocalPort == 0)
                {
                    m_Client = new UdpClient();
                }
                else
                {
                    m_Client = new UdpClient(m_LocalPort);

                }

                //SOCKETEventArrive("Initialize succeed by " + m_LocalPort.ToString() + " port");
            }
            catch
            {
                //SOCKETEventArrive("Initialize failed by " + m_LocalPort.ToString() + " port");
            }
        }



        /// <summary> 
        /// 关闭UDP对象 
        /// </summary> 
        public void DisConnection()
        {
            if (m_Client != null)
            {
                this.Done = true;
                if (recvThread != null)
                {
                    this.recvThread.Abort();
                }
                m_Client.Close();
                m_Client = null;
                //SOCKETEventArrive("UDP Object Closed");

            }
        }

        public void send(IPEndPoint endpoint, string str)
        {
            UdpClient udp = new UdpClient();
            try
            {
                udp.Connect(endpoint);
                // 连接后传送一个消息给ip主机 
                Byte[] sendBytes = Encoding.UTF8.GetBytes(str);
                udp.Send(sendBytes, sendBytes.Length);
            }
            catch
            {
                //SOCKETEventArrive("Send:" + m_sendText + " failed");
            }
            finally
            {
                udp.Close();
                udp = null;
            }

        }
        public void send(string ip,int port,string str)
        {
            UdpClient udp = new UdpClient();
            try
            {
                udp.Connect(ip, port);
                // 连接后传送一个消息给ip主机 
                Byte[] sendBytes = Encoding.UTF8.GetBytes(str);
                udp.Send(sendBytes, sendBytes.Length);
            }
            catch
            {
                //SOCKETEventArrive("Send:" + m_sendText + " failed");
            }
            finally
            {
                udp.Close();
                udp = null;
            }
        }

        /// <summary> 
        /// 侦听线程 
        /// </summary> 
        public void StartRecvThreadListener()
        {
            try
            {
                // 启动等待连接的线程 
                recvThread = new Thread(new ThreadStart(Received));
                recvThread.Priority = ThreadPriority.Normal;
                recvThread.Start();
                //SOCKETEventArrive("[Received]Thread Start....");
            }
            catch
            {
                //SOCKETEventArrive("[Received]Thread Start failed!");
            }
        }

        /// <summary> 
        /// 循环接收 
        /// </summary> 
        private void Received()
        {
            //Thread.Sleep(2000);
            //ASCII 编码 
            Encoding ASCII = Encoding.Default;
            Thread.Sleep(1); //防止系统资源耗尽 
            while (!m_Done)
            {

                IPEndPoint endpoint = null;
                if (m_Client != null && recvThread.IsAlive)
                {
                    //接收数据   
                    try
                    {
                        Byte[] data = m_Client.Receive(ref endpoint);
                        //得到数据的ACSII的字符串形式 
                        string str = Encoding.UTF8.GetString(data);
                        if (SOCKETEventArrive != null)
                            SOCKETEventArrive(endpoint,str);
                    }
                    catch
                    {
                        Console.WriteLine("asdasaasd");
                        //SOCKETEventArrive(ee.Message.ToString()); 
                    }
                }
                Thread.Sleep(1); //防止系统资源耗尽 
            }
        }
    }
}