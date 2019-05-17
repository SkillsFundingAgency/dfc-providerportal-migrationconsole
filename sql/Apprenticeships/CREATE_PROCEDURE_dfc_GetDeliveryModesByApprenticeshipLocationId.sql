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
CREATE PROCEDURE dfc_GetDeliveryModesByApprenticeshipLocationId
(
		@ApprenticeshipLocationId int
)
AS
BEGIN

  SELECT [DeliveryModeId]
  FROM [Tribal].[ApprenticeshipLocationDeliveryMode]
  WHERE [ApprenticeshipLocationId] = @ApprenticeshipLocationId

END
GO

