using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace TankWarsGame
{
    public enum TypeWall
    {
        Brick,
        Iron
    }

    public class Wall
    {
        public double X { get; }
        public double Y { get; }
        public TypeWall Type { get; }
        public Image Img { get; }
        public bool IsDestroyed { get; private set; } = false;

        private int HitPoints;

        public Wall(double x, double y, TypeWall type)
        {
            X = x;
            Y = y;
            Type = type;

            Img = new Image
            {
                Width = 20,
                Height = 20,
                Source = new BitmapImage(new Uri(
                    $"pack://application:,,,/Assets/{(type == TypeWall.Brick ? "brickwall.png" : "ironwall.png")}"))
            };

            Canvas.SetLeft(Img, X);
            Canvas.SetTop(Img, Y);

            HitPoints = (type == TypeWall.Brick) ? 1 : int.MaxValue; // brick 1 попадання iron — нескінч
        }

        public void TakeHit()
        {
            if (Type == TypeWall.Iron) return;

            HitPoints--;
            if (HitPoints <= 0)
            {
                IsDestroyed = true;
                Img.Visibility = Visibility.Collapsed;
            }
        }
    }

}
