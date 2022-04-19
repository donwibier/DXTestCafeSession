using AutoMapper;
using DXTestCafe.TestProject.Code;
using DXTestCafe.TestProject.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DXTestCafe.TestProject
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options => 
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection"))				
			);
			
			services.AddDatabaseDeveloperPageExceptionFilter();
			services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
			// Add framework services.
			services
				.AddRazorPages()
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

			services.AddAutoMapper(typeof(Startup).Assembly);

			services.AddScoped<IDataStore<int, DTOPost>, PostStore>();
			services.AddScoped<IDataStore<int, DTOReply>, ReplyStore>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();


			//feed some default users
			var seed = new DataInitializer(userManager, roleManager);
			seed.SeedData().GetAwaiter().GetResult();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapDefaultControllerRoute();
				endpoints.MapRazorPages();
			});
		}
	}

	public class DataInitializer
	{
		public readonly string[] Roles = new string[] { "NormalUser", "Administrator" };
		private readonly Tuple<string, string, string, string>[] Users = new Tuple<string, string, string, string>[]
		{
			new Tuple<string, string, string, string>("admin@localhost", "admin", "Test123$", "Administrator"),
			new Tuple<string, string, string, string>("don@localhost", "don", "Test456&", "NormalUser")
		};
		readonly UserManager<ApplicationUser> userManager;
		readonly RoleManager<IdentityRole> roleManager;

		public DataInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
		}
		public async Task SeedData()
		{
			await SeedRoles();
			await SeedUsers();
		}
		public async Task SeedRoles()
		{
			foreach (var role in Roles)
			{
				if (!(await roleManager.RoleExistsAsync(role)))
				{
					IdentityResult roleResult = await roleManager.CreateAsync(new IdentityRole { Name = role });
					if (!roleResult.Succeeded)
						throw new Exception(string.Join("\n", roleResult.Errors.Select(e => e.Description)));
				}
			}
		}

		public async Task SeedUsers()
		{
			foreach (var user in Users)
			{
				var u = await userManager.FindByNameAsync(user.Item1);
				if (u == null)
				{
					ApplicationUser newUser = new ApplicationUser
					{
						UserName = user.Item1,
						Email = user.Item1,
						SEOName = user.Item2,
						Nickname = user.Item2
					};
					var result = await userManager.CreateAsync(newUser, user.Item3);
					if (result.Succeeded)
					{
						var token = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
						await userManager.ConfirmEmailAsync(newUser, token);
						foreach (var r in user.Item4.Split(","))
						{
							var rr = await userManager.AddToRoleAsync(newUser, r);
							if (!rr.Succeeded)
								throw new Exception(string.Join("\n", rr.Errors.Select(e => e.Description)));
						}
					}
					else
					if (!result.Succeeded)
						throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
				}
			}
		}

	}

}
