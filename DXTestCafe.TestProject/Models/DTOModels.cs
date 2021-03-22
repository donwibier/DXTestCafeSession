using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Models
{
	public class DTOOwner
	{
		public string Id { get; set; }
		public string Nickname { get; set; }
	}
	public class DTOPost
	{
		public int Id { get; set; }
		public string SEOName { get; set; }
		public string Url { get; set; }
		public DateTime PostDate { get; set; }
		public bool Active { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }

		public ICollection<DTOReply> Comments { get; set; } = new List<DTOReply>();
		public string OwnerId { get; set; }

		//public int ResponseCount { get; }
		public DTOOwner Owner { get; set; } = new DTOOwner();
		public string Nickname { get; set; }
	}

	public class DTOReply
	{
		public int Id { get; set; }
		public DateTime PostDate { get; set; }
		public bool Active { get; set; }
		public string Remark { get; set; }
		public int PostId { get; set; }
		public string OwnerId { get; set; }
		public string Nickname { get; set; }

		public DTOOwner Owner { get; set; } = new DTOOwner();
	}
}
