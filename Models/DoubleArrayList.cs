using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLab.Models
{
    public class DoubleArrayList : IDisposable
    {
        public double[] lab3au1sharp { get; set; }
        public double[] lab3av1sharp { get; set; }
        public double[] lab3au2sharp { get; set; }
        public double[] lab3av2sharp { get; set; }
        public double[] lab3aPsi1sharp { get; set; }
        public double[] lab3aPsi2sharp { get; set; }
        public double[] lab3aVi1sharp { get; set; }
        public double[] lab3aVi2sharp { get; set; }
        public double[] lab3aNd1sharp { get; set; }
        public double[] lab3aNd2sharp { get; set; }
        public double[] lab3afsharp { get; set; }

        public void Dispose()
        {
            lab3afsharp = null;
            lab3aNd1sharp = null;
            lab3aNd2sharp = null;
            lab3aPsi1sharp = null;
            lab3aPsi2sharp = null;
            lab3au1sharp = null;
            lab3au2sharp = null;
            lab3av1sharp = null;
            lab3av2sharp = null;
            lab3aVi1sharp = null;
            lab3aVi2sharp = null;

            GC.Collect(2);
        }
    }
}
