using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ARunner.BusinessLogic.Models
{
    public class JoggingViewModel
    {
        public string Id { get; set; }
        
        [Required]
        public DateTime Created { get; set; }

        [Required]
        public int Distance { get; set; }

        [Required]
        public int Time { get; set; }

        [Required]
        public string UserId { get; set; }

        public float Average => Distance / (float)Time;
    }
}
