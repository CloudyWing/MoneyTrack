using System.Reflection;
using CloudyWing.MoneyTrack.Infrastructure.DependencyInjection;
using CloudyWing.MoneyTrack.Infrastructure.Localizations;
using CloudyWing.MoneyTrack.Infrastructure.Localizations.Resources;
using CloudyWing.MoneyTrack.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSession();
builder.Services.AddDependencies(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation()
    .AddMvcOptions(options => {
        Microsoft.AspNetCore.Mvc.ModelBinding.Metadata.DefaultModelBindingMessageProvider provider = options.ModelBindingMessageProvider;
        provider.SetAttemptedValueIsInvalidAccessor((x, y) => string.Format(ModelBindingMessage.AttemptedValueIsInvalid, x, y));
        provider.SetMissingBindRequiredValueAccessor(x => string.Format(ModelBindingMessage.MissingBindRequiredValue, x));
        provider.SetMissingKeyOrValueAccessor(() => ModelBindingMessage.MissingKeyOrValue);
        provider.SetMissingRequestBodyRequiredValueAccessor(() => ModelBindingMessage.MissingRequestBodyRequiredValue);
        provider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => string.Format(ModelBindingMessage.NonPropertyAttemptedValueIsInvalid, x));
        provider.SetNonPropertyUnknownValueIsInvalidAccessor(() => ModelBindingMessage.NonPropertyUnknownValueIsInvalid);
        provider.SetNonPropertyValueMustBeANumberAccessor(() => ModelBindingMessage.NonPropertyValueMustBeANumber);
        provider.SetUnknownValueIsInvalidAccessor(x => string.Format(ModelBindingMessage.UnknownValueIsInvalid, x));
        provider.SetValueIsInvalidAccessor(x => string.Format(ModelBindingMessage.ValueIsInvalid, x));
        provider.SetValueMustBeANumberAccessor(x => string.Format(ModelBindingMessage.ValueMustBeANumber, x));
        provider.SetValueMustNotBeNullAccessor(x => string.Format(ModelBindingMessage.ValueMustNotBeNull, x));

        options.ModelMetadataDetailsProviders.Add(new LocalizationValidationMetadataProvider(typeof(ValidationMetadataMessage)));
    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddOptions<AccountOptions>()
        .Bind(builder.Configuration.GetSection(AccountOptions.OptionsName))
        .ValidateDataAnnotations()
        .ValidateOnStart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
