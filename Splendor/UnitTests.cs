using System.Diagnostics;

namespace Splendor
{


    public class UnitTests
    {


        void Start()
        {
            testGems();
            //		testBoard ();
            //		testBuy ();
        }


        void testGems()
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
        }

        public static void testBoard()
        {
            Board b = Board.current;
            testBoard(b);
            Debug.Assert(b.currentPlayer == Splendor.currentPlayer, "Board is misrepresenting players: " + b.currentPlayer.detailedInfo + " != " + Splendor.currentPlayer.detailedInfo);
        }

        public static void testBoard(Board b)
        {
            Gem g = b.gems;
            foreach (Player p in b.players)
            {
                g += p.gems;
            }
            Debug.Assert(g == new Gem(4, 4, 4, 4, 4, 8));
        }

    }

}
