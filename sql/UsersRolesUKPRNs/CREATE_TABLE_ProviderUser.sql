/****** Object:  Table [dbo].[ProviderUser]    Script Date: 12/02/2019 16:03:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProviderUser](
	[UserId] [nvarchar](450) NOT NULL,
	[ProviderId] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProviderUser]  WITH CHECK ADD  CONSTRAINT [FK_ProviderUser_Provider] FOREIGN KEY([ProviderId])
REFERENCES [dbo].[Provider] ([ProviderId])
GO

ALTER TABLE [dbo].[ProviderUser] CHECK CONSTRAINT [FK_ProviderUser_Provider]
GO

ALTER TABLE [dbo].[ProviderUser]  WITH CHECK ADD  CONSTRAINT [FK_ProviderUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[ProviderUser] CHECK CONSTRAINT [FK_ProviderUser_User]
GO


