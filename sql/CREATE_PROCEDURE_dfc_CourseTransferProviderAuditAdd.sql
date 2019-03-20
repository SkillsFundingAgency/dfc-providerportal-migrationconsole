USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferProviderAuditAdd]    Script Date: 01/02/2019 15:28:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_CourseTransferProviderAuditAdd]
(
			@CourseTransferId int
           ,@Ukprn int
           ,@CoursesToBeMigrated int
           ,@CoursesGoodToBeMigrated int
           ,@CoursesGoodToBeMigratedPending int
           ,@CoursesGoodToBeMigratedLive int
           ,@CoursesNotGoodToBeMigrated INT
           ,@CoursesLARSless int
           ,@MigrationSuccesses int
           ,@MigrationFailures int
           ,@ProviderReportFileName varchar(255)
           ,@TimeTaken varchar(50)
           ,@MigrationNote nvarchar(max)
)
AS
BEGIN

	INSERT INTO [CourseTransfer_ProviderAudit]
           ([CourseTransferId]
		   ,[Ukprn]
           ,[CoursesToBeMigrated]
           ,[CoursesGoodToBeMigrated]
           ,[CoursesGoodToBeMigratedPending]
           ,[CoursesGoodToBeMigratedLive]
           ,[CoursesNotGoodToBeMigrated]
		   ,[CoursesLARSless]
           ,[MigrationSuccesses]
           ,[MigrationFailures]
           ,[ProviderReportFileName]
           ,[TimeTaken]
           ,[MigrationNote])
     VALUES
           (@CourseTransferId
		   ,@Ukprn
           ,@CoursesToBeMigrated
           ,@CoursesGoodToBeMigrated
           ,@CoursesGoodToBeMigratedPending
           ,@CoursesGoodToBeMigratedLive
           ,@CoursesNotGoodToBeMigrated
		   ,@CoursesLARSless
           ,@MigrationSuccesses
           ,@MigrationFailures
           ,@ProviderReportFileName
           ,@TimeTaken
           ,@MigrationNote)

END
GO


