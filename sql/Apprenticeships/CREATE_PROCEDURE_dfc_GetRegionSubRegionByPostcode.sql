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
CREATE PROCEDURE dfc_GetRegionSubRegionByPostcode
(
		@Postcode nvarchar(50)
)
AS
BEGIN

	SELECT R.GOR10NM AS [Region]
		,SR.LAD18NM AS [SubRegion]	
	FROM [ONSPD].[ONSPD_NOV_2018_UK] P
	JOIN [ONSPD].[ONSPD_Region] R ON P.rgn = R.GOR10CD
	JOIN  [ONSPD].[ONSPD_LA_UA] SR on SR.LAD18CD = P.oslaua
	WHERE  P.pcd = @Postcode

END
GO

