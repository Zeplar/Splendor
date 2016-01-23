using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor
{
    public static class RecordHistory
    {
        public enum actions { BUY, RESERVE, RESERVETOP, TAKEGEMS };
        const string directory = @"..\..\..\..\Splendor\History\";
        const string suffix = @".csv";
        const string name = @"game";
        static StreamWriter file;
        static bool init;
        static string plottingDirectory;

        public static void initialize()
        {
            if (!GameController.recording)
            {
                return;
            }

            //Create the game file
            Debug.Assert(Directory.Exists(directory));
            int i = 0;
            while (File.Exists(directory + name + i + suffix))
            {
                i++;
            }

            file = new StreamWriter(directory + name + i + suffix);
            file.AutoFlush = true;
            //Record the starting deck orders
            foreach (Deck d in GameController.decks)
            {
                foreach (Card c in d.getAllCards())
                {
                    file.Write(c.id + ",");
                }
                file.WriteLine();
            }
            init = true;
        }

        /// <summary>
        /// Records the board and current player state.
        /// </summary>
        public static void record()
        {
            if (!GameController.recording)
            {
                return;
            }
            file.Write("Board State: ");
            foreach (Card c in GameController.boardCards)
            {
                file.Write(c + ", ");
            }
            file.WriteLine(" Gems: " + Gem.board);
            file.WriteLine(GameController.currentPlayer + " gems: " + GameController.currentPlayer.gems);
            file.Write(GameController.currentPlayer + " has colors " + GameController.currentPlayer.discount);
            file.WriteLine();
        }

        public static void record(string s)
        {
            if (!GameController.recording) {
                return;
            }

            Debug.Assert(init, "File not initialized");
            file.WriteLine(s);
        }

        public static void record(actions a, Card c)
        {
            if (!GameController.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.WriteLine(a + "," + c);
        }

        public static void record(actions a, Gem g)
        {
            if (!GameController.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.WriteLine(a + "," + g);
        }

        public static void close()
        {
            if (!GameController.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.Close();
            init = false;
        }

        public static void plot(string info)
        {
            if (plottingDirectory == null)
            {
                string folder = @"C:\Users\JHep\Desktop\SelfishGenePlots\" + DateTime.Today.ToLongDateString();
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                int i = 0;
                plottingDirectory = folder + @"\plot_0.csv";
                while (File.Exists(plottingDirectory))
                {
                    i++;
                    plottingDirectory = folder + @"\plot_" + i + ".csv";
                }
            }
            File.AppendAllText(plottingDirectory, info);
        }

        public static void plot<T>(IEnumerable<T> list)
        {
            int i = 0;
            foreach (T x in list)
            {
                plot(i + "," + x.ToString() + Environment.NewLine);
                i++;
            }
        }

        public static void clearPlot()
        {
            plottingDirectory = null;
        }


        public static void recordWins(string winner, string loser, int winscore, int losescore)
        {
            File.AppendAllText(directory + name + suffix, winner + "," + winscore + "," + loser + "," + losescore + "\n");
        }

    }
}
