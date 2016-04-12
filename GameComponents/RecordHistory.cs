using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace Splendor
{
    public class RecordHistory
    {
        public enum actions { BUY, RESERVE, RESERVETOP, TAKEGEMS };
        const string suffix = @".csv";
        const string name = @"game";
        StreamWriter file;
        public string directory { get
            {
                return getDirectory();
            } }
        string plottingDirectory;
        string snapshotDirectory;
        bool plotting;
        bool snapshotting;
        bool recording;

        public static RecordHistory current;

        public RecordHistory(bool record)
        {
            this.plotting = record;
            this.snapshotting = record;
            this.recording = record;
            if (!record) return;
            //Create the game file
            int i = 0;
            while (File.Exists(directory + name + i + suffix))
            {
                i++;
            }
            plottingDirectory = directory + "plot_" + i + suffix;
            snapshotDirectory = directory + "snapshot_" + i + suffix;

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
        }

        public void writeToFile(string file, string line)
        {
            lock (file)
            {
                File.AppendAllText(directory + @"\" + file, line + Environment.NewLine);
            }
        }

        /// <summary>
        /// Records the board and current player state.
        /// </summary>
        public void record()
        {
            if (!recording) return;
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

        public void record(string s)
        {
            if (!recording) return;
            file.WriteLine(s);
        }

        public void record(actions a, Card c)
        {
            if (!recording) return;
            file.WriteLine(a + "," + c);
        }

        public void record(actions a, Gem g)
        {
            if (!recording) return;
            file.WriteLine(a + "," + g);
        }

        public void close()
        {
            file.Close();
        }

        public void snapshot<T>(IEnumerable<T> list)
        {
            if (!snapshotting) return;
            int j = 0;
            foreach (T x in list)
            {
                File.AppendAllText(snapshotDirectory, j + "," + x.ToString() + Environment.NewLine);
                j++;
            }
        }

        private static string getDirectory()
        {
            string folder = @"C:\Users\JHep\Desktop\SelfishGenePlots\" + DateTime.Today.ToLongDateString() + "\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public void plot(string info)
        {
            if (!plotting) return;
            File.AppendAllText(plottingDirectory, info);
        }

        public void plot<T>(IEnumerable<T> list)
        {
            if (!plotting) return;
            int i = 0;
            foreach (T x in list)
            {
                plot(i + "," + x.ToString() + Environment.NewLine);
                i++;
            }
        }


        public void recordWins(string winner, string loser, int winscore, int losescore)
        {
            File.AppendAllText(directory + name + suffix, winner + "," + winscore + "," + loser + "," + losescore + "\n");
        }

    }
}
