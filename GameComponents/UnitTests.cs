using System.Diagnostics;
using System.Linq;
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
            testEncoder();
        }

        public static void testBoard(Board b)
        {
            Gem g = b.Gems;
            foreach (Player p in b.Players)
                g += p.gems;
            Debug.Assert(g == Gem.StartingGems, "Gems didn't add up: " + g);

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
                int cardCount = 0;
                foreach (Player p in b.Players)
                {
                    cardCount += p.field.Count + p.reserve.Count;
                }
                foreach (Player p in b.PrevBoard.Players)
                {
                    cardCount -= p.field.Count + p.reserve.Count;
                }
                Debug.Assert(cardCount == 0 || cardCount == 1, "Cards didn't add up");
            }
            

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
