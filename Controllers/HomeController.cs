using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineLab.Models;
using Steema.TeeChart.Export;
using Steema.TeeChart.Styles;
using Steema1.Context;
using Steema1.Context.Models;
using Steema1.DllWorkers;
using Steema1.Models;
using ImageMagick;
using System.Threading;
using OnlineLab.Context.Models;
using OnlineLab.DllWorkers;
using System.Runtime;

namespace Steema1.Controllers
{
    public class HomeController : Controller
    {

        PhysDataContext context;
        DllWorker dllWorker;
        int Nx;
        int Ny;
        object locker = new object();

        public HomeController(PhysDataContext _context, DllWorker _dllWorker) 
        {
            context = _context;
            dllWorker = _dllWorker;
        }

        [Authorize]
        public IActionResult Index()
        {
            DataTransferForm model = new DataTransferForm()
            {
                L = 10.0,
                diff = 0,
                dt = 0.001,
                dx = 0.1,
                nju = 0.15,
                R = 10,
                U0 = 10
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ColorGrid(string L, string diff, string dt, string dx, string nju, string R, string U0)
        {
            
            int UserId = int.Parse(HttpContext.User.Claims.Where(c => c.Type == "Id").Select(c => c.Value).FirstOrDefault());

            Nx = dllWorker.Nx;
            Ny = dllWorker.Ny;

            DoubleArrayList listArray = new DoubleArrayList()
            {
                lab3au1sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3av1sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3au2sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3av2sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aPsi1sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aPsi2sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aVi1sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aVi2sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aNd1sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3aNd2sharp = new double[(Nx + 1) * (Ny + 1)],
                lab3afsharp = new double[(Nx + 1) * (Ny + 1)],
            };

            context.InitialParams.RemoveRange(context.InitialParams.Where(c => c.UserId == UserId));
            context.PhysDatas.RemoveRange(context.PhysDatas.Where(c => c.UserId == UserId));
            context.Images.RemoveRange(context.Images.Where(c => c.UserId == UserId));
            context.Tasks.RemoveRange(context.Tasks.Where(c=> c.UserId == UserId));
            context.SaveChanges();

            PhysData physData = new PhysData();
            physData.UserId = UserId;
            InitialParam param = new InitialParam()
            {
                x0 = double.Parse(L),
                nju = double.Parse(nju),
                dif = double.Parse(diff),
                dt = double.Parse(dt),
                dx = double.Parse(dx),
                v0 = double.Parse(U0),
                rs = double.Parse(R),
                UserId = UserId
            };

            context.InitialParams.Add(param);

            int count = 180;
            DataTransferBase64 model = new DataTransferBase64();
            model.base64 = new string[count];
            model.length = count;
            model.frame = count;
            bool start = true;
            PhysData data = new PhysData();

            for (int i = 0; i < count / 6; i++)
            {
                if (i != 0)
                {
                    start = false;
                }
                // счет один для всех, не надо его параллерить
                dllWorker.Start(listArray, param, start);
                data.Dispose();
                data = new PhysData();
                data.UserId = UserId;

                ParseDoubleToString(data, listArray);
                // вот тут надо параллерить.
                Task<string> tsk0 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsVichr(listArray, param, Nx, Ny)); });
                Task<string> tsk1 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsUxy(listArray, param, Nx, Ny)); });
                Task<string> tsk2 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsFlow(listArray, param, Nx, Ny)); });
                Task<string> tsk3 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsVxy(listArray, param, Nx, Ny)); });
                Task<string> tsk4 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsModV(listArray, param, Nx, Ny)); });
                Task<string> tsk5 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsFunToka(listArray, param, Nx, Ny)); });

                tsk0.Start();
                tsk1.Start();
                tsk2.Start();
                tsk3.Start();
                tsk4.Start();
                tsk5.Start();

                Task.WaitAll(tsk0,tsk1,tsk2,tsk3,tsk4,tsk5);
                model.base64[i * 6] = tsk0.Result;//ЗАВИХР
                model.base64[i * 6 + 1] = tsk1.Result;//ТЕЧЕНИЕ ПЫЛИ ВРОДЕ
                model.base64[i * 6 + 2] = tsk2.Result;// ФУН ТОКА
                model.base64[i * 6 + 3] = tsk3.Result;// МОДУЛЬ СКОРОСТЕЙ
                model.base64[i * 6 + 4] = tsk4.Result;// U(x,y) чото
                model.base64[i * 6 + 5] = tsk5.Result;// V(x,y) чото

                tsk0.Dispose();
                tsk1.Dispose();
                tsk2.Dispose();
                tsk3.Dispose();
                tsk4.Dispose();
                tsk5.Dispose();

                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            }
           
            listArray.Dispose();
            listArray = null;
            data.Shot = count;
            data.UserId = UserId;
            context.PhysDatas.Add(data);           
            context.SaveChanges();
            context.ChangeTracker.Clear();

            data.Dispose();
            physData.Dispose();
            physData = null;
            data = null;

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2);
            GC.WaitForPendingFinalizers();

            return View("ColorGrid", model);
            //return ViewComponent("ColorGrid", new{ L = Double.Parse(L) , diff = Double.Parse(diff), dt = Double.Parse(dt), dx = Double.Parse(dx), nju = Double.Parse(nju), R = Double.Parse(R), U0 = Double.Parse(U0) });
        }

        [HttpPost]
        public IActionResult ColorGridNewFrame(int num, int lastframe)
        {
            int UserId = int.Parse(HttpContext.User.Claims.Where(c => c.Type == "Id").Select(c => c.Value).FirstOrDefault());

            if (num == -1 && lastframe == -1) 
            {
                if (context.Tasks.Where(c => c.UserId == UserId).Select(c => c.StartStatus).FirstOrDefault() == 1)
                {
                    return StatusCode(500);
                }
                else 
                {
                    return StatusCode(200);
                }
            }

            DataTransferBase64 model = new DataTransferBase64();
            context.Images.RemoveRange(context.Images.Where(c => c.UserId == UserId && c.frame <= lastframe));
            

            if (context.Tasks.Where(c => c.UserId == UserId).Select(c => c.StartStatus).FirstOrDefault() == 1) 
            {
                return StatusCode(500);
            }

            context.Tasks.Remove(context.Tasks.Where(c => c.UserId == UserId).FirstOrDefault());
            context.SaveChanges();
            model.length = num * 6;
            model.frame = lastframe + (num * 6);

            
            model.base64 = context.Images.Where(c => c.frame > lastframe).OrderBy(c => c.frame).Select(c => c.Image64).Take(num * 6).ToArray();

            int i = 0;

            while (model.base64.Length != num * 6  && i != 10) 
            {
                context = new PhysDataContext();
                Thread.Sleep(200);
                model.base64 = context.Images.Where(c => c.frame > lastframe).OrderBy(c => c.frame).Select(c => c.Image64).Take(num).ToArray();
                i++;
                
            }

            if (i == 10) 
            {
                return StatusCode(501);
            }

            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            return View("ColorGrid", model);
        }

        [HttpPost]
        public IActionResult ColorGridStart(int num, int lastframe) 
        {

            Nx = dllWorker.Nx;
            Ny = dllWorker.Ny;
            int UserId = int.Parse(HttpContext.User.Claims.Where(c => c.Type == "Id").Select(c => c.Value).FirstOrDefault());
            TaskExecution execution = new TaskExecution();
            execution.StartStatus = 1;
            execution.UserId = UserId;
            context.Tasks.Add(execution);
            context.SaveChanges();

            Task.Run( () => 
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                using (context = new PhysDataContext()) 
                {
                    InitialParam param = context.InitialParams.Where(c => c.UserId == UserId).FirstOrDefault();
                    var physDataLast = context.PhysDatas.Where(c => c.UserId == UserId).FirstOrDefault();
                    context.PhysDatas.RemoveRange(context.PhysDatas.Where(c => c.UserId == UserId));
                    DoubleArrayList listArray = ParseStringToDouble(physDataLast);
                    PhysData last = new PhysData();
                    DllWorker worker = new DllWorker();
                    for (int i = 0; i < num; i++)
                    {
                        
                        worker.Start(listArray, param, false);
                        ParseDoubleToString(last, listArray);

                        Task<string> tsk0 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsVichr(listArray, param, Nx, Ny)); });
                        Task<string> tsk1 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsUxy(listArray, param, Nx, Ny)); });
                        Task<string> tsk2 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsFlow(listArray, param, Nx, Ny)); });
                        Task<string> tsk3 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsVxy(listArray, param, Nx, Ny)); });
                        Task<string> tsk4 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsModV(listArray, param, Nx, Ny)); });
                        Task<string> tsk5 = new Task<string>(() => { return Convert.ToBase64String(DrawGraphicsFunToka(listArray, param, Nx, Ny)); });

                        ImagePresentation image0 = new ImagePresentation();//ЗАВИХР
                        ImagePresentation image1 = new ImagePresentation(); //ТЕЧЕНИЕ ПЫЛИ ВРОДЕ
                        ImagePresentation image2 = new ImagePresentation();// ФУН ТОКА
                        ImagePresentation image3 = new ImagePresentation();// МОДУЛЬ СКОРОСТЕЙ
                        ImagePresentation image4 = new ImagePresentation();// U(x,y) чото
                        ImagePresentation image5 = new ImagePresentation();// V(x,y) чото

                        tsk0.Start();
                        tsk1.Start();
                        tsk2.Start();
                        tsk3.Start();
                        tsk4.Start();
                        tsk5.Start();
                        Task.WaitAll(tsk0, tsk1, tsk2, tsk3, tsk4, tsk5);

                        image0.Image64 = tsk0.Result;
                        image0.frame = lastframe + (i * 6) + 1;
                        image0.UserId = UserId;

                        image1.Image64 = tsk1.Result;
                        image1.frame = lastframe + (i * 6) + 1 + 1;
                        image1.UserId = UserId;

                        image2.Image64 = tsk2.Result;
                        image2.frame = lastframe + (i * 6) + 2 + 1;
                        image2.UserId = UserId;

                        image3.Image64 = tsk3.Result;
                        image3.frame = lastframe + (i * 6) + 3 + 1;
                        image3.UserId = UserId;

                        image4.Image64 = tsk4.Result;
                        image4.frame = lastframe + (i * 6) + 4 + 1;
                        image4.UserId = UserId;

                        image5.Image64 = tsk5.Result;
                        image5.frame = lastframe + (i * 6) + 5 + 1;
                        image5.UserId = UserId;

                        context.Images.AddRange(new List<ImagePresentation>() { image0, image1, image2, image3, image4, image5 });
                        context.SaveChanges();
                        context.ChangeTracker.Clear();

                        image0.Dispose();
                        image0 = null;
                        image1.Dispose();
                        image1 = null;
                        image2.Dispose();
                        image2 = null;
                        image3.Dispose();
                        image3 = null;
                        image4.Dispose();
                        image4 = null;
                        image5.Dispose();
                        image5 = null;
                        tsk0.Dispose();
                        tsk0 = null;
                        tsk1.Dispose();
                        tsk1 = null;
                        tsk2.Dispose();
                        tsk2 = null;
                        tsk3.Dispose();
                        tsk3 = null;
                        tsk4.Dispose();
                        tsk4 = null;
                        tsk5.Dispose();
                        tsk5 = null;

                        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                        GC.Collect(2);
                        GC.WaitForPendingFinalizers();
                    }

                    param = null;
                    dllWorker = null;
                    last.UserId = UserId;
                    context.PhysDatas.Add(last);                                        
                    var task = context.Tasks.Where(c => c.UserId == UserId).FirstOrDefault();
                    task.StartStatus = 0;
                    context.SaveChanges();
                    context.Dispose();
                    last.Dispose();
                    listArray.Dispose();
                    listArray = null;
                    physDataLast.Dispose();

                    //GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();

                }
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            });  
            
            return Ok();
        }

        public byte[] DrawGraphicsVichr(DoubleArrayList list, InitialParam param, int NX, int NY) 
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartVichrImage(list, param, NX, NY);
            FileStream fstream = new FileStream("Vichr.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();         
            fstream.Dispose();
            return imgByte;
        }

        public byte[] DrawGraphicsUxy(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartUxyImage(list, param, NX, NY);
            FileStream fstream = new FileStream("Uxy.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();
            fstream.Dispose();
            return imgByte;
        }

        public byte[] DrawGraphicsFlow(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartPileImage(list, param, NX, NY);
            FileStream fstream = new FileStream("Pile.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();
            fstream.Dispose();
            return imgByte;
        }

        public byte[] DrawGraphicsVxy(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartVxyImage(list, param, NX, NY);
            FileStream fstream = new FileStream("Vxy.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();
            fstream.Dispose();
            fstream = null;
            return imgByte;
        }

        public byte[] DrawGraphicsModV(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartModuleImage(list, param, NX, NY);
            FileStream fstream = new FileStream("Module.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();
            fstream.Dispose();
            fstream = null;
            return imgByte;
        }

        public byte[] DrawGraphicsFunToka(DoubleArrayList list, InitialParam param, int NX, int NY)
        {
            DllDirectX directXdll = new DllDirectX();
            directXdll.StartFunctionTokaImage(list, param, NX, NY);
            FileStream fstream = new FileStream("FunctionToka.png", FileMode.Open);
            byte[] imgByte = new byte[fstream.Length];
            fstream.Read(imgByte, 0, (int)fstream.Length);
            fstream.Close();        
            fstream.Dispose();
            return imgByte;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void ParseDoubleToString(PhysData physData, DoubleArrayList list)
        {
            physData.lab3afstring = String.Join(";", list.lab3afsharp.Select(p => p.ToString()).ToArray());
            physData.lab3aNd1string = String.Join(";", list.lab3aNd1sharp.Select(p => p.ToString()).ToArray());
            physData.lab3aNd2string = String.Join(";", list.lab3aNd2sharp.Select(p => p.ToString()).ToArray());
            physData.lab3aPsi1string = String.Join(";", list.lab3aPsi1sharp.Select(p => p.ToString()).ToArray());
            physData.lab3aPsi2string = String.Join(";", list.lab3aPsi2sharp.Select(p => p.ToString()).ToArray());
            physData.lab3au1string = String.Join(";", list.lab3au1sharp.Select(p => p.ToString()).ToArray());
            physData.lab3au2string = String.Join(";", list.lab3au2sharp.Select(p => p.ToString()).ToArray());
            physData.lab3av1string = String.Join(";", list.lab3av1sharp.Select(p => p.ToString()).ToArray());
            physData.lab3av2string = String.Join(";", list.lab3av2sharp.Select(p => p.ToString()).ToArray());
            physData.lab3aVi1string = String.Join(";", list.lab3aVi1sharp.Select(p => p.ToString()).ToArray());
            physData.lab3aVi2string = String.Join(";", list.lab3aVi2sharp.Select(p => p.ToString()).ToArray());
        }

        public DoubleArrayList ParseStringToDouble(PhysData physData)
        {
            DoubleArrayList doublelist = new DoubleArrayList();
            
            doublelist.lab3afsharp = Array.ConvertAll(physData.lab3afstring.Split(';'), Double.Parse);
            doublelist.lab3aNd1sharp = Array.ConvertAll(physData.lab3aNd1string.Split(';'), Double.Parse);
            doublelist.lab3aNd2sharp = Array.ConvertAll(physData.lab3aNd2string.Split(';'), Double.Parse);
            doublelist.lab3aPsi1sharp = Array.ConvertAll(physData.lab3aPsi1string.Split(';'), Double.Parse);
            doublelist.lab3aPsi2sharp = Array.ConvertAll(physData.lab3aPsi2string.Split(';'), Double.Parse);
            doublelist.lab3au1sharp = Array.ConvertAll(physData.lab3au1string.Split(';'), Double.Parse);
            doublelist.lab3au2sharp = Array.ConvertAll(physData.lab3au2string.Split(';'), Double.Parse);
            doublelist.lab3av1sharp = Array.ConvertAll(physData.lab3av1string.Split(';'), Double.Parse);
            doublelist.lab3av2sharp = Array.ConvertAll(physData.lab3av2string.Split(';'), Double.Parse);
            doublelist.lab3aVi1sharp = Array.ConvertAll(physData.lab3aVi1string.Split(';'), Double.Parse);
            doublelist.lab3aVi2sharp = Array.ConvertAll(physData.lab3aVi2string.Split(';'), Double.Parse);

            return doublelist;
        }
    }
}
