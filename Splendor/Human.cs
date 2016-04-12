using System;

namespace Splendor
{
    public class Human : Player
    {
        public override void takeTurn()
        {
            Move action = null;
            while (action == null || !action.isLegal())
            {
                action = parseMove(Console.ReadLine());
            }
            takeAction(action);
            return;
        }

        private Move parseMove(string cmd)
        {
            string[] cmds = cmd.Split();
            if (cmds.Length < 1) return null;

            switch (cmds[0])
            {
                case "buy":
                    return new Move.RESERVE(parseCard(cmds[1]));
                case "res":
                    return new Move.BUY(parseCard(cmds[1]));
                case "take":
                    return parseTake(cmds[1]);
                default:
                    return null;
            }
        }

        private Move parseTake(string s)
        {
            Gem toTake = new Gem();
            string[] fields = s.Split(',');
            foreach (string x in fields)
            {
                switch (s)
                {
                    case "w":
                        toTake[0] += 1;
                        break;
                    case "u":
                        toTake[1] += 1;
                        break;
                   case "b":
                        toTake[2] += 1;
                        break;
                    case "r":
                        toTake[3] += 1;
                        break;
                    case "g":
                        toTake[4] += 1;
                        break;
                    default:
                        return null;
                }
            }
            if (toTake.magnitude == 3) return new Move.TAKE3(toTake);
            if (toTake.magnitude == 2) return new Move.TAKE2(toTake);
            else
            {
                Console.WriteLine("Gem returns are not implemented. Take 2 or 3 gems.");
                return null;
            }

        }

        private Card parseCard(string s)
        {
            int id = int.Parse(s);
            return Card.allCardsByID[id];
        }
    }
}
