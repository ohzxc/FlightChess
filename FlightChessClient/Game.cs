using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace FlightChessClient
{
    public static class Game
    {
        /// <summary>
        /// 游戏逻辑
        /// </summary>
        /// <param name="map">地图数据</param>
        /// <param name="currentPlayer">本轮玩家</param>
        /// <param name="anotherPlayer">另一位玩家</param>
        /// <param name="num">骰子数</param>
        public static string PlayGame(Map map, Player currentPlayer, Player anotherPlayer, int num)
        {
            Game.PlayerMove(currentPlayer, num);
            var result = string.Empty;
            if (currentPlayer.PlayerPo == 99)
            {
                MessageBox.Show(currentPlayer.PlayerName + "赢了，游戏结束");//赢了
                return currentPlayer.PlayerName + "赢了，游戏结束\n";
            }
            if (currentPlayer.PlayerPo == anotherPlayer.PlayerPo && currentPlayer.PlayerPo != 0)
            {
                Game.PlayerMove(anotherPlayer, -6);
                //PlayGame(map, anotherPlayer, currentPlayer, 0);
                result += (anotherPlayer.PlayerName + "被" + currentPlayer.PlayerName + "踩了，后退6格！\n");
            }
            switch (map.Maps[currentPlayer.PlayerPo])
            {
                case 1:
                    result += Luckyturn(currentPlayer, anotherPlayer);
                    break;
                case 2:
                    result += LandMine(currentPlayer);
                    //PlayGame(map, currentPlayer, anotherPlayer, 0);
                    break;
                case 3:
                    result += Pause(currentPlayer);
                    break;
                case 4:
                    result += TimeTunnel(currentPlayer);
                    //PlayGame(map, currentPlayer, anotherPlayer, 0);
                    break;
                default: result += currentPlayer.PlayerName + "踩到方块。"; break;
            }
            if (currentPlayer.PlayerPo == 99)
            {
                MessageBox.Show(currentPlayer.PlayerName + "赢了，游戏结束");//赢了
                return result + currentPlayer.PlayerName + "赢了，游戏结束\n";
            }
            return result ;
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
        /// 玩家总步数更新
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="steps">所在步数</param>
        /// <returns></returns>
        public static void PlayerMoveExt(Player player, int steps)
        {
            player.PlayerPo = steps;
            if (player.PlayerPo < 0)
                player.PlayerPo = 0;
            if (player.PlayerPo > 99)
                player.PlayerPo = 99;
            var Pos = Map.Num2Po(steps);
            Grid.SetColumn(player.PlayerUI, Pos[1]);
            Grid.SetRow(player.PlayerUI, Pos[0]);
        }

        /// <summary>
        /// 幸运转轮
        /// </summary>
        /// <param name="currentPlayer">该轮玩家</param>
        /// <param name="anotherPlayer">另一个玩家</param>
        /// <returns></returns>
        public static string Luckyturn(Player currentPlayer, Player anotherPlayer)
        {
            if (MessageBox.Show("选是轰炸玩家，对方退6格，选否玩家交换位置", "请选择操作", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                Game.PlayerMove(anotherPlayer, -6);
                return (currentPlayer.PlayerName + "踩到幸运转轮，并且选择了轰炸玩家" + anotherPlayer.PlayerName + "。");
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
                return (currentPlayer.PlayerName + "踩到幸运转轮，并且选择了交换位置。");
            }
        }

        /// <summary> 
        /// 地雷
        /// </summary>
        /// <param name="player">遭遇地雷的玩家</param>
        public static string LandMine(Player player)
        {
            Game.PlayerMove(player, 0);
            MessageBox.Show("玩家踩到地雷！退6格");
            Game.PlayerMove(player, -6);
            return (player.PlayerName + "踩到地雷，后退6格。");
        }

        /// <summary>
        /// 暂停一局
        /// </summary>
        /// <param name="player">玩家</param>
        public static string Pause(Player player)
        {
            player.Flag++;
            MessageBox.Show("踩到暂停");
            return (player.PlayerName + "踩到暂停。");
        }

        /// <summary>
        /// 时空隧道
        /// </summary>
        /// <param name="player">玩家</param>
        public static string TimeTunnel(Player player)
        {
            Game.PlayerMove(player, 0);
            MessageBox.Show("玩家进入时空隧道，前进10格！");
            Game.PlayerMove(player, 10);
            return (player.PlayerName + "进入是空隧道,前进10格。");
        }

        /// <summary>
        /// 判断谁走，true1走，false2走
        /// </summary>
        /// <param name="_Player1">玩家一</param>
        /// <param name="_Player2">玩家二</param>
        /// <returns></returns>
        public static Player CompareFlag(Player _Player1, Player _Player2)
        {
            if (_Player2.Flag + _Player1.Flag > 2)
            {
                var tmp = (_Player1.Flag < _Player1.Flag) ? (_Player2.Flag - _Player1.Flag) : (_Player1.Flag - _Player2.Flag);
                _Player1.Flag = (_Player1.Flag < _Player1.Flag) ? 0 : 1;
                _Player2.Flag = (_Player1.Flag == 0) ? 1 : 0;
            }
            return (_Player1.Flag < _Player2.Flag) ? _Player1 : _Player2;
            //anotherPlayer = (currentPlayer == _Player1) ? _Player2 : _Player1;
        }
    }
}
