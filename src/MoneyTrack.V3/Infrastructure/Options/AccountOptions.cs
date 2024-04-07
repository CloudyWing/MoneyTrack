using System.ComponentModel.DataAnnotations;

namespace CloudyWing.MoneyTrack.Infrastructure.Options {
    public class AccountOptions {
        public const string OptionsName = "Account";

        [Required]
        public string UserId { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
