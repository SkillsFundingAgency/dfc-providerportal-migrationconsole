USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[Course]    Script Date: 04/02/2019 14:29:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Course](
	[CourseId] [int] IDENTITY(1,1) NOT NULL,
	[ProviderId] [int] NOT NULL,
	[CourseTitle] [nvarchar](255) NOT NULL,
	[CourseSummary] [nvarchar](2000) NULL,
	[CreatedByUserId] [nvarchar](128) NOT NULL,
	[CreatedDateTimeUtc] [datetime] NOT NULL,
	[ModifiedByUserId] [nvarchar](128) NULL,
	[ModifiedDateTimeUtc] [datetime] NULL,
	[AddedByApplicationId] [int] NOT NULL,
	[RecordStatusId] [int] NOT NULL,
	[LearningAimRefId] [varchar](10) NULL,
	[QualificationLevelId] [int] NULL,
	[EntryRequirements] [nvarchar](4000) NOT NULL,
	[ProviderOwnCourseRef] [nvarchar](255) NULL,
	[Url] [nvarchar](255) NULL,
	[BookingUrl] [nvarchar](255) NULL,
	[AssessmentMethod] [nvarchar](4000) NULL,
	[EquipmentRequired] [nvarchar](4000) NULL,
	[WhenNoLarQualificationTypeId] [int] NULL,
	[WhenNoLarQualificationTitle] [nvarchar](255) NULL,
	[AwardingOrganisationName] [nvarchar](150) NULL,
	[UcasTariffPoints] [int] NULL,
 CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_Application] FOREIGN KEY([AddedByApplicationId])
--REFERENCES [dbo].[Application] ([ApplicationId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_Application]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_CreatedByUserId]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_LearningAim] FOREIGN KEY([LearningAimRefId])
--REFERENCES [dbo].[LearningAim] ([LearningAimRefId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_LearningAim]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_ModifiedByUserId]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_Provider] FOREIGN KEY([ProviderId])
--REFERENCES [dbo].[Provider] ([ProviderId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_Provider]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_QualificationLevel] FOREIGN KEY([QualificationLevelId])
--REFERENCES [dbo].[QualificationLevel] ([QualificationLevelId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_QualificationLevel]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_QualificationType] FOREIGN KEY([WhenNoLarQualificationTypeId])
--REFERENCES [dbo].[QualificationType] ([QualificationTypeId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_QualificationType]
--GO

--ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK_Course_RecordStatus] FOREIGN KEY([RecordStatusId])
--REFERENCES [dbo].[RecordStatus] ([RecordStatusId])
--GO

--ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK_Course_RecordStatus]
--GO


