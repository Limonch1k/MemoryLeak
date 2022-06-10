using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLab.Context.Models
{
    public class TaskExecution
    {
        public int Id { get; set; }

        public int StartStatus { get; set; }

        public int UserId { get; set; }
    }
}
