using System.ComponentModel.DataAnnotations;

namespace Dice.Models
{
    public class WaysToRollInputModel
    {
        [Required]
        public int TargetSum { get; set; }
        [Required, Range(1, 20)]
        public int Dice { get; set; }
        [Required, Range(1, 100)]
        public int Sides { get; set; }
    }
}
