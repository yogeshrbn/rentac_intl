-- TDS persistence helpers (post-insert UPDATE) so live p_LedgerTransactions_ins / p_ledgerTransactionDetail_ins
-- do not need to be replaced in-repo. Run PreRun scripts first.
--
-- DBA (optional hardening): merge into native procs instead of these helpers:
--   p_LedgerTransactions_ins — add @totalTds DECIMAL(18,2) = 0; INSERT TotalTds column.
--   p_ledgerTransactionDetail_ins — add @tdsAmount DECIMAL(18,2) = 0; INSERT TdsAmount column.
--   Header/detail SELECTs used for edit (e.g. TRANSACTION_BY_ID / p_LedgerTransactionDetail_sel) — return TotalTds, TdsAmount.
--
USE [$AppDb$]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_LedgerTransactions_updTotalTds' AND type = 'P')
--    DROP PROCEDURE dbo.p_LedgerTransactions_updTotalTds;
--GO

--CREATE PROCEDURE dbo.p_LedgerTransactions_updTotalTds
--    @LedgerTransactionId INT,
--    @CompanyId INT,
--    @TotalTds DECIMAL(18, 2)
--AS
--BEGIN
--    SET NOCOUNT ON;
--    UPDATE dbo.LedgerTransactions
--    SET TotalTds = @TotalTds
--    WHERE LedgerTransactionId = @LedgerTransactionId
--      AND CompanyId = @CompanyId;
--END
--GO

--IF EXISTS (SELECT 1 FROM sys.procedures WITH (NOLOCK) WHERE NAME = 'p_ledgerTransactionDetail_setTds' AND type = 'P')
--    DROP PROCEDURE dbo.p_ledgerTransactionDetail_setTds;
--GO

--CREATE PROCEDURE dbo.p_ledgerTransactionDetail_setTds
--    @CompanyId INT,
--    @LedgerTransactionId INT,
--    @BillId INT,
--    @TdsAmount DECIMAL(18, 2)
--AS
--BEGIN
--    SET NOCOUNT ON;
--    IF OBJECT_ID(N'dbo.LedgerTransactionDetails', N'U') IS NOT NULL
--    BEGIN
--        UPDATE dbo.LedgerTransactionDetails
--        SET TdsAmount = @TdsAmount
--        WHERE CompanyId = @CompanyId
--          AND LedgerTransactionId = @LedgerTransactionId
--          AND BillId = @BillId;
--    END
--    ELSE IF OBJECT_ID(N'dbo.LedgerTransactionDetail', N'U') IS NOT NULL
--    BEGIN
--        UPDATE dbo.LedgerTransactionDetail
--        SET TdsAmount = @TdsAmount
--        WHERE CompanyId = @CompanyId
--          AND LedgerTransactionId = @LedgerTransactionId
--          AND BillId = @BillId;
--    END
--END
--GO
