USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseTransfer_CourseAudit]    Script Date: 01/02/2019 15:19:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseTransfer_CourseAudit](
	[CourseMigrationCourseAuditId] [int] IDENTITY(1,1) NOT NULL,
	[CourseTransferId] [int] NULL,
	[Ukprn] [int] NULL,
	[CourseId] [int] NULL,
	[LARS] [varchar](10) NULL,
	[CourseRecordStatus] [int] NULL,
	[CourseRuns] [int] NULL,
	[CourseRunsLive] [int] NULL,
	[CourseRunsPending] [int] NULL,
	[CourseRunsReadyToGoLive] [int] NULL,
	[CourseRunsLARSless] [int] NULL,
	[MigrationSuccess] [int] NULL,
	[CourseMigrationNote] [nvarchar](max) NULL,
 CONSTRAINT [PK_CourseTransfer_CourseAudit] PRIMARY KEY CLUSTERED 
(
	[CourseMigrationCourseAuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CourseTransfer_CourseAudit]  WITH CHECK ADD  CONSTRAINT [FK_CourseTransfer_CourseAudit_CourseTransfer] FOREIGN KEY([CourseTransferId])
REFERENCES [dbo].[CourseTransfer] ([CourseTransferId])
GO

ALTER TABLE [dbo].[CourseTransfer_CourseAudit] CHECK CONSTRAINT [FK_CourseTransfer_CourseAudit_CourseTransfer]
GO
