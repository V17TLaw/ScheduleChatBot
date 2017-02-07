namespace Schedule.ChatBot.Models
{
    public class RescheduleMessage
    {
        public int OldAppointmentId { get; set; }
        public Appointment NewAppointment { get; set; }
    }
}