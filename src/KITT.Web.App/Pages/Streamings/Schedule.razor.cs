﻿using Blazorise;
using KITT.Web.App.Clients;
using KITT.Web.Models.Streamings;
using Microsoft.AspNetCore.Components;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace KITT.Web.App.Pages.Streamings
{
    public partial class Schedule
    {
        [Inject]
        public IStreamingsClient Client { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public INotificationService Notification { get; set; }

        private ViewModel model = new();

        private string errorMessage;

        async Task ScheduleStreamingAsync()
        {
            try
            {
                await Client.ScheduleStreamingAsync(model.ToApiModel());
                await Notification.Success("Streaming scheduled successfully!");

                Navigation.NavigateTo("/streamings");
            }
            catch (ApplicationException ex)
            {
                errorMessage = ex.Message;
            }
        }

        class ViewModel
        {
            [Required]
            public string Title { get; set; }

            [Required]
            public string Slug { get; set; }

            [Required]
            public DateTime ScheduleDate { get; set; } = DateTime.Now;

            [Required]
            public TimeSpan StartingTime { get; set; } = DateTime.Now.TimeOfDay;

            [Required]
            public TimeSpan EndingTime { get; set; } = DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(1));

            [Required]
            public string HostingChannelUrl { get; set; }

            public string StreamingAbstract { get; set; }

            public ScheduleStreamingModel ToApiModel()
            {
                return new ScheduleStreamingModel
                {
                    Title = this.Title,
                    ScheduleDate = this.ScheduleDate,
                    EndingTime = this.ScheduleDate.Add(this.EndingTime),
                    HostingChannelUrl = $"https://www.twitch.tv/{this.HostingChannelUrl}",
                    Slug = this.Slug,
                    StartingTime = this.ScheduleDate.Add(this.StartingTime),
                    StreamingAbstract = this.StreamingAbstract
                };
            }
        }
    }
}
