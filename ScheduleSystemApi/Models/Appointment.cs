using System;

namespace ScheduleSystemApi.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int DurationInHours { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ScheduledBy { get; set; }
        public string Status { get; set; }
        public string Disposition { get; set; }
        public int PreviousAppointmentId { get; set; }
        public Patient Patient { get; set; }
        public Therapist Therapist { get; set; }
    }
}