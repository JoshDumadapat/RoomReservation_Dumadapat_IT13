-- Database setup for Room Reservation System
-- Run this script in SSMS to create the database and tables

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'DB_RoomReservation_Dumadapat_IT13')
BEGIN
    CREATE DATABASE [DB_RoomReservation_Dumadapat_IT13];
END
GO

USE [DB_RoomReservation_Dumadapat_IT13]
GO

-- Creating reservations table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblReservations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblReservations](
        [ReservationID] [int] IDENTITY(1,1) NOT NULL,
        [CustomerFname] [nvarchar](100) NOT NULL,
        [CustomerLname] [nvarchar](100) NOT NULL,
        [ContactNum] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NULL,
        [RoomType] [nvarchar](50) NOT NULL,
        [CheckInDate] [datetime] NOT NULL,
        [CheckOutDate] [datetime] NOT NULL,
        [Status] [nvarchar](50) NOT NULL,
        [DateCreated] [datetime] NOT NULL,
        [RoomPrice] [decimal](10, 2) NOT NULL,
        CONSTRAINT [PK_tblReservations] PRIMARY KEY CLUSTERED ([ReservationID] ASC)
    )
END
GO

-- Creating users table for admin accounts
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblUsers]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblUsers](
        [AdminID] [int] IDENTITY(1,1) NOT NULL,
        [Fname] [nvarchar](50) NULL,
        [Lname] [nvarchar](50) NULL,
        [Cnumber] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NOT NULL,
        [Password] [nvarchar](255) NOT NULL,
        CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED ([AdminID] ASC)
    )
END
GO

-- Creating table for cancelled reservations
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblRemovedReservations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[tblRemovedReservations](
        [RemovedID] [int] IDENTITY(1,1) NOT NULL,
        [ReservationID] [int] NULL,
        [CustomerFname] [nvarchar](100) NOT NULL,
        [CustomerLname] [nvarchar](100) NOT NULL,
        [ContactNum] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NULL,
        [RoomType] [nvarchar](50) NOT NULL,
        [CheckinDate] [datetime] NULL,
        [CheckOutDate] [datetime] NULL,
        [Status] [nvarchar](50) NOT NULL,
        [ReasonNote] [nvarchar](500) NULL,
        [RemovedBy] [nvarchar](100) NULL,
        [DateRemoved] [datetime] NOT NULL,
        [RoomPrice] [decimal](10, 2) NOT NULL,
        CONSTRAINT [PK_tblRemovedReservations] PRIMARY KEY CLUSTERED ([RemovedID] ASC)
    )
END
GO

-- Adding default admin users
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblUsers])
BEGIN
    INSERT INTO [dbo].[tblUsers] (Fname, Lname, Cnumber, Email, Password)
    VALUES 
        ('John', 'Doe', '09123456789', 'john.doe@vanderson.com', 'admin123'),
        ('Jane', 'Smith', '09987654321', 'jane.smith@vanderson.com', 'admin456'),
        ('Robert', 'Johnson', '09555123456', 'robert.johnson@vanderson.com', 'admin789');
END
GO

PRINT 'Setup complete!';
GO

