using Dice.BusinessLogic.Interfaces;
using Dice.Controllers;
using Dice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace DiceUnitTests.Controllers
{
    public class DiceControllerTests
    {
        private readonly Mock<IProbabilityCalculator> _mockProbabilityCalculator;
        private readonly DiceController _diceController;
        public DiceControllerTests()
        {
            _mockProbabilityCalculator = new Mock<IProbabilityCalculator>();
            _diceController = new DiceController(_mockProbabilityCalculator.Object);
        }
        [Fact(DisplayName = "Proper Input With Reasonable Values Returns 200")]
        public async void ProperInputWithReasonableValuesReturns200()
        {
            _mockProbabilityCalculator
                .Setup(pc => pc.ProbabilityToWinLoseTie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ProbabilityModel { Win = (decimal).5, Lose = (decimal).3, Tie = (decimal).2 });

            var result = await _diceController.Get(new ProbabilityInputModel { Dice1 = 1, Dice2 = 2, Sides = 6 });
            var okResult = (OkObjectResult)result.Result;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode.Value);
        }

        [Fact(DisplayName = "Throws OverflowException Handles By Returning NotFound")]
        public async void ThrowsOverflowExceptionHandlesByReturningNotFound()
        {
            _mockProbabilityCalculator
                .Setup(pc => pc.ProbabilityToWinLoseTie(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new OverflowException());

            var result = await _diceController.Get(new ProbabilityInputModel { Dice1 = 1, Dice2 = 2, Sides = 6 });
            var okResult = (NotFoundObjectResult)result.Result;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, okResult.StatusCode.Value);
        }

    }
}
