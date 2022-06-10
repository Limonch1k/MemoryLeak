using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace Steema1.Context.Models
{
    public class PhysData : IDisposable
    {
        public int Id { get; set; }
        public int Shot { get; set; }

        public string lab3au1string { get; set; }

        public string lab3au2string { get; set; }

        public string lab3av1string { get; set; }

        public string lab3av2string { get; set; }

        public string lab3aPsi1string { get; set; }

        public string lab3aPsi2string { get; set; }

        public string lab3aVi1string { get; set; }

        public string lab3aVi2string { get; set;}

        public string lab3aNd1string { get; set; }
        public string lab3aNd2string { get; set; }

        public string lab3afstring { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public unsafe void Dispose()
        {
            lab3afstring = null;
            lab3aNd1string = null;
            lab3aNd2string = null;
            lab3aPsi1string = null;
            lab3aPsi2string = null;
            lab3au1string = null;
            lab3au2string = null;
            lab3av1string = null;
            lab3av2string = null;
            lab3aVi1string = null;
            lab3aVi2string = null;
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }
    }
}
