﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dfc.CourseDirectory.Models.Enums
{
    //public enum RecordStatus
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,
    //    [Description("Pending")]
    //    Pending = 1,
    //    [Description("Live")]
    //    Live = 2,
    //    [Description("Archived")]
    //    Archived = 3,
    //    [Description("Deleted")]
    //    Deleted = 4,
    //    [Description("Ready To Go Live")]
    //    ReadyToGoLive = 5
    //}

    // BitMask
    //[Flags]
    public enum RecordStatus
    {
        [Description("Undefined")]
        Undefined = 0,       
        [Description("Live")]
        Live = 1,
        [Description("Pending")]
        Pending = 2,
        [Description("Archived")]
        Archived = 4,
        [Description("Deleted")]
        Deleted = 8,
        [Description("BulkUload Pending")]
        BulkUloadPending = 16,
        [Description("BulkUpload Ready To Go Live")]
        BulkUploadReadyToGoLive = 32,
        [Description("API Pending")]
        APIPending = 64,
        [Description("API Ready To Go Live")]
        APIReadyToGoLive = 128,
        [Description("Migration Pending")]
        MigrationPending = 256,
        [Description("Migration Ready To Go Live")]
        MigrationReadyToGoLive = 512,
        [Description("Migration Deleted")]
        MigrationDeleted = 1024,
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
