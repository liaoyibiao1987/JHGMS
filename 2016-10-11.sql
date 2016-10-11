/*
GMSOA 的部署脚本

此代码由工具生成。
如果重新生成此代码，则对此文件的更改可能导致
不正确的行为并将丢失。
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "GMSOA"
:setvar DefaultFilePrefix "GMSOA"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
请检测 SQLCMD 模式，如果不支持 SQLCMD 模式，请禁用脚本执行。
要在启用 SQLCMD 模式后重新启用脚本，请执行:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'要成功执行此脚本，必须启用 SQLCMD 模式。';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
PRINT N'正在改变 [dbo].[Business]...';


GO
ALTER TABLE [dbo].[Business] ALTER COLUMN [Message] NVARCHAR (2000) NULL;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'数据库更新的事务处理部分成功。'
COMMIT TRANSACTION
END
ELSE PRINT N'数据库更新的事务处理部分失败。'
GO
DROP TABLE #tmpErrors
GO
PRINT N'更新完成。';


GO



/*
GMSOA 的部署脚本

此代码由工具生成。
如果重新生成此代码，则对此文件的更改可能导致
不正确的行为并将丢失。
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "GMSOA"
:setvar DefaultFilePrefix "GMSOA"
:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"
:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\"

GO
:on error exit
GO
/*
请检测 SQLCMD 模式，如果不支持 SQLCMD 模式，请禁用脚本执行。
要在启用 SQLCMD 模式后重新启用脚本，请执行:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'要成功执行此脚本，必须启用 SQLCMD 模式。';
        SET NOEXEC ON;
    END


GO
USE [$(DatabaseName)];


GO

IF (SELECT OBJECT_ID('tempdb..#tmpErrors')) IS NOT NULL DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL READ COMMITTED
GO
BEGIN TRANSACTION
GO
PRINT N'正在删除 DF__Business__Create__46E78A0C...';


GO
ALTER TABLE [dbo].[Business] DROP CONSTRAINT [DF__Business__Create__46E78A0C];


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO
PRINT N'正在开始重新生成表 [dbo].[Business]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_Business] (
    [Id]         INT             IDENTITY (1, 1) NOT NULL,
    [StaffID]    INT             NULL,
    [CustomerID] INT             NULL,
    [Message]    NVARCHAR (2000) NULL,
    [Log]        TEXT            NULL,
    [CreateTime] DATETIME        DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[Business])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Business] ON;
        INSERT INTO [dbo].[tmp_ms_xx_Business] ([Id], [StaffID], [CustomerID], [Message], [Log], [CreateTime])
        SELECT   [Id],
                 [StaffID],
                 [CustomerID],
                 [Message],
                 [Log],
                 [CreateTime]
        FROM     [dbo].[Business]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_Business] OFF;
    END

DROP TABLE [dbo].[Business];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_Business]', N'Business';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
IF @@ERROR <> 0
   AND @@TRANCOUNT > 0
    BEGIN
        ROLLBACK;
    END

IF @@TRANCOUNT = 0
    BEGIN
        INSERT  INTO #tmpErrors (Error)
        VALUES                 (1);
        BEGIN TRANSACTION;
    END


GO

IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT N'数据库更新的事务处理部分成功。'
COMMIT TRANSACTION
END
ELSE PRINT N'数据库更新的事务处理部分失败。'
GO
DROP TABLE #tmpErrors
GO
PRINT N'更新完成。';


GO
