using Dice.BusinessLogic.Interfaces;
using System.Collections.Concurrent;
using System.Numerics;

namespace Dice.BusinessLogic;

public class MathHelper : IMathHelper
{
    // Process-lifetime memoization. MathHelper is registered as a singleton and this
    // cache is keyed only on the inputs, so results are shared across every request.
    private readonly ConcurrentDictionary<(int sum, int dice, int sides), BigInteger> _waysCache = new();

    public BigInteger WaysToRoll(int sum, int dice, int sides)
        => _waysCache.GetOrAdd((sum, dice, sides), key => ComputeWaysToRoll(key.sum, key.dice, key.sides));

    private static BigInteger ComputeWaysToRoll(int sum, int dice, int sides)
    {
        var kMax = (int)Math.Floor((double)(sum - dice) / sides);

        BigInteger total = 0;
        for (int k = 0; k <= kMax; k++)
        {
            var term = Combinations(dice, k) * Combinations(sum - sides * k - 1, dice - 1);
            total += (k & 1) == 0 ? term : -term;
        }

        return total;
    }

    // C(n, k) via the multiplicative formula: O(min(k, n-k)) exact BigInteger steps,
    // with no large intermediate factorials. Returns 0 for out-of-range k.
    internal static BigInteger Combinations(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;

        k = Math.Min(k, n - k);
        BigInteger result = 1;
        for (int i = 0; i < k; i++)
        {
            result = result * (n - i) / (i + 1);
        }
        return result;
    }
}
