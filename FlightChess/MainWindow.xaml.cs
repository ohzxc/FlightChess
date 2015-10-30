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
            pi1.txtPlayerName.Text = "玩家一";
            pi1.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) };
            pi2.txtPlayerName.Text = "玩家二";
            pi2.ellAvatar.Fill = new SolidColorBrush() { Color=Color.FromArgb(255,0,0,255 )};
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
            if (btnStart.Content.ToString() == "开始游戏")
            {
                btnPlay.IsEnabled = true;
                btnStart.Content = "停止游戏";
                btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 100, 50) };
                if (String.IsNullOrEmpty(pi1.txtPlayerName.Text) || String.IsNullOrEmpty(pi2.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi1.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi2.txtPlayerName.Text))
                {
                    MessageBox.Show("玩家名不能为空，请重新输入。");
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
                            //gdMap.Children.Remove(o as TextBox);
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
                if (flagMode == true)
                {
                    //发送开局信息
                    byte[] buffer = Encoding.UTF8.GetBytes("游戏开始");
                    List<byte> list = new List<byte>();
                    list.Add(11);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    socketSend.Send(list.ToArray());
                }
                output("游戏开始");
                flag = true;//已开始游戏标志
                
            }
            else
            {
                btnPlay.IsEnabled = false;
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
                if (flagMode == true)
                {
                    //发送结束信息
                    byte[] buffer = Encoding.UTF8.GetBytes("游戏结束");
                    List<byte> list = new List<byte>();
                    list.Add(11);
                    list.AddRange(buffer);
                    //将泛型集合转换为数组
                    socketSend.Send(list.ToArray());
                }
                output("游戏结束");
                flag =false;
            }
            
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Player currentPlayer;
            Player anotherPlayer;
            if (flagMode == false)
            {
                if (_Player2.Flag + _Player1.Flag > 2)
                {
                    var tmp = (_Player1.Flag < _Player1.Flag) ? (_Player2.Flag - _Player1.Flag) : (_Player1.Flag - _Player2.Flag);
                    _Player1.Flag = (_Player1.Flag < _Player1.Flag) ? 0 : 1;
                    _Player2.Flag = (_Player1.Flag == 0) ? 1 : 0;
                }
                currentPlayer = (_Player1.Flag < _Player2.Flag) ? _Player1 : _Player2;
                anotherPlayer = (currentPlayer == _Player1) ? _Player2 : _Player1;
            }
            else
            {
                currentPlayer = _Player1;
                anotherPlayer = _Player2;
            }
            var num = (new Random()).Next(1, 7);
            
            output("玩家"+currentPlayer.PlayerName+"掷出了"+ num.ToString()+"点。");
            Game.PlayGame(_Map, currentPlayer, anotherPlayer, num);
            if (anotherPlayer.Flag > 1)
                currentPlayer.Flag++;
            anotherPlayer.Flag--;
            if (flagMode == true)
            {
                #region 发送本轮信息
                //p1位置
                byte[] buffer = Encoding.UTF8.GetBytes(_Player1.PlayerPo.ToString());
                List<byte> list = new List<byte>();
                list.Add(12);//P1位置
                list.AddRange(buffer);
                //将泛型集合转换为数组
                socketSend.Send(list.ToArray());

                //p2位置
                var buffer1 = Encoding.UTF8.GetBytes(_Player2.PlayerPo.ToString());
                var list1 = new List<byte>();
                list1.Add(13);//P2位置
                list1.AddRange(buffer1);
                //将泛型集合转换为数组
                socketSend.Send(list1.ToArray());

                //游戏记录
                //var buffer2 = Encoding.UTF8.GetBytes("游戏记录");
                //var list2 = new List<byte>();
                //list2.Add(14);
                //list2.AddRange(buffer2);
                ////将泛型集合转换为数组
                //socketSend.Send(list2.ToArray());


                ////flag
                //list = null;
                //buffer = null;
                //buffer = Encoding.UTF8.GetBytes("游戏开始");
                //list.Add(14);
                //list.AddRange(buffer);
                ////将泛型集合转换为数组
                //socketSend.Send(list.ToArray());
                //output("游戏开始");
            }
            #endregion

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

                var th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(socketWatch);
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
                //将泛型集合转换为数组
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
                        output("对方:" + s);
                    }
                    //else if (buffer[0] == 11)//开始，结束
                    //{
                    //    string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                    //    output(s);
                    //}
                    else if (buffer[0] == 12)//p1位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        output("我的目前位置" + s);
                        Move1(s);
                    }
                    else if (buffer[0] == 13)//p2位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        output("对方目前位置" + s);
                        if (Convert.ToInt32(s) == 99)
                        {
                            MessageBox.Show(_Player2.PlayerName + "胜利");
                            output(_Player2.PlayerName + "胜利");
                        }
                        Move2(s);
                    }
                    else if (buffer[0] == 14)
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        output(s);
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
            if (txtIP.Visibility == Visibility.Visible)
            {
                txtIP.Visibility = txtPort.Visibility = btnListen.Visibility = Visibility.Collapsed;
            }
        }
        //连接成功开始按钮有效
        private void start(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(StartAct), msg);
        }
        private void StartAct(string msg)
        {
            if (btnStart.IsEnabled == false)
                btnStart.IsEnabled = true;
            else
                btnStart.IsEnabled = false;
        }
        //更新P1位置
        private void Move1(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(MoveAct1), msg);
        }
        private void MoveAct1(string msg)
        {
            Game.PlayerMoveExt(_Player1, Convert.ToInt32(msg));
        }
        //更新P2位置
        private void Move2(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(MoveAct2), msg);
        }
        private void MoveAct2(string msg)
        {
            Game.PlayerMoveExt(_Player2, Convert.ToInt32(msg));
        }
        #endregion
    }
}
