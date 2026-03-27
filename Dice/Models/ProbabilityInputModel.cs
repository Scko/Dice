using System.ComponentModel.DataAnnotations;

namespace Dice.Models;

public class ProbabilityInputModel
{
    [Required, Range(1, 31)]
    public int Dice1 { get; init; }
    [Required, Range(1, 31)]
    public int Dice2 { get; init; }
    // Sides is capped lower than WaysToRoll (100) because the combinatorial explosion
    // of sides^(d1+d2) total outcomes overflows at high side counts with many dice.
    [Required, Range(1, 20)]
    public int Sides { get; init; }
}
