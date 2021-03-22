using AutoMapper;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Models
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<ApplicationUser, DTOOwner>()
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.Nickname, o => o.MapFrom(s => s.Nickname));

			CreateMap<ContentPost, DTOPost>()
				//.ForPath(d => d.Owner, o => o.MapFrom(s => s.Owner))
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.Url, o => o.MapFrom(s => $"/details/{s.Owner.Nickname}/{s.SEOName}"))
				.ForMember(d => d.Nickname, o => o.MapFrom(s => s.Owner.Nickname))
				.ForMember(d => d.OwnerId, o => o.MapFrom(s => s.Owner.Id))
				.ForMember(d => d.SEOName, o => o.MapFrom(s => s.SEOName))
				.ForMember(d => d.PostDate, o => o.MapFrom(s => s.PostDate))
				.ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
				.ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
				.ForMember(d => d.Content, o => o.MapFrom(s => s.Content));

			CreateMap<DTOPost, ContentPost>()
				.ForPath(s => s.Comments, opt => opt.Ignore())
				.ForPath(s => s.Owner, opt => opt.Ignore())
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.SEOName, o => o.MapFrom(s => s.SEOName))
				.ForMember(d => d.PostDate, o => o.MapFrom(s => s.PostDate))
				.ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
				.ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
				.ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
				.ForMember(d => d.OwnerId, o => o.MapFrom(s => s.OwnerId));


			//.ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
			//.ForMember(d => d.Owner.Nickname, o => o.MapFrom(s => s.Owner.Nickname))
			//.ForMember(d => d.Owner.Id, o => o.MapFrom(s => s.OwnerId))
			////.ForMember(d => d.ResponseCount, o => o.MapFrom(s => s.Comments.Where(f => f.Active).Count()))



			CreateMap<ContentReply, DTOReply>()
				.ForPath(d => d.Owner, o => o.MapFrom(s => s.Owner))
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.PostDate, o => o.MapFrom(s => s.PostDate))
				.ForMember(d => d.Nickname, o => o.MapFrom(s => s.Owner.Nickname))
				.ForMember(d => d.OwnerId, o => o.MapFrom(s => s.Owner.Id))
				.ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
				.ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
				.ForMember(d => d.PostId, o => o.MapFrom(s => s.PostId));

			CreateMap<DTOReply, ContentReply>()
				.ForPath(s => s.Owner, opt => opt.Ignore())
				.ForPath(s => s.Post, opt => opt.Ignore())
				.ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
				.ForMember(d => d.PostDate, o => o.MapFrom(s => s.PostDate))
				.ForMember(d => d.Active, o => o.MapFrom(s => s.Active))
				.ForMember(d => d.Remark, o => o.MapFrom(s => s.Remark))
				.ForMember(d => d.PostId, o => o.MapFrom(s => s.PostId))
				.ForMember(d => d.OwnerId, o => o.MapFrom(s => s.Owner.Id));





		}
	}
}
