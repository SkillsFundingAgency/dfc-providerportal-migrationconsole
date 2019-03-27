USE [dfc-coursedirectory]
GO

SELECT * FROM [LearningAim] -- 128428 -- 128714
TRUNCATE TABLE [LearningAim]

SELECT * FROM [Provider] -- 5979 -- 
TRUNCATE TABLE [Provider]

SELECT * FROM [Course]
TRUNCATE TABLE [Course]

SELECT * FROM [CourseInstance]
TRUNCATE TABLE [CourseInstance]

SELECT * FROM [CourseInstanceStartDate]
TRUNCATE TABLE [CourseInstanceStartDate]

SELECT * FROM [CourseInstanceVenue]
TRUNCATE TABLE [CourseInstanceVenue]



------------- CourseMigration if it is going to be used

SELECT * FROM [CourseMigration]
TRUNCATE TABLE [CourseMigration]

------------- User Management --------------

SELECT * FROM [ProviderUser]
TRUNCATE TABLE [ProviderUser]