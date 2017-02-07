using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduleSystemApi.Models
{
    public class RescheduleMessage
    {
        public int OldAppointmentId { get; set; }
        public Appointment NewAppointment { get; set; }
    }
}