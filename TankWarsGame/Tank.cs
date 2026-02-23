using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankWarsGame
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Tank
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Direction Direction { get; set; } = Direction.Up;
        public int Lives { get; set; }
        public double Speed { get; set; } = 2.0;

        public Tank(double x, double y, int lives = 3 )
        {
            X = x;
            Y = y;
            Lives = lives;
        }

        public void SetDirection(Direction dir)
        {
            Direction = dir;
        }

        public void MoveForward()
        {
            switch (Direction)
            {
                case Direction.Up: Y -= Speed; break;
                case Direction.Down: Y += Speed; break;
                case Direction.Left: X -= Speed; break;
                case Direction.Right: X += Speed; break;
            }
        }

        public void TakeDamage(int amount = 1)
        {
            Lives = Math.Max(0, Lives - amount);
        }
    }
}
