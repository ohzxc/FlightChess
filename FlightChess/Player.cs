using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

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
        /// 是否可掷色子的标志
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 判断是否踩到了机关
        /// </summary>
        /// <param name="type">机关类型1：转轮，2：地雷，3：暂停，4：隧道</param>
        public void PlayGame(int type,out StringBuilder msg)
        {
            msg = default(StringBuilder);
            switch (type)
            {
                case 1: break;
                case 2: msg.Append("玩家"+PlayerName+"踩到了地雷退后4步。/n"); break;
                case 3: break;
                case 4: break;
                default:break;
            }
        }
    }
}
