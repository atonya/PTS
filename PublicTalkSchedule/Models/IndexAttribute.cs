using System;

namespace PublicTalkSchedule.Models
{
    internal class IndexAttribute : Attribute
    {
        public bool IsUnique { get; set; }
    }
}