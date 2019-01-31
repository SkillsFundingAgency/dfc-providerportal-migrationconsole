USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferProviderAuditAdd]    Script Date: 30/01/2019 14:15:11 ******/
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
            @Ukprn int
           ,@TransferMethod int
           ,@BatchNumber int
           ,@DeploymentEnvironment int
           ,@MigrationDate datetime
           ,@CoursesToBeMigrated int
           ,@CoursesGoodToBeMigrated int
           ,@CoursesGoodToBeMigratedPending int
           ,@CoursesGoodToBeMigratedLive int
           ,@CoursesNotGoodToBeMigrated int
           ,@MigrationSuccesses int
           ,@MigrationFailures int
           ,@MigrationReportFileName varchar(255)
           ,@TimeTaken varchar(50)
           ,@MigrationNote nvarchar(max)
)
AS
BEGIN

	INSERT INTO [CourseTransfer_ProviderAudit]
           ([Ukprn]
           ,[TransferMethod]
           ,[BatchNumber]
           ,[DeploymentEnvironment]
           ,[MigrationDate]
           ,[CoursesToBeMigrated]
           ,[CoursesGoodToBeMigrated]
           ,[CoursesGoodToBeMigratedPending]
           ,[CoursesGoodToBeMigratedLive]
           ,[CoursesNotGoodToBeMigrated]
           ,[MigrationSuccesses]
           ,[MigrationFailures]
           ,[MigrationReportFileName]
           ,[TimeTaken]
           ,[MigrationNote])
     VALUES
           (@Ukprn
           ,@TransferMethod
           ,@BatchNumber
           ,@DeploymentEnvironment
           ,@MigrationDate
           ,@CoursesToBeMigrated
           ,@CoursesGoodToBeMigrated
           ,@CoursesGoodToBeMigratedPending
           ,@CoursesGoodToBeMigratedLive
           ,@CoursesNotGoodToBeMigrated
           ,@MigrationSuccesses
           ,@MigrationFailures
           ,@MigrationReportFileName
           ,@TimeTaken
           ,@MigrationNote)

END
GO


