using ScheduleSystemApi.Models;
using System;
using System.Collections.Generic;

namespace ScheduleSystemApi.Engine
{
    public interface IScheduleEngine
    {
        Appointment GetAppointment(int id);
        Appointment GetNextAppointmentByPatientPhoneNumber(string phoneNumber, DateTime? asOfDate = null);
        Appointment CreateAppointment(Appointment appointment);
        Appointment CancelAppointment(int id, string disposition = null);
        Patient GetPatient(int id);
        IEnumerable<Appointment> GetPatientAppointments(int patientId);
        Appointment RescheduleAppointment(int oldAppointmentId, Appointment newAppointment);
    }
}
