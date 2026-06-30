-- Quotation validity end date (separate from contract period From/To).
IF COL_LENGTH('dbo.Quotation', 'ValidUntil') IS NULL
BEGIN
    ALTER TABLE dbo.Quotation ADD ValidUntil DATE NULL;
END
GO
