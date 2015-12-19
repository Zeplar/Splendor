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

        static bool reg1 = Register("Human", () => new Human());

        private void showState()
        {
            Console.Clear();
            Console.WriteLine("Board:");
            Console.WriteLine(Gem.board + "    Nobles: " + Splendor.nobles.viewableCards.String());
            Console.WriteLine("Tier3: " + Splendor.decks[2].viewableCards.String());
            Console.WriteLine("Tier2: " + Splendor.decks[1].viewableCards.String());
            Console.WriteLine("Tier1: " + Splendor.decks[0].viewableCards.String());
            Console.WriteLine();
            Console.WriteLine("Opponent: " + opponent.gems + "|Field: " + opponent.field.String());
            Console.WriteLine("Self: " + gems + "|Field: " + field.String() + "   |Reserve: " + reserve.String());

        }

        private void getInput()
        {
            while (true)
                {
                    string input = Console.ReadLine();

                }
        }
    }
}
