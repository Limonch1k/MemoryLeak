using OnlineLab.Models;
using Steema1.Context.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OnlineLab.DllWorkers
{
    public class DllDirectX
    {

        public DllDirectX()
        {
        }

        [DllImport("DirectXDll.dll")]
        extern static int SaveToVichrImage(double[] lab3au1, int xparam, int yparam, double t0);

        [DllImport("DirectXDll.dll")]
        extern static int SaveToUxyImage(double[] lab3au1, int xparam, int yparam, double t0);
        [DllImport("DirectXDll.dll")]
        extern static int SaveToModuleImage(double[] lab3au1, double[] lab3av1, int xparam, int yparam, double t0);
        [DllImport("DirectXDll.dll")]
        extern static int SaveToVxyImage(double[] lab3au1, int xparam, int yparam, double t0);
        [DllImport("DirectXDll.dll")]
        extern static int SaveToPileImage(double[] lab3au1, int xparam, int yparam);
        [DllImport("DirectXDll.dll")]
        extern static int SaveToFunctionTokaImage(double[] lab3au1, int xparam, int yparam, double x0, double t0);
        public void StartVichrImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            SaveToVichrImage(list.lab3aVi1sharp, NX, NY, param.x0 / param.v0);
        }

        public void StartUxyImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {           
            SaveToUxyImage(list.lab3au1sharp, NX, NY, param.x0);
        }

        public void StartModuleImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            SaveToModuleImage(list.lab3au1sharp, list.lab3av1sharp, NX, NY, param.x0);           
        }

        public void StartVxyImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            SaveToVxyImage(list.lab3av1sharp, NX, NY, param.x0);
        }

        public void StartPileImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            SaveToPileImage(list.lab3aNd1sharp, NX, NY);
        }

        public void StartFunctionTokaImage(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            SaveToFunctionTokaImage(list.lab3aPsi1sharp, NX, NY, param.x0, param.x0 / param.v0);
        }
    }
}
