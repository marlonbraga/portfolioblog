using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace marlonbraga.dev.Models.Context {
	public class Context : DbContext {
		public Context(DbContextOptions<Context> options) : base(options){}
		public DbSet<Post> Posts { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<PostTag> PostTags { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<PostTag>()
                .HasKey(t => new { t.IdPost, t.IdTag });
            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.IdPost);
            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.IdTag);
        }
	}
}
