﻿using System.Windows.Shapes;

namespace FlightChess
{
    public class Player
    {
        public Ellipse PlayerUI { get; set; }
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// 玩家位置
        /// </summary>
        public int PlayerPo { get; set; }
        /// <summary>
        /// 是否可掷色子的标志，0可，1不可
        /// </summary>
        public int Flag { get; set; }
    }
}
