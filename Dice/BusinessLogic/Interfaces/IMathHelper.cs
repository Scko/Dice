using System.Numerics;

namespace Dice.BusinessLogic.Interfaces
{
    public interface IMathHelper
    {
        double Combinations(int n, int k);
        BigInteger Factorial(int num);
        double FactorialStirlingApproximation(int num);
    }
}
