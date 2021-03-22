using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace DXTestCafe.TestProject.Code
{
	public enum NavigationPosition
	{
		Left,
		Top,
		Right,
		Bottom
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class NavigationAttribute : Attribute
	{
		public NavigationAttribute(string title, string url, int order, string iconClass, NavigationPosition position)
		{
			Title = title;
			Position = position;
			IconClass = iconClass;
			Order = order;
			Url = url;
		}
		public virtual string Title { get; protected set; }
		public NavigationPosition Position { get; }
		public string IconClass { get; protected set; }
		public int Order { get; protected set; }
		public string Url { get; protected set; }
	}

	public class NavigationItem
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public string IconClass { get; set; }
		public int Order { get; set; }
	}
	public class NavigationHelper
	{
		class AuthHelper
		{
			public string Schemes { get; set; }
			public string Policy { get; set; }
			public string Roles { get; set; }
		}
		class NavHelper
		{
			public Type Tpe { get; set; }
			public string Authorized { get; set; }
			public NavigationAttribute[] Attributes { get; set; }
			public AuthHelper[] Auth { get; set; }
		}

		static ConcurrentDictionary<Assembly, IEnumerable<NavHelper>> cache =
			new ConcurrentDictionary<Assembly, IEnumerable<NavHelper>>();


		static IEnumerable<NavHelper> getCacheItem(Assembly asm)
		{
			var results = from t in asm.GetTypes()
						  let attributes = t.GetCustomAttributes(typeof(NavigationAttribute), true)
						  let auth = t.GetCustomAttributes<AuthorizeAttribute>(true).ToArray()
						  where attributes != null && attributes.Length > 0
						  select new NavHelper
						  {
							  Tpe = t,
							  Attributes = attributes.Cast<NavigationAttribute>().ToArray(),
							  Auth =
							  (auth == null || auth.Length == 0)
								  ? new AuthHelper[]
									  { }
								  : (from a in auth
									 select new AuthHelper
									 { Policy = a.Policy, Roles = a.Roles, Schemes = a.AuthenticationSchemes }).ToArray()
						  };

			return results;
		}
		static IEnumerable<NavHelper> GetItems(Assembly asm)
		{
			var result = cache.GetOrAdd(asm, getCacheItem(asm));

			return result;
		}

		public static IEnumerable<NavigationItem> GetItems(Assembly asm, NavigationPosition position)
		{
			var items = from n in GetItems(asm)
						let attr = n.Attributes.FirstOrDefault(a => a.Position == position)
						where n.Attributes.Any(a => a.Position == position)
						select new NavigationItem
						{
							Title = attr.Title,
							IconClass = attr.IconClass,
							Url = attr.Url,
							Order = attr.Order
						};

			return items.OrderBy(i => i.Order).ThenBy(i => i.Title).ToArray();
		}
	}
}
