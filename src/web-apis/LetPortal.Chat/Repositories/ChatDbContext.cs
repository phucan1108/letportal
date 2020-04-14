using LetPortal.Chat.Entities;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Chat.Repositories
{
    public class ChatDbContext : DbContext
    {
        public ConnectionType ConnectionType => _options.ConnectionType;

        public DbSet<ChatRoom> ChatRooms { get; set; }

        public DbSet<ChatSession> ChatSessions { get; set; }

        public DbSet<Participant> Participants { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<ChatUser> ChatUsers { get; set; }

        private readonly DatabaseOptions _options;

        public ChatDbContext(DatabaseOptions options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var chatRoomBuilder = modelBuilder.Entity<ChatRoom>();
            chatRoomBuilder
                .HasKey(a => a.Id);   

            chatRoomBuilder
                .HasMany(a => a.Participants)
                .WithOne(b => b.ChatRoom)
                .OnDelete(DeleteBehavior.Cascade);
            chatRoomBuilder
                .HasMany(a => a.Sessions)
                .WithOne(b => b.ChatRoom)
                .OnDelete(DeleteBehavior.Cascade);

            var participantBuilder = modelBuilder.Entity<Participant>();
            participantBuilder
                .HasKey(a => a.Id); 

            var chatSessionBuilder = modelBuilder.Entity<ChatSession>();
            chatSessionBuilder
                .HasKey(a => a.Id);
            chatSessionBuilder
                .HasMany(a => a.Conversations)
                .WithOne(b => b.ChatSession)
                .OnDelete(DeleteBehavior.Cascade);

            var conversationBuilder = modelBuilder.Entity<Conversation>();
            conversationBuilder
                .HasKey(a => a.Id);

            var chatUserBuilder = modelBuilder.Entity<ChatUser>();
            chatUserBuilder
                .HasKey(a => a.Id);

            if (ConnectionType == ConnectionType.MySQL)
            {
                chatUserBuilder.Property(a => a.Deactivate).HasColumnType("BIT");
            }

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToCamelCase(property.GetColumnName()));
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.MySQL)
            {
                optionsBuilder.UseMySql(_options.ConnectionString);
            }
        }

        private string ToCamelCase(string column)
        {
            return char.ToLowerInvariant(column[0]) + column.Substring(1);
        }
    }
}
