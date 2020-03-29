using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Configurations;
using LetPortal.Chat.Persistences;
using LetPortal.Chat.Repositories;
using LetPortal.Chat.Repositories.ChatRooms;
using LetPortal.Chat.Repositories.ChatSessions;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
            }

            if (builder.ConnectionType == ConnectionType.PostgreSQL
                || builder.ConnectionType == ConnectionType.MySQL
                || builder.ConnectionType == ConnectionType.SQLServer)
            {
                builder.Services.AddTransient<ChatDbContext>();
                builder.Services.AddTransient<IChatRoomRepository, ChatRoomEFRepository>();
                builder.Services.AddTransient<IChatSessionRepository, ChatSessionEFRepository>();
            }

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
