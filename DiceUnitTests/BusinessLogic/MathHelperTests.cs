using Dice.BusinessLogic;
using System.Numerics;
using Xunit;

namespace DiceUnitTests.BusinessLogic;

public class MathHelperTests
{
    private readonly MathHelper _mathHelper = new MathHelper();

    // Combinations (internal, multiplicative)

    [Fact(DisplayName = "Combinations C(5,2) returns 10")]
    public void Combinations_5Choose2_Returns10()
    {
        Assert.Equal(new BigInteger(10), MathHelper.Combinations(5, 2));
    }

    [Fact(DisplayName = "Combinations C(n,0) returns 1")]
    public void Combinations_NChoose0_Returns1()
    {
        Assert.Equal(BigInteger.One, MathHelper.Combinations(7, 0));
    }

    [Fact(DisplayName = "Combinations C(n,n) returns 1")]
    public void Combinations_NChooseN_Returns1()
    {
        Assert.Equal(BigInteger.One, MathHelper.Combinations(6, 6));
    }

    [Fact(DisplayName = "Combinations C(10,3) returns 120")]
    public void Combinations_10Choose3_Returns120()
    {
        Assert.Equal(new BigInteger(120), MathHelper.Combinations(10, 3));
    }

    [Fact(DisplayName = "Combinations C(1,1) returns 1")]
    public void Combinations_1Choose1_Returns1()
    {
        Assert.Equal(BigInteger.One, MathHelper.Combinations(1, 1));
    }

    [Fact(DisplayName = "Combinations with k > n returns 0")]
    public void Combinations_KGreaterThanN_Returns0()
    {
        Assert.Equal(BigInteger.Zero, MathHelper.Combinations(3, 5));
    }

    [Fact(DisplayName = "Combinations with negative k returns 0")]
    public void Combinations_NegativeK_Returns0()
    {
        Assert.Equal(BigInteger.Zero, MathHelper.Combinations(5, -1));
    }

    [Fact(DisplayName = "Combinations stays exact well beyond 2^53")]
    public void Combinations_LargeValue_IsExact()
    {
        // C(100,50) = 100891344545564193334812497256, far beyond double precision.
        Assert.Equal(
            BigInteger.Parse("100891344545564193334812497256"),
            MathHelper.Combinations(100, 50));
    }

    // WaysToRoll

    [Fact(DisplayName = "WaysToRoll 1 die 6 sides target 1 returns 1")]
    public void WaysToRoll_OneDieSixSidesTarget1_Returns1()
    {
        Assert.Equal(BigInteger.One, _mathHelper.WaysToRoll(1, 1, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 7 returns 6")]
    public void WaysToRoll_TwoDiceSixSidesTarget7_Returns6()
    {
        Assert.Equal(new BigInteger(6), _mathHelper.WaysToRoll(7, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 2 returns 1 (only 1+1)")]
    public void WaysToRoll_TwoDiceSixSidesTarget2_Returns1()
    {
        Assert.Equal(BigInteger.One, _mathHelper.WaysToRoll(2, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll 2 dice 6 sides target 12 returns 1 (only 6+6)")]
    public void WaysToRoll_TwoDiceSixSidesTarget12_Returns1()
    {
        Assert.Equal(BigInteger.One, _mathHelper.WaysToRoll(12, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll impossible sum (above max) returns 0")]
    public void WaysToRoll_ImpossibleSum_Returns0()
    {
        // Max with 2d6 is 12; 13 is impossible
        Assert.Equal(BigInteger.Zero, _mathHelper.WaysToRoll(13, 2, 6));
    }

    [Fact(DisplayName = "WaysToRoll sum below minimum (sum < dice) returns 0")]
    public void WaysToRoll_SumBelowMinimum_Returns0()
    {
        // Minimum with 3 dice is 3; a sum of 2 cannot be rolled.
        Assert.Equal(BigInteger.Zero, _mathHelper.WaysToRoll(2, 3, 6));
    }

    [Fact(DisplayName = "WaysToRoll total across all sums equals sides^dice")]
    public void WaysToRoll_SummedOverAllSums_EqualsTotalOutcomes()
    {
        // Every outcome of 3d6 maps to exactly one sum, so the counts must total 6^3 = 216.
        BigInteger total = 0;
        for (int sum = 3; sum <= 18; sum++)
            total += _mathHelper.WaysToRoll(sum, 3, 6);

        Assert.Equal(new BigInteger(216), total);
    }

    [Fact(DisplayName = "WaysToRoll stays exact for large counts (20 dice, 100 sides)")]
    public void WaysToRoll_LargeInputs_IsExact()
    {
        // The count here exceeds 2^53, so a double-based implementation would lose precision.
        // Validate exactness via the symmetry W(sum) == W(min+max-sum) for the distribution.
        var low = _mathHelper.WaysToRoll(1000, 20, 100);
        var high = _mathHelper.WaysToRoll(20 + 2000 - 1000, 20, 100); // mirror around the centre
        Assert.True(low > new BigInteger(9_007_199_254_740_992L)); // > 2^53
        Assert.Equal(low, high);
    }
}
