USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_GetCoursesByProviderUKPRN]    Script Date: 01/02/2019 15:31:34 ******/
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
	@ProviderName NVARCHAR (200) OUTPUT,
	@AdvancedLearnerLoan BIT OUTPUT
)
AS
BEGIN

	 SELECT  [Ukprn] 
			,CourseId 
			,c.CourseTitle
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
	  WHERE [Ukprn] =  @ProviderUKPRN AND p.[RecordStatusId] = 2 AND c.[RecordStatusId] = 2
   
	SELECT @ProviderName = [ProviderName] 
	FROM [Provider] 
	WHERE [Ukprn] =  @ProviderUKPRN AND [RecordStatusId] = 2

	SELECT @AdvancedLearnerLoan = [Loans24Plus] 
	FROM [Provider] 
	WHERE [Ukprn] =  @ProviderUKPRN AND RecordStatusId = 2

END



GO


