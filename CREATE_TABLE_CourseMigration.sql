USE [SFA_CourseDirectory]
GO

/****** Object:  Table [dbo].[CourseMigration]    Script Date: 30/01/2019 14:17:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseMigration](
	[Ukprn] [int] NOT NULL,
	[ReadyToMigrate] [bit] NULL,
 CONSTRAINT [PK_UkprnCourseMigration] PRIMARY KEY CLUSTERED 
(
	[Ukprn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


