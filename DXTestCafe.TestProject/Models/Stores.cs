using AutoMapper;
using DevExtreme.AspNet.Mvc;
using DXTestCafe.TestProject.Code;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Models
{
	public class PostStore : EFDataStore<ApplicationDbContext, int, DTOPost, ContentPost>
	{
		public PostStore(ApplicationDbContext context, IMapper mapper)
			: base(context, mapper, new DataValidator<ApplicationDbContext, int, DTOPost, ContentPost>())
		{

		}

		public override int DBModelKey(ContentPost model)
		{
			return model.Id;

		}

		public override int ModelKey(DTOPost model)
		{

			return model.Id;
		}


	}

	public class ReplyStore : EFDataStore<ApplicationDbContext, int, DTOReply, ContentReply>
	{
		public ReplyStore(ApplicationDbContext context, IMapper mapper)
			: base(context, mapper, new DataValidator<ApplicationDbContext, int, DTOReply, ContentReply>())
		{

		}

		public override int DBModelKey(ContentReply model)
		{
			return model.Id;
		}

		public override int ModelKey(DTOReply model)
		{
			return model.Id;
		}
	}
}
