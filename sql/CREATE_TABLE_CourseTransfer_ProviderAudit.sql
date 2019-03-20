USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseTransfer_ProviderAudit]    Script Date: 01/02/2019 15:21:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseTransfer_ProviderAudit](
	[CourseMigrationProviderAuditId] [int] IDENTITY(1,1) NOT NULL,
	[CourseTransferId] [int] NULL,
	[Ukprn] [int] NULL,
	[CoursesToBeMigrated] [int] NULL,
	[CoursesGoodToBeMigrated] [int] NULL,
	[CoursesGoodToBeMigratedPending] [int] NULL,
	[CoursesGoodToBeMigratedLive] [int] NULL,
	[CoursesNotGoodToBeMigrated] [int] NULL,
	[CoursesLARSless] [int] NULL,
	[MigrationSuccesses] [int] NULL,
	[MigrationFailures] [int] NULL,
	[TimeTaken] [varchar](50) NULL,
	[ProviderReportFileName] [varchar](255) NULL,
	[MigrationNote] [nvarchar](max) NULL,
 CONSTRAINT [PK_UkprnCourseMigration_Audit] PRIMARY KEY CLUSTERED 
(
	[CourseMigrationProviderAuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CourseTransfer_ProviderAudit]  WITH CHECK ADD  CONSTRAINT [FK_CourseTransfer_ProviderAudit_CourseTransfer] FOREIGN KEY([CourseTransferId])
REFERENCES [dbo].[CourseTransfer] ([CourseTransferId])
GO

ALTER TABLE [dbo].[CourseTransfer_ProviderAudit] CHECK CONSTRAINT [FK_CourseTransfer_ProviderAudit_CourseTransfer]
GO


