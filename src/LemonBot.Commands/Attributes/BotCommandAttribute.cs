using System;

namespace LemonBot.Commands.Attributes
{
    public class BotCommandAttribute : Attribute
    {
        public BotCommandAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; }

        public string HelpText { get; set; }
    }
}
