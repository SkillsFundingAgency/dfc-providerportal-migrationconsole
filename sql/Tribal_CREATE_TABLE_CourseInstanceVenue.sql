USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[CourseInstanceVenue]    Script Date: 04/02/2019 14:05:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseInstanceVenue](
	[CourseInstanceId] [int] NOT NULL,
	[VenueId] [int] NOT NULL,
 CONSTRAINT [PK_CourseInstanceVenue] PRIMARY KEY CLUSTERED 
(
	[CourseInstanceId] ASC,
	[VenueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[CourseInstanceVenue]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstanceVenue_CourseInstance] FOREIGN KEY([CourseInstanceId])
--REFERENCES [dbo].[CourseInstance] ([CourseInstanceId])
--GO

--ALTER TABLE [dbo].[CourseInstanceVenue] CHECK CONSTRAINT [FK_CourseInstanceVenue_CourseInstance]
--GO

--ALTER TABLE [dbo].[CourseInstanceVenue]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstanceVenue_Venue] FOREIGN KEY([VenueId])
--REFERENCES [dbo].[Venue] ([VenueId])
--GO

--ALTER TABLE [dbo].[CourseInstanceVenue] CHECK CONSTRAINT [FK_CourseInstanceVenue_Venue]
--GO


