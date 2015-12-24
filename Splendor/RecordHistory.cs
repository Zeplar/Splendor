using System.IO;
using System.Diagnostics;
using System;
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
            file.AutoFlush = true;
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

        /// <summary>
        /// Records the board and current player state.
        /// </summary>
        public static void record()
        {
            if (!Splendor.recording)
            {
                return;
            }
            file.Write("Board State: ");
            foreach (Card c in Splendor.boardCards)
            {
                file.Write(c + ", ");
            }
            file.WriteLine(" Gems: " + Gem.board);
            file.WriteLine(Splendor.currentPlayer + " gems: " + Splendor.currentPlayer.gems);
            file.Write(Splendor.currentPlayer + " has colors " + Splendor.currentPlayer.discount);
            file.WriteLine();
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

    }
}
