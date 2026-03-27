namespace Dice.BusinessLogic.Interfaces;

public interface IMathHelper
{
    double Combinations(int n, int k);
    double WaysToRoll(int sum, int dice, int sides);
}
