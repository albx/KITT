using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KITT.Web.Models.Lives
{
    public class CreateLiveModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime ScheduleDate { get; set; } = DateTime.Now;

        [Required]
        public TimeSpan StartingTime { get; set; } = DateTime.Now.TimeOfDay;

        [Required]
        public TimeSpan EndingTime { get; set; } = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

        public string TwitchChannelUrl { get; set; }

        public bool IsRecurring { get; set; }

        [Required]
        public string CategoryId { get; set; }
    }
}
