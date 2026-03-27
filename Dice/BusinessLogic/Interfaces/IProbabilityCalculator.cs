using Dice.Models;

namespace Dice.BusinessLogic.Interfaces;

public interface IProbabilityCalculator
{
    ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides);
}
