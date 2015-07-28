USE [master]
GO
CREATE DATABASE [UnicloudTestDB] ON
( FILENAME = N'D:\Users\IElkanyuk.ITCORP\Desktop\IElkanyuk_solution\InstallBD\UnicloudTestDB.mdf' ),
( FILENAME = N'D:\Users\IElkanyuk.ITCORP\Desktop\IElkanyuk_solution\InstallBD\UnicloudTestDB_log.ldf' )
FOR ATTACH ;
GO
