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
CREATE PROCEDURE dfc_GetLocationByLocationIdPerProvider
(
		 @LocationId int
		,@ProviderId int
)
AS
BEGIN


SELECT L.[LocationName]
      ,L.[AddressId]
      ,L.[Telephone]
      ,L.[Email]
      ,L.[Website]
	  ,A.AddressLine1
	  ,A.AddressLine2
	  ,A.Town
	  ,A.County
	  ,A.Postcode
	  ,GL.Lat AS Latitude
	  ,GL.Lng AS Longitude 
  FROM [Tribal].[Location] L
  LEFT OUTER JOIN [Tribal].[Address] A ON L.[AddressId] = A.[AddressId]
  LEFT OUTER JOIN [Tribal].[GeoLocation] GL ON A.[Postcode] = GL.[Postcode]
  WHERE RecordStatusId = 2 AND [LocationId] = @LocationId AND [ProviderId] = @ProviderId

END
GO

