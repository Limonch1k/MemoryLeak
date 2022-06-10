using OnlineLab.Models;
using Steema1.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steema1.DllWorkers
{
    public class DllWorker
    {
        [DllImport("DiplomFirstDll.dll")]
        extern static void CalculationStart(double x0, double nju, double dif, double dt, double dx, double v0, double rs, bool start,
            double[] lab3au1sharp, double[] lab3av1sharp, double[] lab3au2sharp, double[] lab3av2sharp, double[] lab3aPsi1sharp, double[] lab3aPsi2sharp,
            double[] lab3aVi1sharp, double[] lab3aVi2sharp, double[] lab3aNd1sharp, double[] lab3aNd2sharp, double[] lab3afsharp);

        [DllImport("DiplomFirstDll.dll", EntryPoint = "?getNX@@YAHXZ")]
        extern static int getNX();

        [DllImport("DiplomFirstDll.dll", EntryPoint = "?getNY@@YAHXZ")]
        extern static int getNY();

        public int Nx { get; }
        public int Ny { get; }

        public DllWorker()
        {
            Nx = getNX();
            Ny = getNY();
        }

        public void Start(DoubleArrayList list, InitialParam param, bool start)
        {
            CalculationStart(param.x0, param.nju, param.dif, param.dt, param.dx, param.v0, param.rs, start,
                list.lab3au1sharp, list.lab3av1sharp, list.lab3au2sharp, list.lab3av2sharp, list.lab3aPsi1sharp, list.lab3aPsi2sharp,
                list.lab3aVi1sharp, list.lab3aVi2sharp, list.lab3aNd1sharp, list.lab3aNd2sharp, list.lab3afsharp);
        }
    }
}
