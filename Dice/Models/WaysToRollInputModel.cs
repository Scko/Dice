using System.ComponentModel.DataAnnotations;

namespace Dice.Models;

public class WaysToRollInputModel
{
    [Required, Range(1, int.MaxValue)]
    public int TargetSum { get; init; }
    [Required, Range(1, 20)]
    public int Dice { get; init; }
    [Required, Range(1, 100)]
    public int Sides { get; init; }
}
