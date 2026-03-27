using Dice.BusinessLogic.Interfaces;
using Dice.Models;

namespace Dice.BusinessLogic;

public class DiceProbabilityCalculator : IProbabilityCalculator
{
    private readonly IMathHelper _mathHelper;

    public DiceProbabilityCalculator(IMathHelper mathHelper)
    {
        _mathHelper = mathHelper;
    }

    public ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides)
    {
        var cache = new Dictionary<int, Dictionary<int, double>>();
        var totalPossible = Math.Pow(sides, d1 + d2);
        var pTie  = ProbabilityToTieWithCache(d1, d2, sides, cache)  / totalPossible;
        var pLose = ProbabilityToLoseWithCache(d1, d2, sides, cache) / totalPossible;
        var pWin  = ProbabilityToWinWithCache(d1, d2, sides, cache)  / totalPossible;
        return new ProbabilityModel
        {
            Win  = Math.Round(pWin,  4),
            Lose = Math.Round(pLose, 4),
            Tie  = Math.Round(pTie,  4)
        };
    }

    private double ProbabilityToWinWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> cache)
    {
        double prob = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = GetCached(cache, i, d2, sides);

            double waysToRollD1 = 0;
            for (int j = d1 * sides; j > i; j--)
            {
                waysToRollD1 += GetCached(cache, j, d1, sides);
            }
            prob += waysToRollD1 * waysToRollD2;
        }
        return prob;
    }

    private double ProbabilityToLoseWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> cache)
    {
        double prob = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = GetCached(cache, i, d2, sides);

            double waysToRollD1 = 0;
            for (int j = d1; j < i; j++)
            {
                waysToRollD1 += GetCached(cache, j, d1, sides);
            }
            prob += waysToRollD1 * waysToRollD2;
        }
        return prob;
    }

    private double ProbabilityToTieWithCache(int d1, int d2, int sides, Dictionary<int, Dictionary<int, double>> cache)
    {
        double prob = 0;
        for (int i = d2; i <= d2 * sides; i++)
        {
            var waysToRollD2 = GetCached(cache, i, d2, sides);
            var waysToRollD1 = GetCached(cache, i, d1, sides);
            prob += waysToRollD1 * waysToRollD2;
        }
        return prob;
    }

    private double GetCached(Dictionary<int, Dictionary<int, double>> cache, int sum, int dice, int sides)
    {
        if (!cache.TryGetValue(sum, out var sumCache))
            cache[sum] = sumCache = new Dictionary<int, double>();

        if (!sumCache.TryGetValue(dice, out var value))
            sumCache[dice] = value = _mathHelper.WaysToRoll(sum, dice, sides);

        return value;
    }
}
