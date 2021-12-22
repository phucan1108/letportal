using LetPortal.Core;
using LetPortal.Core.Logger;
using LetPortal.Core.Persistences;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Hubs;
using LetPortal.Notification.Repositories.Channels;
using LetPortal.Notification.Repositories.MessageGroups;
using LetPortal.Notification.Repositories.Messages;
using LetPortal.Notification.Repositories.NotificationBoxMessages;
using LetPortal.Notification.Repositories.NotificationMessageQueues;
using LetPortal.Notification.Repositories.Subcribers;
using LetPortal.Notification.Services;
using LetPortal.Notification.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Notification
{
    public static class NotificationExtensions
    {
        public const string NOTIFICATION_POLICY_CORS = "NotificationCors";
        public static ILetPortalBuilder AddNotificationService(
            this ILetPortalBuilder builder)
        {
            RegisterServices(builder.Services);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(NOTIFICATION_POLICY_CORS, corsBuilder =>
                {
                    corsBuilder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(builder.CorsOptions.AllowedHosts.ToArray())
                            .AllowCredentials()
                            .WithExposedHeaders(LetPortal.Core.Constants.TokenExpiredHeader);
                });
            });

            builder.Services.AddHostedService<GlobalChannelBackgroundService>();
            builder.Services.AddHostedService<IndividualChannelBackgroundService>();
            builder.Services.AddHostedService<RoleChannelBackgroundService>();
            builder.Services.AddHostedService<MessageQueueBackgroundService>();

            builder.Services.AddTransient(typeof(NotificationHubClient), serviceProvider =>
            {
                return new NotificationHubClient(
                    serviceProvider.GetService<NotificationRealTimeContext>(),
                    serviceProvider.GetService<IServiceLogger<NotificationHubClient>>(),
                    serviceProvider.GetService<ISubcriberService>());
            });
            return builder;
        }

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IChannelService, ChannelService>();
            services.AddTransient<ISubcriberService, SubcriberService>();
            services.AddSingleton<NotificationRealTimeContext>();
            services.AddSingleton<GlobalNotificationChannel>();
            services.AddSingleton<RolesNotificationChannel>();
            services.AddSingleton<IndividualNotificationChannel>();
        }

        public static IApplicationBuilder UseNotificationCors(this IApplicationBuilder builder)
        {
            return builder.UseCors(NOTIFICATION_POLICY_CORS);
        }

        public static IDatabaseOptionsBuilder RegisterNotificationRepos(this IDatabaseOptionsBuilder builder)
        {
            builder.Services.RegisterRepos(builder.DatabaseOptions);

            return builder;
        }

        public static void RegisterRepos(
            this IServiceCollection services,
            DatabaseOptions databaseOptions)
        {
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                services.AddSingleton<IChannelRepository, ChannelMongoRepository>();
                services.AddSingleton<IMessageGroupRepository, MessageGroupMongoRepository>();
                services.AddSingleton<IMessageRepository, MessageMongoRepository>();
                services.AddSingleton<INotificationBoxMessageRepository, NotificationBoxMessageMongoRepository>();
                services.AddSingleton<ISubscriberRepository, SubcriberMongoRepository>();
                services.AddSingleton<INotificationMessageQueueRepository, NotificationMessageQueueMongoRepository>();
            }
        }
    }
}
