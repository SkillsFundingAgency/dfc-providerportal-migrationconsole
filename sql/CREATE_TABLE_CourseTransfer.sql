USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseTransfer]    Script Date: 01/02/2019 15:18:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseTransfer](
	[CourseTransferId] [int] IDENTITY(1,1) NOT NULL,
	[StartTransferDateTime] [datetime] NOT NULL,
	[TransferMethod] [int] NOT NULL,
	[DeploymentEnvironment] [int] NULL,
	[CreatedById] [nvarchar](128) NULL,
	[CreatedByName] [nvarchar](255) NULL,
	[Ukprn] [int] NULL,
	[CountProvidersToBeMigrated] [int] NULL,
	[CountProvidersMigrated] [int] NULL,
	[CountProvidersNotMigrated] [int] NULL,
	[CountAllCoursesToBeMigrated] [int] NULL,
	[CountCoursesGoodToBeMigrated] [int] NULL,
	[CountCoursesNotGoodToBeMigrated] [int] NULL,
	[CountCoursesGoodToBeMigratedLive] [int] NULL,
	[CountCoursesGoodToBeMigratedPending] [int] NULL,
	[CountAllCoursesLARSless] [int] NULL,
	[CountAllCoursesMigrated] [int] NULL,
	[CountAllCoursesNotMigrated] [int] NULL,
	[CompleteTransferDateTime] [datetime] NULL,
	[TimeTaken] [varchar](50) NULL,
	[BulkUploadFileName] [nvarchar](1000) NULL,
	[AdminReportFileName] [varchar](255) NULL,
	[TransferNote] [nvarchar](max) NULL,
 CONSTRAINT [PK_CourseTransfer] PRIMARY KEY CLUSTERED 
(
	[CourseTransferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


