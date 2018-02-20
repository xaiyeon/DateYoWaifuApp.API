using DateYoWaifuApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DateYoWaifuApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        // public DbSet<Value> Values { get; set;}

        public DbSet<User> Users { get; set;}

        public DbSet<Photo> Photos { get; set;}

        // This need to override the onModel create class
        public DbSet<Like> Likes { get; set;}

        public DbSet<Message> Messages { get; set;}

        
        protected override void OnModelCreating(ModelBuilder builder) {
            // This is for the user likes
            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});
            builder.Entity<Like>()
                .HasOne(usr => usr.Likee)
                .WithMany(usr => usr.Liker)
                .HasForeignKey(usr => usr.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                .HasOne(usr => usr.Liker)
                .WithMany(usr => usr.Likee)
                .HasForeignKey(usr => usr.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // This is for user messages
            builder.Entity<Message>()
                .HasOne(x => x.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
                .HasOne(x => x.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}