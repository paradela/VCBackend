
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/27/2015 23:26:01
-- Generated from EDMX file: C:\Users\Ricardo\Source\Repos\VCBackend\VCBackend\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ModelContainer];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceSet] DROP CONSTRAINT [FK_UserDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountVCard]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VCardSet] DROP CONSTRAINT [FK_AccountVCard];
GO
IF OBJECT_ID(N'[dbo].[FK_UserAccount]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccountSet] DROP CONSTRAINT [FK_UserAccount];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountVCardToken]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VCardTokenSet] DROP CONSTRAINT [FK_AccountVCardToken];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountLoadRequest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LoadRequestSet] DROP CONSTRAINT [FK_AccountLoadRequest];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountPaymentRequest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PaymentRequestSet] DROP CONSTRAINT [FK_AccountPaymentRequest];
GO
IF OBJECT_ID(N'[dbo].[FK_Default_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceSet_Default] DROP CONSTRAINT [FK_Default_inherits_Device];
GO
IF OBJECT_ID(N'[dbo].[FK_Mobile_inherits_Device]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DeviceSet_Mobile] DROP CONSTRAINT [FK_Mobile_inherits_Device];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[AccountSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccountSet];
GO
IF OBJECT_ID(N'[dbo].[DeviceSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceSet];
GO
IF OBJECT_ID(N'[dbo].[VCardSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VCardSet];
GO
IF OBJECT_ID(N'[dbo].[VCardTokenSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VCardTokenSet];
GO
IF OBJECT_ID(N'[dbo].[PaymentRequestSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PaymentRequestSet];
GO
IF OBJECT_ID(N'[dbo].[LoadRequestSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LoadRequestSet];
GO
IF OBJECT_ID(N'[dbo].[DeviceSet_Default]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceSet_Default];
GO
IF OBJECT_ID(N'[dbo].[DeviceSet_Mobile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceSet_Mobile];
GO

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
    [UserAccount_Account_Id] int  NOT NULL
);
GO

-- Creating table 'DeviceSet'
CREATE TABLE [dbo].[DeviceSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Token] nvarchar(max)  NOT NULL,
    [DeviceId] nvarchar(max)  NULL
);
GO

-- Creating table 'VCardSet'
CREATE TABLE [dbo].[VCardSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Data] nvarchar(max)  NOT NULL,
    [AccountId] int  NOT NULL,
    [AccountVCard_VCard_Id] int  NOT NULL
);
GO

-- Creating table 'VCardTokenSet'
CREATE TABLE [dbo].[VCardTokenSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Data] nvarchar(max)  NOT NULL,
    [AccountId] int  NOT NULL,
    [Validity] time  NOT NULL,
    [AccountVCardToken_VCardToken_Id] int  NOT NULL
);
GO

-- Creating table 'PaymentRequestSet'
CREATE TABLE [dbo].[PaymentRequestSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PaymentId] nvarchar(max)  NOT NULL,
    [State] nvarchar(max)  NOT NULL,
    [ProductId] nvarchar(max)  NULL,
    [Price] nvarchar(max)  NOT NULL,
    [Currency] nvarchar(max)  NOT NULL,
    [PaymentMethod] nvarchar(max)  NULL,
    [RedirectURL] nvarchar(max)  NULL,
    [PaymentData] nvarchar(max)  NULL,
    [PayerId] nvarchar(max)  NULL,
    [AccountId] int  NOT NULL
);
GO

-- Creating table 'LoadRequestSet'
CREATE TABLE [dbo].[LoadRequestSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountId] int  NOT NULL,
    [ProdId] nvarchar(max)  NOT NULL,
    [Quantity] int  NOT NULL,
    [Price] float  NOT NULL,
    [DateInitial] datetime  NULL,
    [CardAuth] nvarchar(max)  NOT NULL
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

-- Creating primary key on [Id] in table 'VCardTokenSet'
ALTER TABLE [dbo].[VCardTokenSet]
ADD CONSTRAINT [PK_VCardTokenSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PaymentRequestSet'
ALTER TABLE [dbo].[PaymentRequestSet]
ADD CONSTRAINT [PK_PaymentRequestSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LoadRequestSet'
ALTER TABLE [dbo].[LoadRequestSet]
ADD CONSTRAINT [PK_LoadRequestSet]
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

-- Creating foreign key on [AccountVCard_VCard_Id] in table 'VCardSet'
ALTER TABLE [dbo].[VCardSet]
ADD CONSTRAINT [FK_AccountVCard]
    FOREIGN KEY ([AccountVCard_VCard_Id])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountVCard'
CREATE INDEX [IX_FK_AccountVCard]
ON [dbo].[VCardSet]
    ([AccountVCard_VCard_Id]);
GO

-- Creating foreign key on [UserAccount_Account_Id] in table 'AccountSet'
ALTER TABLE [dbo].[AccountSet]
ADD CONSTRAINT [FK_UserAccount]
    FOREIGN KEY ([UserAccount_Account_Id])
    REFERENCES [dbo].[UserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserAccount'
CREATE INDEX [IX_FK_UserAccount]
ON [dbo].[AccountSet]
    ([UserAccount_Account_Id]);
GO

-- Creating foreign key on [AccountVCardToken_VCardToken_Id] in table 'VCardTokenSet'
ALTER TABLE [dbo].[VCardTokenSet]
ADD CONSTRAINT [FK_AccountVCardToken]
    FOREIGN KEY ([AccountVCardToken_VCardToken_Id])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountVCardToken'
CREATE INDEX [IX_FK_AccountVCardToken]
ON [dbo].[VCardTokenSet]
    ([AccountVCardToken_VCardToken_Id]);
GO

-- Creating foreign key on [AccountId] in table 'LoadRequestSet'
ALTER TABLE [dbo].[LoadRequestSet]
ADD CONSTRAINT [FK_AccountLoadRequest]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountLoadRequest'
CREATE INDEX [IX_FK_AccountLoadRequest]
ON [dbo].[LoadRequestSet]
    ([AccountId]);
GO

-- Creating foreign key on [AccountId] in table 'PaymentRequestSet'
ALTER TABLE [dbo].[PaymentRequestSet]
ADD CONSTRAINT [FK_AccountPaymentRequest]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[AccountSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountPaymentRequest'
CREATE INDEX [IX_FK_AccountPaymentRequest]
ON [dbo].[PaymentRequestSet]
    ([AccountId]);
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