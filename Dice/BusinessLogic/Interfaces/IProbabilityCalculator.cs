using Dice.Models;
using System.Numerics;

namespace Dice.BusinessLogic.Interfaces;

public interface IProbabilityCalculator
{
    ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides);
    BigInteger WaysToRoll(int sum, int dice, int sides);
}
