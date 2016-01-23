using System.Diagnostics;

namespace Splendor
{


    public class UnitTests
    {


        [Conditional("DEBUG")]
        public static void testGems()
        {
            Gem test = new Gem(1, 1, 1, 1, 1, 0);
            Gem testGold = new Gem(0, 0, 0, 0, 0, 1);
            Gem test2 = new Gem(1, 0, 0, 0, 0, 0);
            Debug.Assert(test > test2);
            Debug.Assert(test >= test2);
            Debug.Assert(testGold >= test2);
            Debug.Assert(!(testGold > test2));
            Debug.Assert(!(test2 >= test));
            Debug.Assert(!(test2 > test));

            Debug.Assert(test.takeaway(test2) == new Gem(0, 1, 1, 1, 1, 0));

            foreach (Gem g in Gem.ThreeNetThree)
            {
                int i = 0;
                foreach (Gem k in Gem.ThreeNetThree)
                {
                    if (g == k) i++;
                    Debug.Assert(i < 2, "AllThree contained duplicates!");
                }
            }
        }

        [Conditional("DEBUG")]
        public static void testBoard()
        {
            Board b = Board.current;
            testBoard(b);
            Debug.Assert(b.currentPlayer == GameController.currentPlayer, "Board is misrepresenting players: " + b.currentPlayer.detailedInfo + " != " + GameController.currentPlayer.detailedInfo);
        }

        public static void testBoard(Board b)
        {
            Gem g = b.gems;
            foreach (Player p in b.players)
            {
                g += p.gems;
            }
            Debug.Assert(g == new Gem(4, 4, 4, 4, 4, 8), "Gems didn't add up.");
            ScoringMethods.testScoringMethods(b);
        }

        [Conditional("DEBUG")]
        public static void testEncoder()
        {
            Board current = Board.current;
            byte[] encoded = current.Encode();
            Board decoded = Board.Decode(encoded);
            Debug.Assert(decoded.Equals(Board.current), "Board did not encode correctly.");
        }

    }

}
