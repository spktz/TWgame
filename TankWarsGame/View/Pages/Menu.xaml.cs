using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Drawing.Text;

namespace TankWarsGame.View.Windows
{
    
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
            this.Loaded += Menu_Loaded;
        }

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Properties.Contains("PlayerName"))
            {
                string saved = Application.Current.Properties["PlayerName"] as string;
                if (!string.IsNullOrWhiteSpace(saved))
                {
                    PlayerNameTextBox.Text = saved;
                }
                else
                {
                    PlayerNameTextBox.Text = "player1";
                }
            }
            else
            {
                PlayerNameTextBox.Text = "player1";
            }
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            string playerName = PlayerNameTextBox.Text;
            if (string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("Будь ласка, введіть ім'я гравця.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Application.Current.Properties["PlayerName"] = playerName;
            NavigationService.Navigate(new Pages.GamePage());
        }

        private void statsGame(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.StatsPage());
        }

        private void exitGame(object sender, RoutedEventArgs e)
        {
                Application.Current.Shutdown();
        }

        private void deleteStatsGame(object sender, RoutedEventArgs e)
        {
            GameStatsManager.ClearAll();
            MessageBox.Show("Статистика видалена", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PlayerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
