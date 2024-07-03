-- Tabela User
CREATE TABLE [dbo].[Users] (
    [idUser] INT IDENTITY(1,1) PRIMARY KEY,
    [EmailUser] NVARCHAR(255) NOT NULL,
    [PasswordUser] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [Active] BIT NOT NULL
);

-- Tabela Theme
CREATE TABLE [dbo].[Themes] (
    [idTheme] INT IDENTITY(1,1) PRIMARY KEY,
    [idUser] INT NOT NULL,
    [NameTheme] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [Active] BIT NOT NULL,
    CONSTRAINT FK_Theme_User FOREIGN KEY ([idUser]) REFERENCES [dbo].[Users]([idUser]) ON DELETE CASCADE
);

-- Tabela Category
CREATE TABLE [dbo].[Categories] (
    [idCategory] INT IDENTITY(1,1) PRIMARY KEY,
    [idTheme] INT NOT NULL,
    [NameCategory] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [Active] BIT NOT NULL,
    CONSTRAINT FK_Category_Theme FOREIGN KEY ([idTheme]) REFERENCES [dbo].[Themes]([idTheme]) ON DELETE CASCADE
);

-- Tabela Item 
CREATE TABLE [dbo].[Itens] (
    [idItem] INT IDENTITY(1,1) PRIMARY KEY,
    [idCategory] INT NOT NULL,
    [TextItem] NVARCHAR(255) NULL,
    [Quantity] INT NULL,
    [DateItem] DATETIME NULL,
    [Price] DECIMAL(18, 2) NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [Active] BIT NOT NULL,
    CONSTRAINT FK_Item_Category FOREIGN KEY ([idCategory]) REFERENCES [dbo].[Categories]([idCategory]) ON DELETE CASCADE
);

---- Tabela Alert
CREATE TABLE [dbo].[Alerts] (
    [idAlert] INT IDENTITY(1,1) PRIMARY KEY,
    [idUser] INT NOT NULL,
    [idCategory] INT NOT NULL,
    [AlertTime] DATETIME NOT NULL,
    [RepeatInterval] INT NULL,
    [RepeatUnit] NVARCHAR(50) NULL,
    [Active] BIT NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Alert_User FOREIGN KEY ([idUser]) REFERENCES [dbo].[Users]([idUser]) ON DELETE CASCADE,
    CONSTRAINT FK_Alert_Item FOREIGN KEY ([idCategory]) REFERENCES [dbo].Categories([idCategory]) ON DELETE CASCADE
);
