using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TankWarsGame
{
    public static class GameStatsManager
    {
        private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stats.json"); //директорія де запущений ехефайл
        private static GameStats Stats;
        public static List<GameStatsInfo> LoadAll()
        {
            if (Stats != null)
                return Stats.Info;

            if (!File.Exists(path))
            {
                Stats = new GameStats();
                return Stats.Info;
            }

            try
            {
                string json = File.ReadAllText(path);
                Stats = JsonConvert.DeserializeObject<GameStats>(json)
                               ?? new GameStats(); //якщо десеріалізація не вдалася створюємо порожній gstats
            }
            catch
            {
                Stats = new GameStats();
            }

            return Stats.Info;
        }

        public static void AddStats(GameStatsInfo st)
        {
            var list = LoadAll();
            list.Add(st);
            SaveAll();
        }

        public static void SaveAll()
        {
            if (Stats == null)
                Stats = new GameStats();

                var formatting = Formatting.Indented; //відступи
                string json = JsonConvert.SerializeObject(Stats, formatting);
                File.WriteAllText(path, json); // запис у файл
        }

        public static void ClearAll()
        {
            Stats = new GameStats();
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }
    }
}
