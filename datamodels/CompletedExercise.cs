using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abituria.datamodels
{
    class CompletedExercise
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public bool IsComplete { get; set; }
    }
}