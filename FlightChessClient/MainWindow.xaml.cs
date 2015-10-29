using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace FlightChess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 开始游戏标志
        /// </summary>
        public bool flag { get; set; }
        /// <summary>
        /// 地图已加载标志
        /// </summary>
        public bool flagMap { get; set; }
        /// <summary>
        /// 地图
        /// </summary>
        Map _Map { get; set; }
        /// <summary>
        /// 玩家一
        /// </summary>
        Player _Player1 { get; set; }
        /// <summary>
        /// 玩家二
        /// </summary>
        Player _Player2 { get; set; }

        private delegate void outputDelegate(string msg);
        public MainWindow()
        {
            InitializeComponent();
            pi1.txtPlayerName.Text = "玩家一";
            pi1.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) };
            pi2.txtPlayerName.Text = "玩家二";
            pi2.ellAvatar.Fill = new SolidColorBrush() { Color=Color.FromArgb(255,0,0,255 )};
            flag = false;
            flagMap = false;
        }

        #region 事件
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (btnStart.Content.ToString() == "开始游戏")
            {
                btnStart.Content = "停止游戏";
                btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 100, 50) };
                if (String.IsNullOrEmpty(pi1.txtPlayerName.Text) || String.IsNullOrEmpty(pi2.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi1.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi2.txtPlayerName.Text))
                {
                    MessageBox.Show("玩家名不能为空，请重新输入。");
                    return;
                }
                #region 初始化
                //if (flag == true)
                //{
                //    MessageBox.Show("游戏已经开始。");
                //    return;
                //}
                //初始化地图
                _Map = new Map()
                {
                    landMine = new int[] { 5, 13, 17, 33, 38, 50, 64, 80, 94 },
                    luckyturn = new int[] { 6, 23, 40, 55, 69, 83 },
                    pause = new int[] { 9, 27, 60, 93 },
                    timeTunnel = new int[] { 20, 25, 45, 63, 72, 88, 90 }
                };
                _Map.InitailMap();
                if (flagMap != true)
                {
                    Map.DrawMap(_Map.Maps, gdMap);
                    flagMap = true;
                }
                else
                {
                    foreach (var o in gdMap.Children)
                    {
                        if (o is TextBox)
                        {
                            //gdMap.Children.Remove(o as TextBox);
                            (o as TextBox).Visibility = Visibility.Visible;
                        }
                    }
                }
                
                //初始化玩家
                pi1.txtPlayerName.IsEnabled = false;
                pi2.txtPlayerName.IsEnabled = false;
                output("游戏开始！");
                _Player1 = new Player() { PlayerName = pi1.txtPlayerName.Text, PlayerPo = 0, Flag = 0, PlayerUI = ellPlayer1 };
                _Player2 = new Player() { PlayerName = pi2.txtPlayerName.Text, PlayerPo = 0, Flag = 1, PlayerUI = ellPlayer2 };
                ellPlayer1.Visibility = Visibility.Visible;
                ellPlayer2.Visibility = Visibility.Visible;
                Grid.SetZIndex(ellPlayer1, 2);
                Grid.SetZIndex(ellPlayer2, 2);
                flag = true;//已开始游戏标志
                #endregion
            }
            else
            {
                btnStart.Content = "开始游戏";
                btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 100, 255, 50) };
                foreach(var o in gdMap.Children)
                {
                    if(o is TextBox)
                    {
                        //gdMap.Children.Remove(o as TextBox);
                        (o as TextBox).Visibility = Visibility.Hidden;
                    }
                }
                pi1.txtPlayerName.IsEnabled = true;
                pi2.txtPlayerName.IsEnabled = true;
                tbGameRecord.Text="";
                Grid.SetColumn(ellPlayer1, 0);
                Grid.SetRow(ellPlayer1, 0);
                Grid.SetColumn(ellPlayer2, 0);
                Grid.SetRow(ellPlayer2, 0);
                ellPlayer1.Visibility = Visibility.Hidden;
                ellPlayer2.Visibility = Visibility.Hidden;
                _Map = null;
                _Player1 = null;
                _Player2 = null;
                flag =false;
            }
            
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (flag == false)
            {
                MessageBox.Show("游戏未开始。");
                return;
            }
            if(_Player2.Flag + _Player1.Flag > 2)
            {
                var tmp = (_Player1.Flag < _Player1.Flag) ? (_Player2.Flag - _Player1.Flag) : (_Player1.Flag - _Player2.Flag);
                _Player1.Flag = (_Player1.Flag < _Player1.Flag) ? 0 : 1;
                _Player2.Flag = (_Player1.Flag ==0 ) ? 1 : 0;
            }
            var currentPlayer = (_Player1.Flag < _Player2.Flag) ? _Player1 : _Player2;
            var anotherPlayer = (currentPlayer == _Player1) ? _Player2 : _Player1;
            var num = (new Random()).Next(1, 7);
            output("玩家"+currentPlayer.PlayerName+"掷出了"+ num.ToString()+"点。");
            Game.PlayGame(_Map, currentPlayer, anotherPlayer, num);
            if (anotherPlayer.Flag > 1)
                currentPlayer.Flag++;
            anotherPlayer.Flag--; 

        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            var socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ip = IPAddress.Parse(txtIP.Text);
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtPort.Text));
            socketWatch.Bind(point);
            output("监听成功.");
            socketWatch.Listen(1);

            var th = new Thread(Listen);
            th.IsBackground = true;
            th.Start(socketWatch);
        }
        #endregion

        Socket socketSend;

        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            //等待客户端的连接 并且创建一个负责通信的Socket
            while (true)
            {
                try
                {
                    //负责跟客户端通信的Socket
                    socketSend = socketWatch.Accept();
                    //将远程连接的客户端的IP地址和Socket存入集合中
                    //dicSocket.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
                    //将远程连接的客户端的IP地址和端口号存储下拉框中
                    //cboUsers.Items.Add(socketSend.RemoteEndPoint.ToString());
                    //192.168.11.78：连接成功
                    output(socketSend.RemoteEndPoint.ToString() + ":连接成功。");
                    //tbGameRecord.Text += socketSend.RemoteEndPoint.ToString() + ":连接成功\n";
                    //MessageBox.Show(socketSend.RemoteEndPoint.ToString() + ":连接成功\n");
                    //开启 一个新线程不停的接受客户端发送过来的消息
                    Thread th = new Thread(Recive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }
                catch 
                { }
            }
            
        }
        //Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();

        void Recive(object o)
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                try
                {
                    //客户端连接成功后，服务器应该接受客户端发来的消息
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    //实际接受到的有效字节数
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    output(socketSend.RemoteEndPoint.ToString() + "：" + str+"。");
                }
                catch
                { }
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string str = tbMsg.Text;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            List<byte> list = new List<byte>();
            list.Add(0);
            list.AddRange(buffer);
            //将泛型集合转换为数组
            byte[] newBuffer = list.ToArray();
            //buffer = list.ToArray();不可能
            //获得用户在下拉框中选中的IP地址
            string ip = socketSend.RemoteEndPoint.ToString();
            socketSend.Send(newBuffer);
            //     socketSend.Send(buffer);
        }

        private void output(string msg)
        {
            this.tbGameRecord.Dispatcher.Invoke(new outputDelegate(outputAction), msg);
        }

        private void outputAction(string msg)
        {
            this.tbGameRecord.Text+=msg+"\n";
        }
    }
}
