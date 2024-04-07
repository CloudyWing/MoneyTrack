using System;

namespace CloudyWing.MoneyTrack.Infrastructure.Options {
    public class AccountOptions {
        public AccountOptions(string userId, string password) {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }

        public string UserId { get; }

        public string Password { get; }
    }
}
