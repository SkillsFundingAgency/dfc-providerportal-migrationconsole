USE [SFA_CourseDirectory]
GO

/****** Object:  StoredProcedure [dbo].[dfc_CourseTransferCourseAuditAdd]    Script Date: 01/02/2019 15:28:17 ******/
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
		    @CourseTransferId int
		   ,@Ukprn int
           ,@CourseId int
           ,@LARS varchar(10)
           ,@CourseRecordStatus int
           ,@CourseRuns int
           ,@CourseRunsLive int
           ,@CourseRunsPending INT
           ,@CourseRunsReadyToGoLive INT
           ,@CourseRunsLARSless int
           ,@MigrationSuccess int
           ,@CourseMigrationNote nvarchar(max)
)
AS
BEGIN

INSERT INTO [CourseTransfer_CourseAudit]
           ([CourseTransferId]
		   ,[Ukprn]
           ,[CourseId]
           ,[LARS]
           ,[CourseRecordStatus]
           ,[CourseRuns]
           ,[CourseRunsLive]
           ,[CourseRunsPending]
		   ,[CourseRunsReadyToGoLive]
		   ,[CourseRunsLARSless]
           ,[MigrationSuccess]
           ,[CourseMigrationNote])
     VALUES
           (@CourseTransferId
		   ,@Ukprn
           ,@CourseId
           ,@LARS
           ,@CourseRecordStatus
           ,@CourseRuns
           ,@CourseRunsLive
           ,@CourseRunsPending
		   ,@CourseRunsReadyToGoLive
		   ,@CourseRunsLARSless
           ,@MigrationSuccess
           ,@CourseMigrationNote)

END
GO


