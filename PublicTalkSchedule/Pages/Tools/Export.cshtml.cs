using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using OfficeOpenXml;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Microsoft.AspNetCore.Authorization;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class ExportModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        private IHostingEnvironment _hostingEnvironment;

        public ExportModel(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }


        ////public IList<Speaker> Speaker { get; set; }


        //used to filter records as ALL Dates, Filled Dates, Open Dates
        [BindProperty]
        public string Option { get; set; }
        public string[] Options = new[] { " Congregation", " Talks", " Speakers", " Speaker's Talks" };     //NOTE the space at the beginning of the string


        // ***** success message *****
        [TempData]
        public string Message { get; set; }






        public void OnGet()
        {

        }



        public async Task<IActionResult> OnPostExportExcelAsync()

        {
            //determine data type (type of records to load). Options are congregation, talks, speakers or speaker's talks
            //then upload records based on the data type
            string dataType = Request.Form["Option"];

            // ------------------------------ congregation data table ------------------------------
            if (dataType == " Congregation")
            {
                var exSPK = await _db.Congregation.ToListAsync();


                // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.
                var stream = new MemoryStream();

                // the next line of code set the EPPlus LicenseContext parameter. this app uses the nonComerical license
                // setting thsi parameter is required for the following line of code to run
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(exSPK, true);
                    package.Save();
                }
                stream.Position = 0;

                string excelName = $"CongData-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                // above I define the name of the file using the current datetime.

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                // this will be the actual export.
            }


            // ------------------------------ talklist data table ------------------------------
            else if (dataType == " Talks")
            {
                var exSPK = await _db.TalkList.ToListAsync();


                // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.
                var stream = new MemoryStream();

                // the next line of code set the EPPlus LicenseContext parameter. this app uses the nonComerical license
                // setting thsi parameter is required for the following line of code to run
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(exSPK, true);
                    package.Save();
                }
                stream.Position = 0;

                string excelName = $"TalksData-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                // above I define the name of the file using the current datetime.

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                // this will be the actual export.
                                
            }


            // ------------------------------ speaker data table ------------------------------
            else if (dataType == " Speakers")
            {
                var exSPK = await _db.Speaker.ToListAsync();


                // above code loads the data using LINQ with EF (query of table), you can substitute this with any data source.
                var stream = new MemoryStream();

                // the next line of code set the EPPlus LicenseContext parameter. this app uses the nonComerical license
                // setting thsi parameter is required for the following line of code to run
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells.LoadFromCollection(exSPK, true);
                    package.Save();
                }
                stream.Position = 0;

                string excelName = $"SpeakerData-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                // above I define the name of the file using the current datetime.

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                // this will be the actual export.
            }

            
            // ------------------------------ speakers talk list data table ------------------------------
            else if (dataType == " Speaker's Talks")
            {
                Message = "TalkList records successfully exported";
            }
            
            //required to provide an exit path for the if/else if statement if none of the radio buttons are selected
            return Page();
                        
        }
        
    }
        
}