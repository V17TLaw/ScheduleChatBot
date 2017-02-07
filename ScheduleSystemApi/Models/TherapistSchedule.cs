using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleSystemApi.Models
{
    public class TherapistSchedule
    {
        public int Id { get; set; }
        public string DayOfWeek { get; set; }
        public int ShiftStart { get; set; }
        public int ShiftEnd { get; set; }
    }
}