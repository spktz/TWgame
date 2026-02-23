using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TankWarsGame
{
    public struct MoveDirection
    {
        public Direction D { get; }
        public double dx { get; }
        public double dy { get; }

        public MoveDirection(Direction dir)
        {
            D = dir;
            switch (dir)
            {
                case Direction.Up:
                    dx = 0;
                    dy = -1;
                    break;
                case Direction.Down:
                    dx = 0;
                    dy = 1;
                    break;
                case Direction.Left:
                    dx = -1;
                    dy = 0;
                    break;
                case Direction.Right:
                    dx = 1;
                    dy = 0;
                    break;
                default:
                    dx = 0;
                    dy = 0;
                    break;
            }
        }

        public MoveDirection(double dx, double dy)
        {
            D = default;
            var len = Math.Sqrt(dx * dx + dy * dy);
            if (len > 0)
            {
                this.dx = dx / len;
                this.dy = dy / len;
            }
            else
            {
                this.dx = 0;
                this.dy = 0;
            }
        }
    }
    public class Bullet
    {
        public double X { get; set; }
        public double Y { get; set; }
        public MoveDirection Direction { get; }
        public double Speed { get; } = 8;

        public Image Img { get; }
        public bool IsActive { get; set; } = true;

        public EnemyTank OwnerEnemy { get; }
        public Tank PlayerOwnerOfBullet { get; }


        public Bullet(double x, double y, MoveDirection dir, Image img, EnemyTank ownerEnemy)
        {
            X = x;
            Y = y;
            Direction = dir;
            Img = img;
            OwnerEnemy = ownerEnemy;

        }
        public Bullet(double x, double y, MoveDirection dir, Image img, Tank ownerPlayer)
        {
            X = x;
            Y = y;
            Direction = dir;
            Img = img;
            PlayerOwnerOfBullet = ownerPlayer;
        }

        public void Update()
        {
            if (!IsActive) return;
            X += Direction.dx * Speed;
            Y += Direction.dy * Speed;
            Canvas.SetLeft(Img, X - Img.Width / 2);
            Canvas.SetTop(Img, Y - Img.Height / 2);
        }

        public void Deactivate()
        {
            IsActive = false;
            Img.Visibility = Visibility.Collapsed;
        }
    }
}
