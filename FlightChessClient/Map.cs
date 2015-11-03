using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace FlightChessClient
{
    public class Map
    {
        #region 变量
        /// <summary>
        /// 地图数据
        /// </summary>
        public int[] Maps = new int[100];
        /// <summary>
        /// 幸运轮盘
        /// </summary>
        public int[] luckyturn { get; set; }
        /// <summary>
        /// 地雷
        /// </summary>
        public int[] landMine { get; set; }
        /// <summary>
        /// 暂停
        /// </summary>
        public int[] pause { get; set; }
        /// <summary>
        /// 时空隧道
        /// </summary>
        public int[] timeTunnel { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 初始化地图数据
        /// </summary>
        public void InitailMap()
        {
            foreach(int o in luckyturn)
            {
                Maps[o] = 1;
            }
            foreach (int o in landMine)
            {
                Maps[o] = 2;
            }
            foreach (int o in pause)
            {
                Maps[o] = 3;
            }
            foreach (int o in timeTunnel)
            {
                Maps[o] = 4;
            }
        }
        /// <summary>
        /// 初始化地图界面
        /// </summary>
        /// <param name="mapdata">地图数据</param>
        /// <param name="o">填充地图的控件</param>
        public static void DrawMap(int[] mapdata, object o)
        {
            if (!(o is Grid)) return;
            var gdMap= o as Grid;
            var Pos = new int[2] { 0, 0 };
            
            for (var i = 0; i < 100; i++)
            {
                var con = new TextBox() {IsEnabled=false, Height = 46, Width = 46,HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center,TextWrapping=TextWrapping.Wrap };
                if (i == 0)
                {
                    con.Text = "起点";
                }
                else if (i == 99)
                {
                    con.Text = "终点";
                }
                switch (mapdata[i])
                {
                    case 1:
                        con.Text = "幸运转轮";
                        break;
                    case 2:
                        con.Text = "地雷";                       
                        break;
                    case 3:
                        con.Text = "暂停一轮";
                        break;
                    case 4:
                        con.Text = "时空隧道";
                        break;
                    default:break;
                }
                AddControls(con, gdMap, i);
            }
        }
        /// <summary>
        /// 将数字转化成坐标
        /// </summary>
        /// <param name="num">地图上的步数</param>
        public static int[] Num2Po(int num)
        {
            if((num / 10) % 2 == 0)//奇数行
            {
                return new int[2] { num/10, (num%10) };
            }
            return new int[2] { num / 10, 9 - (num % 10) };
        }
        /// <summary>
        /// 填充控件
        /// </summary>
        /// <param name="o">子控件</param>
        /// <param name="grid">父控件</param>
        /// <param name="i">位置</param>
        public static void AddControls(UIElement con,object grid,int i)
        {
            var gdMap= grid as Grid;
            //var con = o as TextBox;
            var Pos = Num2Po(i);
            Grid.SetColumn(con, Pos[1]);
            Grid.SetRow(con, Pos[0]);
            gdMap.Children.Add(con);
        }
        #endregion
    }
}
