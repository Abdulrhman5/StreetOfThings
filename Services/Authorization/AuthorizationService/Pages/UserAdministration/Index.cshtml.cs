using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using Models;
using ApplicationLogic.AppUserQueries;

namespace AuthorizationService
{
    public class IndexModel : PageModel
    {
        private readonly IUserGetter _userService;


        public IndexModel(IUserGetter userService)
        {
             _userService = userService;
        }

        public IList<UserForAdministrationDto> Users { get;set; }

        public void OnGetAsync()
        {
            Users = _userService.GetUsers().ToList();

        }
    }
}
