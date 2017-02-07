CREATE TABLE [dbo].[PatientAppointment]
(
	  [Id] int identity(1,1) not null primary key
	, [AppointmentDate] DateTime not null
	, [DurationInHours] tinyint not null
	, [ScheduledDate] DateTime not null
	, [ScheduledBy] varchar(20) not null default 'ScheduleSystem'
	, [Status] varchar(20) not null
	, [PreviousAppointmentId] int
	, [Disposition] varchar(50) null
	, [PatientId] int foreign key references [Patient]([Id])
	, [TherapistId] int foreign key references [Therapist]([Id]), 
)
