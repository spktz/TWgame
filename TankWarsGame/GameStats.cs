using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankWarsGame
{
    public class GameStats
    {
        public List<GameStatsInfo> Info { get; set; } = new List<GameStatsInfo>(); //навіть пристворенні нового списку він не буде нулл
    }
}
