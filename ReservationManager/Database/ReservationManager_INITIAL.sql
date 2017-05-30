USE [master]
GO
CREATE DATABASE [ReservationManager]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ReservationManager', FILENAME = N'{DBPATH}\ReservationManager.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ReservationManager_log', FILENAME = N'{DBPATH}\ReservationManager_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
USE ReservationManager
GO
/****** Object:  Table [dbo].[Catégorie]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[idCat] [int] IDENTITY(1,1) NOT NULL,
	[catName] [nvarchar](10) NOT NULL,
	[placesNumber] [int] NOT NULL,
 CONSTRAINT [PK_Catégorie] PRIMARY KEY CLUSTERED 
(
	[idCat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Client]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Client](
	[idC] [int] IDENTITY(1,1) NOT NULL,
	[clientLName] [nvarchar](20) NOT NULL,
	[clientFName] [nvarchar](20) NOT NULL,
	[postalCode] [int] NULL,
	[bdd] [date] NULL,
 CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED 
(
	[idC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Réservation]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reservation](
	[numS] [int] NOT NULL,
	[numC] [int] NOT NULL,
	[numCat] [int] NOT NULL,
	[nbr] [int] NOT NULL,
 CONSTRAINT [PK_Réservation] PRIMARY KEY CLUSTERED 
(
	[numS] ASC,
	[numC] ASC,
	[numCat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Show]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Show](
	[idS] [int] IDENTITY(1,1) NOT NULL,
	[showName] [nvarchar](20) NOT NULL,
	[showDate] [date] NOT NULL,
	[description] [nvarchar](max) NULL,
	[poster] [image] NULL,
 CONSTRAINT [PK_Spectacle] PRIMARY KEY CLUSTERED 
(
	[idS] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tarif]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriceList](
	[numS] [int] NOT NULL,
	[numCat] [int] NOT NULL,
	[price] [money] NOT NULL,
 CONSTRAINT [PK_Tarif] PRIMARY KEY CLUSTERED 
(
	[numS] ASC,
	[numCat] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 22/03/2017 21:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[idU] [int] IDENTITY(1,1) NOT NULL,
	[login] [nvarchar](10) NOT NULL,
	[pwd] [nvarchar](10) NOT NULL,
	[admin] [smallint] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[idU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Category] ON 

GO
INSERT [dbo].[Category] ([idCat], [catName], [placesNumber]) VALUES (1, N'CatA', 50)
GO
INSERT [dbo].[Category] ([idCat], [catName], [placesNumber]) VALUES (2, N'CatB', 100)
GO
INSERT [dbo].[Category] ([idCat], [catName], [placesNumber]) VALUES (3, N'CatC', 200)
GO
SET IDENTITY_INSERT [dbo].[Category] OFF
GO
SET IDENTITY_INSERT [dbo].[Client] ON 

GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) VALUES (1, N'Albin',N'De Smet', 1000, CAST(N'2000-01-01' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) 
VALUES (2, N'Bernard', N'Dupond',1010, CAST(N'1998-03-02' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) 
VALUES (3, N'Christophe', N'Frere',1020, CAST(N'1978-05-04' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd)
VALUES (4, N'Danièle', N'Lombard',1030, CAST(N'1965-12-06' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) 
VALUES (5, N'Emmanuelle', N'Van Uit',1040, CAST(N'2005-08-05' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) 
VALUES (6, N'Françoise', N'Valporte',1050, CAST(N'1983-07-25' AS Date))
GO
INSERT [dbo].[Client] (idC, clientFName, clientLName,  postalCode, bdd) 
VALUES (7, N'Gaston', N'Lagaffe',1060, CAST(N'1994-09-14' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Client] OFF
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (1, 1, 1, 3)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (1, 2, 2, 5)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (1, 2, 3, 1)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (1, 4, 1, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (2, 5, 1, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (2, 6, 3, 1)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (2, 7, 2, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (3, 1, 1, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (3, 1, 2, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (3, 3, 1, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (3, 3, 2, 2)
GO
INSERT [dbo].[Reservation] ([numS], [numC], [numCat], [nbr]) VALUES (3, 3, 3, 2)
GO
SET IDENTITY_INSERT [dbo].Show ON 

GO
INSERT [dbo].Show (idS,showName , showDate, [description], poster) VALUES (1, N'The Spectacle !', CAST(N'2017-05-06' AS Date),N'Le spectacle du siècle', NULL)
GO
INSERT [dbo].Show (idS,showName , showDate, [description], poster) VALUES (2, N'Magic One', CAST(N'2017-05-13' AS Date), N'La magie comme vous ne l''avez jamais vue', NULL)
GO
INSERT [dbo].Show (idS,showName , showDate, [description], poster) VALUES (3, N'Super Dance', CAST(N'2017-06-01' AS Date), N'Apprenez la danse', NULL)
GO
INSERT [dbo].Show (idS,showName , showDate, [description], poster) VALUES (4, N'Music All Night Long', CAST(N'2017-06-07' AS Date), N'Dansez toute la nuit !', NULL)
GO
SET IDENTITY_INSERT [dbo].[Show] OFF
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (1, 1, 50.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (1, 2, 35.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (1, 3, 25.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (2, 1, 60.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (2, 2, 45.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (2, 3, 30.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (3, 1, 25.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (3, 2, 20.0000)
GO
INSERT [dbo].[PriceList] ([numS], [numCat], [price]) VALUES (4, 1, 40.0000)
GO
SET IDENTITY_INSERT [dbo].[User] ON 

GO
INSERT [dbo].[User] ([idU], [login], [pwd], [admin]) VALUES (1, N'admin', N'admin', 1)
GO
INSERT [dbo].[User] ([idU], [login], [pwd], [admin]) VALUES (2, N'vendor', N'vendor', 0)
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET ANSI_PADDING ON
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Client] ON [dbo].[Client]
(
	[clientLName], [clientFName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/****** Object:  Index [IX_User]    Script Date: 22/03/2017 21:01:21 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_User] ON [dbo].[User]
(
	[login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Category] FOREIGN KEY([numCat])
REFERENCES [dbo].[Category] ([idCat])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Category]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Client] FOREIGN KEY([numC])
REFERENCES [dbo].[Client] ([idC])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Client]
GO
ALTER TABLE [dbo].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Show] FOREIGN KEY([numS])
REFERENCES [dbo].[Show] ([idS])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_Show]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_Category] FOREIGN KEY([numCat])
REFERENCES [dbo].[Category] ([idCat])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_Category]
GO
ALTER TABLE [dbo].[PriceList]  WITH CHECK ADD  CONSTRAINT [FK_PriceList_Show] FOREIGN KEY([numS])
REFERENCES [dbo].[Show] ([idS])
GO
ALTER TABLE [dbo].[PriceList] CHECK CONSTRAINT [FK_PriceList_Show]
GO
USE [master]
GO
ALTER DATABASE ReservationManager SET  READ_WRITE 
GO




