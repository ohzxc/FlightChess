﻿using System;
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
        #region 变量
        /// <summary>
        /// 开始游戏标志
        /// </summary>
        bool flag { get; set; }
        /// <summary>
        /// 地图已加载标志
        /// </summary>
        bool flagMap { get; set; }
        /// <summary>
        /// 游戏模式，false单机，true联机
        /// </summary>
        bool flagMode { get; set; }
        /// <summary>
        /// 终端标识，true主机，false客机
        /// </summary>
        bool flagEnd { get; set; }
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
        /// <summary>
        /// 通讯套接字
        /// </summary>
        Socket socketSend { get; set; }
        
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            
            flag = false;
            flagMap = false;

            //选择游戏模式
            if (MessageBox.Show("选是“单机模式”，选否联机模式", "请选择操作", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                //单机
                txtIP.Visibility = txtPort.Visibility = btnListen.Visibility=tbMsg.Visibility=btnSend.Visibility = Visibility.Collapsed;
                flagMode = false;
            }
            else
            {
                //联机
                btnStart.IsEnabled = false;
                flagMode = true;
                pi2.IsEnabled = false;
            }
        }

        #region 按钮事件
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnStart.Content.ToString() == "开始游戏")
                {
                    btnStart.Content = "结束游戏";
                    btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 100, 50) };
                    if (String.IsNullOrEmpty(pi1.txtPlayerName.Text) || String.IsNullOrEmpty(pi2.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi1.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi2.txtPlayerName.Text))
                    {
                        MessageBox.Show("昵称不能为空，请重新输入。");
                        return;
                    }
                    #region 初始化
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
                                (o as TextBox).Visibility = Visibility.Visible;
                            }
                        }
                    }

                    //初始化玩家
                    pi1.txtPlayerName.IsEnabled = false;
                    pi2.txtPlayerName.IsEnabled = false;
                    btnPlay.IsEnabled = true;
                    _Player1 = new Player() { PlayerName = pi1.txtPlayerName.Text, PlayerPo = 0, Flag = 0, PlayerUI = ellPlayer1 };
                    _Player2 = new Player() { PlayerName = pi2.txtPlayerName.Text, PlayerPo = 0, Flag = 1, PlayerUI = ellPlayer2 };
                    ellPlayer1.Visibility = Visibility.Visible;
                    ellPlayer2.Visibility = Visibility.Visible;
                    Grid.SetZIndex(ellPlayer1, 2);
                    Grid.SetZIndex(ellPlayer2, 2);
                    #endregion

                    if (flagMode == true&&flagEnd==true)
                    {
                        //发送开局信息
                        var buffer = Encoding.UTF8.GetBytes("游戏开始");
                        var list = new List<byte>();
                        list.Add(11);
                        list.AddRange(buffer);
                        socketSend.Send(list.ToArray());
                        //p1昵称
                        var buffer1 = Encoding.UTF8.GetBytes(_Player1.PlayerName);
                        var list1 = new List<byte>();
                        list1.Add(15);//昵称
                        list1.AddRange(buffer1);
                        socketSend.Send(list1.ToArray());
                    }
                    output("游戏开始");
                    flag = true;//已开始游戏标志

                }
                else
                {
                    btnStart.Content = "开始游戏";
                    btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 100, 255, 50) };
                    //隐藏地图,角色归位
                    foreach (var o in gdMap.Children)
                    {
                        if (o is TextBox)
                        {
                            (o as TextBox).Visibility = Visibility.Hidden;
                        }
                    }
                    pi1.txtPlayerName.IsEnabled = true;
                    pi2.txtPlayerName.IsEnabled = false;
                    btnPlay.IsEnabled = false;
                    Grid.SetColumn(ellPlayer1, 0);
                    Grid.SetRow(ellPlayer1, 0);
                    Grid.SetColumn(ellPlayer2, 0);
                    Grid.SetRow(ellPlayer2, 0);
                    ellPlayer1.Visibility = Visibility.Hidden;
                    ellPlayer2.Visibility = Visibility.Hidden;
                    _Map = null;
                    _Player1 = null;
                    _Player2 = null;
                    if (flagMode == true && flagEnd == true)
                    {
                        //发送结束信息
                        byte[] buffer = Encoding.UTF8.GetBytes("游戏结束");
                        List<byte> list = new List<byte>();
                        list.Add(11);
                        list.AddRange(buffer);
                        socketSend.Send(list.ToArray());
                    }
                    output("游戏结束");
                    flag = false;
                }
            }
            catch { }
            
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            #region 判断玩家
            Player currentPlayer;
            Player anotherPlayer;
            var result = string.Empty;
            if (flagMode == false)
            {
                currentPlayer = Game.CompareFlag(_Player1, _Player2);
                anotherPlayer = (currentPlayer == _Player1) ? _Player2 : _Player1;
            }
            else if (flagEnd==true)
            {
                currentPlayer = _Player1;
                anotherPlayer = _Player2;
            }
            else
            {
                currentPlayer = _Player2;
                anotherPlayer = _Player1;
            }
            #endregion
            var num = (new Random()).Next(1, 7);
            result +=  currentPlayer.PlayerName + "掷出了" + num.ToString() + "点。\n";
            result += Game.PlayGame(_Map, currentPlayer, anotherPlayer, num);
            output(result);
            if (flagMode == true)
            {
                if (_Player2.Flag > 1)
                    _Player1.Flag++;
                _Player2.Flag--;
                
                #region 发送本轮信息

                //p1位置
                byte[] buffer = Encoding.UTF8.GetBytes(_Player1.PlayerPo.ToString());
                List<byte> list = new List<byte>();
                list.Add(12);//P1位置
                list.AddRange(buffer);
                //将泛型集合转换为数组
                socketSend.Send(list.ToArray());

                //p2位置
                byte[] buffer1 = Encoding.UTF8.GetBytes(_Player2.PlayerPo.ToString());
                List<byte> list1 = new List<byte>();
                list1.Add(13);//P2位置
                list1.AddRange(buffer1);
                socketSend.Send(list1.ToArray());
                Thread.Sleep(50);

                //游戏日志
                byte[] buffer2 = Encoding.UTF8.GetBytes(result);
                List<byte> list2 = new List<byte>();
                list2.Add(7);
                list2.AddRange(buffer2);
                socketSend.Send(list2.ToArray());
                Thread.Sleep(50);

                //flag
                List<byte> list3 = new List<byte>();
                list3.Add(14);
                list3.AddRange(Encoding.UTF8.GetBytes(_Player1.Flag.ToString()));
                list3.AddRange(Encoding.UTF8.GetBytes("/"));
                list3.AddRange(Encoding.UTF8.GetBytes(_Player2.Flag.ToString()));
                socketSend.Send(list3.ToArray());
                ChangeBtnState(_Player1.Flag.ToString() + "/" + _Player2.Flag.ToString());
                #endregion
            }
            else
            {
                if (anotherPlayer.Flag > 1)
                    currentPlayer.Flag++;
                anotherPlayer.Flag--;
            }
            pi1.txtPo.Text = _Player1.PlayerPo.ToString();
            pi2.txtPo.Text = _Player2.PlayerPo.ToString();

        }

        private void btnListen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                var socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ip = IPAddress.Parse(txtIP.Text);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtPort.Text));
                socketWatch.Bind(point);
                output("监听成功");
                socketWatch.Listen(1);

                #region 主机 
                flagEnd = true;
                pi1.txtPlayerName.Text = "PlayerA";
                pi1.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) };
                pi2.txtPlayerName.Text = "PlayerB";
                pi2.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 255) };
                #endregion
                var th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(socketWatch);
            }
            catch { }
        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (String.IsNullOrEmpty(pi1.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi1.txtPlayerName.Text))
                //{
                //    MessageBox.Show("昵称不能为空，请重新输入。");
                //    return;
                //}
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ip = IPAddress.Parse(txtIP.Text);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtPort.Text));
                socketSend.Connect(point);
                output("连接成功.");

                #region 客机  
                flagEnd = false;
                btnStart.IsEnabled = false;
                if (txtIP.Visibility == Visibility.Visible)
                {
                    txtIP.Visibility = txtPort.Visibility = btnListen.Visibility = btnLink.Visibility = Visibility.Collapsed;
                }
                pi1.txtPlayerName.Text = "PlayerB";
                pi1.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 255) };
                pi2.txtPlayerName.Text = "PlayerA";
                pi2.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) };
                #endregion
                var th = new Thread(Recive);
                th.IsBackground = true;
                th.Start(socketSend);
            }
            catch { }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str = tbMsg.Text.Trim();
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                List<byte> list = new List<byte>();
                list.Add(7);
                list.AddRange(buffer);
                socketSend.Send(list.ToArray());
                output("我：" + str);
                tbMsg.Text = "";
            }
            catch { }
        }


        #endregion

        #region 通讯
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
                    output(socketSend.RemoteEndPoint.ToString() + ":连接成功。");
                    start("");
                    //开启 一个新线程不停的接受客户端发送过来的消息
                    Thread th = new Thread(Recive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }
                catch 
                { }
            }
            
        }
       
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
                    
                    if (buffer[0] == 7)//发送正常消息
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        output("对方：" + s);
                    }
                    else if (buffer[0] == 11)//开始，结束
                    {
                        //string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        InitMap("");
                        //output(s);
                    }
                    else if (buffer[0] == 12)//p1位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        //output("我的目前位置" + s);
                        Move1(s);
                    }
                    else if (buffer[0] == 13)//p2位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        var tmp = s.Split('\a');
                        if (Convert.ToInt32(tmp[0]) == 99)
                        {
                            MessageBox.Show(_Player2.PlayerName + "胜利");
                            output(_Player2.PlayerName + "胜利");
                        }
                        Move2(tmp[0]);
                    }
                    else if (buffer[0] == 14)
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        ChangeBtnState(s);
                    }
                    else if (buffer[0] == 15)//昵称
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        _Player2.PlayerName = s;
                        ChangeName(s);
                    }
                }
                catch
                { }
            }
        }
        #endregion

        #region 为了跨线程修改控件
        private delegate void outputDelegate(string msg);
        //输出游戏记录
        private void output(string msg)
        {
            this.tbGameRecord.Dispatcher.Invoke(new outputDelegate(outputAction), msg);
        }
        private void outputAction(string msg)
        {
            this.tbGameRecord.Text+=msg+"\n";
            
        }
        //连接成功开始按钮有效
        private void start(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(StartAct), msg);
        }
        private void StartAct(string msg)
        {
            btnStart.IsEnabled = true;
            if (txtIP.Visibility == Visibility.Visible)
            {
                txtIP.Visibility = txtPort.Visibility = btnListen.Visibility=btnLink.Visibility = Visibility.Collapsed;
            }
        }
        //更新P1位置
        private void Move1(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(MoveAct1), msg);
        }
        private void MoveAct1(string msg)
        {
            Game.PlayerMoveExt(_Player1, Convert.ToInt32(msg));
            pi1.txtPo.Text = msg;
        }
        //更新P2位置
        private void Move2(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(MoveAct2), msg);
        }
        private void MoveAct2(string msg)
        {
            Game.PlayerMoveExt(_Player2, Convert.ToInt32(msg));
            pi2.txtPo.Text = msg;
        }
        //修改玩家昵称
        private void ChangeName(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(ChangeNameeAct), msg);
        }
        private void ChangeNameeAct(string msg)
        {
            pi2.txtPlayerName.Text = msg;
        }
        //修改按钮状态
        private void ChangeBtnState(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(ChangeBtnStateAct), msg);
        }
        private void ChangeBtnStateAct(string msg)
        {
            var tmp = msg.Split('/');
            _Player1.Flag = Convert.ToInt32(tmp[1]);
            _Player2.Flag = Convert.ToInt32(tmp[0]);

            if (Game.CompareFlag(_Player1, _Player2) == _Player1)
            {
                btnPlay.IsEnabled = true;
            }
            else
            {
                btnPlay.IsEnabled = false;
            }

        }
        //初始化地图
        private void InitMap(string msg)
        {
            this.gdMap.Dispatcher.Invoke(new outputDelegate(InitMapAct), msg);
        }
        private void InitMapAct(string msg)
        {
            btnStart_Click(default(object), default(RoutedEventArgs));
            //btnPlay.IsEnabled = true;
        }
        #endregion


    }
}
