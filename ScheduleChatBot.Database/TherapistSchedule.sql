CREATE TABLE [dbo].[TherapistSchedule]
(
	  [Id] int identity(1,1) not null primary key
	, [DayOfWeek] varchar(10) not null
	, [ShiftStart] smallint not null
	, [ShiftEnd] smallint not null
	, [TherapistId] int not null foreign key references [Therapist]([Id])
)
