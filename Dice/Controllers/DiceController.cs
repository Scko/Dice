using System;
using Dice.BusinessLogic.Interfaces;
using Dice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dice.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiceController : ControllerBase
{
    private readonly IProbabilityCalculator _probabilityCalculator;
    private readonly IMathHelper _mathHelper;

    public DiceController(IProbabilityCalculator probabilityCalculator, IMathHelper mathHelper)
    {
        _probabilityCalculator = probabilityCalculator;
        _mathHelper = mathHelper;
    }

    /// <summary>
    /// Calculates the win, lose, and tie chances when one person rolls any number of dice against another persons roll of any other number of dice for any number of sides.
    /// </summary>
    /// <param name="probabilityInputModel">Dice1, Dice2 (1-31 each), Sides (1-20)</param>
    /// <returns>ProbabilityModel containing the win, tie, and loss percentage</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<ProbabilityModel> Get([FromQuery] ProbabilityInputModel probabilityInputModel)
    {
        try
        {
            return Ok(_probabilityCalculator.ProbabilityToWinLoseTie(
                probabilityInputModel.Dice1, probabilityInputModel.Dice2, probabilityInputModel.Sides));
        }
        catch (OverflowException)
        {
            return UnprocessableEntity("Inputs too large to calculate.");
        }
    }

    /// <summary>
    /// Calculates the number of ways to roll a target sum with a given number of dice.
    /// </summary>
    /// <param name="waysToRollInputModel">TargetSum (≥1), Dice (1-20), Sides (1-100)</param>
    /// <returns>The number of ways to roll the target sum. Returns 0 if the sum is impossible.</returns>
    [HttpGet("WaysToRoll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public ActionResult<double> GetWaysToRoll([FromQuery] WaysToRollInputModel waysToRollInputModel)
    {
        try
        {
            return _mathHelper.WaysToRoll(
                waysToRollInputModel.TargetSum, waysToRollInputModel.Dice, waysToRollInputModel.Sides);
        }
        catch (OverflowException)
        {
            return UnprocessableEntity("Inputs too large to calculate.");
        }
    }
}
