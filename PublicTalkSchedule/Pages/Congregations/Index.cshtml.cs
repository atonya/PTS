using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Congregations
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }


        [BindProperty]
        public CongregationViewModel CongregationVM { get; set; }

        //used to have day of week appear in SelectList (aka dropdown)
        public DayOfWeek DayOfWeek { get; set; }


        //OnGet
        public async Task<IActionResult> OnGet(int productPage = 1, string searchCongName=null) 
        {
                        
            CongregationVM = new CongregationViewModel()
            {
                CongList = await _db.Congregation.ToListAsync()
            };

            
            StringBuilder param = new StringBuilder();
            param.Append("/Congregations?productPage=:");           //used to track the congregations on a page
            param.Append("&searchCongName=");

            // if a value is in the congregation name search field, append it to the query
            if (searchCongName != null)
            {
                param.Append(searchCongName);
            }

            // if a congregation name (or pat of a name) is entered int he search box,
            // filter the query using the value in the search box.
            if (searchCongName != null)
            {
                CongregationVM.CongList = await _db.Congregation.Where(u => u.CongName.ToLower().StartsWith(searchCongName.ToLower())).ToListAsync();
            }



            // ******************** code for page pagination ********************
            var count = CongregationVM.CongList.Count;                      //determine the number of records


            CongregationVM.PagingInfo = new PagingInfo                          //set value fo properties in PageInfo class
            {
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationCongregationPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            CongregationVM.CongList = CongregationVM.CongList.OrderBy(p => p.CongName)
                 .Skip((productPage - 1) * Utility.SD.PaginationCongregationPageSize)
                 .Take(SD.PaginationCongregationPageSize).ToList();
            
            return Page();
        }
            
    }
}