using System;

namespace LemonBot.Commands
{
    public record CommandDescriptor
    {
        public string Prefix { get; init; }

        public string HelpText { get; init; }

        public Type CommandType { get; init; }
    }
}
