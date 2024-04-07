using System.Security.Claims;
using CloudyWing.MoneyTrack.Infrastructure.Options;
using CloudyWing.MoneyTrack.Infrastructure.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace CloudyWing.MoneyTrack.Models.Application.IndexModel {
    public class LoginAppService : ApplicationService {
        private readonly AccountOptions accountOptions;
        public LoginAppService(IOptionsSnapshot<AccountOptions> accountOptionsAccessor, IServiceProvider? serviceProvider) : base(serviceProvider) {
            ExceptionUtils.ThrowIfNull(() => accountOptionsAccessor);

            accountOptions = accountOptionsAccessor.Value;
        }

        public async Task<bool> LoginAsync(LoginInputModel? input) {
            ExceptionUtils.ThrowIfNull(() => input);

            if (input.UserId?.Equals(accountOptions.UserId, StringComparison.CurrentCultureIgnoreCase) == true
                && input.Password == accountOptions.Password
            ) {
                List<Claim> claims = [
                    new Claim(ClaimTypes.NameIdentifier, input.UserId!),
                ];

                ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties {
                        AllowRefresh = true,
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    }
                );

                return true;
            }

            return false;
        }

        public async Task LogoutAsync() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
