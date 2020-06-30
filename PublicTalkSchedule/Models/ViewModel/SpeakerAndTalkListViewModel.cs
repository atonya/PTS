using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SpeakerAndTalkListViewModel
    {
        public Speaker SpeakerObj { get; set; }
        public TalkList TalkObj { get; set; }
        public IEnumerable<SpeakerTalk> SpkTalks { get; set; }
    }
}
