USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_GetCourseInstancesByCourseId]    Script Date: 01/02/2019 15:31:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_GetCourseInstancesByCourseId] 
(
	@CourseId int
)
AS
BEGIN

	SELECT [CourseId] 
			,civ.[VenueId] 
			,ci.[CourseInstanceId]
			,[ProviderOwnCourseInstanceRef]  
			,[AttendanceTypeId]
			,[StartDateDescription]
			,cisd.[StartDate]
			,[Url]
			,[Price]
			,[PriceAsText]
			,[DurationUnitId]
			,[DurationUnit]
			,[StudyModeId]
			,[AttendanceTypeId]
	  FROM [CourseInstance] ci
	  LEFT OUTER JOIN [CourseInstanceVenue] civ ON ci.CourseInstanceId = civ.CourseInstanceId
	  LEFT OUTER JOIN [CourseInstanceStartDate] cisd ON ci.CourseInstanceId = cisd.CourseInstanceId  
	  WHERE [CourseId] = @CourseId AND [RecordStatusId] = 2

END
GO


