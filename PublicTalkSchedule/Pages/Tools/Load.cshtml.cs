using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PublicTalkSchedule.Data;
using PublicTalkSchedule.Models;
using PublicTalkSchedule.Utility;

namespace PublicTalkSchedule
{
    [Authorize(Roles = SD.DataEntryEndUser + "," + SD.AdminEndUser)]
    public class LoadModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private IHostingEnvironment _environment;
        public LoadModel(ApplicationDbContext db, IHostingEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

        // ***** success message *****
        [TempData]
        public string Message { get; set; }

        //variables used in OnPost Upload (OnPostUploadAsync)
        public StreamReader sr { get; set; }
        public string line { get; set; }
        

        //used to filter records as ALL Dates, Filled Dates, Open Dates
        [BindProperty]
        public string Option { get; set; }
        public string[] Options = new[] { " Congregation", " Talks", " Speakers", " Speaker's Talks" };     //NOTE the space at the beginning of the string


        [BindProperty]
        ////public TalkList TalkList { get; set; }
        public IFormFile Upload { get; set; }

        public string Result { get; private set; }


        // ********** OnGet **********
        public void OnGet()
        {

        }


        // ********** OnPost Upload **********
        public async Task OnPostUploadAsync()
        {
                       
            //path and file name of text file selected by user to read
            ////var file = Path.Combine(_environment.ContentRootPath, "uploads", Upload.FileName);
            var file = Path.Combine(_environment.ContentRootPath, "wwwroot/App_Data", Upload.FileName);


            //variables used to coount the number of records processed
            int lineCount = System.IO.File.ReadLines(file).Count();                 //total number of rows in the file
            int newRecCount = 0;                                                    //new records uploaded
            int updatedRecCount = 0;
            int skippedRecCount = 0;
            int totalRecCount = newRecCount + updatedRecCount + skippedRecCount;
            DayOfWeek mtgDay = 0;                                                   //used to convert string value to DayOfWeek

        //determine data type (type of records to load). Options are congregation, talks, speakers or speaker's talks
        //then upload records based on the data type
        string dataType = Request.Form["Option"];


            // --------------- congregation data records ---------------
            if (dataType == " Congregation")
            {
                //read the first string in the file
                sr = new System.IO.StreamReader(file);
                line = sr.ReadLine();


                //setp through the file one row at a time, parseing the fields
                do
                {
                    var text = line.Split('|');
                    string strId = text[0];
                    var id = Int32.Parse(strId);
                    string congName = text[1];
                    string cirNum = text[2];
                    string kh_address = text[3];
                    string kh_city = text[4];
                    string kh_phone = text[5];
                    string tc_lastName = text[6];
                    string tc_firstName = text[7];
                    string tc_mobilePhone = text[8];
                    string tc_homePhone = text[9];
                    string tc_email = text[10];
                    string cobe_lastName = text[11];
                    string cobe_firstName = text[12];
                    string cobe_mobilePhone = text[13];
                    string cobe_homePhone = text[14];
                    string strMtgDay = text[15];
                    string strMtgTime = text[16];
                    var mtgTime = DateTime.Parse(strMtgTime);

                    //convert from string (strMtgDay) to DayOfWeek (mtgDay)
                    switch (strMtgDay)
                    {
                        case "Sun.":
                            mtgDay = DayOfWeek.Sunday;
                            break;
                        case "Mon.":
                            mtgDay = DayOfWeek.Monday;
                            break;
                        case "Tue.":
                            mtgDay = DayOfWeek.Tuesday;
                            break;
                        case "Wed.":
                            mtgDay = DayOfWeek.Wednesday;
                            break;
                        case "Thu.":
                            mtgDay = DayOfWeek.Thursday;
                            break;
                        case "Fri.":
                            mtgDay = DayOfWeek.Friday;
                            break;
                        case "Sat.":
                            mtgDay = DayOfWeek.Saturday;
                            break;
                    }


                    //determine if the congregation number is already in the database
                    var CongIdQuery = from u in _db.Congregation.AsNoTracking()     //Seacch the db for a talk with an ID that matches id
                                      where u.Id == id
                                      select u.Id;

                    int congIdCount = await CongIdQuery.CountAsync();           //Determine how many records were returned, expecting 0


                    if (congIdCount < 1)        //if the congregation is not in the table, add it
                    {

                        _db.Congregation.AddRange(
                        new Congregation
                        {

                            Id = id, CongName = congName, CircuitNum = cirNum, khAddress = kh_address, khCity = kh_city,
                            khPhone = kh_phone, tcLastName = tc_lastName, tcFirstName = tc_firstName, tcMobilePhone = tc_mobilePhone,
                            tcHomePhone = tc_mobilePhone, tcEmail = tc_email, cobeLastName = cobe_lastName, cobeFirstName = cobe_firstName,
                            cobeMobilePhone = cobe_mobilePhone, cobeHomePhone = cobe_homePhone, MtgDay = mtgDay, MtgTime = mtgTime
                        }
                        );
                        _db.SaveChanges();

                        newRecCount = newRecCount + 1;
                    }
                    else
                    {

                        //update database by column based on congregation number
                        var congFromDB = await _db.Congregation.FirstOrDefaultAsync(s => s.Id == id);
                        //congFromDB.Id = Congregation.Id;
                        congFromDB.CircuitNum = cirNum;
                        congFromDB.CongName = congName;
                        congFromDB.khAddress = kh_address;
                        congFromDB.khCity = kh_city;
                        congFromDB.khPhone = kh_phone;
                        congFromDB.tcLastName = tc_lastName;
                        congFromDB.tcFirstName = tc_firstName;
                        congFromDB.tcMobilePhone = tc_mobilePhone;
                        congFromDB.tcHomePhone = tc_homePhone;
                        congFromDB.tcEmail = tc_email;
                        congFromDB.cobeLastName = cobe_lastName;
                        congFromDB.cobeFirstName = cobe_firstName;
                        congFromDB.cobeMobilePhone = cobe_mobilePhone;
                        congFromDB.cobeHomePhone = cobe_homePhone;
                        congFromDB.MtgDay = mtgDay;
                        congFromDB.MtgTime = mtgTime;

                        await _db.SaveChangesAsync();

                        updatedRecCount = updatedRecCount + 1;
                    }

                    line = sr.ReadLine();

                }

                while (line != null);

                Message = newRecCount + " New Record  -  and  -  " + updatedRecCount + " Updated Records  -  equals -  " + lineCount + " Congregations successfully uploaded";
            }



            // ------------------------------ talk data records ------------------------------
            else if (dataType == " Talks")
            {
                //read the first string in the file
                sr = new System.IO.StreamReader(file);
                line = sr.ReadLine();

                //setp through the file one row at a time, parseing the fields
                do
                {
                    var text = line.Split('|');
                    string strId = text[0];
                    var id = Int32.Parse(strId);
                    string title = text[1];
                    string catDes = text[2];


                    //determine if the talk number is already in the database
                    var TalkIdQuery = from u in _db.TalkList.AsNoTracking()     //Seacch the db for a talk with an ID that matches id
                                      where u.Id == id
                                      select u.Id;

                    int talkIdCount = await TalkIdQuery.CountAsync();           //Determine how many records were returned, expecting 0

                    if (talkIdCount < 1)        //if the talk is not in the table, add it
                    {
                        _db.TalkList.AddRange(
                        new TalkList { Id = id, Title = title, CatDescription = catDes }
                        );
                        _db.SaveChanges();

                        newRecCount = newRecCount + 1;
                    }
                    else        //if the talk is in the table, update it
                    {
                        _db.TalkList.UpdateRange(
                        new TalkList { Id = id, Title = title, CatDescription = catDes }
                        );
                        _db.SaveChanges();

                        updatedRecCount = updatedRecCount + 1;
                    }

                    line = sr.ReadLine();

                }
                while (line != null);

                Message = newRecCount + "   New Records  -  and  -  " + updatedRecCount + "  Updated Records  -  equals " + lineCount + " Talks successfully uploaded";

            }




            // ------------------------------ speaker's data records ------------------------------
            else if (dataType == " Speakers")
            {
                //read the first string in the file
                sr = new System.IO.StreamReader(file);
                line = sr.ReadLine();

                //setp through the file one row at a time, parseing the fields
                do
                {
                    var text = line.Split('|');
                    string strId = text[0];
                    ////var id = Int32.Parse(strId);
                    ////string strSpkNum = text[1];
                    //// spkNum = Int32.Parse(strSpkNum);
                    string lastName = text[1];
                    string firstName = text[2];
                    string mobilPhone = text[3];
                    string homePhone = text[4];
                    string sEmail = text[5];
                    string eldMs = text[6];
                    string strCongNum = text[7];
                    var congNum = Int32.Parse(strCongNum);
                    string congName = text[8];

                    if (strId == "")            //if the speaker is not in the table, add him
                    {
                        _db.Speaker.AddRange(
                        new Speaker
                        {
                            //SpkNum = spkNum,
                            spkLastName = lastName,
                            spkFirstName = firstName,
                            spkMobilePhone = mobilPhone,
                            spkHomePhone = homePhone,
                            spkEmail = sEmail,
                            EldMs = eldMs,
                            CongId = congNum,
                            CongName = congName,
                        }
                        );
                        _db.SaveChanges();

                        newRecCount = newRecCount + 1;
                    }
                    else                         //if the speaker is in the table, update his data
                    {
                        //convert speaker number in datafile to an integer
                        var id = Int32.Parse(strId);


                        //update database by column
                        var speakerFromDB = await _db.Speaker.FirstOrDefaultAsync(s => s.Id == id);
                        //speakerFromDB.Id = id;
                        speakerFromDB.spkLastName = lastName;
                        speakerFromDB.spkFirstName = firstName;
                        speakerFromDB.spkMobilePhone = mobilPhone;
                        speakerFromDB.spkHomePhone = homePhone;
                        speakerFromDB.spkEmail = sEmail;
                        speakerFromDB.EldMs = eldMs;
                        speakerFromDB.CongId = congNum;
                        speakerFromDB.CongName = congName;

                        await _db.SaveChangesAsync();

                        updatedRecCount = updatedRecCount + 1;
                    }

                    line = sr.ReadLine();
                }
                while (line != null);

                Message = newRecCount + "   New Records  -  and  -  " + updatedRecCount + "  Updated Records  -  equals  -  " + lineCount + " Speakers successfully uploaded";

            }





            // ------------------------------ Speaker's assigned talks data ------------------------------
            else if (dataType == " Speaker's Talks")
            {
                //read the first string in the file
                sr = new System.IO.StreamReader(file);
                line = sr.ReadLine();

                //setp through the file one row at a time, parseing the fields
                do
                {                    
                    var text = line.Split('|');
                    string strSpkNum = text[0];
                    var spkNum = Int32.Parse(strSpkNum);
                    string strTalkNum = text[1];
                    var talkNum = Int32.Parse(strTalkNum);
                    string title = text[2];


                    //determine if the speaker/talk number is already in the database. 
                    //SpkNum and TalkNum combined must be qunick, query to confirm the count is either 0 or 1
                    var spkTalkIdQuery = from u in _db.SpeakerTalk.AsNoTracking()     //Seacch the db for a talk with an ID that matches id
                                         where u.SpkNum == spkNum && u.TalkNum == talkNum
                                      select u.Id;

                    int spkTalkIdCount = await spkTalkIdQuery.CountAsync();           //Determine how many records were returned, expecting 0

                    if (spkTalkIdCount == 0)        //if the speaker/talk is not in the table, add it
                    {
                        _db.SpeakerTalk.AddRange(
                        new SpeakerTalk { SpkNum = spkNum, TalkNum = talkNum, Title = title }
                        );
                        _db.SaveChanges();

                        newRecCount = newRecCount + 1;
                    }
                    else if (spkTalkIdCount == 1)       //if the speaker/talk number is in the table, update it
                    {
                        _db.SpeakerTalk.UpdateRange(
                        new SpeakerTalk { SpkNum = spkNum, TalkNum = talkNum, Title = title }
                        );
                        _db.SaveChanges();

                        updatedRecCount = updatedRecCount + 1;
                    }
                    else 
                    {
                        //do nothing
                        skippedRecCount = skippedRecCount + 1;
                    }

                    line = sr.ReadLine();

                }
                while (line != null);

                Message = newRecCount + "   New Records  -  plus  -  " + updatedRecCount + "  Updated Records  -  plus  -  " + skippedRecCount + "  Skipped Records" +
                                        "  -  equals " + lineCount + " Speaker/Talk records successfully processed";

            }
        }
    }
}
