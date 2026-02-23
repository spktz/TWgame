using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Runtime.InteropServices;

namespace TankWarsGame
{

    public class TankController
    {
        private readonly Image _tankImg;
        private readonly Canvas _canvas;
       
        private readonly Tank _tank;
        private readonly double _speed = 2;

        private readonly BulletController _bulletController;

        private bool _up, _down, _left, _right;
        private Direction _direction = Direction.Up;

        private DateTime _lastShot = DateTime.MinValue;
        private static readonly TimeSpan ShotCD = TimeSpan.FromMilliseconds(500);

        private readonly List<Wall> _walls;
      
        private CollisionCheck _collision;
        private readonly List<Image> _enemyImages;


        public TankController(Canvas canvas, Image img, BulletController bullet, Tank tank, List<Wall> walls, CollisionCheck collision, List<Image> enemyImages)
        {
            _canvas = canvas;
            _tankImg = img;
            _tank = tank;
            _bulletController = bullet;
            _walls = walls;
            _collision = collision;
            _enemyImages = enemyImages;


            _canvas.PreviewKeyDown += OnKeyDown;
            _canvas.PreviewKeyUp += OnKeyUp;
            _canvas.Loaded += (sender, e) =>
            {
                Keyboard.Focus(_canvas);
            };


            CompositionTarget.Rendering += OnRenderingPlayerTank;
        }
        public void Pause()
        {
            CompositionTarget.Rendering -= OnRenderingPlayerTank;
        }
        public void Resume()
        {
            CompositionTarget.Rendering += OnRenderingPlayerTank;
        }





        private void FireBullet()
        {
            double x = Canvas.GetLeft(_tankImg) + _tankImg.Width / 2;
            double y = Canvas.GetTop(_tankImg) + _tankImg.Height / 2;

            MoveDirection dir = new MoveDirection(_direction);

            _bulletController.Fire(x, y, dir, _tank);
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                return;

            switch (e.Key)
            {
                case Key.W: 
                    _up = true;
                    break;
                case Key.S: 
                    _down = true;
                    break;
                case Key.A: 
                    _left = true;
                    break;
                case Key.D: 
                    _right = true;
                    break;
            }

            if (e.Key == Key.Space)
            {
                var now = DateTime.UtcNow;
                if (now - _lastShot >= ShotCD) // перевірка кд
                {
                    FireBullet();
                    _lastShot = now;
                }
            }
            e.Handled = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W: _up = false; break;
                case Key.S: _down = false; break;
                case Key.A: _left = false; break;
                case Key.D: _right = false; break;
            }
            e.Handled = true;
        }

        private void OnRenderingPlayerTank(object sender, EventArgs e)
        {
            double dx = 0, dy = 0;


            if (_up && !_down)
            {
                _direction = Direction.Up;
                _tank.SetDirection(_direction);
                
                dy = -_speed;
            }


            else if (_down && !_up)
            {
                _direction = Direction.Down;
                _tank.SetDirection(_direction);
                
                dy = _speed;
            }


            else if (_left && !_right)
            {
                _direction = Direction.Left;
                _tank.SetDirection(_direction);
                
                dx = -_speed;
            }


            else if (_right && !_left)
            {
                _direction = Direction.Right;
                _tank.SetDirection(_direction);
                
                dx = _speed;
            }


            if (dx != 0 || dy != 0)
            {
               
                double newX = _tank.X + dx;
                double newY = _tank.Y + dy;

                // рект танка після руху
                var tankRect = new Rect(newX, newY, _tankImg.Width, _tankImg.Height);

               
                if (_collision.CanMove(tankRect, _tankImg))
                {
                    _tank.X = newX;
                    _tank.Y = newY;
                    Canvas.SetLeft(_tankImg, newX);
                    Canvas.SetTop(_tankImg, newY);
                }
            }
            Canvas.SetLeft(_tankImg, _tank.X);
            Canvas.SetTop(_tankImg, _tank.Y);


            var angle = DirectionTank(_direction);

            _tankImg.RenderTransform = new RotateTransform(angle);
        }

        public static double DirectionTank(Direction d)
        {
            switch (d)
            {
                case Direction.Up: return 0;
                case Direction.Right: return 90;
                case Direction.Down: return 180;
                case Direction.Left: return 270;
                default: return 0;
            }
        }
    }
}
