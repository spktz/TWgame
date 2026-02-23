using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TankWarsGame
{
    public enum EnemyType
    {
        Basic,    
        Heavy
    }
    public class EnemyTank : Tank
    {
        public TimeSpan ShotCooldown { get; private set; }
        public EnemyType Type { get; set; }
        public double PreviousSpawnX { get; set; }
        public double PreviousSpawnY { get; set; }

        public EnemyTank(double x, double y, EnemyType type = EnemyType.Basic)
            : base(x, y)
        {
            Type = type;
            switch (Type)
            {
                case EnemyType.Basic:
                    Lives = 1;
                    ShotCooldown = TimeSpan.FromMilliseconds(700);
                    break;
                case EnemyType.Heavy:
                    Lives = 2;
                    ShotCooldown = TimeSpan.FromMilliseconds(500);
                    break;
            }
        }

        public void TakeDamage()
        {
            Lives = Math.Max(0, Lives - 1);
        }
    }
}
