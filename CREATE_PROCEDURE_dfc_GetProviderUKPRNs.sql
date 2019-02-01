USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_GetProviderUKPRNs]    Script Date: 01/02/2019 15:32:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_GetProviderUKPRNs] 

AS
BEGIN

	SELECT [Ukprn]
	FROM [CourseMigration]
	WHERE [ReadyToMigrate] = 1

END

GO


