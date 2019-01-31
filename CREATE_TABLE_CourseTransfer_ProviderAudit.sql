USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseTransfer_ProviderAudit]    Script Date: 30/01/2019 14:20:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseTransfer_ProviderAudit](
	[CourseMigrationProviderAuditId] [int] IDENTITY(1,1) NOT NULL,
	[Ukprn] [int] NULL,
	[TransferMethod] [int] NULL,
	[BatchNumber] [int] NULL,
	[DeploymentEnvironment] [int] NULL,
	[MigrationDate] [datetime] NULL,
	[CoursesToBeMigrated] [int] NULL,
	[CoursesGoodToBeMigrated] [int] NULL,
	[CoursesGoodToBeMigratedPending] [int] NULL,
	[CoursesGoodToBeMigratedLive] [int] NULL,
	[CoursesNotGoodToBeMigrated] [int] NULL,
	[MigrationSuccesses] [int] NULL,
	[MigrationFailures] [int] NULL,
	[MigrationReportFileName] [varchar](255) NULL,
	[TimeTaken] [varchar](50) NULL,
	[MigrationNote] [nvarchar](max) NULL,
 CONSTRAINT [PK_UkprnCourseMigration_Audit] PRIMARY KEY CLUSTERED 
(
	[CourseMigrationProviderAuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CourseTransfer_ProviderAudit]  WITH CHECK ADD  CONSTRAINT [FK_UkprnCourseMigration_Audit_UkprnCourseMigration_Audit] FOREIGN KEY([Ukprn])
REFERENCES [dbo].[CourseMigration] ([Ukprn])
GO

ALTER TABLE [dbo].[CourseTransfer_ProviderAudit] CHECK CONSTRAINT [FK_UkprnCourseMigration_Audit_UkprnCourseMigration_Audit]
GO


