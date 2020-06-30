using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SchedIncomingViewModel
    {
        public ScheduleIn ScheduleInObj { get; set; }
        public Speaker SpeakerObj { get; set; }
        public SpeakerTalk SpkTalkObj { get; set; }
        public IEnumerable<ScheduleIn> ScheduleIn { get; set; }
        public PagingInfo PagingInfo { get; set; }              //data from PagingInfo class
    }
}
