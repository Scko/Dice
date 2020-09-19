using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceMVC.Models
{
    public class DiceViewModel
    {
        public int dice1 { get; set; }
        public int dice2 { get; set; }
        public double Win { get; set; }
        public double Lose { get; set; }
        public double Tie { get; set; }
    }
}
