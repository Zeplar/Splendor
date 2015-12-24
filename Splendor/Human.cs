using System;

namespace Splendor
{
    public class Human : Player
    {
        public override void takeTurn()
        {
            showState();
            getInput();
            return;
        }

        private bool buyCard(string s)
        {
            int id = int.Parse(s);
            if (!Board.current.viewableCards.Exists(x => x.id == id)) return false;
            Move.BUY b = new Move.BUY(Card.allCardsByID[id]);
            if (b.isLegal())
            {
                b.takeAction();
                return true;
            }
            return false;
        }

        private bool reserveCard(string s)
        {
            if (s[0] != 'r') return false;
            int id = int.Parse(s.Substring(1));
            if (!Board.current.viewableCards.Exists(x => x.id == id)) return false;
            Move.RESERVE r = new Move.RESERVE(Card.allCardsByID[id]);
            if (r.isLegal())
            {
                r.takeAction();
                return true;
            }
            return false;
        }

        private bool takeGems(string g)
        {
            Gem gems = parseGems(g);
            if (Move.getAllLegalMoves().Exists(x => (x.moveType == 0 && ((Move.TAKE2)x).color == gems) || (x.moveType == 1 && ((Move.TAKE3)x).colors == gems)))
            {
                Move.getAllLegalMoves().Find(x => (x.moveType == 0 && ((Move.TAKE2)x).color == gems) || (x.moveType == 1 && ((Move.TAKE3)x).colors == gems)).takeAction();
                return true;
            }
            return false;
        }

        private Gem parseGems(string g)
        {
            if (g.Length != 5) return Gem.zero;
            char[] split = g.ToCharArray();
            int[] i = new int[5];
            for (int j=0; j < 5; j++)
            {
                i[j] = (int)char.GetNumericValue(split[j]);
            }
            return new Gem(i);
        }

        private void getInput()
        {
            while (true)
                {
                    string input = Console.ReadLine();
                    if (takeGems(input)) return;
                    if (reserveCard(input)) return;
                    if (buyCard(input)) return;
                }
        }
    }
}
