using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket通信_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket socketSend;
        private void btnStart_Click(object sender, EventArgs e)
        {
            //创建负责通信的Socket
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(txtServer.Text);
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtPort.Text));
            //获得要连接的远程服务器应用程序的IP地址和端口号
            socketSend.Connect(point);
            ShowMsg("连接成功");

            Thread th = new Thread(Recive);
            th.IsBackground = true;
            th.Start();
        }
        void ShowMsg(string str)
        {
            txtLog.AppendText(str + "\r\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = txtMsg.Text.Trim();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            socketSend.Send(buffer);
        }
        /// <summary>
        /// 不停的接收服务器发来的消息
        /// </summary>
        void Recive()
        {
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                int r = socketSend.Receive(buffer);
                //实际接收到的有效字节数
                if (r == 0)
                {
                    break;
                }

                if (buffer[0] == 0)
                {

                    string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                    ShowMsg(socketSend.RemoteEndPoint + ":" + s);
                }
                else if (buffer[0] == 1)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.InitialDirectory = @"C:\Users\Xuheyao\Desktop";
                    sfd.Title = "请选择要保存的文件";
                    sfd.Filter = "所有文件|*.*";
                    sfd.ShowDialog(this);

                    string path = sfd.FileName;
                    using (FileStream fsWrite = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fsWrite.Write(buffer, 1, r - 1);
                    }
                    MessageBox.Show("保存成功");
                }
                else if (buffer[0] == 2)
                {
                    ZD();
                }
            }
        }
        /// <summary>
        /// 震动
        /// </summary>
        void ZD()
        {
            Point p = this.Location;
            int x = p.X;
            int y = p.Y;
            int j;
            for (int i = 0; i < 200; i++)
            {
                this.Location = new Point(x, y + 5);
                for (j = 0; j < 10000; j++) ;
                this.Location = new Point(x + 5, y + 5);
                for (j = 0; j < 10000; j++) ;
                this.Location = new Point(x + 5, y);
                for (j = 0; j < 10000; j++) ;
                this.Location = new Point(x, y + 5);
                for (j = 0; j < 10000; j++) ;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
