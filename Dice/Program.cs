using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace Dice
{
    class Program
    {
        static void Main(string[] args)
        {
            //Probilities that for any number of dice d1 will win, tie, or lose to d2.
            var prevCalc = new Dictionary<int, Dictionary<int, decimal>>();
            var sides = 6;
            var d1 = 1;
            var d2 = 1;

            decimal pTie, pLose, pWin, prob;
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            pTie = ProbabilityToTieWithCache(d1, d2, sides, prevCalc);
            pLose = ProbabilityToLoseWithCache(d1, d2, sides, prevCalc);
            pWin = ProbabilityToWinWithCache(d1, d2, sides, prevCalc);
            prob = pTie + pLose + pWin;
            stopWatch.Stop();
            var timeWithCache = stopWatch.ElapsedMilliseconds;

            stopWatch.Reset();

            stopWatch.Start();
            pTie = ProbabilityToTie(d1, d2, sides);
            pLose = ProbabilityToLose(d1, d2, sides);
            pWin = ProbabilityToWin(d1, d2, sides);
            prob = pTie + pLose + pWin;
            stopWatch.Stop();
            var time = stopWatch.ElapsedMilliseconds;

            var output = prob;
        }

        public static decimal ProbabilityToWinWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
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

        public static decimal ProbabilityToLoseWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
        {
            decimal prob = 0;
            var totalPossible = (decimal)Math.Pow(sides, d1 + d2);
            for (int i = d2; i <= d2 * sides; i++)
            {
                //var waysToRollD2 = WaysToRoll(i, d2, sides);
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

        public static decimal ProbabilityToTieWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, decimal>> prevCalc)
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

        public static decimal ProbabilityToWin(int d1, int d2, int sides)
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

        public static decimal ProbabilityToLose(int d1, int d2, int sides)
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

        public static decimal ProbabilityToTie(int d1, int d2, int sides)
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
        public static decimal WaysToRoll(int sum, int dice, int sides)
        {
            var kMax = (int)Math.Floor((double)(sum - dice) / sides);

            decimal total = 0;
            for (int k = 0; k <= kMax; k++)
            {
                total += (decimal)Math.Pow(-1, k) * Combinations(dice, k) * Combinations((sum - sides * k - 1), (dice - 1));
            }

            return total;
        }

        public static decimal Combinations(int n, int k)
        {
            BigInteger a = Factorial(n);
            BigInteger b = Factorial(k);
            BigInteger c = Factorial(n - k);
            return (decimal)(a / (b * c));

        }
        public static BigInteger Factorial(int num)
        {
            BigInteger sum = 1;
            for (int i = 1; i <= num; i++)
            {
                sum *= i;
            }
            return sum;
        }

    }
}
