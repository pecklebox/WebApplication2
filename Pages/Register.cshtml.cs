using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.ViewModels;

namespace WebApplication1.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<IdentityUser> userManager { get; }
        private SignInManager<IdentityUser> signInManager { get; }

		private readonly RoleManager<IdentityRole> roleManager;

		[BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager; 
            this.signInManager = signInManager;
            this.roleManager = roleManager;

		}

        public void OnGet()
        {
        }

        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email
                };

				IdentityRole role = await roleManager.FindByIdAsync("Admin");
				if (role == null)
				{
					IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
					if (!result2.Succeeded)
					{
						ModelState.AddModelError("", "Create role admin failed");
					}
				}


				IdentityRole roleHR = await roleManager.FindByIdAsync("HR");
				if (role == null)
				{
					IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("HR"));
					if (!result2.Succeeded)
					{
						ModelState.AddModelError("", "Create role HR failed");
					}
				}

				var result = await userManager.CreateAsync(user, RModel.Password); 
                if (result.Succeeded)
                {
					result = await userManager.AddToRoleAsync(user, "Admin");
					result = await userManager.AddToRoleAsync(user, "HR");
					await signInManager.SignInAsync(user, false); return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return Page();
        }

    }
}
