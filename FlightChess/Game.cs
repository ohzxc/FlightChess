using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace FlightChess
{
    public static class Game
    {
        public static void PlayGame(Map map,Player currentPlayer, Player anotherPlayer,int num)
        {
            Game.PlayerMove(currentPlayer, num);
            //currentPlayer.PlayerPo += num;
            if (currentPlayer.PlayerPo == 99)
            {
                MessageBox.Show(currentPlayer.PlayerName + "赢了，游戏结束");//赢了
                return;
            }
            if (currentPlayer.PlayerPo == anotherPlayer.PlayerPo&& currentPlayer.PlayerPo!=0)
            {   
                MessageBox.Show(anotherPlayer.PlayerName+"被"+currentPlayer.PlayerName+"踩了，后退6格！");
                Game.PlayerMove(anotherPlayer, -6);
                //PlayGame(map, anotherPlayer, currentPlayer, 0);
            }
            switch (map.Maps[currentPlayer.PlayerPo])
            {
                case 1:
                    Luckyturn(currentPlayer, anotherPlayer);
                    break;
                case 2:
                    LandMine(currentPlayer);
                    //PlayGame(map, currentPlayer, anotherPlayer, 0);
                    break;
                case 3:
                    Pause(currentPlayer);
                    break;
                case 4:
                    TimeTunnel(currentPlayer);
                    //PlayGame(map, currentPlayer, anotherPlayer, 0);
                    break;
                default: break;
            }
            
        }
        /// <summary>
        /// 玩家移动
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="steps">移动步数</param>
        /// <returns></returns>
        public static void PlayerMove(Player player, int steps)
        {
            player.PlayerPo += steps;
            if (player.PlayerPo < 0)
                player.PlayerPo = 0;
            if (player.PlayerPo > 99)
                player.PlayerPo = 99;

            var Pos = Map.Num2Po(player.PlayerPo);
            Grid.SetColumn(player.PlayerUI, Pos[1]);
            Grid.SetRow(player.PlayerUI, Pos[0]);
        }
        /// <summary>
        /// 幸运转轮
        /// </summary>
        /// <param name="currentPlayer">该轮玩家</param>
        /// <param name="anotherPlayer">另一个玩家</param>
        /// <returns></returns>
        public static void Luckyturn(Player currentPlayer,Player anotherPlayer)
        {
            if (MessageBox.Show("选是对方退6格，选否玩家交换位置", "请选择操作", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                Game.PlayerMove(anotherPlayer, -6);
                //Game.UpdateUiPo(anotherPlayer);
            }
            else
            {
                //交换位置
                var tmp = currentPlayer.PlayerPo;
                currentPlayer.PlayerPo = anotherPlayer.PlayerPo;
                anotherPlayer.PlayerPo = tmp;
                Game.PlayerMove(currentPlayer, 0);
                Game.PlayerMove(anotherPlayer, 0);
            }    
        }
        /// <summary>
        /// 地雷
        /// </summary>
        /// <param name="player">遭遇地雷的玩家</param>
        public static void LandMine(Player player)
        {
            Game.PlayerMove(player, 0);
            MessageBox.Show("玩家踩到地雷！退6格");
            Game.PlayerMove(player, -6);
        }
        /// <summary>
        /// 暂停一局
        /// </summary>
        /// <param name="player">玩家</param>
        public static void Pause(Player player)
        {
            player.Flag++;
        }
        /// <summary>
        /// 时空隧道
        /// </summary>
        /// <param name="player">玩家</param>
        public static void TimeTunnel(Player player)
        {
            Game.PlayerMove(player, 0);
            MessageBox.Show("玩家进入时空隧道，前进10格！");
            Game.PlayerMove(player, 10);
        }
        /// <summary>
        /// 踩人事件
        /// </summary>
        /// <param name="currentPlayer">踩人玩家</param>
        /// <param name="anotherPlayer">被踩玩家</param>
        /// <returns></returns>
        public static void SamePo(Player currentPlayer, Player anotherPlayer)
        {

        }
    }
}
