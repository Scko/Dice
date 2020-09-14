
using System.ComponentModel.DataAnnotations;

namespace Dice.Models
{
    public class ProbabilityInputModel
    {
        [Required, Range(1, 31)]
        public int Dice1 { get; set; }
        [Required, Range(1, 31)]
        public int Dice2 { get; set; }
        [Required, Range(1, 20)]
        public int Sides { get; set; }
    }
}
