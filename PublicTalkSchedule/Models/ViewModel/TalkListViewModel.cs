using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicTalkSchedule.Models;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class TalkListViewModel
    {
        public List<TalkList> TalkListList { get; set; }    //data from TalkList class
        public PagingInfo PagingInfo { get; set; }          //data from PagingInfo class
    }
}
