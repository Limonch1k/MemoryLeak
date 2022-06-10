using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Steema1.Context.Models
{
    public class InitialParam
    {
        public int Id { get; set; }
        public double x0 { get; set; }
        public double nju { get; set; }
        public double dif { get; set; }
        public double dt { get; set; }
        public double dx { get; set; }
        public double v0 { get; set; }
        public double rs { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
