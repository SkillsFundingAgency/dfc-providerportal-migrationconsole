USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferCourseAuditAdd]    Script Date: 30/01/2019 14:02:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[dfc_CourseTransferCourseAuditAdd]
(
			@Ukprn int
           ,@TransferMethod int
           ,@BatchNumber int
           ,@MigrationDate datetime
           ,@CourseId int
           ,@LARS varchar(10)
           ,@RecordStatus int
           ,@CourseRuns int
           ,@CourseRunsLive int
           ,@CourseRunsPending int
           ,@MigrationSuccess int
           ,@CourseMigrationNote nvarchar(max)
)
AS
BEGIN

INSERT INTO [CourseTransfer_CourseAudit]
           ([Ukprn]
           ,[TransferMethod]
           ,[BatchNumber]
           ,[MigrationDate]
           ,[CourseId]
           ,[LARS]
           ,[RecordStatus]
           ,[CourseRuns]
           ,[CourseRunsLive]
           ,[CourseRunsPending]
           ,[MigrationSuccess]
           ,[CourseMigrationNote])
     VALUES
           (@Ukprn
           ,@TransferMethod
           ,@BatchNumber
           ,@MigrationDate
           ,@CourseId
           ,@LARS
           ,@RecordStatus
           ,@CourseRuns
           ,@CourseRunsLive
           ,@CourseRunsPending
           ,@MigrationSuccess
           ,@CourseMigrationNote)

END
GO


