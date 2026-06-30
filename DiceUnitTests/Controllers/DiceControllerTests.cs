using Dice.BusinessLogic.Interfaces;
using Dice.Controllers;
using Dice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Numerics;
using Xunit;

namespace DiceUnitTests.Controllers;

public class DiceControllerTests
{
    private readonly Mock<IProbabilityCalculator> _mockProbabilityCalculator;
    private readonly DiceController _diceController;

    public DiceControllerTests()
    {
        _mockProbabilityCalculator = new Mock<IProbabilityCalculator>();
        _diceController = new DiceController(_mockProbabilityCalculator.Object);
    }

    // GET /api/dice

    [Fact(DisplayName = "Get: valid input returns 200 with probability model")]
    public void Get_ValidInput_Returns200()
    {
        _mockProbabilityCalculator
            .Setup(pc => pc.ProbabilityToWinLoseTie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new ProbabilityModel { Win = .5, Lose = .3, Tie = .2 });

        var result = _diceController.Get(new ProbabilityInputModel { Dice1 = 1, Dice2 = 2, Sides = 6 });
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact(DisplayName = "Get: delegates to the probability calculator with the model's values")]
    public void Get_DelegatesToCalculator()
    {
        _mockProbabilityCalculator
            .Setup(pc => pc.ProbabilityToWinLoseTie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new ProbabilityModel());

        _diceController.Get(new ProbabilityInputModel { Dice1 = 1, Dice2 = 2, Sides = 6 });

        _mockProbabilityCalculator.Verify(pc => pc.ProbabilityToWinLoseTie(1, 2, 6), Times.Once());
    }

    // GET /api/dice/waystoroll

    [Fact(DisplayName = "GetWaysToRoll: valid input returns correct value")]
    public void GetWaysToRoll_ValidInput_ReturnsCorrectValue()
    {
        _mockProbabilityCalculator
            .Setup(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(new BigInteger(6));

        var result = _diceController.GetWaysToRoll(new WaysToRollInputModel { TargetSum = 7, Dice = 2, Sides = 6 });

        // Controller returns BigInteger directly (implicit ActionResult<BigInteger>), so value is in result.Value
        Assert.Equal(new BigInteger(6), result.Value);
    }

    [Fact(DisplayName = "GetWaysToRoll: delegates to the probability calculator with the model's values")]
    public void GetWaysToRoll_DelegatesToCalculator()
    {
        _mockProbabilityCalculator
            .Setup(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(BigInteger.Zero);

        _diceController.GetWaysToRoll(new WaysToRollInputModel { TargetSum = 7, Dice = 2, Sides = 6 });

        _mockProbabilityCalculator.Verify(m => m.WaysToRoll(7, 2, 6), Times.Once());
    }
}
