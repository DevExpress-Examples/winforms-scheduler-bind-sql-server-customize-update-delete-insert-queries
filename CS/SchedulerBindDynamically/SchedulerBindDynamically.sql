SET NOCOUNT ON
GO

USE master
GO
if exists (select * from sysdatabases where name='SchedulerBindDynamically')
	drop database SchedulerBindDynamically
GO

DECLARE @device_directory NVARCHAR(520)
SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)
FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1

EXECUTE (N'CREATE DATABASE SchedulerBindDynamically
  ON PRIMARY (NAME = N''SchedulerBindDynamically'', FILENAME = N''' + @device_directory + N'SchedulerBindDynamically.mdf'')
  LOG ON (NAME = N''SchedulerBindDynamically_log'',  FILENAME = N''' + @device_directory + N'SchedulerBindDynamically.ldf'')')
GO

exec sp_dboption 'SchedulerBindDynamically','trunc. log on chkpt.','true'
exec sp_dboption 'SchedulerBindDynamically','select into/bulkcopy','true'
GO


USE [SchedulerBindDynamically]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CarScheduling](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Subject] [nvarchar](50) NULL,
	[TimeStamp] [datetime] NULL,
 CONSTRAINT [PK_CarScheduling] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO