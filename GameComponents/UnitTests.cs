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
            Debug.Assert(b.currentPlayer == GameController.currentPlayer, "Board is misrepresenting players: ");
        }

        public static void testBoard(Board b)
        {
            Gem g = b.Gems;
            g += b.currentPlayer.Gems;
            g += b.notCurrentPlayer.Gems;
            Debug.Assert(g == new Gem(4, 4, 4, 4, 4, 8), "Gems didn't add up: " + g);

            if (b.PrevBoard != null)
            {
                if (b.BoardCards.FindAll(c => c.Deck != Card.Decks.nobles).Count < 12)
                {
                    Debug.Assert(
                           GameController.decks[0].getAllCards().Count < 4
                        || GameController.decks[1].getAllCards().Count < 4
                        || GameController.decks[2].getAllCards().Count < 4,
                           "Cards didn't add up: " + b.BoardCards.String());
                }
                Debug.Assert(b != b.PrevBoard, "Bad board constructor: " + b.PrevMove);
                Debug.Assert(b.notCurrentPlayer.Field.Count == b.PrevBoard.currentPlayer.Field.Count, "Player cards didn't add up");
                Debug.Assert(b.currentPlayer.Field.Count == b.PrevBoard.notCurrentPlayer.Field.Count, "Player cards didn't add up");
            }

        }

        //[Conditional("DEBUG")]
        //public static void testEncoder()
        //{
        //    Board current = Board.current;
        //    byte[] encoded = current.Encode();
        //    Board decoded = Board.Decode(encoded);
        //    Debug.Assert(decoded.Equals(Board.current), "Board did not encode correctly.");
        //}

    }

}
