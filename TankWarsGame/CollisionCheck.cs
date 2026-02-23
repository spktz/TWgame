using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.CompilerServices;

namespace TankWarsGame
{
    public class CollisionCheck
    {
        private readonly List<Wall> _walls;
        private readonly List<Image> _allTankImages;
        
        public CollisionCheck(List<Wall> walls, List<Image> allTankImages)
        {
            _walls = walls;
            _allTankImages = allTankImages;
        }

            
        public bool CanMove(Rect rect, Image self)
        {
                
            foreach (var wall in _walls)
            {
                if (wall.IsDestroyed)
                continue;

                double wx = Canvas.GetLeft(wall.Img);
                double wy = Canvas.GetTop(wall.Img);
                Rect wallRect = new Rect(wx, wy, wall.Img.Width, wall.Img.Height);

                if (rect.IntersectsWith(wallRect))
                        return false;
            }


             foreach (var tankImg in _allTankImages)
              {   
                  if (tankImg == self) continue; //якщо це тей же спрайт - пропуск
                  if (tankImg.Visibility != Visibility.Visible) continue; //якщо танка уже немає
                  double tx = Canvas.GetLeft(tankImg);
                  double ty = Canvas.GetTop(tankImg);
                  Rect tankRect = new Rect(tx, ty, tankImg.Width, tankImg.Height);

                  if (rect.IntersectsWith(tankRect))
                  return false;
              }
            return true;
             
        }

    }
}
