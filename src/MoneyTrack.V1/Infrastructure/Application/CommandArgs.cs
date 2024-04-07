using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    public class CommandArgs {
        public CommandArgs(object sender, string name, object arguments) {
            Sender = sender;
            Name = name;
            IEnumerable<string> args = string.IsNullOrEmpty(arguments?.ToString())
                ? Array.Empty<string>() : arguments.ToString()?.Split(',');
            Arguments = args.ToList().AsReadOnly();
        }

        public object Sender { get; }

        public string Name { get; }

        public IReadOnlyList<string> Arguments { get; }
    }
}
