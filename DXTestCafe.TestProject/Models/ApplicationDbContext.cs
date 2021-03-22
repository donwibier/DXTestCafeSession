using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DXTestCafe.TestProject.Models
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<ContentPost>()
				.HasMany(c => c.Comments)
				.WithOne(c => c.Post)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ApplicationUser>()
				.HasMany(p => p.OwnedPosts)
				.WithOne(p => p.Owner)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<ApplicationUser>()
				.HasMany(p => p.OwnedReplies)
				.WithOne(p => p.Owner)
				.OnDelete(DeleteBehavior.Cascade);

		}
	}
}

