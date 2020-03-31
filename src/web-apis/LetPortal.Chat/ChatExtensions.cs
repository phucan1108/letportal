using LetPortal.Chat.Configurations;
using LetPortal.Chat.Hubs;
using LetPortal.Chat.Persistences;
using LetPortal.Chat.Repositories;
using LetPortal.Chat.Repositories.ChatRooms;
using LetPortal.Chat.Repositories.ChatSessions;
using LetPortal.Chat.Repositories.ChatUsers;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Chat
{
    public static class ChatExtensions
    {
        public const string CHAT_POLICY_CORS = "ChatCors";
        public static ILetPortalBuilder AddChat(this ILetPortalBuilder builder)
        {
            var services = builder.Services;
            services.AddSingleton<IChatContext, ChatContext>();
            if (builder.ConnectionType == ConnectionType.MongoDB)
            {
                MongoDbRegistry.RegisterEntities();
                builder.Services.AddSingleton<IChatRoomRepository, ChatRoomMongoRepository>();
                builder.Services.AddSingleton<IChatSessionRepository, ChatSessionMongoRepository>();
                builder.Services.AddSingleton<IChatUserRepository, ChatUserMongoRepository>();
            }

            if (builder.ConnectionType == ConnectionType.PostgreSQL
                || builder.ConnectionType == ConnectionType.MySQL
                || builder.ConnectionType == ConnectionType.SQLServer)
            {
                builder.Services.AddTransient<ChatDbContext>();
                builder.Services.AddTransient<IChatRoomRepository, ChatRoomEFRepository>();
                builder.Services.AddTransient<IChatSessionRepository, ChatSessionEFRepository>();
                builder.Services.AddTransient<IChatUserRepository, ChatUserEFRepository>();
            }

            services.AddTransient(typeof(HubChatClient), serviceProvider =>
            {
                return new HubChatClient(
                    serviceProvider.GetService<IChatContext>(),
                    serviceProvider.GetService<IChatRoomRepository>(),
                    serviceProvider.GetService<IChatSessionRepository>(),
                    serviceProvider.GetService<IChatUserRepository>());
            });

            var chatOptions = builder.Configuration.GetSection("ChatOptions").Get<ChatOptions>();
            services.AddCors(options =>
            {
                options.AddPolicy(CHAT_POLICY_CORS, builder =>
                {
                    builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(chatOptions.AllowedHosts.ToArray())
                            .AllowCredentials()
                            .WithExposedHeaders(LetPortal.Core.Constants.TokenExpiredHeader);
                });
            });

            return builder;
        }

        public static IApplicationBuilder UseChatCors(this IApplicationBuilder builder)
        {
            return builder.UseCors(CHAT_POLICY_CORS);
        }
    }
}
