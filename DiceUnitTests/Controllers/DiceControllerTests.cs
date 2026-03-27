using Dice.BusinessLogic.Interfaces;
using Dice.Controllers;
using Dice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DiceUnitTests.Controllers;

public class DiceControllerTests
{
    private readonly Mock<IProbabilityCalculator> _mockProbabilityCalculator;
    private readonly Mock<IMathHelper> _mockMathHelper;
    private readonly DiceController _diceController;

    public DiceControllerTests()
    {
        _mockProbabilityCalculator = new Mock<IProbabilityCalculator>();
        _mockMathHelper = new Mock<IMathHelper>();
        _diceController = new DiceController(_mockProbabilityCalculator.Object, _mockMathHelper.Object);
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

    [Fact(DisplayName = "Get: OverflowException returns 422")]
    public void Get_OverflowException_Returns422()
    {
        _mockProbabilityCalculator
            .Setup(pc => pc.ProbabilityToWinLoseTie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Throws(new OverflowException());

        var result = _diceController.Get(new ProbabilityInputModel { Dice1 = 1, Dice2 = 2, Sides = 6 });
        var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, unprocessableResult.StatusCode);
    }

    // GET /api/dice/waystoroll

    [Fact(DisplayName = "GetWaysToRoll: valid input returns correct value")]
    public void GetWaysToRoll_ValidInput_ReturnsCorrectValue()
    {
        _mockMathHelper
            .Setup(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(6);

        var result = _diceController.GetWaysToRoll(new WaysToRollInputModel { TargetSum = 7, Dice = 2, Sides = 6 });

        // Controller returns double directly (implicit ActionResult<double>), so value is in result.Value
        Assert.Equal((double)6, result.Value);
    }

    [Fact(DisplayName = "GetWaysToRoll: OverflowException returns 422")]
    public void GetWaysToRoll_OverflowException_Returns422()
    {
        _mockMathHelper
            .Setup(m => m.WaysToRoll(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Throws(new OverflowException());

        var result = _diceController.GetWaysToRoll(new WaysToRollInputModel { TargetSum = 7, Dice = 2, Sides = 6 });
        var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(result.Result);

        Assert.Equal(StatusCodes.Status422UnprocessableEntity, unprocessableResult.StatusCode);
    }
}
