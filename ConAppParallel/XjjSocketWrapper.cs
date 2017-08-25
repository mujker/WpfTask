using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConAppParallel
{
    class XjjSocketWrapper : IDisposable
    {
        private readonly object locker = new object();
        private int MaxMem = 1024 * 1024;
        private bool isOpen = false;
        /// <summary>
        /// 主机
        /// </summary>
        private string ip = "127.0.0.1";
        public string IP
        {
            get { return this.ip; }
            set { this.ip = value; }
        }

        /// <summary>
        /// 端口
        /// </summary>
        private int port = 0;
        public int Port
        {
            get { return this.port; }
            set { this.port = value; }
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        private int timeout = 1000;
        public int TimeOut
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }
        private Socket socket = null;
        private bool IsCanRead = true;

        public bool IsOpen()
        {
            return this.isOpen;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            //lock (locker)
            //{
            try
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, timeout);
                this.socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, timeout);
                this.socket.Connect(IPAddress.Parse(ip), port);
                isOpen = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            // }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            isOpen = false;
            if (this.socket != null)
            {
                if (this.socket.Connected)
                {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Disconnect(true);
                    this.socket.Close();
                }
                this.socket = null;
            }
        }
        /// <summary>
        /// 重连
        /// </summary>
        public void ReConnect()
        {
            DisConnect();
            Thread.Sleep(1000);
            Connect();
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>读到的数据</returns>
        public byte[] Read(int length)
        {
            //lock (locker)
            //{
            byte[] data = new byte[length];
            try
            {
                this.socket.Receive(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
            //}
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="data">写入内容</param>
        public void Write(byte[] data)
        {
            //lock (locker)
            //{
            try
            {
                this.socket.Send(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //}
        }
        /// <summary>
        /// 发送指令并得到返回结果
        /// </summary>
        /// <param name="sendData">发送的内容</param>
        /// <returns>接收的内容</returns>
        public string Receive(string sendData, bool isEncrypt = false)
        {
            //lock (locker)
            //{
            try
            {
                if (isEncrypt)
                {
                    if (sendData.IndexOf("#") > -1)
                        sendData = sendData.Substring(0, sendData.IndexOf("#"));
//                    sendData = DataPacketCodec.encrypt(sendData);
                    sendData += "#";
                }
                try
                {
                    if (IsCanRead)
                        this.Write(System.Text.Encoding.UTF8.GetBytes(sendData)); //发送读请求
                }
                catch
                {
                    this.ReConnect();
                    return null;
                }
                byte[] receiveData = null;
                try
                {
                    if (IsCanRead)
                        receiveData = this.Read(MaxMem);
                }
                catch
                {
                    return null;
                }
                string strReceive = "";
                if (receiveData != null)
                    strReceive = System.Text.Encoding.UTF8.GetString(receiveData);
                else
                    return null;
                if (isEncrypt)
                {
                    if (strReceive.IndexOf("#") > -1)
                        strReceive = strReceive.Substring(0, strReceive.IndexOf("#"));
//                    strReceive = DataPacketCodec.decrypt(strReceive);
                }
                return strReceive;
            }
            catch
            {
                return null;
            }
            // }
        }

        public int Send(string sendData, bool isEncrypt = false)//下发写入指令 
        {
            //lock (locker)
            //{
            try
            {
                IsCanRead = false;
                if (isEncrypt)
                {
                    if (sendData.IndexOf("#") > -1)
                        sendData = sendData.Substring(0, sendData.IndexOf("#"));
//                    sendData = DataPacketCodec.encrypt(sendData);
                    sendData += "#";
                }
                try
                {
                    this.Write(System.Text.Encoding.UTF8.GetBytes(sendData));
                    Thread.Sleep(1000);
                }
                catch
                {
                    this.ReConnect();
                    return 0;
                }

            }
            catch
            {
                return 0;
            }
            finally
            {
                IsCanRead = true;
            }
            return 1;
            // }
        }
        public string ReceiveDES(string sendData)
        {
            return Receive(sendData, true);
        }

        public int SendDES(string sendData)
        {
            return Send(sendData, true);
        }

        #region IDisposable 成员
        public void Dispose()
        {
            lock (locker)
            {
                if (this.socket != null)
                {
                    this.socket.Close();
                }
            }
        }
        #endregion
    }
}
