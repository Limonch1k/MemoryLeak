using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLab.Context.Models
{
    public class ImagePresentation : IDisposable
    {
        public int Id { get; set; }
        public string Image64 { get; set; }
        public int frame { get; set; }
        public int UserId { get; set; }

        public void Dispose()
        {
            Image64 = null;
            GC.Collect(2);
        }
    }
}
