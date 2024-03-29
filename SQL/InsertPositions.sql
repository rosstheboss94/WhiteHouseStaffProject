USE [WhiteHouseETL]
GO
/****** Object:  StoredProcedure [dbo].[InsertPositions]    Script Date: 1/8/2024 9:29:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[InsertPositions]
    @Positions PositionTableType READONLY
AS
BEGIN
	
	CREATE TABLE ##PositionsTemp (
		[Row Number] int,
		[Position Title] varchar(1000) NULL,
		[Pay Basis] varchar(100) NULL,
		[Status] varchar(100) NULL
	)

	INSERT INTO ##PositionsTemp([Row Number], [Position Title], [Pay Basis], Status)
	SELECT RowNumber, PositionTitle, PayBasis, Status FROM @Positions

	INSERT INTO Positions([Position Title], [Pay Basis], Status)
	SELECT 
		MAX(PositionTitle) AS PositionTitle,
		MAX(PayBasis) AS PayBasis,
		MAX(Status) AS Status
	FROM @Positions
	GROUP BY PositionTitle, PayBasis, Status;

END;
