/****** Object:  StoredProcedure [dbo].[dfc_GetUserAuthorisationDetailsByEmail]    Script Date: 12/02/2019 16:04:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      <Author, , Name>
-- Create Date: <Create Date, , >
-- Description: <Description, , >
-- =============================================
CREATE PROCEDURE [dbo].[dfc_GetUserAuthorisationDetailsByEmail]
(
	@Email nvarchar (256)
)
AS
BEGIN

	  SELECT usr.id AS UserId 
				,usr.Email 
				,usr.UserName 
				,usr.[Name] AS NameOfUser 
				,roles.Id AS RoleId
				,roles.[Name] AS RoleName 
				,pr.Ukprn AS UKPRN
				--,usr.LockoutEnabled 
				--,usr.LockoutEndDateUtc
	  FROM [AspNetUsers] usr 
	  LEFT OUTER JOIN [AspNetUserRoles] usrrole ON usr.Id = usrrole.UserId
	  LEFT OUTER JOIN [AspNetRoles] roles ON usrrole.RoleId = roles.Id
	  LEFT OUTER JOIN [ProviderUser] prusr ON usr.Id = prusr.UserId
	  LEFT OUTER JOIN [Provider] pr ON prusr.ProviderId = pr.ProviderId
	  WHERE usr.[Email] =  @Email AND usr.[IsDeleted] = 0

END
GO


