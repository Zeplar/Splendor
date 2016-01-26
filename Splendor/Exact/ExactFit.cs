Up
                score += scoringFunction.evaluate(next);
                current = next;
                //Do the "Generate-next-move" loop for greedy
                nextMove = Greedy.getGreedyMove(current, ScoringMethods.Lead);
                if (nextMove != null)
                {
                    current = current.generate(nextMove);
                }
                else
                {
                    return score;
                }
            }
            return score;
        }

        private uint hash(Board b)
        {
            Gem bp = b.currentPlayer.gems + b.currentPlayer.discount;
            Byte[] buyingPower = new Byte[6];
            for (int i=0; i < 6; i++)
            {
                buyingPower[i] = Convert.ToByte(bp[i] % 8);
            }

            Byte[] fieldState = new Byte[2];
            List<Card> startingCards = GameController.boardCards;
            for (int i=0; i < 8 && i < startingCards.Count; i++)
            {
                if (b.boardCards.Contains(startingCards[i]))
                {
                    fieldState[0] <<= 1;
                    fieldState[0] += 1;
                }
            }
            for (int i = 8; i < 16 && i < startingCards.Count; i++)
            {
                if (b.boardCards.Contains(startingCards[i]))
                {
                    fieldState[1] <<= 1;
                    fieldState[1] += 1;
                }
            }

            uint res1, res2;
            res1 = buyingPower[0]; res1 <<= 8; res1 += buyingPower[1]; res1 <<= 8; res1 += buyingPower[2]; res1 <<= 8; res1 += buyingPower[3];
            res2 = buyingPower[4]; res2 <<= 8; res2 += buyingPower[5]; res2 <<= 8; res2 += fieldState[0]; res2 <<= 8; res2 += fieldState[1];
            return res1 ^ res2;
        }

    }
}
