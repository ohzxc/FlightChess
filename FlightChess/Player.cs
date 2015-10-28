using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightChess
{
    class Player
    {
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// 玩家位置
        /// </summary>
        public int PlayerPo { get; set; }
        /// <summary>
        /// 是否可掷色子的标志
        /// </summary>
        public int Flag { get; set; }
    }
}
