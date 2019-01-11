USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_GetCoursesByProviderUKPRN]    Script Date: 11/01/2019 14:34:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_GetCoursesByProviderUKPRN] 
(
	@ProviderUKPRN INT,
	@ProviderName NVARCHAR (200) OUTPUT
)
AS
BEGIN

SELECT  [Ukprn] 
		,CourseId 
		,CourseTitle 
		,c.LearningAimRefId 
		,c.QualificationLevelId 
		,la.LearningAimAwardOrgCode 
		,la.Qualification 
		,c.CourseSummary 
		,c.EntryRequirements
		,c.AssessmentMethod 
		,c.EquipmentRequired 
  FROM [Provider] p 
  LEFT OUTER JOIN [Course] c ON p.ProviderId = c.ProviderId
  LEFT OUTER JOIN [LearningAim] la ON c.LearningAimRefId = la.LearningAimRefId
  WHERE [Ukprn] =  @ProviderUKPRN
   
SELECT @ProviderName = [ProviderName] FROM [Provider] WHERE [Ukprn] =  @ProviderUKPRN

END
GO


