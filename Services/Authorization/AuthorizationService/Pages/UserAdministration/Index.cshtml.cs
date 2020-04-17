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
        private readonly IUserService _userService;


        public IndexModel(IUserService userService)
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
