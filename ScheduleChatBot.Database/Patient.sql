CREATE TABLE [dbo].[Patient]
(
	  [Id] int identity(1,1) not null primary key
	, [FirstName] varchar(50) not null
	, [LastName] varchar(75) not null
	, [ContactPhone] varchar(14)
	, [EmailAddress] varchar(100)
)
