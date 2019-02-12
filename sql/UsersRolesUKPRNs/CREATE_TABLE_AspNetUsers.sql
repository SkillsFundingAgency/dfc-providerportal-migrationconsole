/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 12/02/2019 15:31:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[AddressId] [int] NULL,
	[PasswordResetRequired] [bit] NULL,
	[ProviderUserTypeId] [int] NULL,
	[CreatedByUserId] [nvarchar](128) NULL,
	[CreatedDateTimeUtc] [datetime] NULL,
	[ModifiedByUserId] [nvarchar](128) NULL,
	[ModifiedDateTimeUtc] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[LegacyUserId] [int] NULL,
	[LastLoginDateTimeUtc] [datetime] NULL,
	[IsSecureAccessUser] [bit] NULL,
	[SecureAccessUserId] [int] NULL,
	[ShowUserWizard] [bit] NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




