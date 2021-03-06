select * from [ScheduleChatBot].[dbo].[Patient];
select * from [ScheduleChatBot].[dbo].[Therapist];
select * from [ScheduleChatBot].[dbo].[TherapistSchedule];
select * from [ScheduleChatBot].[dbo].[PatientAppointment];

--get schedule for therapist
declare @TherapistId tinyint
set @TherapistId = 1 --Heather

select *
from   [ScheduleChatBot].[dbo].[TherapistSchedule]
where  [TherapistId] = @TherapistId;

--get appointments for patient
declare @PatientId tinyint
set @PatientId = 2 --Eric

select top 1 pa.*
       , p.[Id] as 'PatientId'
       , p.[FirstName] as 'PatientFirstName'
	   , p.[LastName] as 'PatientLastName'
	   , p.[ContactPhone] as 'PatientContactPhone'
	   , p.[EmailAddress] as 'PatientEmailAddress'
	   , t.[Id] as 'TherapistId'
	   , t.[FirstName] as 'TherapistFirstName'
	   , t.[LastName] as 'TherapistLastName'
	   , t.[Specialty] as 'TherapistSpecialty'
	   , t.[EmailAddress] as 'TherapistEmailAddress'
from   [ScheduleChatBot].[dbo].[PatientAppointment] pa
	inner join [ScheduleChatBot].[dbo].[Patient] p on pa.[PatientId] = p.[Id]
	inner join [ScheduleChatBot].[dbo].[Therapist] t on pa.[TherapistId] = t.[Id]
where  p.[ContactPhone] = '5551234'
   and pa.[AppointmentDate] >= sysdatetime()
   and pa.[Status] = 'Scheduled'
order by [AppointmentDate] asc;