using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using razorweb.models;

namespace razorweb.Pages.Blog
{
    public class CreateModel : PageModel
    {
        private readonly razorweb.models.MyBlogContext _context;

        public CreateModel(razorweb.models.MyBlogContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            _context.Add(Article);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
