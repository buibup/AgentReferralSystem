-- create table

-- Drop table

-- DROP TABLE AgentReferralSystem.dbo.Agent

CREATE TABLE AgentReferralSystem.dbo.Agent (
	AgentId bigint NOT NULL,
	AgentCode nvarchar(20) NOT NULL,
	AgentDesc nvarchar(500) NOT NULL,
	AgreementDate datetime2(7) NOT NULL,
	DateFrom datetime2(7) NOT NULL,
	DateTo datetime2(7) NOT NULL,
	Remark nvarchar(1000) NOT NULL,
	CONSTRAINT PK_AGENT PRIMARY KEY (AgentId)
) go;

-- Drop table

-- DROP TABLE AgentReferralSystem.dbo.SaleType

CREATE TABLE AgentReferralSystem.dbo.SaleType (
	SaleTypeId bigint NOT NULL,
	SaleTypeName nvarchar(500) NOT NULL,
	CONSTRAINT PK_SALETYPE PRIMARY KEY (SaleTypeId)
) go;


-- Drop table

-- DROP TABLE AgentReferralSystem.dbo.AgentsSaleTypes

CREATE TABLE AgentReferralSystem.dbo.AgentsSaleTypes (
	BaseCommission decimal(3,2) NOT NULL,
	Target decimal(10,2) NOT NULL,
	TargetPeriod int NOT NULL,
	IncreaseIfTargetMet decimal(3,2) NOT NULL,
	Maximum decimal(3,2) NOT NULL,
	ResetToBase int NOT NULL,
	ApplicableTargetInrease nvarchar(100) NOT NULL,
	SaleTypeId bigint NOT NULL,
	AgentId bigint NOT NULL,
	CONSTRAINT AgentsSaleTypes_fk0 FOREIGN KEY (SaleTypeId) REFERENCES AgentReferralSystem.dbo.SaleType(SaleTypeId) ON DELETE RESTRICT ON UPDATE CASCADE,
	CONSTRAINT AgentsSaleTypes_fk1 FOREIGN KEY (AgentId) REFERENCES AgentReferralSystem.dbo.Agent(AgentId) ON DELETE RESTRICT ON UPDATE CASCADE
) go;


-- create procedure

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Delete Agent and AgentsSaleTypes>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteAgent]
		@AgentId int
AS
BEGIN

-- delete agent saletypes
	DELETE FROM AgentsSaleTypes
	WHERE AgentId=@AgentId;

	-- delete agent
	DELETE FROM Agent
	WHERE AgentId=@AgentId;

END;


-- =============================================  
-- Author:      <bui>  
-- Create date: <2018-08-16>  
-- Description: <Get Agent By Id>  
-- EXEC GetUsers  
-- =============================================  
CREATE PROCEDURE [dbo].[GetAgentById]  
	@AgentId int
AS  
BEGIN  
    SET NOCOUNT ON;  
    SELECT a.AgentId, a.AgentCode, a.AgentDesc,
		a.AgreementDate, a.DateFrom, a.DateTo,
		a.Remark
	FROM Agent a
	
	WHERE a.AgentId = @AgentId
END;

-- =============================================  
-- Author:      <bui>  
-- Create date: <2018-08-16>  
-- Description: <Get Agent All>  
-- EXEC GetUsers  
-- =============================================  
CREATE PROCEDURE [dbo].[GetAgents]  
AS  
BEGIN  
    SET NOCOUNT ON;  
    SELECT a.AgentId, a.AgentCode, a.AgentDesc,
		a.AgreementDate, a.DateFrom, a.DateTo,
		a.Remark, ast.BaseCommission, ast.Target,
		ast.TargetPeriod, ast.IncreaseIfTargetMet,
		ast.Maximum, ast.ResetToBase, ast.ApplicableTargetInrease,
		st.SaleTypeName
	FROM Agent(NOLOCK) a
	join AgentsSaleTypes ast on a.AgentId = ast.AgentId
	join SaleType st on ast.SaleTypeId = st.SaleTypeId
	ORDER BY a.AgentId ASC  
END;

