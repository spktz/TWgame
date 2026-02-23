using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TankWarsGame
{
    public class EnemySpawn
    {
        private readonly Canvas _canvas;
        private readonly BulletController _bulletController;
        private readonly CollisionCheck _collision;
        private readonly Image _playerImg;

        private readonly List<Enemy> _enemies;
        private readonly List<Image> _allTankImages;

        private readonly double _enemyWidth;
        private readonly double _enemyHeight;
        private readonly double _enemySpeed;

        private readonly Random random = new Random();
        private DispatcherTimer _spawnTimer;

        private bool _isPaused = false;
        private List<(double x, double y)> _spawnpoints;
        private int _remainingToSpawn;
        private int Count;

        private int _currWave = 1;
        private readonly HashSet<(double x, double y)> _occupiedSpawns = new HashSet<(double x, double y)>();

        public EnemySpawn(Canvas canvas, BulletController bulletController, 
            CollisionCheck collision, List<Enemy> enemies, List<Image> allTankImages, Image playerImg,
            double enemyWidth, double enemyHeight, double enemySpeed)
        {
            _canvas = canvas;
            _bulletController = bulletController;
            _collision = collision;
            _allTankImages = allTankImages;
            _enemies = enemies;
            _playerImg = playerImg;
            _enemyWidth = enemyWidth;
            _enemyHeight = enemyHeight;
            _enemySpeed = enemySpeed;

            _spawnTimer = new DispatcherTimer();
            _spawnTimer.Tick += OnSpawnTimerTick;
        }

       
        public void AddEnemy(double x, double y)
        {
            EnemyType typeToSpawn = TypeByWave(_currWave);
            EnemyTank enemyModel = new EnemyTank(x, y, typeToSpawn);
  
            Image img = new Image
            {
                Width = _enemyWidth,
                Height = _enemyHeight,  
                Source = new BitmapImage(new Uri("/Assets/enemyTank.png", UriKind.Relative)),
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
            };

            Canvas.SetLeft(img, x);
            Canvas.SetTop(img, y);
            _canvas.Children.Add(img);

            double speed = (_currWave <= 2) ? 1.3 : (_currWave <= 4) ? 1.6 : 1.9;
            double detectionRadius = (_currWave < 4) ? 100 : 150;
            double wanderChance = (_currWave < 4) ? 0.02 : 0.008;

            EnemyController controller = new EnemyController(_canvas, img, enemyModel, _bulletController, _collision, _playerImg, speed, detectionRadius,
            wanderChance);

            Enemy enemy = new Enemy(img, enemyModel, controller);
            _enemies.Add(enemy);
            _allTankImages.Add(img);

            enemyModel.PreviousSpawnX = x;
            enemyModel.PreviousSpawnY = y;

        }

        public void StartSpawn(List<(double x, double y)> spawnpoints, int cnt, TimeSpan spawnInterval, int waveNum)
        {
            if (spawnpoints == null || spawnpoints.Count == 0 || cnt <= 0)
                return;

            _spawnpoints = spawnpoints;
            _remainingToSpawn = cnt;
            _currWave = waveNum;
            Count = cnt;

            //інтервал таймера та запуск
            _spawnTimer.Interval = spawnInterval;
            _spawnTimer.Start();
        }

        public int RemainingEnemies => _remainingToSpawn;

        private EnemyType TypeByWave(int wave)
        {
            return (wave >= 4) ? EnemyType.Heavy : EnemyType.Basic;
        }

        public void StopSpawn()
        {
            _spawnTimer.Stop();
            _remainingToSpawn = 0;
        }

        private void OnSpawnTimerTick(object sender, EventArgs e)
        {
            if (_remainingToSpawn <= 0)
            {
                _spawnTimer.Stop();
                return;
            }

            //знайти вільну точку спавну
            const int m_att = 10;

            for (int attempt = 0; attempt < m_att; attempt++)
            {
                int idx = random.Next(_spawnpoints.Count);
                var (x, y) = _spawnpoints[idx];

                double px = Canvas.GetLeft(_playerImg);
                double py = Canvas.GetTop(_playerImg);
                double dist = Math.Sqrt((x - px) * (x - px) + (y - py) * (y - py));

                if (dist < 100)
                    continue;


                var tempRect = new Rect(x, y, _enemyWidth, _enemyHeight);
                if (_collision.CanMove(tempRect, null))
                {
                    AddEnemy(x, y);
                    _remainingToSpawn--;
                    break;
                }
            }
        }

        public void Pause()
        {
            if (_spawnTimer != null && _spawnTimer.IsEnabled)
            {
                _spawnTimer.Stop();
                _isPaused = true;
            }
        }

        public void Resume()
        {
            if (_spawnTimer != null && _isPaused)
            {
                _spawnTimer.Start();
                _isPaused = false;
            }
        }

    }
}
