USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferUpdate]    Script Date: 01/02/2019 15:29:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_CourseTransferUpdate] 
(
	   @CourseTransferId int
	  ,@CountProvidersToBeMigrated int
      ,@CountProvidersMigrated int
      ,@CountProvidersNotMigrated int
      ,@CountAllCoursesToBeMigrated int
      ,@CountCoursesGoodToBeMigrated int
      ,@CountCoursesNotGoodToBeMigrated int
      ,@CountCoursesGoodToBeMigratedLive int
      ,@CountCoursesGoodToBeMigratedPending INT
      ,@CountAllCoursesLARSless int
      ,@CountAllCoursesMigrated int
      ,@CountAllCoursesNotMigrated int
      ,@CompleteTransferDateTime datetime
      ,@TimeTaken varchar(50)
	  ,@BulkUploadFileName nvarchar(1000)
      ,@AdminReportFileName varchar(255)
      ,@TransferNote nvarchar(max)
)
AS
BEGIN
	
	UPDATE [CourseTransfer]
   SET [CountProvidersToBeMigrated] = @CountProvidersToBeMigrated
      ,[CountProvidersMigrated] = @CountProvidersMigrated
      ,[CountProvidersNotMigrated] = @CountProvidersNotMigrated
      ,[CountAllCoursesToBeMigrated] = @CountAllCoursesToBeMigrated
      ,[CountCoursesGoodToBeMigrated] = @CountCoursesGoodToBeMigrated
      ,[CountCoursesNotGoodToBeMigrated] = @CountCoursesNotGoodToBeMigrated
      ,[CountCoursesGoodToBeMigratedLive] = @CountCoursesGoodToBeMigratedLive
      ,[CountCoursesGoodToBeMigratedPending] = @CountCoursesGoodToBeMigratedPending
	  ,[CountAllCoursesLARSless] = @CountAllCoursesLARSless
      ,[CountAllCoursesMigrated] = @CountAllCoursesMigrated
      ,[CountAllCoursesNotMigrated] = @CountAllCoursesNotMigrated
      ,[CompleteTransferDateTime] = @CompleteTransferDateTime
      ,[TimeTaken] = @TimeTaken
	  ,[BulkUploadFileName] = @BulkUploadFileName
      ,[AdminReportFileName] = @AdminReportFileName
      ,[TransferNote] = @TransferNote
 WHERE [CourseTransferId] = @CourseTransferId

END
GO


