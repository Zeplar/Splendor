using System.IO;
using System.Diagnostics;
namespace Splendor
{
    class RecordHistory
    {
        public enum actions { BUY, RESERVE, RESERVETOP, TAKEGEMS };
        const string directory = @"..\..\..\..\Splendor\History\";
        const string suffix = @".csv";
        const string name = @"game";
        static StreamWriter file;
        static bool init;


        public static void initialize()
        {

            //Create the game file
            if (!Directory.Exists(directory))
            {
                Trace.TraceError("Directory does not exist: " + directory);
                return;
            }
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
            //Record the starting player
            foreach (Player p in Splendor.players)
            {
                if (p.turnOrder == 0)
                {
                    file.WriteLine(p + " goes first");
                }
            }

            init = true;
        }


        public static void record(string s)
        {
            if (!init)
            {
                Trace.TraceError("File not initialized");
                return;
            }
            file.WriteLine(s);
        }

        public static void record(actions a, Card c)
        {
            if (!init)
            {
               Trace.TraceError("File not initialized");
                return;
            }
            file.WriteLine(a + "," + c);
        }

        public static void record(actions a, Gem g)
        {
            if (!init)
            {
                Trace.TraceError("File not initialized");
                return;
            }
            file.WriteLine(a + "," + g);
        }

        public static void close()
        {
            if (!init)
            {
                Trace.TraceError("File not initialized");
                return;
            }
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
