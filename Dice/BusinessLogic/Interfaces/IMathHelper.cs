using System.Numerics;

namespace Dice.BusinessLogic.Interfaces;

public interface IMathHelper
{
    BigInteger WaysToRoll(int sum, int dice, int sides);
}
