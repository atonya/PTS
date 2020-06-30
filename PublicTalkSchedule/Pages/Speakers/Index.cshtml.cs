using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Models.ViewModel;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule.Pages.Speakers
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }


        // declare search variables
        public SelectList SpkId { get; set; }
        public string searchSpkName { get; set; }
        //public string searchSpkName {get;set;}
        public SelectList CongId { get; set; }
        public string searchCongName { get; set; }


        // ***** success message - displays message from create, edit or delete page *****
        [TempData]
        public string Message { get; set; }

        // ***** error message - displays message from create, edit or delete page *****
        [TempData]
        public string ErrorMessage { get; set; }

        

        [BindProperty]
        public SpeakerViewModel SpeakerVM { get; set; }


        //OnGet
        public async Task OnGetAsync(int spkId, string searchCongName, string searchSpkName, int productPage = 1)
        {
            SpeakerVM = new SpeakerViewModel()
            {
                SpeakerList = await _db.Speaker.ToListAsync()
            };



            // query speaker table to get a list of all speakers, insert them into the SelectList (SpkID)
            IQueryable<string> spknameQuery = from s in _db.Speaker       //query database for congregation based 
                                              orderby s.spkLastName, s.spkFirstName
                                              select s.FullName;

            SpkId = new SelectList(await spknameQuery.ToListAsync());



            // query congregation table to get a list of all congregations, insert them into the SelectList (CongId)
            IQueryable<string> congnameQuery = from n in _db.Congregation       //query database fo congregation based 
                                       orderby n.CongName
                                       select n.CongName;

            CongId = new SelectList(await congnameQuery.ToListAsync());



            // build pagination rules and set search fields and their criteria
            StringBuilder param = new StringBuilder();
            param.Append("/Speakers?productPage=:");           //used to track the speakers on a page
            param.Append("&searchSpkName=");


            // if a value is in the speaker name search field, append it to the query
            if (searchSpkName != null)
            {
                param.Append(searchSpkName);
            }

            // if a value is in the congregation search field, append it to the query
            param.Append("&searchCongName=");
            if (searchCongName != null)
            {
                param.Append(searchCongName);
            }



            // if a speaker name is selected in the SelectList filter the query using the value in the search box.
            if (searchSpkName != null)
            {
                //Parse last and first names from searchSpkName. use them in where clause of quey to select speaker
                //this step is required because FullName is [NotMapped] in database
                string lastName = searchSpkName.Substring(0, searchSpkName.IndexOf(','));
                string firstName = searchSpkName.Substring(searchSpkName.LastIndexOf(',') + 2);
                
                //query to select speaker
                SpeakerVM.SpeakerList = await _db.Speaker.Where(u => u.spkLastName == lastName).Where(u => u.spkFirstName == firstName).ToListAsync();

                //original query no longer supported in VS 2019 because FullName is [NotMapped] in database
                ////SpeakerVM.SpeakerList = await _db.Speaker.Where(u => u.FullName.ToLower().Contains(searchSpkName.ToLower())).ToListAsync();


            }
            else
            {
                // OR if a congregation name is selected in the SelectList filter the query using the value in the search box.
                if (searchCongName != null)
                {
                    SpeakerVM.SpeakerList = await _db.Speaker.Where(u => u.CongName.ToLower().Contains(searchCongName.ToLower())).ToListAsync();
                }
            }


            // ******************** code for page pagination ********************
            var count = SpeakerVM.SpeakerList.Count;                      //determine the number of records

            SpeakerVM.PagingInfo = new PagingInfo                          //set value fo properties in PageInfo class
            {
                CurrentPage = productPage,
                ItemsPerPage = SD.PaginationSpeakerPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            SpeakerVM.SpeakerList = SpeakerVM.SpeakerList.OrderBy(p => p.FullName)
                 .Skip((productPage - 1) * Utility.SD.PaginationSpeakerPageSize)
                 .Take(SD.PaginationSpeakerPageSize).ToList();
            
        }
    }
}