using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abituria.datamodels
{
    class Account
    {
        public int Id { get; set; }
        public User AccountHolder { get; set; }
        public string Password { get; set; }
        public IEnumerable<CompletedExercise> CompletedExercises { get; set; }
    }
}