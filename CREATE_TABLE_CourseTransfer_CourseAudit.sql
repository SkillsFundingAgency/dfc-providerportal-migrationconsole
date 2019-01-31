USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseTransfer_CourseAudit]    Script Date: 30/01/2019 14:18:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseTransfer_CourseAudit](
	[CourseMigrationCourseAuditId] [int] IDENTITY(1,1) NOT NULL,
	[Ukprn] [int] NULL,
	[TransferMethod] [int] NULL,
	[BatchNumber] [int] NULL,
	[MigrationDate] [datetime] NULL,
	[CourseId] [int] NULL,
	[LARS] [varchar](10) NULL,
	[RecordStatus] [int] NULL,
	[CourseRuns] [int] NULL,
	[CourseRunsLive] [int] NULL,
	[CourseRunsPending] [int] NULL,
	[MigrationSuccess] [int] NULL,
	[CourseMigrationNote] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CourseTransfer_CourseAudit]  WITH CHECK ADD  CONSTRAINT [FK_UkprnCourseMigration_CourseAudit_UkprnCourseMigration] FOREIGN KEY([Ukprn])
REFERENCES [dbo].[CourseMigration] ([Ukprn])
GO

ALTER TABLE [dbo].[CourseTransfer_CourseAudit] CHECK CONSTRAINT [FK_UkprnCourseMigration_CourseAudit_UkprnCourseMigration]
GO


