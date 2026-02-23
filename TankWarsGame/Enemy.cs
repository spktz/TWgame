using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TankWarsGame
{
    public class Enemy
    {
        public Image Img { get; }
        public EnemyTank Model { get; }
        public EnemyController Controller { get; }

        public Enemy(Image img, EnemyTank model, EnemyController controller)
        {
            Img = img;
            Model = model;
            Controller = controller;
        }
    }
}
