USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[CourseInstanceStartDate]    Script Date: 04/02/2019 12:27:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseInstanceStartDate](
	[CourseInstanceStartDateId] [int] IDENTITY(1,1) NOT NULL,
	[CourseInstanceId] [int] NOT NULL,
	[StartDate] [date] NOT NULL,
	[IsMonthOnlyStartDate] [bit] NOT NULL,
	[PlacesAvailable] [int] NULL,
 CONSTRAINT [PK_CourseInstanceStartDates] PRIMARY KEY CLUSTERED 
(
	[CourseInstanceStartDateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[CourseInstanceStartDate] ADD  CONSTRAINT [DF_CourseInstanceStartDates_IsMonthOnlyStartDate]  DEFAULT ((0)) FOR [IsMonthOnlyStartDate]
--GO

--ALTER TABLE [dbo].[CourseInstanceStartDate]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstanceStartDate_CourseInstance] FOREIGN KEY([CourseInstanceId])
--REFERENCES [dbo].[CourseInstance] ([CourseInstanceId])
--GO

--ALTER TABLE [dbo].[CourseInstanceStartDate] CHECK CONSTRAINT [FK_CourseInstanceStartDate_CourseInstance]
--GO


