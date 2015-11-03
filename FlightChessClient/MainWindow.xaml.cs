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

namespace FlightChessClient
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
            pi2.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 255) };
            flag = false;
            flagMap = false;

            //选择游戏模式
            if (MessageBox.Show("选是“单机模式”，选否联机模式", "请选择操作", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                //单机
                txtIP.Visibility = txtPort.Visibility = btnLink.Visibility = tbMsg.Visibility = btnSend.Visibility = Visibility.Collapsed;
                flagMode = false;
            }
            else
            {
                //联机
                //btnStart.Visibility = Visibility.Hidden;
                flagMode = true;
                pi1.IsEnabled = false;
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
                    btnPlay.IsEnabled = true;
                    //output("游戏开始！");
                    _Player1 = new Player() { PlayerName = pi1.txtPlayerName.Text, PlayerPo = 0, Flag = 0, PlayerUI = ellPlayer1 };
                    _Player2 = new Player() { PlayerName = pi2.txtPlayerName.Text, PlayerPo = 0, Flag = 1, PlayerUI = ellPlayer2 };
                    ellPlayer1.Visibility = Visibility.Visible;
                    ellPlayer2.Visibility = Visibility.Visible;
                    Grid.SetZIndex(ellPlayer1, 2);
                    Grid.SetZIndex(ellPlayer2, 2);
                    if (flagMode == true)
                    {
                        //P2昵称
                        var buffer = Encoding.UTF8.GetBytes(_Player2.PlayerName);
                        var list = new List<byte>();
                        list.Add(15);//P2昵称
                        list.AddRange(buffer);
                        //将泛型集合转换为数组
                        socketSend.Send(list.ToArray());
                    }
                    else
                    {
                        btnPlay.IsEnabled = true;
                    }
                    flag = true;//已开始游戏标志
                    #endregion
                }
                else  if(flagMode==false)
                {
                    btnStart.Content = "开始游戏";
                    btnStart.Background = new SolidColorBrush() { Color = Color.FromArgb(255, 100, 255, 50) };
                    foreach (var o in gdMap.Children)
                    {
                        if (o is TextBox)
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
                    flag = false;
                }
            }
            catch { }
            
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Player currentPlayer;
            Player anotherPlayer;
            var result = string.Empty;
            if (flagMode == false)
            {
                currentPlayer = Game.CompareFlag(_Player1, _Player2);
                anotherPlayer = (currentPlayer == _Player1) ? _Player2 : _Player1;
            }
            else
            {
                currentPlayer = _Player2;
                anotherPlayer = _Player1;
            }
            var num = (new Random()).Next(1, 7);

            result += currentPlayer.PlayerName + "掷出了" + num.ToString() + "点。\n";
            result += Game.PlayGame(_Map, currentPlayer, anotherPlayer, num);
            output(result);
            

            if (flagMode == true)
            {
                if (_Player1.Flag > 1)
                    _Player2.Flag++;
                _Player1.Flag--;
                #region 发送本轮信息

                
                //p1位置
                byte[] buffer = Encoding.UTF8.GetBytes(_Player1.PlayerPo.ToString());
                List<byte> list = new List<byte>();
                list.Add(12);//P1位置
                list.AddRange(buffer);
                socketSend.Send(list.ToArray());

                //p2位置
                var buffer1 = Encoding.UTF8.GetBytes(_Player2.PlayerPo.ToString());
                var list1 = new List<byte>();
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
                ChangeBtnState(_Player1.Flag.ToString()+"/"+ _Player2.Flag.ToString());

                Thread.Sleep(50);
                byte[] buffer4 = Encoding.UTF8.GetBytes("B" + "/" + _Player2.PlayerName + "/" + num.ToString());
                socketSend.Send(buffer4);
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

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ip = IPAddress.Parse(txtIP.Text);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtPort.Text));
                socketSend.Connect(point);
                output("连接成功.");
                if (txtIP.Visibility == Visibility.Visible)
                {
                    txtIP.Visibility = txtPort.Visibility = btnLink.Visibility = Visibility.Collapsed;
                }
                var th = new Thread(Recive);
                th.IsBackground = true;
                th.Start();
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

        void Recive()
        {
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
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        InitMap("");
                        output(s);
                    }
                    else if (buffer[0] == 12)//p1位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        if (Convert.ToInt32(s) == 99)
                        {
                            MessageBox.Show(_Player1.PlayerName + "胜利");
                            output(_Player1.PlayerName + "胜利");
                        }
                        Move1(s);
                    }
                    else if (buffer[0] == 13)//p2位置
                    {
                        var s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        Move2(s);
                    }
                    else if (buffer[0] == 14)
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        ChangeBtnState(s);
                    }
                    else if (buffer[0] == 15)//昵称
                    {
                        string s = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        _Player1.PlayerName = s;
                        ChangeName(s);
                    }
                    else if (buffer[0] == 65)//昵称
                    {
                        string s = Encoding.UTF8.GetString(buffer, 2, r - 2);
                        var tmp = s.Split('/');
                        Move1(tmp[0]);
                        _Player1.PlayerName = tmp[1];
                        ChangeName(tmp[1]);
                        output("对方掷了" + tmp[2] + "点。");
                        Move2(tmp[3]);
                    }
                }
                catch
                { }
            }
        }     

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
            pi1.txtPlayerName.Text = msg;
        }
        //修改按钮状态
        private void ChangeBtnState(string msg)
        {
            this.btnStart.Dispatcher.Invoke(new outputDelegate(ChangeBtnStateAct), msg);
        }
        private void ChangeBtnStateAct(string msg)
        {
            var tmp = msg.Split('/');
            _Player1.Flag = Convert.ToInt32(tmp[0]);
            _Player2.Flag = Convert.ToInt32(tmp[1]);

            if (Game.CompareFlag(_Player1, _Player2) == _Player2)
            {
                btnPlay.IsEnabled = true;
            }
            else
            {
                btnPlay.IsEnabled = true;
            }

        }
        #endregion
    }
}
