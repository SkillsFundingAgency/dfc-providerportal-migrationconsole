using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dfc.CourseDirectory.Models.Enums
{
    public enum RecordStatus
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Pending")]
        Pending = 1,
        [Description("Live")]
        Live = 2,
        [Description("Archived")]
        Archived = 3,
        [Description("Deleted")]
        Deleted = 4,
        [Description("Ready To Go Live")]
        ReadyToGoLive = 5
    }

    // BitMask
    //public enum RecordStatus
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,       
    //    [Description("Live")]
    //    Live = 1,
    //    [Description("Pending")]
    //    Pending = 2,
    //    [Description("Archived")]
    //    Archived = 4,
    //    [Description("Deleted")]
    //    Deleted = 8,
    //    [Description("BulkUload Pending")]
    //    BulkUloadPending = 16,
    //    [Description("BulkUpload Ready To Go Live")]
    //    BulkUploadReadyToGoLive = 32,
    //    [Description("API Pending")]
    //    APIPending = 64,
    //    [Description("API Ready To Go Live")]
    //    APIReadyToGoLive = 128,
    //    [Description("Migration Pending")]
    //    MigrationPending = 256,
    //    [Description("Migration Ready To Go Live")]
    //    MigrationReadyToGoLive = 512,
    //    [Description("Migration Deleted")]
    //    MigrationDeleted = 1024,
    //}

    [Flags]
    public enum BitMaskStatus
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("All Live")]
        AllLive = 1,
        // For Bulk Upload Interface
        [Description("All BulkUload Pending")]
        AllBulkUloadPending = 16,
        [Description("All BulkUpload Ready To Go Live")]
        AllBulkUploadReadyToGoLive = 32,
        [Description("BulkUload Pending and Ready To Go Live")]
        BulkUloadPendingAndReadyToGoLive = 48,

        // For Migrattion
        // All Live goes directly in YourCourses
        //
        [Description("All Migration Pending")]
        AllMigrationPending = 256,
        [Description("All Migration Ready To Go Live")]
        AllMigrationReadyToGoLive = 512,
        //[Description("All Migration Deleted")] // Or where it will go (e.g DELETED TAB)
        //AllMigrationDeleted = 1024,
        [Description("Live And Migration Pending")]
        LiveAndMigrationPending = 257,
        [Description("Live And Migration Ready To Go Live")]
        LiveAndMigrationReadyToGoLive = 513,
        [Description("Live And Migration Deleted")] // where ELSE it will go (e.g DELETED TAB)
        LiveAndMigrationDeleted = 1025,


        [Description("Live And Migration Pending And Ready")]
        LiveAndMigrationPendingAndReady = 769,
        [Description("Live And Migration Pending And Ready And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        LiveAndMigrationPendingAndReadyAndDeleted = 1793,
        [Description("Live And Migration Ready And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        LiveAndMigrationReadyAndDeleted = 1537,
        [Description("Live And Migration Pending And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        LiveAndMigrationPendingAndDeleted = 1281,


        [Description("Migration Pending And Ready To Go Live")]
        MigrationPendingAndReadyToGoLive = 768,
        [Description("Migration Pending And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        MigrationPendingAndDeleted = 1280,

        [Description("Migration Pending And Ready To Go Live And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        MigrationPendingAndReadyToGoLiveAndDelete = 1782,

        //[Description("Migration Pending And Ready To Go Live And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        //MigrationPendingAndReadyToGoLiveAndDelete = 1782,
        //[Description("Migration Pending And Ready To Go Live And Deleted")] // where ELSE it will go (e.g DELETED TAB)
        //MigrationPendingAndReadyToGoLiveAndDelete = 1782,
    }

    public enum TransferMethod
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("BulkUpload")]
        BulkUpload = 1,
        [Description("API")]
        API = 2,
        [Description("CourseMigrationTool")]
        CourseMigrationTool = 3,
        [Description("CourseMigrationToolCsvFile")]
        CourseMigrationToolCsvFile = 4,
        [Description("CourseMigrationToolSingleUkprn")]
        CourseMigrationToolSingleUkprn = 5
    }

    public enum MigrationSuccess
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Success")]
        Success = 1,
        [Description("Failure")]
        Failure = 2
    }

    public enum DeploymentEnvironment
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Local")]
        Local = 1,
        [Description("Dev")]
        Dev = 2,
        [Description("Sit")]
        Sit = 3,
        [Description("PreProd")]
        PreProd = 4,
        [Description("Prod")]
        Prod = 5
    }

    public class Enums
    {
    }
}
