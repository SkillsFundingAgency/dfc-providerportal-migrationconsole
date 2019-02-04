USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[CourseInstance]    Script Date: 04/02/2019 14:15:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CourseInstance](
	[CourseInstanceId] [int] IDENTITY(1,1) NOT NULL,
	[CourseId] [int] NOT NULL,
	[RecordStatusId] [int] NOT NULL,
	[ProviderOwnCourseInstanceRef] [varchar](255) NULL,
	[OfferedByProviderId] [int] NULL,
	[DisplayProviderId] [int] NULL,
	[StudyModeId] [int] NULL,
	[AttendanceTypeId] [int] NULL,
	[AttendancePatternId] [int] NULL,
	[DurationUnit] [int] NULL,
	[DurationUnitId] [int] NULL,
	[DurationAsText] [nvarchar](150) NULL,
	[StartDateDescription] [nvarchar](150) NULL,
	[EndDate] [date] NULL,
	[TimeTable] [nvarchar](200) NULL,
	[Price] [decimal](10, 2) NULL,
	[PriceAsText] [nvarchar](1000) NULL,
	[AddedByApplicationId] [int] NOT NULL,
	[LanguageOfInstruction] [nvarchar](100) NULL,
	[LanguageOfAssessment] [nvarchar](100) NULL,
	[ApplyFromDate] [date] NULL,
	[ApplyUntilDate] [date] NULL,
	[ApplyUntilText] [nvarchar](100) NULL,
	[EnquiryTo] [nvarchar](255) NULL,
	[ApplyTo] [nvarchar](255) NULL,
	[Url] [nvarchar](255) NULL,
	[CanApplyAllYear] [bit] NOT NULL,
	[CreatedByUserId] [nvarchar](128) NOT NULL,
	[CreatedDateTimeUtc] [datetime] NOT NULL,
	[ModifiedByUserId] [nvarchar](128) NULL,
	[ModifiedDateTimeUtc] [datetime] NULL,
	[PlacesAvailable] [int] NULL,
	[BothOfferedByDisplayBySearched] [bit] NOT NULL,
	[VenueLocationId] [int] NULL,
	[OfferedByOrganisationId] [int] NULL,
	[DisplayedByOrganisationId] [int] NULL,
 CONSTRAINT [PK_CourseInstance] PRIMARY KEY CLUSTERED 
(
	[CourseInstanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[CourseInstance] ADD  DEFAULT ((0)) FOR [BothOfferedByDisplayBySearched]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_Application] FOREIGN KEY([AddedByApplicationId])
--REFERENCES [dbo].[Application] ([ApplicationId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_Application]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_AttendancePattern] FOREIGN KEY([AttendancePatternId])
--REFERENCES [dbo].[AttendancePattern] ([AttendancePatternId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_AttendancePattern]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_AttendanceType] FOREIGN KEY([AttendanceTypeId])
--REFERENCES [dbo].[AttendanceType] ([AttendanceTypeId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_AttendanceType]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_Course] FOREIGN KEY([CourseId])
--REFERENCES [dbo].[Course] ([CourseId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_Course]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_CreatedByUserId]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_DisplayByOrganisation] FOREIGN KEY([DisplayedByOrganisationId])
--REFERENCES [dbo].[Organisation] ([OrganisationId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_DisplayByOrganisation]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_DurationUnit] FOREIGN KEY([DurationUnitId])
--REFERENCES [dbo].[DurationUnit] ([DurationUnitId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_DurationUnit]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_ModifiedByUserId]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_OfferedByOrganisation] FOREIGN KEY([OfferedByOrganisationId])
--REFERENCES [dbo].[Organisation] ([OrganisationId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_OfferedByOrganisation]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_Provider_Display] FOREIGN KEY([DisplayProviderId])
--REFERENCES [dbo].[Provider] ([ProviderId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_Provider_Display]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_Provider_Offered] FOREIGN KEY([OfferedByProviderId])
--REFERENCES [dbo].[Provider] ([ProviderId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_Provider_Offered]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_RecordStatus] FOREIGN KEY([RecordStatusId])
--REFERENCES [dbo].[RecordStatus] ([RecordStatusId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_RecordStatus]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_StudyType] FOREIGN KEY([StudyModeId])
--REFERENCES [dbo].[StudyMode] ([StudyModeId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_StudyType]
--GO

--ALTER TABLE [dbo].[CourseInstance]  WITH CHECK ADD  CONSTRAINT [FK_CourseInstance_VenueLocation] FOREIGN KEY([VenueLocationId])
--REFERENCES [dbo].[VenueLocation] ([VenueLocationId])
--GO

--ALTER TABLE [dbo].[CourseInstance] CHECK CONSTRAINT [FK_CourseInstance_VenueLocation]
--GO


