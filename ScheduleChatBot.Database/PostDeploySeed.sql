/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

delete from [dbo].[TherapistSchedule];
delete from [dbo].[PatientAppointment];
delete from [dbo].[Patient];
delete from [dbo].[Therapist];

--patients
insert into [dbo].[Patient] values ('Jane','Doe','5551234','janedoe@gmail.com');
insert into [dbo].[Patient] values ('Eric','Smith','5555678','ericsmith@hotmail.com');
insert into [dbo].[Patient] values ('Sarah','Brady','5559012','sbrady@yahoo.com');

--therapists
insert into [dbo].[Therapist] values ('Heather','Jones','Neurology','heather.jones@clinic.org');
insert into [dbo].[Therapist] values ('James','Arthur','Orthopedic','james.arthur@clinic.org');
insert into [dbo].[Therapist] values ('Kelly','James','Orthopedic','kelly.james@clinic.org');

--therapist schedule

--heather
insert into [dbo].[TherapistSchedule] values ('Monday',700,1730,1);
insert into [dbo].[TherapistSchedule] values ('Tuesday',700,1730,1);
insert into [dbo].[TherapistSchedule] values ('Wednesday',700,1730,1);
insert into [dbo].[TherapistSchedule] values ('Thursday',700,1730,1);

--james
insert into [dbo].[TherapistSchedule] values ('Monday',900,1630,2);
insert into [dbo].[TherapistSchedule] values ('Tuesday',900,1630,2);
insert into [dbo].[TherapistSchedule] values ('Wednesday',1000,1730,2);
insert into [dbo].[TherapistSchedule] values ('Thursday',900,1630,2);
insert into [dbo].[TherapistSchedule] values ('Friday',900,1630,2);

--kelly
insert into [dbo].[TherapistSchedule] values ('Monday',700,1630,3);
insert into [dbo].[TherapistSchedule] values ('Tuesday',900,1630,3);
insert into [dbo].[TherapistSchedule] values ('Wednesday',700,1630,3);
insert into [dbo].[TherapistSchedule] values ('Thursday',900,1630,3);
insert into [dbo].[TherapistSchedule] values ('Friday',700,1330,3);

--patient appointments
insert into [dbo].[PatientAppointment] values ('2017-04-04 8:00AM',1,sysdatetime(),'ScheduleSystem','Scheduled',null,null,2,1);
insert into [dbo].[PatientAppointment] values ('2017-04-05 10:00AM',1,sysdatetime(),'ScheduleSystem','Scheduled',null,null,1,2);
insert into [dbo].[PatientAppointment] values ('2017-04-05 3:00PM',1,sysdatetime(),'ScheduleSystem','Scheduled',null,null,3,3);