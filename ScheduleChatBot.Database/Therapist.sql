CREATE TABLE [dbo].[Therapist]
(
	  [Id] int identity(1,1) not null primary key
	, [FirstName] varchar(50) not null
	, [LastName] varchar(75) not null
	, [Specialty] varchar(50) not null
	, [EmailAddress] varchar(100) not null
)
