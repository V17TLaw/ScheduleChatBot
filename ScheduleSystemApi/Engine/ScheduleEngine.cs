using ScheduleSystemApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ScheduleSystemApi.Engine
{
    public class ScheduleEngine : IScheduleEngine
    {
        private readonly string _connectionString;

        public ScheduleEngine(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public Appointment GetAppointment(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Appointment, Patient, Therapist, Appointment>(
                    @"select   pa.*
                             , p.[Id] as 'PatientId'
                             , p.[FirstName]
	                         , p.[LastName]
	                         , p.[ContactPhone]
	                         , p.[EmailAddress]
	                         , t.[Id] as 'TherapistId'
	                         , t.[FirstName]
	                         , t.[LastName]
	                         , t.[Specialty]
	                         , t.[EmailAddress]
                      from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	                      inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	                      inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
                      where  pa.[Id] = @AppointmentId",
                    map: (appt, patient, therapist) =>
                    {
                        appt.Patient = patient;
                        appt.Therapist = therapist;
                        return appt;
                    },
                    splitOn: "PatientId,TherapistId",
                    param: new
                    {
                        AppointmentId = id
                    }).FirstOrDefault();
            }
        }

        public Appointment CreateAppointment(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            if (appointment.Patient == null)
                throw new ArgumentException("Cannot create an appointment without a patient");

            if (appointment.Therapist == null)
                throw new ArgumentException("Cannot create an appointment without a therapist");

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Appointment, Patient, Therapist, Appointment>(
                    @"declare @AppointmentResult as table (Id int)

                      insert into [ScheduleChatBot].[dbo].[PatientAppointment] ([AppointmentDate],[DurationInHours],[ScheduledDate],[ScheduledBy],[Status],[PreviousAppointmentId],[PatientId],[TherapistId]) output Inserted.Id into @AppointmentResult
                      values (@AppointmentDate,@DurationInHours,@ScheduledDate,@ScheduledBy,@Status,@PreviousAppointmentId,@PatientId,@TherapistId);

                      select   pa.*
                             , p.[Id] as 'PatientId'
                             , p.[FirstName]
	                         , p.[LastName]
	                         , p.[ContactPhone]
	                         , p.[EmailAddress]
	                         , t.[Id] as 'TherapistId'
	                         , t.[FirstName]
	                         , t.[LastName]
	                         , t.[Specialty]
	                         , t.[EmailAddress]
                      from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	                      inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	                      inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
                          inner join @AppointmentResult ar on pa.[Id] = ar.[Id]",
                    map: (appt, patient, therapist) =>
                    {
                        appt.Patient = patient;
                        appt.Therapist = therapist;
                        return appt;
                    },
                    splitOn: "PatientId,TherapistId",
                    param: new
                    {
                        appointment.AppointmentDate,
                        appointment.DurationInHours,
                        ScheduledDate = DateTime.Now,
                        appointment.ScheduledBy,
                        appointment.Status,
                        PreviousAppointmentId = appointment.PreviousAppointmentId <= 0 ? (Nullable<int>)null: appointment.PreviousAppointmentId,
                        appointment.Patient.PatientId,
                        appointment.Therapist.TherapistId
                    }).Single();
            }
        }

        private Appointment UpdateAppointmentStatus(int appointmentId, string status, string disposition)
        {
            if (appointmentId <= 0)
                throw new ArgumentException("An appointment id is required.", nameof(appointmentId));

            if (string.IsNullOrEmpty(status))
                throw new ArgumentNullException(nameof(status));

            var originalAppointment = GetAppointment(appointmentId);

            if (!string.IsNullOrEmpty(originalAppointment.Disposition) && string.IsNullOrEmpty(disposition))
                disposition = originalAppointment.Disposition;

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Appointment, Patient, Therapist, Appointment>(
                    @"update [ScheduleChatBot].[dbo].[PatientAppointment]
                      set    [Status] = @Status,
                             [Disposition] = @Disposition
                      where  [Id] = @AppointmentId;

                      select   pa.*
                             , p.[Id] as 'PatientId'
                             , p.[FirstName]
	                         , p.[LastName]
	                         , p.[ContactPhone]
	                         , p.[EmailAddress]
	                         , t.[Id] as 'TherapistId'
	                         , t.[FirstName]
	                         , t.[LastName]
	                         , t.[Specialty]
	                         , t.[EmailAddress]
                      from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	                      inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	                      inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
                      where  pa.[Id] = @AppointmentId",
                    map: (appointment, patient, therapist) =>
                    {
                        appointment.Patient = patient;
                        appointment.Therapist = therapist;
                        return appointment;
                    },
                    splitOn: "PatientId,TherapistId",
                    param: new
                    {
                        AppointmentId = appointmentId,
                        Status = status,
                        Disposition = disposition
                    }).FirstOrDefault();
            }
        }

        public Appointment CancelAppointment(int id, string disposition = null)
        {
            var appointment = GetAppointment(id);

            if (appointment == null)
                return null;

            return UpdateAppointmentStatus(appointment.Id, "Cancelled", disposition);
        }

        public Patient GetPatient(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Patient>(
                    @"select   [Id] as 'PatientId'
                             , [FirstName]
                             , [LastName]
                             , [ContactPhone]
                             , [EmailAddress]
                      from   [ScheduleChatBot].[dbo].[Patient]
                      where  [Id] = @PatientId", param: new { PatientId = id }).FirstOrDefault();
            }
        }

        public IEnumerable<Appointment> GetPatientAppointments(int patientId)
        {
            var patient = GetPatient(patientId);

            if (patient == null)
                return null;

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Appointment, Patient, Therapist, Appointment>(
                    @"select   pa.*
                             , p.[Id] as 'PatientId'
                             , p.[FirstName]
	                         , p.[LastName]
	                         , p.[ContactPhone]
	                         , p.[EmailAddress]
	                         , t.[Id] as 'TherapistId'
	                         , t.[FirstName]
	                         , t.[LastName]
	                         , t.[Specialty]
	                         , t.[EmailAddress]
                      from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	                      inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	                      inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
                      where  p.[Id] = @Id",
                    map: (appt, pt, therapist) =>
                    {
                        appt.Patient = pt;
                        appt.Therapist = therapist;
                        return appt;
                    },
                    splitOn: "PatientId,TherapistId",
                    param: new
                    {
                        Id = patient.PatientId
                    }).AsEnumerable();
            }
        }

        public Appointment RescheduleAppointment(int oldAppointmentId, Appointment newAppointment)
        {
            if (oldAppointmentId <= 0)
                throw new ArgumentException("An original appointment id is required.", nameof(oldAppointmentId));

            var oldAppointment = GetAppointment(oldAppointmentId);

            if (oldAppointment == null)
                throw new ArgumentException("Original appointment not found. Cannot reschedule.", nameof(oldAppointmentId));

            if (newAppointment == null)
                throw new ArgumentNullException(nameof(newAppointment));

            newAppointment.PreviousAppointmentId = oldAppointment.Id;
            var appointment = CreateAppointment(newAppointment);

            UpdateAppointmentStatus(oldAppointment.Id, "Rescheduled", null);

            return appointment;
        }

        public Appointment GetNextAppointmentByPatientPhoneNumber(string phoneNumber, DateTime? asOfDate = null)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                throw new ArgumentNullException(nameof(phoneNumber));

            var phone = Regex.Replace(phoneNumber, "[^0-9]", "");

            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Appointment, Patient, Therapist, Appointment>(
                    @"select top 1 pa.*
                           , p.[Id] as 'PatientId'
                           , p.[FirstName]
	                       , p.[LastName]
	                       , p.[ContactPhone]
	                       , p.[EmailAddress]
	                       , t.[Id] as 'TherapistId'
	                       , t.[FirstName]
	                       , t.[LastName]
	                       , t.[Specialty]
	                       , t.[EmailAddress]
                    from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	                    inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	                    inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
                    where  p.[ContactPhone] = @ContactPhone
                       and pa.[AppointmentDate] >= @AsOfDate
                       and pa.[Status] = 'Scheduled'
                    order by [AppointmentDate] asc",
                    map: (appt, pt, therapist) =>
                    {
                        appt.Patient = pt;
                        appt.Therapist = therapist;
                        return appt;
                    },
                    splitOn: "PatientId,TherapistId",
                    param: new
                    {
                        ContactPhone = phone,
                        AsOfDate = asOfDate ?? DateTime.Now
                    }).SingleOrDefault();
            }
        }
    }
}