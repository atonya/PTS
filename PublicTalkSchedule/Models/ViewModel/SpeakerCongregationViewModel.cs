using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicTalkSchedule.Models.ViewModel
{
    public class SpeakerCongregationViewModel
    {
        public Speaker SpkList { get; set; }                    //data from Speaker class
        public Congregation CongList { get; set; }              //data from Congregation class
        public PagingInfo PagingInfo { get; set; }              //data from PagingInfo class
    }
}
