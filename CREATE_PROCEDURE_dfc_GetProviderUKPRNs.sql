USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_GetProviderUKPRNs]    Script Date: 29/01/2019 16:18:45 ******/
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
(
	@LastBatchNumber BIT OUTPUT
)
AS
BEGIN

	SELECT [Ukprn]
	FROM [UkprnCourseMigration]
	WHERE [ReadyToMigrate] = 1

	SELECT @LastBatchNumber = ISNULL(MAX([BatchNumber]), 0)
	FROM [UkprnCourseMigration_ProviderAudit]

END
GO


