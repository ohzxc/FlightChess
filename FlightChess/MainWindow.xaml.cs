using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace FlightChess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 初始化标志
        /// </summary>
        public bool flag { get; set; }
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
        public MainWindow()
        {
            InitializeComponent();
            pi1.txtPlayerName.Text = "玩家一";
            pi1.ellAvatar.Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) };
            pi2.txtPlayerName.Text = "玩家二";
            pi2.ellAvatar.Fill = new SolidColorBrush() { Color=Color.FromArgb(255,0,0,255 )};
            flag = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(pi1.txtPlayerName.Text) || String.IsNullOrEmpty(pi2.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi1.txtPlayerName.Text) || String.IsNullOrWhiteSpace(pi2.txtPlayerName.Text))
            {
                MessageBox.Show("玩家名不能为空，请重新输入。");
                return;
            }
            #region 初始化
            if (flag == true)
            {
                MessageBox.Show("游戏已经开始。");
                return;
            }
            //初始化地图
            _Map = new Map()
            {
                landMine = new int[] { 5, 13, 17, 33, 38, 50, 64, 80, 94 },
                luckyturn = new int[] { 6, 23, 40, 55, 69, 83 },
                pause = new int[] { 9, 27, 60, 93, 2, 3, 4, 7, 8 },
                timeTunnel = new int[] { 20, 25, 45, 63, 72, 88, 90 }
            };        
            _Map.InitailMap();
            Map.DrawMap(_Map.Maps, gdMap);
            //初始化玩家
            pi1.txtPlayerName.IsEnabled = false;
            pi2.txtPlayerName.IsEnabled = false;
            _Player1 = new Player() { PlayerName = pi1.txtPlayerName.Text, PlayerPo = 0, Flag = 0 };
            _Player2 = new Player() { PlayerName = pi2.txtPlayerName.Text, PlayerPo = 0, Flag = 1 };
            Map.AddControls(new Ellipse() {
                Name = "ellPlayer1",
                Height = 10,
                Width = 10,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10),
                Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 255, 0, 0) }
            }, gdMap, 0);
            Map.AddControls(new Ellipse()
            {
                Name = "ellPlayer2",
                Height = 10,
                Width = 10,
                Margin = new Thickness(10),
                HorizontalAlignment =HorizontalAlignment.Right,
                Fill = new SolidColorBrush() { Color = Color.FromArgb(255, 0, 0, 255) }
            }, gdMap, 0);
            flag = true;//已初始化标志
            #endregion
            //if ((sender as Button).Content.ToString() == "开始游戏")
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
