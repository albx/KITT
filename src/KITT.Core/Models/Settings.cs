using System;

namespace KITT.Core.Models
{
    public class Settings
    {
        public Guid Id { get; protected set; }

        public string UserId { get; protected set; }

        public string TwitchChannel { get; protected set; }

        #region Public methods
        public void Update(string twitchChannel)
        {
            if (string.IsNullOrWhiteSpace(twitchChannel))
            {
                throw new ArgumentException("value cannot be empty", nameof(twitchChannel));
            }

            TwitchChannel = twitchChannel;
        }
        #endregion

        #region Factory methods
        public static Settings CreateNew(string userId, string twitchChannel)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("value cannot be empty", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(twitchChannel))
            {
                throw new ArgumentException("value cannot be empty", nameof(twitchChannel));
            }

            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TwitchChannel = twitchChannel
            };

            return settings;
        }
        #endregion
    }
}
