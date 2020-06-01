using Dice.BusinessLogic.Interfaces;
using Dice.Models;
using System;
using System.Collections.Generic;

namespace Dice.BusinessLogic
{
    public class DiceProbabilityCalculator : IProbabilityCalculator
    {
        private readonly IMathHelper _mathHelper;
        public DiceProbabilityCalculator(IMathHelper mathHelper)
        {
            _mathHelper = mathHelper;
        }

        public ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides)
        {
            var prevCalc = new Dictionary<int, Dictionary<int, decimal>>();
            decimal pTie, pLose, pWin;
            pTie = ProbabilityToTieWithCache(d1, d2, sides, prevCalc);
            pLose = ProbabilityToLoseWithCache(d1, d2, sides, prevCalc);
            pWin = ProbabilityToWinWithCache(d1, d2, sides, prevCalc);
            return new ProbabilityModel { Lose = pLose, Tie = pTie, Win = pWin };
        }
        
        public decimal ProbabilityToWinWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
        {
            decimal prob = 0;
            var totalPossible = (decimal)Math.Pow(sides, d1 + d2);
            for (int i = d2; i <= d2 * sides; i++)
            {
                decimal waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    var temp = WaysToRoll(i, d2, sides);
                    waysToRollD2 += temp;
                    prevCalc[i] = new Dictionary<int, decimal>();
                    prevCalc[i][d2] = temp;
                }
                decimal waysToRollD1 = 0;
                for (int j = d1 * sides; j > i; j--)
                {
                    if (prevCalc.ContainsKey(j) && prevCalc[j].ContainsKey(d1))
                    {
                        waysToRollD1 += prevCalc[j][d1];
                    }
                    else
                    {
                        var temp = WaysToRoll(j, d1, sides);
                        waysToRollD1 += temp;
                        prevCalc[j] = new Dictionary<int, decimal>();
                        prevCalc[j][d1] = temp;
                    }
                }
                prob += waysToRollD1 * waysToRollD2 / totalPossible;
            }

            return prob;
        }

        public decimal ProbabilityToLoseWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
        {
            decimal prob = 0;
            var totalPossible = (decimal)Math.Pow(sides, d1 + d2);
            for (int i = d2; i <= d2 * sides; i++)
            {
                decimal waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    var temp = WaysToRoll(i, d2, sides);
                    waysToRollD2 += temp;
                    prevCalc[i] = new Dictionary<int, decimal>();
                    prevCalc[i][d2] = temp;
                }
                decimal waysToRollD1 = 0;
                for (int j = d1; j < i; j++)
                {
                    if (prevCalc.ContainsKey(j) && prevCalc[j].ContainsKey(d1))
                    {
                        waysToRollD1 += prevCalc[j][d1];
                    }
                    else
                    {
                        var temp = WaysToRoll(j, d1, sides);
                        waysToRollD1 += temp;
                        prevCalc[j] = new Dictionary<int, decimal>();
                        prevCalc[j][d1] = temp;
                    }
                }
                prob += waysToRollD1 * waysToRollD2 / totalPossible;
            }

            return prob;
        }

        public decimal ProbabilityToTieWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
        {
            decimal prob = 0;
            var totalPossible = (decimal)Math.Pow(sides, d1 + d2);
            for (int i = d2; i <= d2 * sides; i++)
            {
                decimal waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    var temp = WaysToRoll(i, d2, sides);
                    waysToRollD2 += temp;
                    prevCalc[i] = new Dictionary<int, decimal>();
                    prevCalc[i][d2] = temp;
                }
                decimal waysToRollD1 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d1))
                {
                    waysToRollD1 += prevCalc[i][d1];
                }
                else
                {
                    var temp = WaysToRoll(i, d1, sides);
                    waysToRollD1 += temp;
                    prevCalc[i] = new Dictionary<int, decimal>();
                    prevCalc[i][d1] = temp;
                }
                prob += waysToRollD1 * waysToRollD2 / totalPossible;
            }

            return prob;
        }

        private decimal ProbabilityToWin(int d1, int d2, int sides)
        {
            decimal prob = 0;
            int min = d2;
            int max = d2 * sides;
            for (int i = min; i <= max; i++)
            {
                var waysToRollD2 = WaysToRoll(i, d2, sides);
                decimal waysToRollD1 = 0;
                for (int j = d1 * sides; j > i; j--)
                {
                    waysToRollD1 += WaysToRoll(j, d1, sides);
                }
                prob += waysToRollD1 * waysToRollD2 / (decimal)Math.Pow(sides, d1 + d2);
            }

            return prob;
        }

        private decimal ProbabilityToLose(int d1, int d2, int sides)
        {
            decimal prob = 0;
            int min = d2;
            int max = d2 * sides;
            for (int i = min; i <= max; i++)
            {
                var waysToRollD2 = WaysToRoll(i, d2, sides);
                decimal waysToRollD1 = 0;
                for (int j = d1; j < i; j++)
                {
                    waysToRollD1 += WaysToRoll(j, d1, sides);
                }
                prob += waysToRollD1 * waysToRollD2 / (decimal)Math.Pow(sides, d1 + d2);
            }

            return prob;
        }

        private decimal ProbabilityToTie(int d1, int d2, int sides)
        {
            decimal prob = 0;
            int min = d2;
            int max = d2 * sides;
            for (int i = min; i <= max; i++)
            {
                var waysToRollD2 = WaysToRoll(i, d2, sides);
                var waysToRollD1 = WaysToRoll(i, d1, sides);
                prob += waysToRollD1 * waysToRollD2 / (decimal)Math.Pow(sides, d1 + d2);
            }

            return prob;
        }
        public decimal WaysToRoll(int sum, int dice, int sides)
        {
            var kMax = (int)Math.Floor((double)(sum - dice) / sides);

            decimal total = 0;
            for (int k = 0; k <= kMax; k++)
            {
                total += (decimal)Math.Pow(-1, k) * _mathHelper.Combinations(dice, k) * _mathHelper.Combinations((sum - sides * k - 1), (dice - 1));
            }

            return total;
        }

    }
}