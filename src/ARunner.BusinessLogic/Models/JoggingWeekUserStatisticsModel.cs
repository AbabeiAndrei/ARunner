using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.BusinessLogic.Models
{
    public class JoggingWeekUserStatisticsModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int RunTotal { get; set; }
        public int TimeSpendRunning { get; set; }
        public double AverageSpeed { get; set; }
    }
}
