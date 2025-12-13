using System.Collections.Generic;
using LetPortal.Core;
using LetPortal.Identity;
using LetPortal.Portal;
using LetPortal.Notification;
using LetPortal.Chat;
using LetPortal.Identity.AppParts.Controllers;
using LetPortal.Microservices.Server;
using LetPortal.Notification.AppParts.Controllers;
using LetPortal.Portal.AppParts.Controllers;
using LetPortal.ServiceManagement.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.HttpOverrides;
using LetPortal.Chat.Hubs;
using LetPortal.Notification.Hubs;
using LetPortal.Microservices.Server.Services;
using LetPortal.Core.Microservices.Configurations.Server;
using LetPortal.Notification.Services.Rpc;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddSaturnServerConfig(builder.Environment);
builder.WebHost.ConfigureKestrel(options =>
{
    // Important note: If Saturn is consuming Log Stream via HTTP2, the line size should be adjusted
    options.Limits.MaxRequestLineSize = 256 * 1024; // 256Kb
});
var services = builder.Services;
services.AddHttpContextAccessor();
services.AddMemoryCache();
// Enable IHttpClientFactory
services.AddHttpClient();
services.ActAsServer(builder.Configuration, options =>
{
    options.EnableDatabaseOptions = true;
    options.EnableSerilog = true;
    options.EnableServiceMonitor = true;
    options.EnableBuiltInCors = true;
    options.ServerName = "Saturn";
});

// Changed from 0.9.0: We need to separate DatabaseOptions
// This is very useful for reusability
services
    .AddDatabaseOptions(builder.Configuration)
        .RegisterIdentityRepos()
        .RegisterPortalRepos()
        .RegisterNotificationRepos()
        .RegisterSaturnServerRepos();

// Changed from 0.9.0: AddLetPortal is just providing Builder for combining all Features
services
    .AddLetPortal(builder.Configuration)
    .AddIdentity()
    .AddJwtValidator(
        hubSegments: new List<string>
        {
                        "/chathub",
                        "/videohub",
                        "/notificationhub"
        })
    .AddChat()
    .AddNotification()
    .AddPortal(options =>
    {
        options.EnableFileServer = true;
    });

services
    .AddControllers()
    .AddApplicationPart(typeof(AppsController).Assembly)
    .AddApplicationPart(typeof(AccountsController).Assembly)
    .AddApplicationPart(typeof(ConfigurationController).Assembly)
    .AddApplicationPart(typeof(NotificationsController).Assembly)
    .AddNewtonsoftJson(options =>
    {
        // Important note: we still use Newtonsoft instead of .NET JSON because they still don't support Timezone
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    });

// Call SignalR when Chat is used
services.AddSignalR();

// Enable Grpc for Microservices
services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 1 * 1024 * 1024 * 5; // 5 MB
    options.MaxSendMessageSize = 1 * 1024 * 1024 * 5; // 5 MB
});

services.AddOpenApiDocument();
var app = builder.Build();

if (builder.Environment.IsDevelopment() || builder.Environment.IsDockerEnv())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//app.UseHttpsRedirection();
app.UseSaturnServer(app.Lifetime, options =>
{
    options.AllowCheckTraceId = false;
    options.AllowCheckUserSessionId = false;
    options.AllowWrapException = true;
    options.UseBuiltInCors = true;
    options.UseGenerateTraceId = true;
    options.SkipCheckUrls = [
                        "swagger",
                        "api/accounts/login",
                        "api/accounts/forgot-password",
                        "api/accounts/recovery-password"];
});

app.UseOpenApi();
app.UseSwaggerUi();
app.UseReDoc();

app.UseDefaultFiles();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<HubChatClient>("/chathub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});
app.MapHub<HubVideoClient>("/videohub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});
app.MapHub<NotificationHubClient>("/notificationhub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});
app.MapGrpcService<ServiceConfigurationService>();
app.MapGrpcService<LogCollectorService>();
app.MapGrpcService<ServiceMonitorService>();
app.MapGrpcService<NotificationServiceRpc>();

app.MapFallbackToFile("/index.html");

app.Run();
