using Dice.BusinessLogic.Interfaces;
using System.Numerics;

namespace Dice.BusinessLogic;

public class MathHelper : IMathHelper
{
    public double Combinations(int n, int k)
    {
        BigInteger a = Factorial(n);
        BigInteger b = Factorial(k);
        BigInteger c = Factorial(n - k);
        return (double)(a / (b * c));
    }

    public BigInteger Factorial(int num)
    {
        if (num < 0)
            throw new ArgumentOutOfRangeException(nameof(num), "Factorial is not defined for negative numbers.");

        BigInteger result = 1;
        for (int i = 1; i <= num; i++)
        {
            result *= i;
        }
        return result;
    }

    public double WaysToRoll(int sum, int dice, int sides)
    {
        var kMax = (int)Math.Floor((double)(sum - dice) / sides);

        double total = 0;
        for (int k = 0; k <= kMax; k++)
        {
            total += Math.Pow(-1, k) * Combinations(dice, k) * Combinations((sum - sides * k - 1), (dice - 1));
        }

        return total;
    }
}
