
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/14/2015 01:02:01
-- Generated from EDMX file: C:\Users\Ricardo\Source\Repos\VCBackend\VCBackend\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [VCBackend.Models.VCardContext];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'AccountSet'
CREATE TABLE [dbo].[AccountSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Balance] float  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'DeviceSet'
CREATE TABLE [dbo].[DeviceSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [DeviceId] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Token] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'VCardSet'
CREATE TABLE [dbo].[VCardSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TokenizedId] int  NOT NULL,
    [Data] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'AccountSet_Online'
CREATE TABLE [dbo].[AccountSet_Online] (
    [OnlineId] int IDENTITY(1,1) NOT NULL,
    [Id] int  NOT NULL,
    [VCard_Id] int  NOT NULL
);
GO

-- Creating table 'AccountSet_Tokenized'
CREATE TABLE [dbo].[AccountSet_Tokenized] (
    [TokenizedId] int IDENTITY(1,1) NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'DeviceSet_Default'
CREATE TABLE [dbo].[DeviceSet_Default] (
    [Id] int  NOT NULL
);
GO

-- Creating table 'DeviceSet_Mobile'
CREATE TABLE [dbo].[DeviceSet_Mobile] (
    [Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AccountSet'
ALTER TABLE [dbo].[AccountSet]
ADD CONSTRAINT [PK_AccountSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeviceSet'
ALTER TABLE [dbo].[DeviceSet]
ADD CONSTRAINT [PK_DeviceSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VCardSet'
ALTER TABLE [dbo].[VCardSet]
ADD CONSTRAINT [PK_VCardSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AccountSet_Online'
ALTER TABLE [dbo].[AccountSet_Online]
ADD CONSTRAINT [PK_AccountSet_Online]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AccountSet_Tokenized'
ALTER TABLE [dbo].[AccountSet_Tokenized]
ADD CONSTRAINT [PK_AccountSet_Tokenized]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeviceSet_Default'
ALTER TABLE [dbo].[DeviceSet_Default]
ADD CONSTRAINT [PK_DeviceSet_Default]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeviceSet_Mobile'
ALTER TABLE [dbo].[DeviceSet_Mobile]
ADD CONSTRAINT [PK_DeviceSet_Mobile]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'DeviceSet'
ALTER TABLE [dbo].[DeviceSet]
ADD CONSTRAINT [FK_UserDevice]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDevice'
CREATE INDEX [IX_FK_UserDevice]
ON [dbo].[DeviceSet]
    ([UserId]);
GO

-- Creating foreign key on [User_Id] in table 'AccountSet'
ALTER TABLE [dbo].[AccountSet]
ADD CONSTRAINT [FK_UserAccount]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccount'
CREATE INDEX [IX_FK_UserAccount]
ON [dbo].[AccountSet]
    ([User_Id]);
GO

-- Creating foreign key on [VCard_Id] in table 'AccountSet_Online'
ALTER TABLE [dbo].[AccountSet_Online]
ADD CONSTRAINT [FK_OnlineVCard]
    FOREIGN KEY ([VCard_Id])
    REFERENCES [dbo].[VCardSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OnlineVCard'
CREATE INDEX [IX_FK_OnlineVCard]
ON [dbo].[AccountSet_Online]
    ([VCard_Id]);
GO

-- Creating foreign key on [TokenizedId] in table 'VCardSet'
ALTER TABLE [dbo].[VCardSet]
ADD CONSTRAINT [FK_TokenizedVCard]
    FOREIGN KEY ([TokenizedId])
    REFERENCES [dbo].[AccountSet_Tokenized]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TokenizedVCard'
CREATE INDEX [IX_FK_TokenizedVCard]
ON [dbo].[VCardSet]
    ([TokenizedId]);
GO

-- Creating foreign key on [Id] in table 'AccountSet_Online'
ALTER TABLE [dbo].[AccountSet_Online]
ADD CONSTRAINT [FK_Online_inherits_Account]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'AccountSet_Tokenized'
ALTER TABLE [dbo].[AccountSet_Tokenized]
ADD CONSTRAINT [FK_Tokenized_inherits_Account]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'DeviceSet_Default'
ALTER TABLE [dbo].[DeviceSet_Default]
ADD CONSTRAINT [FK_Default_inherits_Device]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[DeviceSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'DeviceSet_Mobile'
ALTER TABLE [dbo].[DeviceSet_Mobile]
ADD CONSTRAINT [FK_Mobile_inherits_Device]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[DeviceSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------