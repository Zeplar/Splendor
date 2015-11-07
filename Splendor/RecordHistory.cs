using System.IO;
using System.Diagnostics;
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

        public static void initialize()
        {
            if (!Splendor.recording)
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

            //Record the starting deck orders
            foreach (Deck d in Splendor.decks)
            {
                foreach (Card c in d.getAllCards())
                {
                    file.Write(c.id + ",");
                }
                file.WriteLine();
            }
            init = true;
        }

        public static void record()
        {
            if (!Splendor.recording)
            {
                return;
            }
            file.Write("Board State: ");
            foreach (Card c in Splendor.viewableCards)
            {
                file.Write(c + ", ");
            }
            file.WriteLine();
            file.Write(Splendor.currentPlayer + " has colors " + Splendor.currentPlayer.discount);
        }

        public static void record(string s)
        {
            if (!Splendor.recording) {
                return;
            }

            Debug.Assert(init, "File not initialized");
            file.WriteLine(s);
        }

        public static void record(actions a, Card c)
        {
            if (!Splendor.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.WriteLine(a + "," + c);
        }

        public static void record(actions a, Gem g)
        {
            if (!Splendor.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.WriteLine(a + "," + g);
        }

        public static void close()
        {
            if (!Splendor.recording)
            {
                return;
            }
            Debug.Assert(init, "File not initialized");
            file.Close();
            init = false;
        }



        public static void recordWins(string winner, string loser, int winscore, int losescore)
        {
            File.AppendAllText(directory + name + suffix, winner + "," + winscore + "," + loser + "," + losescore + "\n");
        }

        public static void recordMinimaxTree(int[][] history)
        {
            using (FileStream fs = new FileStream(directory + "Tree" + ".txt", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            { 

                for (int i = 0; i < history.Length; i++)
                {
                    for (int j = 0; j < history[i].Length; j++)
                    {
                        while (j < history[i].Length && history[i][j] == -200)
                        {
                            sw.Write(" ");
                            j++;
                        }
                        if (j < history[i].Length)
                        {
                            sw.Write(history[i][j] + "-");
                        }
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
                sw.WriteLine();
            }
        }

    }
}
