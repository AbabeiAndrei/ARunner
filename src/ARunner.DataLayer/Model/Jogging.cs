using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ARunner.DataLayer.Model
{
    public class Jogging
    {
        public string Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Distance in meters
        /// </summary>
        [Required]
        public int Distance { get; set; }
         
        /// <summary>
        /// Time of jogging in minutes
        /// </summary>
        [Required]
        public int Time { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public RowState RowState { get; set; }
    }
}
