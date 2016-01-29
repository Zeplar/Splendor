using System;
using System.Collections.Generic;
using AForge.Genetic;
using System.Diagnostics;
using System.Text;

namespace Splendor.BuyOrder
{
    public class BuyFit : IFitnessFunction
    {
        public List<Card> cards;
        private ScoringMethods.Function scoringFunction;
        private bool wonPreviousGeneration = false;
        private ScoringMethods.Function greedy = ScoringMethods.Points.opponent();

        public BuyFit(ScoringMethods.Function scoringFunction)
        {
            this.scoringFunction = scoringFunction;
        }

        private bool predictWin(Board current, Move pred)
        {
            if (current.gameOver)
            {
                if (current.winner == 0) { RecordHistory.record("!!! " + GameController.currentPlayer + " now thinks it's going to win! Pred. Greedy move " + pred); wonPreviousGeneration = true; }
                else if (current.winner == 1) { RecordHistory.record("!!! " + GameController.currentPlayer + " now thinks it's going to lose!"); wonPreviousGeneration = false; }
                return true;
            }
            return false;
        }
        public double Evaluate(IChromosome chromosome)
        {
            PermutationChromosome c = (PermutationChromosome)chromosome;
            Board current = Board.current;
            int i = 0;
            double score = 0;
            Move pred = null;
            while (i < 10)
            {

                if (predictWin(current, pred)) break;
                current = simulateMyTurn(c, current);
                score += scoringFunction.evaluate(current);
                if (predictWin(current, pred)) break;
                if (i == 0) pred = simulateGreedyTurn(current);
                current = current.generate(simulateGreedyTurn(current));
                i++;
            }
            wonPreviousGeneration = false;
            return score / i;


        }

        private Move simulateGreedyTurn(Board current)
        {
            Move m = Greedy.getGreedyMove(current, greedy);
            return m;
        }

        public Board simulateMyTurn(PermutationChromosome c, Board current)
        {
            int nextBuy = 0;
            if (current.gameOver) return current;
            while (!current.viewableCards.Contains(cards[c.Value[nextBuy]]))
            {
                nextBuy++;
                if (nextBuy >= current.viewableCards.Count) return current;
            }
            
            Move nextMove = new BuySeeker(cards[c.Value[nextBuy]], current).getMove();
            if (nextMove == null) throw new Exception("BuySeeker shouldn't return null on !gameOver boards");
            return current.generate(nextMove);
        }
    }
}
