using Dice.BusinessLogic.Interfaces;
using System;
using System.Numerics;

namespace Dice.BusinessLogic
{
    public class MathHelper : IMathHelper
    {
        public decimal Combinations(int n, int k)
        {
            BigInteger a = Factorial(n);
            BigInteger b = Factorial(k);
            BigInteger c = Factorial(n - k);
            return (decimal)(a / (b * c));
        }
        public BigInteger Factorial(int num)
        {
            BigInteger sum = 1;
            for (int i = 1; i <= num; i++)
            {
                sum *= i;
            }
            return sum;
        }

        public double FactorialStirlingApproximation(int num)
        {
            double sum = Math.Sqrt(2 * Math.PI * num) * Math.Pow(num / Math.E, num);
            return sum;
        }
    }
}
