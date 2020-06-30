using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SpeakerViewModel
    {
        public List<Speaker> SpeakerList { get; set; }          //data from TalkList class
        public PagingInfo PagingInfo { get; set; }              //data from PagingInfo class
    }
}
