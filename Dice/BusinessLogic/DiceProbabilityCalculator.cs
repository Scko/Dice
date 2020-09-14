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
            var prevCalc = new Dictionary<int, Dictionary<int, double>>();
            double pTie, pLose, pWin;
            var totalPossible = Math.Pow(sides, d1 + d2);
            pTie = ProbabilityToTieWithCache(d1, d2, sides, prevCalc) / totalPossible;
            pLose = ProbabilityToLoseWithCache(d1, d2, sides, prevCalc) / totalPossible;
            pWin = ProbabilityToWinWithCache(d1, d2, sides, prevCalc) / totalPossible;
            return new ProbabilityModel { Lose = Math.Round(pLose,4), Tie = Math.Round(pTie, 4), Win = Math.Round(pWin, 4) };
        }
        
        public double ProbabilityToWinWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> prevCalc)
        {
            double prob = 0;
            for (int i = d2; i <= d2 * sides; i++)
            {
                double waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    prevCalc[i] = new Dictionary<int, double>() { { d2, WaysToRoll(i, d2, sides) } };
                    waysToRollD2 += prevCalc[i][d2];
                }

                double waysToRollD1 = 0;
                for (int j = d1 * sides; j > i; j--)
                {
                    if (prevCalc.ContainsKey(j) && prevCalc[j].ContainsKey(d1))
                    {
                        waysToRollD1 += prevCalc[j][d1];
                    }
                    else
                    {
                        prevCalc[j] = new Dictionary<int, double>() { { d1, WaysToRoll(j, d1, sides) } };
                        waysToRollD1 += prevCalc[j][d1];
                    }
                }
                prob += waysToRollD1 * waysToRollD2;
            }

            return prob;
        }

        public double ProbabilityToLoseWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> prevCalc)
        {
            double prob = 0;
            for (int i = d2; i <= d2 * sides; i++)
            {
                double waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    prevCalc[i] = new Dictionary<int, double>() { { d2, WaysToRoll(i, d2, sides) } };
                    waysToRollD2 += prevCalc[i][d2];
                }
                double waysToRollD1 = 0;
                for (int j = d1; j < i; j++)
                {
                    if (prevCalc.ContainsKey(j) && prevCalc[j].ContainsKey(d1))
                    {
                        waysToRollD1 += prevCalc[j][d1];
                    }
                    else
                    {
                        prevCalc[j] = new Dictionary<int, double>() { { d1, WaysToRoll(j, d1, sides) } };
                        waysToRollD1 += prevCalc[j][d1];
                    }
                }
                prob += waysToRollD1 * waysToRollD2;
            }

            return prob;
        }

        public double ProbabilityToTieWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> prevCalc)
        {
            double prob = 0;
            for (int i = d2; i <= d2 * sides; i++)
            {
                double waysToRollD2 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d2))
                {
                    waysToRollD2 += prevCalc[i][d2];
                }
                else
                {
                    prevCalc[i] = new Dictionary<int, double>() { { d2, WaysToRoll(i, d2, sides) } };
                    waysToRollD2 += prevCalc[i][d2];
                }
                double waysToRollD1 = 0;
                if (prevCalc.ContainsKey(i) && prevCalc[i].ContainsKey(d1))
                {
                    waysToRollD1 += prevCalc[i][d1];
                }
                else
                {
                    prevCalc[i] = new Dictionary<int, double>() { { d1, WaysToRoll(i, d1, sides) } };
                    waysToRollD1 += prevCalc[i][d1];
                }
                prob += waysToRollD1 * waysToRollD2;
            }

            return prob;
        }

        public double WaysToRoll(int sum, int dice, int sides)
        {
            var kMax = (int)Math.Floor((double)(sum - dice) / sides);

            double total = 0;
            for (int k = 0; k <= kMax; k++)
            {
                total += Math.Pow(-1, k) * _mathHelper.Combinations(dice, k) * _mathHelper.Combinations((sum - sides * k - 1), (dice - 1));
            }

            return total;
        }

    }
}