-- =============================================
-- Author:		<bui>
-- Create date: <2018-08-16>
-- Description:	<GetSaleTypeByAgentId>
-- =============================================
CREATE PROCEDURE GetSaleTypeByAgentId
	@AgentId int
AS
BEGIN
	SELECT  ast.BaseCommission, ast.Target,
		ast.TargetPeriod, ast.IncreaseIfTargetMet,
		ast.Maximum, ast.ResetToBase, ast.ApplicableTargetInrease,
		st.SaleTypeName, st.SaleTypeId
	FROM AgentsSaleTypes ast
	join SaleType st on ast.SaleTypeId = st.SaleTypeId
	WHERE ast.AgentId = @AgentId
END;

-- =============================================  
-- Author:      <bui>  
-- Create date: <2018-08-16>  
-- Description: <Add/Edit Agent>  
-- =============================================  
Create PROCEDURE [dbo].[SaveAgent]  
(  
	@AgentId int,
	@AgentCode nvarchar(20),
	@AgentDesc nvarchar(500),
	@AgreementDate datetime2(7),
	@DateFrom datetime2(7),
	@DateTo datetime2(7),
	@Remark nvarchar(1000)
)  
AS  
BEGIN  
    IF EXISTS (SELECT 1 FROM Agent WHERE AgentId = @AgentId)  
    BEGIN  
		UPDATE Agent SET  
		AgentCode=@AgentCode, 
		AgentDesc=@AgentDesc, 
		AgreementDate=@AgreementDate, 
		DateFrom=@DateFrom, 
		DateTo=@DateTo, 
		Remark=@Remark
		WHERE AgentId=@AgentId;
    END  
    ELSE  
    BEGIN  
  
		INSERT INTO Agent
		(AgentId, AgentCode, AgentDesc, AgreementDate, DateFrom, DateTo, Remark)
		VALUES(@AgentId, @AgentCode, @AgentDesc, @AgreementDate, @DateFrom, @DateTo, @Remark); 
  
    END  
END;

-- =============================================
-- Author:		<bui>
-- Create date: <2018-08-16>
-- Description:	<Add/Edit AgentsSaleTypes>
-- =============================================
CREATE PROCEDURE [dbo].[SaveAgentsSaleTypes]
	@BaseCommission decimal(3,2),
	@Target decimal(10,2),
	@TargetPeriod int,
	@IncreaseIfTargetMet decimal(3,2),
	@Maximum decimal(3,2),
	@ResetToBase int,
	@ApplicableTargetInrease nvarchar(100),
	@SaleTypeId int,
	@AgentId int
AS
BEGIN
	IF EXISTS (SELECT 1 FROM AgentsSaleTypes WHERE AgentId = @AgentId AND SaleTypeId = @SaleTypeId) 
	BEGIN
		UPDATE AgentsSaleTypes SET
		BaseCommission = @BaseCommission,
		Target = @Target,
		TargetPeriod = @TargetPeriod,
		IncreaseIfTargetMet = @IncreaseIfTargetMet,
		Maximum = @Maximum,
		ResetToBase = @ResetToBase,
		ApplicableTargetInrease = @ApplicableTargetInrease
		WHERE AgentId = @AgentId AND SaleTypeId = @SaleTypeId;
	END
	ELSE
	BEGIN
		INSERT INTO AgentsSaleTypes
		(BaseCommission, Target, TargetPeriod, IncreaseIfTargetMet, 
		Maximum, ResetToBase, ApplicableTargetInrease, SaleTypeId, AgentId)
		VALUES(@BaseCommission, @Target, @TargetPeriod, @IncreaseIfTargetMet, 
		@Maximum, @ResetToBase, @ApplicableTargetInrease, @SaleTypeId, @AgentId)
	END
END;

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SaveSaleType]
	@SaleTypeId int,
	@SaleTypeName nvarchar(500)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM SaleType Where SaleTypeId = @SaleTypeId) 
	Begin

		Update SaleType Set
		SaleTypeName = @SaleTypeName
		Where SaleTypeId = @SaleTypeId;
	END
	ELSE
	Begin

		INSERT INTO SaleType
		(SaleTypeId, SaleTypeName)
		VALUES(@SaleTypeId, @SaleTypeName);

	END
END;



