using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SchedOutgoingViewModel
    {
        public ScheduleOut ScheduleOutObj { get; set; }
        public List<Speaker> SpeakerObj { get; set; }
        public SpeakerTalk SpkTalkObj { get; set; }
       // public IEnumerable<ScheduleOut> ScheduleOut { get; set; }
        public Congregation CongObj { get; set; }

    }
}
