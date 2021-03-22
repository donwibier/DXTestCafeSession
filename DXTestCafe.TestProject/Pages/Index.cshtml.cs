using DXTestCafe.TestProject.Code;
using DXTestCafe.TestProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DXTestCafe.TestProject.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		readonly IDataStore<int, DTOPost> postStore;
		readonly UserManager<ApplicationUser> userManager;
		readonly SignInManager<ApplicationUser> signInManager;

		public IndexModel(ILogger<IndexModel> logger, IDataStore<int, DTOPost> postStore,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.postStore = postStore;
			_logger = logger;
		}
		[BindProperty]
		public PostViewModel PostModel { get; set; }

		public List<DTOPost> Posts { get; private set; }
		public async Task OnGetAsync()
		{
			var r =
				await postStore.SelectAsync(p => p.Active, 0, 0, new PostStore.OrderBy<DateTime>(p => p.PostDate, true));
			Posts = r;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (!User.Identity.IsAuthenticated)
			{
				throw new Exception(nameof(Unauthorized));
			}

			var user = await userManager.GetUserAsync(User);
			var id = userManager.GetUserId(User);
			if (string.IsNullOrWhiteSpace(id))
			{
				return NotFound(
					$"Unable to load user with ID '{id}'.");
			}
			DTOPost newPost = new DTOPost
			{
				Active = true,
				Title = PostModel.Title,
				Content = PostModel.Content,
				PostDate = PostModel.PostDate,
				SEOName = PostModel.Title.MakeSEOSafe(),
				OwnerId = id
			};
			var r = await postStore.CreateAsync(newPost);

			return RedirectToPage();
		}
	}

}
