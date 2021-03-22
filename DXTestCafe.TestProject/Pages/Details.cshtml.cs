using DXTestCafe.TestProject.Code;
using DXTestCafe.TestProject.Models;
using DXTestCafe.TestProject.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Pages
{
	public class DetailsModel : PageModel
	{
		private readonly ILogger<DetailsModel> _logger;
		readonly IDataStore<int, DTOPost> postStore;
		readonly UserManager<ApplicationUser> userManager;
		readonly SignInManager<ApplicationUser> signInManager;
		readonly IDataStore<int, DTOReply> replyStore;

		public DetailsModel(ILogger<DetailsModel> logger,
			IDataStore<int, DTOPost> postStore, IDataStore<int, DTOReply> replyStore,
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			this.replyStore = replyStore;
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.postStore = postStore;
			_logger = logger;
		}
		//[BindProperty]
		//public int TogglePostId { get; set; }

		[BindProperty(SupportsGet = true)]
		public string NickName { get; set; }

		[BindProperty(SupportsGet = true)]
		public string SEOName { get; set; }

		[BindProperty]
		public ReplyViewModel ReplyModel { get; set; }

		public DTOPost CurrentPost { get; set; }
		public List<DTOReply> Replies { get; set; }
		public bool IsPostOwner { get; private set; }
		public int ResponseCount { get; private set; }

		private bool isAuthenticated = false;
		private DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
		private string userId;
		private ApplicationUser user;
		private async Task InitModel()
		{
			isAuthenticated = User.Identity.IsAuthenticated;
			var posts = await postStore.SelectAsync(p => p.Nickname == NickName && p.SEOName == SEOName, 0, 0);
			CurrentPost = posts.FirstOrDefault();

			user = await userManager.GetUserAsync(User);
			userId = userManager.GetUserId(User);
			IsPostOwner = userId == CurrentPost?.OwnerId;

			if (!IsPostOwner && (!CurrentPost.Active || CurrentPost.PostDate > now))
			{
				CurrentPost = null;
			}
		}

		public async Task<IActionResult> OnGetAsync()
		{
			if (string.IsNullOrWhiteSpace(NickName) || string.IsNullOrWhiteSpace(SEOName))
				return NotFound();

			await InitModel();


			if (CurrentPost == null)
			{
				return NotFound();
			}


			Replies = await replyStore.SelectAsync(
				r => r.PostId == CurrentPost.Id && (IsPostOwner || r.Active),
				0,
				0, new ReplyStore.OrderBy<DateTime>(r => r.PostDate, true));
			ResponseCount = Replies.Count;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(int togglePostId = 0)
		{
			if (string.IsNullOrWhiteSpace(NickName) || string.IsNullOrWhiteSpace(SEOName))
				return NotFound();

			await InitModel();

			if (CurrentPost == null)
			{
				return NotFound();
			}

			if (!isAuthenticated)
			{
				throw new Exception(nameof(Unauthorized));
			}

			if (togglePostId > 0)
			{
				if (IsPostOwner)
				{
					DTOReply reply = replyStore.GetByKey(togglePostId);
					reply.Active = !reply.Active;
					await replyStore.UpdateAsync(reply);
				}
				else
					return Unauthorized();
			}
			else
			{
				if (!ModelState.IsValid)
				{
					return Page();
				}

				DTOReply newReply = new DTOReply
				{
					PostId = CurrentPost.Id,
					Remark = ReplyModel.Content,
					PostDate = DateTime.Now,
					//Active = true,
					OwnerId = userId
				};
				var r = await replyStore.CreateAsync(newReply);
			}
			return RedirectToPage();
		}

	}
}
