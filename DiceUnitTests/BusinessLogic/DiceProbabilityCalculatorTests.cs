using Dice.BusinessLogic;
using Dice.BusinessLogic.Interfaces;
using Moq;
using System.Numerics;
using Xunit;

namespace DiceUnitTests.BusinessLogic;

public class DiceProbabilityCalculatorTests
{
    private readonly Mock<IMathHelper> _mockMathHelper;
    private readonly DiceProbabilityCalculator _calculator;

    public DiceProbabilityCalculatorTests()
    {
        _mockMathHelper = new Mock<IMathHelper>();
        _calculator = new DiceProbabilityCalculator(_mockMathHelper.Object);
    }

    [Fact(DisplayName = "ProbabilityToWinLoseTie 1d6 vs 1d6: win and lose are equal by symmetry")]
    public void ProbabilityToWinLoseTie_EqualDice_WinEqualsLose()
    {
        var realMathHelper = new MathHelper();
        var calculator = new DiceProbabilityCalculator(realMathHelper);

        var result = calculator.ProbabilityToWinLoseTie(1, 1, 6);

        Assert.Equal(result.Win, result.Lose);
    }

    [Fact(DisplayName = "ProbabilityToWinLoseTie 1d6 vs 1d6: tie probability is 1/6")]
    public void ProbabilityToWinLoseTie_EqualDice_TieIsOneSixth()
    {
        var realMathHelper = new MathHelper();
        var calculator = new DiceProbabilityCalculator(realMathHelper);

        var result = calculator.ProbabilityToWinLoseTie(1, 1, 6);

        // 6 ties out of 36 total outcomes = 1/6 ≈ 0.1667
        Assert.Equal(0.1667, result.Tie, 4);
    }

    [Fact(DisplayName = "ProbabilityToWinLoseTie results are rounded to 4 decimal places")]
    public void ProbabilityToWinLoseTie_ResultsRoundedTo4Decimals()
    {
        var realMathHelper = new MathHelper();
        var calculator = new DiceProbabilityCalculator(realMathHelper);

        var result = calculator.ProbabilityToWinLoseTie(2, 3, 6);

        Assert.Equal(System.Math.Round(result.Win,  4), result.Win);
        Assert.Equal(System.Math.Round(result.Lose, 4), result.Lose);
        Assert.Equal(System.Math.Round(result.Tie,  4), result.Tie);
    }

    [Fact(DisplayName = "ProbabilityToWinLoseTie 1d6 vs 1d6: win+lose+tie approximately sum to 1")]
    public void ProbabilityToWinLoseTie_EqualDice_SumsToApproximatelyOne()
    {
        var realMathHelper = new MathHelper();
        var calculator = new DiceProbabilityCalculator(realMathHelper);

        var result = calculator.ProbabilityToWinLoseTie(1, 1, 6);

        // Values are individually rounded to 4dp, so sum may differ by up to 0.001
        var sum = result.Win + result.Lose + result.Tie;
        Assert.InRange(sum, 0.999, 1.001);
    }

    [Fact(DisplayName = "ProbabilityToWinLoseTie delegates WaysToRoll lookups to IMathHelper")]
    public void ProbabilityToWinLoseTie_DelegatesToMathHelper()
    {
        _mockMathHelper
            .Setup(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(BigInteger.One);

        _calculator.ProbabilityToWinLoseTie(1, 1, 6);

        _mockMathHelper.Verify(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
            Times.AtLeastOnce());
    }
}
