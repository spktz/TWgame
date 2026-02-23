using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using TankWarsGame.View.Pages;

namespace TankWarsGame
{

    public class EnemyController
    {
        
        private readonly Canvas _canvas;
        private readonly Image _view;

        private readonly EnemyTank _enemyTank;
        private readonly Image _playerImg;

        private readonly BulletController _bullet;
        private readonly CollisionCheck _collision;
        private readonly Random _rndm = new Random();

        private Direction _dir;
        private double _speed;

        private bool canshoot = true;

        private readonly Rectangle _hpBarBG;
        private readonly Rectangle _hpBarFill;

        private readonly double _detectionR;
        private readonly double _changeWayChance;

        private DateTime _lastShot = DateTime.MinValue;
        private static readonly TimeSpan ShotCD = TimeSpan.FromMilliseconds(500);

        public EnemyController(Canvas canvas, Image view, EnemyTank model,
            BulletController bullets,CollisionCheck collision, Image playerImg, double speed = 1.5, double detectionR = 120, double changeWayChance= 0.02)
        {
            _canvas = canvas;
            _enemyTank = model;
            _bullet = bullets;
            _collision = collision;
            _speed = speed;
            _playerImg = playerImg;
            _view = view;
            _detectionR = detectionR;
            _changeWayChance = changeWayChance;

            _dir = (Direction)_rndm.Next(0, 4); //рандом напрям для початку

            int maxLives = (_enemyTank.Type == EnemyType.Heavy) ? 2 : 1;
            _hpBarBG = new Rectangle
            {
                Width = _view.Width,
                Height = 4,
                Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                RadiusX = 2,
                RadiusY = 2
            };


            _hpBarFill = new Rectangle
            {
                Width = _view.Width,
                Height = 4,
                Fill = Brushes.LimeGreen,
                RadiusX = 2,
                RadiusY = 2
            };

            _canvas.Children.Add(_hpBarBG);
            _canvas.Children.Add(_hpBarFill);


            CompositionTarget.Rendering += OnRenderingEnemy;

        }
       
        public void StopShoot() => canshoot = false;
        public void ResumeShoot() => canshoot = true;

        private void OnRenderingEnemy(object sender, EventArgs e)
        {
            if (_view.Visibility != Visibility.Visible)
            {
                CompositionTarget.Rendering -= OnRenderingEnemy;

                _canvas.Children.Remove(_hpBarBG);
                _canvas.Children.Remove(_hpBarFill);
                return;
            }


            double px = Canvas.GetLeft(_playerImg); 
            double py = Canvas.GetTop(_playerImg);  
            double ex = _enemyTank.X;
            double ey = _enemyTank.Y;
            double distanceToPlayer = Math.Sqrt((px - ex) * (px - ex) + (py - ey) * (py - ey));

            if (distanceToPlayer < _detectionR)
            {
                if (Math.Abs(px - ex) > Math.Abs(py - ey))
                {
                    _dir = (px - ex > 0) ? Direction.Right : Direction.Left;
                }
                else
                {
                    _dir = (py - ey > 0) ? Direction.Down : Direction.Up;
                }
            }
            else
            {
                if (_rndm.NextDouble() < _changeWayChance)
                {
                    _dir = (Direction)_rndm.Next(0, 4);
                }
            }
        

            double dx = 0, dy = 0;
            switch (_dir)
            {
                case Direction.Up: 
                    dy = -_speed; break;
                case Direction.Down: 
                    dy = _speed; break;
                case Direction.Left: 
                    dx = -_speed; break;
                case Direction.Right: 
                    dx = _speed; break;
            }

           
            double newX = _enemyTank.X + dx;  //пробна позиція
            double newY = _enemyTank.Y + dy;
            var rect = new Rect(newX, newY, _view.Width, _view.Height);

           
            if (_collision.CanMove(rect, _view))  //перевірка колізії обєктів
            {
                _enemyTank.X = newX;
                _enemyTank.Y = newY;
                Canvas.SetLeft(_view, newX);
                Canvas.SetTop(_view, newY);

            }
            else
                if (distanceToPlayer >= _detectionR)
            {
                _dir = (Direction)_rndm.Next(0, 4);
            }

            double angle = TankController.DirectionTank(_dir);
            _view.RenderTransform = new RotateTransform(angle);

            bool playerDetect = distanceToPlayer < _detectionR;

            bool randomShot = false;
            if (!playerDetect && _rndm.NextDouble() < 0.01)
                randomShot = true;

            if (canshoot && (playerDetect || randomShot))
            {
                DateTime now = DateTime.UtcNow;
                if (now - _lastShot >= _enemyTank.ShotCooldown)
                {
                    double cx = _enemyTank.X + _view.Width / 2;
                    double cy = _enemyTank.Y + _view.Height / 2;
                    var md = randomShot ? new MoveDirection(_dir) : new MoveDirection(_dir);
                    _bullet.Fire(cx, cy, md, _enemyTank);
                    _lastShot = now;
                }
            }

            UpdateHealthBar();


        }

        private void UpdateHealthBar()
        {
            double ex = Canvas.GetLeft(_view);
            double ey = Canvas.GetTop(_view);

            //хелсбар 8пкс вище від спрайтв
            Canvas.SetLeft(_hpBarBG, ex);
            Canvas.SetTop(_hpBarBG, ey - 8);

            Canvas.SetLeft(_hpBarFill, ex);
            Canvas.SetTop(_hpBarFill, ey - 8);

            int maxLives = (_enemyTank.Type == EnemyType.Heavy) ? 2 : 1;
            double widthPerLife = _view.Width / maxLives;
            double fillWidth = widthPerLife * _enemyTank.Lives;
            _hpBarFill.Width = fillWidth;
        }

        public void Pause() => CompositionTarget.Rendering -= OnRenderingEnemy;
        public void Resume() => CompositionTarget.Rendering += OnRenderingEnemy;

    }
}
