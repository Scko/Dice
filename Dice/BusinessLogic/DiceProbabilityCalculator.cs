using Dice.BusinessLogic.Interfaces;
using Dice.Models;
using System.Numerics;

namespace Dice.BusinessLogic;

public class DiceProbabilityCalculator : IProbabilityCalculator
{
    private readonly IMathHelper _mathHelper;

    public DiceProbabilityCalculator(IMathHelper mathHelper)
    {
        _mathHelper = mathHelper;
    }

    public BigInteger WaysToRoll(int sum, int dice, int sides)
        => _mathHelper.WaysToRoll(sum, dice, sides);

    public ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides)
    {
        var totalPossible = BigInteger.Pow(sides, d1 + d2);
        return new ProbabilityModel
        {
            Win  = Ratio(CountWin(d1, d2, sides),  totalPossible),
            Lose = Ratio(CountLose(d1, d2, sides), totalPossible),
            Tie  = Ratio(CountTie(d1, d2, sides),  totalPossible)
        };
    }

    private BigInteger CountWin(int d1, int d2, int sides)
    {
        BigInteger count = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = _mathHelper.WaysToRoll(i, d2, sides);

            BigInteger waysToRollD1 = 0;
            for (int j = d1 * sides; j > i; j--)
            {
                waysToRollD1 += _mathHelper.WaysToRoll(j, d1, sides);
            }
            count += waysToRollD1 * waysToRollD2;
        }
        return count;
    }

    private BigInteger CountLose(int d1, int d2, int sides)
    {
        BigInteger count = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = _mathHelper.WaysToRoll(i, d2, sides);

            BigInteger waysToRollD1 = 0;
            for (int j = d1; j < i; j++)
            {
                waysToRollD1 += _mathHelper.WaysToRoll(j, d1, sides);
            }
            count += waysToRollD1 * waysToRollD2;
        }
        return count;
    }

    private BigInteger CountTie(int d1, int d2, int sides)
    {
        BigInteger count = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = _mathHelper.WaysToRoll(i, d2, sides);
            var waysToRollD1 = _mathHelper.WaysToRoll(i, d1, sides);
            count += waysToRollD1 * waysToRollD2;
        }
        return count;
    }

    // Counts and total stay within double's range across the validated input ranges
    // (sides ≤ 20, dice ≤ 31), so converting to double for the final ratio is safe.
    private static double Ratio(BigInteger count, BigInteger total)
        => Math.Round((double)count / (double)total, 4);
}
