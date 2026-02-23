using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Xaml;
using TankWarsGame.View.Pages;

namespace TankWarsGame
{
    public class BulletController
    {
        public event Action PlayerHit;
        public event Action<int> EnemyKilled;

        private readonly Canvas _canvas;
        private readonly List<Bullet> _bullets = new List<Bullet>();
        private readonly List<Wall> _walls;

        private readonly Image _playerImg;
        private readonly Tank _playerModel;
        private readonly List<Enemy> _enemies;



        public BulletController(Canvas canvas, List<Wall> walls, Image playerImg, Tank playerModel, List<Enemy> enemies)
        {
            _canvas = canvas;
            _walls = walls;
            _playerImg = playerImg;
            _playerModel = playerModel;
            _enemies = enemies;

            CompositionTarget.Rendering += OnRendering;
        }

        private Image CreateBullet(double x, double y, MoveDirection dir)
        {
            Image img = new Image
            {
                Width = 8,
                Height = 8,
                Source = new BitmapImage(new Uri("/Assets/bullet.png", UriKind.Relative)),
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
            };

            double angle;
            switch (dir.D)
            {
                case Direction.Up: angle = 0; break;
                case Direction.Right: angle = 90; break;
                case Direction.Down: angle = 180; break;
                case Direction.Left: angle = 270; break;
                default: angle = 0; break;
            }

            img.RenderTransform = new RotateTransform(angle);
            Canvas.SetLeft(img, x - img.Width / 2);
            Canvas.SetTop(img, y - img.Height / 2);
            return img;
        }
        public void Fire(double x, double y, MoveDirection dir, Tank ownerPlayer)
        {
            Image img = CreateBullet(x, y, dir);

            _canvas.Children.Add(img);
            _bullets.Add(new Bullet(x, y, dir, img, ownerPlayer));
        }

        public void Fire(double x, double y, MoveDirection dir, EnemyTank ownerEnemy)
        {
            Image img = CreateBullet(x, y, dir);

            _canvas.Children.Add(img);
            _bullets.Add(new Bullet(x, y, dir, img, ownerEnemy));
        }

        public void Pause() => CompositionTarget.Rendering -= OnRendering;

        public void Resume() => CompositionTarget.Rendering += OnRendering;


        private void OnRendering(object sender, EventArgs e)
        {
            foreach (var bullet in _bullets.ToArray())
            {
                if (!bullet.IsActive)
                    continue;

                bullet.Update();

                double bullX = bullet.X;
                double bullY = bullet.Y;
                double bullwidth = bullet.Img.Width;
                double bullhght = bullet.Img.Height;

                if (bullet.X < 0 || bullet.Y < 0 //перевірка виходу за межі канвм
                 || bullet.X > _canvas.ActualWidth
                 || bullet.Y > _canvas.ActualHeight)
                {
                    bullet.Deactivate();
                    continue;
                }

                bool hitSmth = false;

                foreach (var wall in _walls) //перевірка на зіткнення стін
                {
                    if (wall.IsDestroyed)
                        continue;

                    double wx = Canvas.GetLeft(wall.Img);
                    double wy = Canvas.GetTop(wall.Img);
                    double ww = wall.Img.Width;
                    double wh = wall.Img.Height;

                    // перевірка коорд кулі та стіни
                    bool isHitWall = bullX < wx + ww && bullX + bullwidth > wx
                                   && bullY < wy + wh && bullY + bullhght > wy;

                    if (!isHitWall)
                        continue;

                    bullet.Deactivate(); //куля потрапила і зникає

                    double centerX = wx + ww / 2;
                    double centerY = wy + wh / 2;
                    new Explosion(_canvas, centerX, centerY);
                    if (wall.Type == TypeWall.Brick) //  цегла
                        wall.TakeHit();
                    hitSmth = true;
                    break;
                }
                if (hitSmth) continue;

                if (bullet.OwnerEnemy != null)
                {
                    double px = Canvas.GetLeft(_playerImg);
                    double py = Canvas.GetTop(_playerImg);
                    double pW = _playerImg.Width;
                    double pH = _playerImg.Height;

                    bool hitPlayer = bullX < px + pW
                                  && bullX + bullwidth > px
                                  && bullY < py + pH
                                  && bullY + bullhght > py;

                    if (hitPlayer)
                    {
                        bullet.Deactivate();
                        double centerX = px + pW / 2;
                        double centerY = py + pH / 2;
                        new Explosion(_canvas, centerX, centerY);

                        _canvas.Children.Remove(bullet.Img);
                        _bullets.Remove(bullet);

                        _playerModel.TakeDamage();
                        PlayerHit.Invoke();

                        continue;
                    }
                }

                if (bullet.OwnerEnemy == null)
                    foreach (var enemy in _enemies)
                    {
                        if (enemy.Img.Visibility != Visibility.Visible)
                            continue;

                        if (bullet.OwnerEnemy == enemy.Model) //пропуск ітерації якщо куля власника
                            continue;

                        double ex = Canvas.GetLeft(enemy.Img);
                        double ey = Canvas.GetTop(enemy.Img);
                        double eW = enemy.Img.Width;
                        double eH = enemy.Img.Height;

                        bool hitEnemy = bullX < ex + eW
                                         && bullX + bullwidth > ex
                                         && bullY < ey + eH
                                         && bullY + bullhght > ey;

                        if (!hitEnemy)
                            continue;


                        bullet.Deactivate();//куля влучила в бота

                        double centerX = ex + eW / 2;
                        double centerY = ey + eH / 2;
                        new Explosion(_canvas, centerX, centerY);

                        enemy.Model.TakeDamage();
                        if (enemy.Model.Lives <= 0)
                        {
                            enemy.Img.Visibility = Visibility.Collapsed; //приховуємо спрайт
                            EnemyKilled.Invoke(100);
                        }
                        break;
                    }

            }
        }
    }
}



