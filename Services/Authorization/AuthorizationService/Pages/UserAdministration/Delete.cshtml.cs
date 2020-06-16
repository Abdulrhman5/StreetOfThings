using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationLogic.AppUserCommands;
using CommonLibrary;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace AuthorizationService
{
    public class DeleteModel : PageModel
    {
        private IRepository<string, AppUser> _usersRepo;

        private UserControlDeletion _userDeleter;
        public DeleteModel(IRepository<string, AppUser> usersRepo, UserControlDeletion userDeleter)
        {
            _usersRepo = usersRepo;
            _userDeleter = userDeleter;
        }

        [BindProperty]
        public AppUser MyUser { get; set; }

        [BindProperty]
        public CommandResult CommandResult { get; set; }
        public IActionResult OnGet(string id)
        {
            MyUser = _usersRepo.Get(id);

            if(MyUser == null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var result = await _userDeleter.DeleteUser(MyUser.Id);
            if (result.IsSuccessful)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                CommandResult = result;
                return Page();
            }
        }
    }
}