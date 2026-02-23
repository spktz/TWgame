using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xaml;

namespace TankWarsGame
{
    public class MapRender
    {
        private readonly Canvas _canvas;
        private readonly int[,] _map;
        private readonly double _tileSize;
        private readonly List<Wall> _walls = new List<Wall>();

       
        public MapRender(Canvas canvas, int[,] map, double tileSize)
        {
            _canvas = canvas;
            _map = map;
            _tileSize = tileSize;
        }

       
        
        public void RenderMap()
        {
            int rows = _map.GetLength(0);
            int cols = _map.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int tile = _map[r, c];
                    if (tile == 0) continue;

                    double x = c * _tileSize;
                    double y = r * _tileSize;

                    
                    TypeWall type = tile == 1 ? TypeWall.Iron : TypeWall.Brick;
                    var wall = new Wall(x, y, type);

                    _canvas.Children.Add(wall.Img);
                    _walls.Add(wall);
                }
            }
        }

        public List<Wall> Walls => _walls;
    }
}
