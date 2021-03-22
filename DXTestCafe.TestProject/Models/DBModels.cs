using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Models
{
	[Index(nameof(SEOName), IsUnique = true)]
	[Index(nameof(Nickname), IsUnique = true)]
	public class ApplicationUser : IdentityUser
	{
		public string AvatarUrl { get; set; }
		[MaxLength(100)]
		public string SEOName { get; set; }
		public string Nickname { get; set; }
		public ICollection<ContentPost> OwnedPosts { get; set; }
		public ICollection<ContentReply> OwnedReplies { get; set; }
	}

	[Index(nameof(Owner), nameof(SEOName), IsUnique = true)]
	public class ContentPost
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string SEOName { get; set; }
		public DateTime PostDate { get; set; }
		public bool Active { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public ICollection<ContentReply> Comments { get; set; }
		[ForeignKey(nameof(OwnerId))]
		public ApplicationUser Owner { get; set; }
		public string OwnerId { get; set; }
	}

	public class ContentReply
	{
		public int Id { get; set; }
		public DateTime PostDate { get; set; }
		public bool Active { get; set; }
		public string Remark { get; set; }


		[ForeignKey(nameof(PostId))]
		public ContentPost Post { get; set; }
		public int PostId { get; set; }

		[ForeignKey(nameof(OwnerId))]
		public ApplicationUser Owner { get; set; }
		public string OwnerId { get; set; }
	}
}
