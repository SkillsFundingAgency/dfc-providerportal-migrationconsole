-- =======================================================
-- Create Stored Procedure Template for Azure SQL Database
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE dfc_GetApprenticeshipLocationsByApprenticeshipId
(
		@ApprenticeshipId int 
)
AS
BEGIN

 SELECT  [ApprenticeshipLocationId]
      ,[ApprenticeshipId]
      ,[LocationId]
      ,[Radius]
  FROM [Tribal].[ApprenticeshipLocation]
  WHERE RecordStatusId = 2 AND [ApprenticeshipId] = @ApprenticeshipId

END
GO

