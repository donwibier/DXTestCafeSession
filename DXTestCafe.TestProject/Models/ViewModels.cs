using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Models
{
	public class PostViewModel
	{
		[Required]
		public string Title { get; set; }
		[Required]
		public string Content { get; set; }
		public bool Active { get; set; }
		public DateTime PostDate { get; set; }

	}

	public class ReplyViewModel
	{
		[Required]
		public string Content { get; set; }
		public bool Active { get; set; }

	}
}
