USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[LearningAim]    Script Date: 04/02/2019 12:07:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[LearningAim](
	[LearningAimRefId] [varchar](10) NOT NULL,
	[Qualification] [varchar](150) NOT NULL,
	[LearningAimTitle] [nvarchar](255) NOT NULL,
	[LearningAimAwardOrgCode] [nvarchar](20) NOT NULL,
	[ErAppStatus] [nvarchar](50) NULL,
	[SkillsForLife] [nvarchar](5) NULL,
	[QualificationTypeId] [int] NULL,
	[IndependentLivingSkills] [bit] NOT NULL,
	[LearnDirectClassSystemCode1] [nvarchar](12) NULL,
	[LearnDirectClassSystemCode2] [nvarchar](12) NULL,
	[LearnDirectClassSystemCode3] [nvarchar](12) NULL,
	[RecordStatusId] [int] NOT NULL,
	[QualificationLevelId] [int] NULL,
 CONSTRAINT [PK_LearningAimReference] PRIMARY KEY CLUSTERED 
(
	[LearningAimRefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[LearningAim] ADD  DEFAULT ((0)) FOR [IndependentLivingSkills]
--GO

--ALTER TABLE [dbo].[LearningAim] ADD  DEFAULT ((2)) FOR [RecordStatusId]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_LearnDirect1] FOREIGN KEY([LearnDirectClassSystemCode1])
--REFERENCES [dbo].[LearnDirectClassification] ([LearnDirectClassificationRef])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_LearnDirect1]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_LearnDirect2] FOREIGN KEY([LearnDirectClassSystemCode2])
--REFERENCES [dbo].[LearnDirectClassification] ([LearnDirectClassificationRef])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_LearnDirect2]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_LearnDirect3] FOREIGN KEY([LearnDirectClassSystemCode3])
--REFERENCES [dbo].[LearnDirectClassification] ([LearnDirectClassificationRef])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_LearnDirect3]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_LearningAimAwardOrg] FOREIGN KEY([LearningAimAwardOrgCode])
--REFERENCES [dbo].[LearningAimAwardOrg] ([LearningAimAwardOrgCode])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_LearningAimAwardOrg]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_QualificationTypeId] FOREIGN KEY([QualificationTypeId])
--REFERENCES [dbo].[QualificationType] ([QualificationTypeId])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_QualificationTypeId]
--GO

--ALTER TABLE [dbo].[LearningAim]  WITH CHECK ADD  CONSTRAINT [FK_LearningAim_RecordStatus] FOREIGN KEY([RecordStatusId])
--REFERENCES [dbo].[RecordStatus] ([RecordStatusId])
--GO

--ALTER TABLE [dbo].[LearningAim] CHECK CONSTRAINT [FK_LearningAim_RecordStatus]
--GO


