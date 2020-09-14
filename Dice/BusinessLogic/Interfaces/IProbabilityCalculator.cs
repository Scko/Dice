using Dice.Models;

namespace Dice.BusinessLogic.Interfaces
{
    public interface IProbabilityCalculator
    {
        double WaysToRoll(int targetSum, int dice, int sides);
        ProbabilityModel ProbabilityToWinLoseTie(int d1, int d2, int sides);
    }
}
