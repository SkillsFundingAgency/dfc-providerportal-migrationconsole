USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferAdd]    Script Date: 01/02/2019 15:25:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_CourseTransferAdd] 
(
			@StartTransferDateTime datetime
           ,@TransferMethod int
           ,@DeploymentEnvironment int
           ,@CreatedById nvarchar(128)
           ,@CreatedByName nvarchar(255)
           ,@Ukprn int
		   ,@CourseTransferId int OUTPUT
)
AS
BEGIN

	INSERT INTO [CourseTransfer]
           ([StartTransferDateTime]
           ,[TransferMethod]
           ,[DeploymentEnvironment]
           ,[CreatedById]
           ,[CreatedByName]
           ,[Ukprn])
     VALUES
           (@StartTransferDateTime
           ,@TransferMethod
           ,@DeploymentEnvironment
           ,@CreatedById
           ,@CreatedByName
           ,@Ukprn)
		   
	SELECT @CourseTransferId = SCOPE_IDENTITY()

END
GO


