CREATE TABLE PDF
USE PDF

CREATE TABLE [dbo].[PDF_FILES]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Nombre] [varchar](250) NULL,
    [Archivo] [varchar](max) NULL
) 

GO
CREATE PROCEDURE [dbo].[sp_obtener_pdfs]
AS
BEGIN
    SELECT *
    FROM PDF_FILES
END

CREATE PROCEDURE [dbo].[sp_insertar_pdf]
    @Nombre VARCHAR(250),
    @Archivo VARCHAR(MAX)
AS
BEGIN
    INSERT INTO PDF_FILES
    VALUES(@Nombre, @Archivo)
END

CREATE PROCEDURE [dbo].[sp_buscar_pdf]
    @Id INT
as
BEGIN
    SELECT *
    FROM PDF_FILES
    WHERE Id=@Id
END