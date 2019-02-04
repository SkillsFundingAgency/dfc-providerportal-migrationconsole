USE [dfc-coursedirectory]
GO

/****** Object:  Table [dbo].[Provider]    Script Date: 04/02/2019 14:49:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Provider](
	[ProviderId] [int] IDENTITY(1,1) NOT NULL,
	[ProviderName] [nvarchar](200) NOT NULL,
	[ProviderNameAlias] [nvarchar](200) NULL,
	[Loans24Plus] [bit] NOT NULL,
	[Ukprn] [int] NOT NULL,
	[UPIN] [int] NULL,
	[ProviderTypeId] [int] NOT NULL,
	[RecordStatusId] [int] NOT NULL,
	[CreatedByUserId] [nvarchar](128) NOT NULL,
	[CreatedDateTimeUtc] [datetime] NOT NULL,
	[ModifiedByUserId] [nvarchar](128) NULL,
	[ModifiedDateTimeUtc] [datetime] NULL,
	[ProviderRegionId] [int] NULL,
	[IsContractingBody] [bit] NOT NULL,
	[ProviderTrackingUrl] [nvarchar](255) NULL,
	[VenueTrackingUrl] [nvarchar](255) NULL,
	[CourseTrackingUrl] [nvarchar](255) NULL,
	[BookingTrackingUrl] [nvarchar](255) NULL,
	[RelationshipManagerUserId] [nvarchar](128) NULL,
	[InformationOfficerUserId] [nvarchar](128) NULL,
	[AddressId] [int] NULL,
	[Email] [nvarchar](255) NULL,
	[Website] [nvarchar](255) NULL,
	[Telephone] [nvarchar](30) NULL,
	[Fax] [nvarchar](30) NULL,
	[FeChoicesLearner] [decimal](3, 1) NULL,
	[FeChoicesEmployer] [decimal](3, 1) NULL,
	[FeChoicesDestination] [int] NULL,
	[FeChoicesUpdatedDateTimeUtc] [datetime] NULL,
	[QualityEmailsPaused] [bit] NOT NULL,
	[QualityEmailStatusId] [int] NULL,
	[TrafficLightEmailDateTimeUtc] [date] NULL,
	[DFE1619Funded] [bit] NOT NULL,
	[SFAFunded] [bit] NOT NULL,
	[DfENumber] [int] NULL,
	[DfEUrn] [int] NULL,
	[DfEProviderTypeId] [int] NULL,
	[DfEProviderStatusId] [int] NULL,
	[DfELocalAuthorityId] [int] NULL,
	[DfERegionId] [int] NULL,
	[DfEEstablishmentTypeId] [int] NULL,
	[DfEEstablishmentNumber] [int] NULL,
	[StatutoryLowestAge] [int] NULL,
	[StatutoryHighestAge] [int] NULL,
	[AgeRange] [varchar](10) NULL,
	[AnnualSchoolCensusLowestAge] [int] NULL,
	[AnnualSchoolCensusHighestAge] [int] NULL,
	[CompanyRegistrationNumber] [int] NULL,
	[Uid] [int] NULL,
	[SecureAccessId] [int] NULL,
	[BulkUploadPending] [bit] NOT NULL,
	[PublishData] [bit] NOT NULL,
	[MarketingInformation] [nvarchar](900) NULL,
	[NationalApprenticeshipProvider] [bit] NOT NULL,
	[ApprenticeshipContract] [bit] NOT NULL,
	[PassedOverallQAChecks] [bit] NOT NULL,
	[DataReadyToQA] [bit] NOT NULL,
	[RoATPFFlag] [bit] NOT NULL,
	[LastAllDataUpToDateTimeUtc] [datetime] NULL,
	[RoATPProviderTypeId] [int] NULL,
	[RoATPStartDate] [date] NULL,
	[MarketingInformationUpdatedDateUtc] [datetime] NULL,
	[TradingName] [nvarchar](255) NULL,
	--[IsTASOnly]  AS ([dbo].[GetProviderIsTASOnly]([ProviderId],[RoATPFFlag])),
	[MaxLocations] [int] NULL,
	[MaxLocationsUserId] [nvarchar](128) NULL,
	[MaxLocationsDateTimeUtc] [datetime] NULL,
	[TASRefreshOverride] [bit] NOT NULL,
 CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED 
(
	[ProviderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ALTER TABLE [dbo].[Provider] ADD  CONSTRAINT [DF_Table_1_24PlusLoans]  DEFAULT ((0)) FOR [Loans24Plus]
--GO

--ALTER TABLE [dbo].[Provider] ADD  CONSTRAINT [DF_Provider_IsDeleted]  DEFAULT ((0)) FOR [RecordStatusId]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT (getutcdate()) FOR [CreatedDateTimeUtc]
--GO

--ALTER TABLE [dbo].[Provider] ADD  CONSTRAINT [DF_Provider_IsContractingBody]  DEFAULT ((0)) FOR [IsContractingBody]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [QualityEmailsPaused]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [DFE1619Funded]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [SFAFunded]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [BulkUploadPending]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((1)) FOR [PublishData]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [NationalApprenticeshipProvider]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [ApprenticeshipContract]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [PassedOverallQAChecks]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [DataReadyToQA]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [RoATPFFlag]
--GO

--ALTER TABLE [dbo].[Provider] ADD  DEFAULT ((0)) FOR [TASRefreshOverride]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_Address] FOREIGN KEY([AddressId])
--REFERENCES [dbo].[Address] ([AddressId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_Address]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_CreatedByUserId] FOREIGN KEY([CreatedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_CreatedByUserId]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_DfEEstablishmentType] FOREIGN KEY([DfEEstablishmentTypeId])
--REFERENCES [dbo].[DfEEstablishmentType] ([DfEEstablishmentTypeId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_DfEEstablishmentType]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_DfELocalAuthority] FOREIGN KEY([DfELocalAuthorityId])
--REFERENCES [dbo].[DfELocalAuthority] ([DfELocalAuthorityId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_DfELocalAuthority]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_DfEProviderStatus] FOREIGN KEY([DfEProviderStatusId])
--REFERENCES [dbo].[DfEProviderStatus] ([DfEProviderStatusId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_DfEProviderStatus]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_DfEProviderType] FOREIGN KEY([DfEProviderTypeId])
--REFERENCES [dbo].[DfEProviderType] ([DfEProviderTypeId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_DfEProviderType]
--GO

--ALTER TABLE [dbo].[Provider]  WITH NOCHECK ADD  CONSTRAINT [FK_Provider_DfERegion] FOREIGN KEY([DfERegionId])
--REFERENCES [dbo].[DfERegion] ([DfERegionId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_DfERegion]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_InformationOfficer] FOREIGN KEY([InformationOfficerUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_InformationOfficer]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_MaxLocationsUserId] FOREIGN KEY([MaxLocationsUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_MaxLocationsUserId]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_ModifiedByUserId] FOREIGN KEY([ModifiedByUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_ModifiedByUserId]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_ProviderRegion] FOREIGN KEY([ProviderRegionId])
--REFERENCES [dbo].[ProviderRegion] ([ProviderRegionId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_ProviderRegion]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_ProviderType] FOREIGN KEY([ProviderTypeId])
--REFERENCES [dbo].[ProviderType] ([ProviderTypeId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_ProviderType]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_QualityEmailStatus] FOREIGN KEY([QualityEmailStatusId])
--REFERENCES [dbo].[QualityEmailStatus] ([QualityEmailStatusId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_QualityEmailStatus]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_RecordStatus] FOREIGN KEY([RecordStatusId])
--REFERENCES [dbo].[RecordStatus] ([RecordStatusId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_RecordStatus]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_RelationshipManager] FOREIGN KEY([RelationshipManagerUserId])
--REFERENCES [dbo].[AspNetUsers] ([Id])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_RelationshipManager]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_RoATPProviderType] FOREIGN KEY([RoATPProviderTypeId])
--REFERENCES [dbo].[RoATPProviderType] ([RoATPProviderTypeId])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_RoATPProviderType]
--GO

--ALTER TABLE [dbo].[Provider]  WITH CHECK ADD  CONSTRAINT [FK_Provider_Ukrlp] FOREIGN KEY([Ukprn])
--REFERENCES [dbo].[Ukrlp] ([Ukprn])
--GO

--ALTER TABLE [dbo].[Provider] CHECK CONSTRAINT [FK_Provider_Ukrlp]
--GO


