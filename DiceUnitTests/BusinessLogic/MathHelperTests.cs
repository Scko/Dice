using Dice.BusinessLogic;
using System.Numerics;
using Xunit;

namespace DiceUnitTests.BusinessLogic;

public class MathHelperTests
{
    private readonly MathHelper _mathHelper = new MathHelper();

    // Factorial

    [Fact(DisplayName = "Factorial of 0 returns 1")]
    public void Factorial_Zero_ReturnsOne()
    {
        Assert.Equal(new BigInteger(1), _mathHelper.Factorial(0));
    }

    [Fact(DisplayName = "Factorial of 1 returns 1")]
    public void Factorial_One_ReturnsOne()
    {
        Assert.Equal(new BigInteger(1), _mathHelper.Factorial(1));
    }

    [Fact(DisplayName = "Factorial of 5 returns 120")]
    public void Factorial_Five_Returns120()
    {
        Assert.Equal(new BigInteger(120), _mathHelper.Factorial(5));
    }

    [Fact(DisplayName = "Factorial of 10 returns 3628800")]
    public void Factorial_Ten_Returns3628800()
    {
        Assert.Equal(new BigInteger(3628800), _mathHelper.Factorial(10));
    }

    [Fact(DisplayName = "Factorial of negative number throws ArgumentOutOfRangeException")]
    public void Factorial_Negative_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _mathHelper.Factorial(-1));
    }

    // Combinations

    [Fact(DisplayName = "Combinations C(5,2) returns 10")]
    public void Combinations_5Choose2_Returns10()
    {
        Assert.Equal(10, _mathHelper.Combinations(5, 2));
    }

    [Fact(DisplayName = "Combinations C(n,0) returns 1")]
    public void Combinations_NChoose0_Returns1()
    {
        Assert.Equal(1, _mathHelper.Combinations(7, 0));
    }

    [Fact(DisplayName = "Combinations C(n,n) returns 1")]
    public void Combinations_NChooseN_Returns1()
    {
        Assert.Equal(1, _mathHelper.Combinations(6, 6));
    }

    [Fact(DisplayName = "Combinations C(10,3) returns 120")]
    public void Combinations_10Choose3_Returns120()
    {
        Assert.Equal(120, _mathHelper.Combinations(10, 3));
    }

    [Fact(DisplayName = "Combinations C(1,1) returns 1")]
    public void Combinations_1Choose1_Returns1()
    {
        Assert.Equal(1, _mathHelper.Combinations(1, 1));
    }

    // WaysToRoll

    [Fact(DisplayName = "WaysToRoll 1 die 6 sides target 1 returns 1")]
    public void WaysToRoll_OneDieSixSidesTarget1_Returns1()
    {
        Assert.Equal(1, _mathHelper.WaysToRoll(1, 1, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 7 returns 6")]
    public void WaysToRoll_TwoDiceSixSidesTarget7_Returns6()
    {
        Assert.Equal(6, _mathHelper.WaysToRoll(7, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 2 returns 1 (only 1+1)")]
    public void WaysToRoll_TwoDiceSixSidesTarget2_Returns1()
    {
        Assert.Equal(1, _mathHelper.WaysToRoll(2, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 12 returns 1 (only 6+6)")]
    public void WaysToRoll_TwoDiceSixSidesTarget12_Returns1()
    {
        Assert.Equal(1, _mathHelper.WaysToRoll(12, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll impossible sum returns 0")]
    public void WaysToRoll_ImpossibleSum_Returns0()
    {
        // Max with 2d6 is 12; 13 is impossible
        Assert.Equal(0, _mathHelper.WaysToRoll(13, 2, 6));
    }
}
