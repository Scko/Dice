using Dice.BusinessLogic.Interfaces;
using Dice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dice.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class DiceController : ControllerBase
    {
        private readonly IProbabilityCalculator _probabilityCalculator;
        public DiceController(
            IProbabilityCalculator probabilityCalculator)
        {
            _probabilityCalculator = probabilityCalculator;
        }

        /// <summary>
        /// Calculates the win, lose, and tie chances when one person rolls any number of dice against another persons roll of any other number of dice for any number of sides
        /// </summary>
        /// <param name="dice1">The number of dice the first person rolls</param>
        /// <param name="dice2">The number of dice the second person rolls</param>
        /// <param name="sides">The number of sides on the dice</param>
        /// <returns>ProbabilityModel containing the win, tie, and loss percentage</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProbabilityModel>> Get([FromQuery] ProbabilityInputModel probabilityInputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return Ok(_probabilityCalculator.ProbabilityToWinLoseTie(probabilityInputModel.Dice1, probabilityInputModel.Dice2, probabilityInputModel.Sides));
            }
            catch (OverflowException)
            {
                return NotFound("Inputs too large to calculate.");
            }
        }

        /// <summary>
        /// Calculates the possible number of ways to roll the targetSum with a given number of dice.
        /// </summary>
        /// <param name="targetSum">The sum of the dice being rolled</param>
        /// <param name="dice">The number of dice being rolled</param>
        /// <param name="sides">The number of sides on the dice</param>
        /// <returns></returns>
        [HttpGet("WaysToRoll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<decimal>> GetWaysToRoll([FromQuery] WaysToRollInputModel waysToRollInputModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                return _probabilityCalculator.WaysToRoll(waysToRollInputModel.TargetSum, waysToRollInputModel.Dice, waysToRollInputModel.Sides);
            }
            catch (OverflowException)
            {
                return NotFound("Inputs too large to calculate.");
            }
        }
    }
}