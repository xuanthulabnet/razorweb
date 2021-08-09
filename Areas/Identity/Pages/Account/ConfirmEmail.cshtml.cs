using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using razorweb.models;

namespace razorweb.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        public ConfirmEmailModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }


            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound($"Không tìm thấy User có ID = '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));   
            var result = await _userManager.ConfirmEmailAsync(user, code);


            StatusMessage = result.Succeeded ? "Email đã được xác thực" : "Lỗi xác thực email.";
            
            if (result.Succeeded)
            {
               await _signInManager.SignInAsync(user, false);
               return RedirectToPage("/Index");
               
            }
            else
            {
                return Content("Lỗi xác thực email.");
            }
        
            
            // return Page();
        }
    }
}
