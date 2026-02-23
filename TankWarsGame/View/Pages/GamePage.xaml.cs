using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TankWarsGame.View.Pages
{
    public partial class GamePage : Page
    {
        private int lives = 5;
        private int score = 0;
        private int enemiesKilled = 0;
        private int allEnemiesKilled = 0;

        private string playerName;
        private DispatcherTimer waveTimer;

        private Tank tankModel;
        private MapRender render;
        private List<Wall> walls;

        private List<Image> allTankImages;
        private List<Enemy> enemies;

        private BulletController bulletController;
        private TankController playerController;

        private CollisionCheck collision;
        private EnemySpawn enemySpawn;

        private bool _isPaused = false;
        private int _currentWaveTarget;

        private int _currentWave = 0;
        private const int _totalWaves = 5;

        private List<List<(double x, double y)>> waveSpawnPoints;
        private List<int> _waveNumberEnemy;

        public GamePage()
        {
            InitializeComponent();
            if (Application.Current.Properties.Contains("PlayerName"))
                playerName = Application.Current.Properties["PlayerName"] as string;
            else
                playerName = "player12";

            InitializeGame();
        }
        private void InitializeGame()
        {
            tankModel = new Tank(80, 200);

            int[,] levelMap = GenerateLevelMap();

            render = new MapRender(GameCanvas, levelMap, 20);
            render.RenderMap();
            walls = render.Walls;

            allTankImages = new List<Image>();
            enemies = new List<Enemy>();

            collision = new CollisionCheck(walls, allTankImages);

            bulletController = new BulletController(GameCanvas, render.Walls, tankPlayerImg, tankModel, enemies);
            playerController = new TankController(GameCanvas, tankPlayerImg, bulletController, tankModel, walls, collision, allTankImages);

            allTankImages.Add(tankPlayerImg);


            waveSpawnPoints = new List<List<(double x, double y)>>()
            {
                new List<(double x, double y)> { (233,432), (300,200), (500,100) },
                new List<(double x, double y)> { (50,  250), (350, 350), (550, 250) },
                new List<(double x, double y)> { (100,100), (300,200), (500,100), (100,300) },
                new List<(double x, double y)> { (150,150), (450,150), (150,350), (450,350) },
                new List<(double x, double y)> { (100,100), (300,200), (500,100), (200,400), (400,400) }
            };
            _waveNumberEnemy = new List<int> { 5, 8, 10, 12, 15 };

            StartNextWave();

            bulletController.PlayerHit += OnPlayerHit;
            bulletController.EnemyKilled += OnEnemyKilled;
        }

        private void StartNextWave()
        {
            _currentWave++;
            if (_currentWave > _totalWaves)
            {
                var entry = new GameStatsInfo
                {
                    PlayerName = playerName,
                    WavesComplete = _currentWave,
                    FinalScore = score,
                    TotalKills = allEnemiesKilled
                };
                GameStatsManager.AddStats(entry);

                VictoryOverlay.Visibility = Visibility.Visible;
                return;
            }


            foreach (var enemy in enemies.ToList())
            {
                enemy.Img.Visibility = Visibility.Collapsed;
                GameCanvas.Children.Remove(enemy.Img);
            }
            enemies.Clear();
            allTankImages.Clear();
            allTankImages.Add(tankPlayerImg);

            enemiesKilled = 0;
            _currentWaveTarget = _waveNumberEnemy[_currentWave - 1];

            WaveOverlay.Visibility = Visibility.Visible;
            WaveLabel.Text = $"Хвиля {_currentWave}/{_totalWaves}";


            waveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            waveTimer.Tick += (s, e) =>
            {
                WaveOverlay.Visibility = Visibility.Collapsed;
                waveTimer.Stop();

                //через 2 сек запуск спавн
                var points = waveSpawnPoints[_currentWave - 1];
                int count = _currentWaveTarget;
                TimeSpan interval = (_currentWave < 4) ? TimeSpan.FromSeconds(2) : TimeSpan.FromSeconds(1.5);

                enemySpawn = new EnemySpawn(GameCanvas, bulletController, collision, enemies,
                    allTankImages, tankPlayerImg, 33, 33, 1.6 );
                enemySpawn.StartSpawn(points, count, interval, _currentWave);
            };
            waveTimer.Start();
        }

        private void OnRenderingDisplay(object sender, EventArgs e)
        {
            UpdateHeartsDisplay();
            UpdateScoreDisplay();
        }

        private void OnPlayerHit()
        {
            if (lives > 0)
            {
                lives -= 1;
                UpdateHeartsDisplay();
            }

            if (lives == 0)
            {
                var entry = new GameStatsInfo
                {
                    PlayerName = playerName,
                    WavesComplete = _currentWave,
                    FinalScore = score,
                    TotalKills = allEnemiesKilled
                };
                GameStatsManager.AddStats(entry);

                tankPlayerImg.Visibility = Visibility.Collapsed;
                GameOverOverlay.Visibility = Visibility.Visible;

                playerController.Pause();
                bulletController.Pause();

                foreach (var enemy in enemies)
                    enemy.Controller.Pause();

                    enemySpawn.StopSpawn(); //після gameover заборона спавну  
            }
        }

       
        private void OnEnemyKilled(int points)
        {
            score += points;
            enemiesKilled++;
            allEnemiesKilled++;
            UpdateScoreDisplay();

            var dead = enemies.Where(e => e.Model.Lives <= 0).ToList();
            foreach (var enemy in dead)
            {
                GameCanvas.Children.Remove(enemy.Img);
                allTankImages.Remove(enemy.Img);
                enemy.Controller.Pause();
                enemies.Remove(enemy);
            }

            if (enemiesKilled >= _currentWaveTarget)
            {
                StartNextWave();
                UpdateScoreDisplay();
            }
        }


        private void UpdateHeartsDisplay()
        {
            if (lives <= 0)
                HeartsLabel.Text = string.Empty;
            else
                HeartsLabel.Text = string.Join(" ", Enumerable.Repeat("♡", lives));
        }

        private void UpdateScoreDisplay()
        {
            ScoreLabel.Text = $"Score: {score}";
            SpawnInfo.Text = $"Ворогів в цій хвилі: {_currentWaveTarget}, Вбито: {enemiesKilled} ";
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Windows.Menu());
        }


        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && WaveOverlay.Visibility != Visibility.Visible)
            {
                if (!_isPaused)
                    PauseGame();
                else
                    ResumeGame();

                e.Handled = true;
            }
        }

        private void PauseGame()
        {
            
                _isPaused = true;
                PauseOverlay.Visibility = Visibility.Visible;


                playerController.Pause();
                bulletController.Pause();

                foreach (var enemy in enemies)
                {
                    enemy.Controller.StopShoot();
                    enemy.Controller.Pause();
                    enemySpawn.Pause();
                }
        }

        private void ResumeGame()
        {
            _isPaused = false;

            PauseOverlay.Visibility = Visibility.Collapsed;

            playerController.Resume();
            bulletController.Resume();

            foreach (var enemy in enemies)
            {
                enemy.Controller.Resume();
                enemy.Controller.ResumeShoot();
                enemySpawn.Resume();
            }

            GameCanvasContainer.Focus();
            Keyboard.Focus(GameCanvas);
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e) => ResumeGame();
  
        private int[,] GenerateLevelMap()
        {
            const int size = 40;
            const int iron = 1;
            const int brick = 2;

            int[,] levelMap = new int[size, size];

            //рамка 
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                {
                    if (r < 2 || r >= size - 2 || c < 2 || c >= size - 2)
                        levelMap[r, c] = iron;
                }

            //центральні коридори
            for (int c = 2; c < size - 2; c++)
            {
                levelMap[19, c] = brick;
                levelMap[20, c] = brick;
            }
            for (int r = 2; r < size - 2; r++)
            {
                levelMap[r, 19] = brick;
                levelMap[r, 20] = brick;
            }

            //центр кімната 11×11
            for (int r = 14; r <= 25; r++)
            {
                levelMap[r, 14] = brick;
                levelMap[r, 25] = brick;
            }
            for (int c = 14; c <= 25; c++)
            {
                levelMap[14, c] = brick;
                levelMap[25, c] = brick;
            }

            //малі кімнати по кутах
            void BuildRoom(int top, int left)
            {
                int w = 6, h = 6;
                for (int r = top; r < top + h; r++)
                {
                    levelMap[r, left] = brick;
                    levelMap[r, left + w - 1] = brick;
                }
                for (int c = left; c < left + w; c++)
                {
                    levelMap[top, c] = brick;
                    levelMap[top + h - 1, c] = brick;
                }
            }
            BuildRoom(4, 4);                        
            BuildRoom(4, size - 4 - 6);            
            BuildRoom(size - 4 - 6, 4);              
            BuildRoom(size - 4 - 6, size - 4 - 6);

            // перешкоди
            levelMap[10, 10] = brick;
            levelMap[10, 11] = brick;
            levelMap[11, 10] = brick;
            levelMap[11, 11] = brick;

            levelMap[10, 28] = brick;
            levelMap[10, 29] = brick;
            levelMap[11, 28] = brick;
            levelMap[11, 29] = brick;

            levelMap[28, 10] = brick;
            levelMap[28, 11] = brick;
            levelMap[29, 10] = brick;
            levelMap[29, 11] = brick;

            levelMap[28, 28] = brick;
            levelMap[28, 29] = brick;
            levelMap[29, 28] = brick;
            levelMap[29, 29] = brick;

            return levelMap;
        }


    }
}
