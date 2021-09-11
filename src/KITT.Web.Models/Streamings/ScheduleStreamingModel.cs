using System;
using System.ComponentModel.DataAnnotations;

namespace KITT.Web.Models.Streamings
{
    public class ScheduleStreamingModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        public DateTime ScheduleDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime StartingTime { get; set; }

        [Required]
        public DateTime EndingTime { get; set; }

        [Required]
        public string HostingChannelUrl { get; set; }

        public string StreamingAbstract { get; set; }
    }
}
