using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SpeakerTalkViewModel
    {
        public Speaker SpeakerObj { get; set; }
        public TalkList TalkObj { get; set; }
        public IEnumerable<SpeakerTalk> SpeakerTalks { get; set; }

        public List<TalkList> TalkNumList { get; set; }
        public List<TalkShoppingCart> TalkShoppingCart { get; set; }
    }
}
