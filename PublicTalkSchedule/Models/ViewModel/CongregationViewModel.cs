using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class CongregationViewModel
    {
        public List<Congregation> CongList { get; set; }        //data from TalkList class
        public PagingInfo PagingInfo { get; set; }              //data from PagingInfo class
    }
}
