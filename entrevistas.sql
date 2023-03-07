CREATE DATABASE [entrevista]
GO
USE [entrevista]
GO
/****** Object:  Table [dbo].[ENTREVISTA]    Script Date: 2020-10-30 16:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ENTREVISTA](
	[id] [int] NULL,
	[vacante] [int] NOT NULL,
	[prospecto] [int] NOT NULL,
	[fecha_entrevista] [date] NULL,
	[notas] [text] NULL,
	[reclutado] [bit] NULL,
 CONSTRAINT [PK_ENTREVISTA] PRIMARY KEY CLUSTERED 
(
	[vacante] ASC,
	[prospecto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PROSPECTO]    Script Date: 2020-10-30 16:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PROSPECTO](
	[id] [int] NOT NULL,
	[nombre] [varchar](50) NULL,
	[correo] [varchar](50) NULL,
	[fecha_registro] [date] NULL,
 CONSTRAINT [PK_PROSPECTO] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VACANTE]    Script Date: 2020-10-30 16:40:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VACANTE](
	[id] [int] NOT NULL,
	[area] [varchar](50) NULL,
	[sueldo] [money] NULL,
	[activo] [bit] NULL,
 CONSTRAINT [PK_VACANTE] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ENTREVISTA]  WITH CHECK ADD  CONSTRAINT [FK_ENTREVISTA_PROSPECTO] FOREIGN KEY([prospecto])
REFERENCES [dbo].[PROSPECTO] ([id])
GO
ALTER TABLE [dbo].[ENTREVISTA] CHECK CONSTRAINT [FK_ENTREVISTA_PROSPECTO]
GO
ALTER TABLE [dbo].[ENTREVISTA]  WITH CHECK ADD  CONSTRAINT [FK_ENTREVISTA_VACANTE] FOREIGN KEY([vacante])
REFERENCES [dbo].[VACANTE] ([id])
GO
ALTER TABLE [dbo].[ENTREVISTA] CHECK CONSTRAINT [FK_ENTREVISTA_VACANTE]
GO
CREATE PROCEDURE SP_EXISTE_ENTREVISTA
@vacante int,
@prospecto int
AS
BEGIN
DECLARE @resultado BIT
IF (select count(id) from ENTREVISTA where vacante = @vacante and prospecto = @prospecto) > 0
BEGIN
SET @resultado = 1
END
ELSE
BEGIN
SET @resultado = 0
END 
select @resultado
END
GO
CREATE PROCEDURE SP_EXISTE_PROSPECTO
@id int
AS
BEGIN
DECLARE @resultado BIT
IF (select count(id) from PROSPECTO where id = @id) > 0
BEGIN
SET @resultado = 1
END
ELSE
BEGIN
SET @resultado = 0
END 
select @resultado
END
GO
CREATE PROCEDURE SP_EXISTE_VACANTE
@id int
AS
BEGIN
DECLARE @resultado BIT
IF (select count(id) from VACANTE where id = @id) > 0
BEGIN
SET @resultado = 1
END
ELSE
BEGIN
SET @resultado = 0
END 
select @resultado
END
GO
CREATE PROCEDURE SP_CONSULTAR_TABLA
@nombre_tabla varchar(100)
AS
BEGIN
SET NOCOUNT ON;
  DECLARE @consulta NVARCHAR(300);

  SET @consulta = 'select * from ' + QUOTENAME(@nombre_tabla)     
  EXEC sp_executesql @consulta
END
GO
CREATE PROCEDURE SP_ACTUALIZAR_ENTREVISTA
@id int,
@vacante int,
@prospecto int,
@fecha_entrevista date,
@notas text,
@reclutado bit
AS
BEGIN
update ENTREVISTA set id=@id, fecha_entrevista=@fecha_entrevista, notas=@notas, reclutado=@reclutado where vacante = @vacante and prospecto= @prospecto
END
GO
CREATE PROCEDURE SP_ACTUALIZAR_PROSPECTO
@id int,
@nombre varchar(50),
@correo varchar(50),
@fecha_registro date
AS
BEGIN
update PROSPECTO set nombre=@nombre, correo=@correo, fecha_registro=@fecha_registro where id = @id
END
GO
CREATE PROCEDURE SP_ACTUALIZAR_VACANTE
@id int,
@area varchar(50),
@sueldo money,
@activo bit
AS
BEGIN
update VACANTE set area=@area, sueldo=@sueldo, activo=@activo where id = @id
END
GO
CREATE PROCEDURE SP_ELIMINAR_ENTREVISTA
@vacante int,
@prospecto int
AS
BEGIN
delete from ENTREVISTA where vacante = @vacante and prospecto=@prospecto
END
GO
CREATE PROCEDURE SP_ELIMINAR_PROSPECTO
@id int
AS
BEGIN
delete from PROSPECTO where id = @id
END
GO
CREATE PROCEDURE SP_ELIMINAR_VACANTE
@id int
AS
BEGIN
delete from VACANTE where id = @id
END
GO
CREATE PROCEDURE SP_CONSULTAR_ULTIMOID
@nombre_tabla varchar(100)
AS
BEGIN
SET NOCOUNT ON;
  DECLARE @consulta NVARCHAR(300);

  SET @consulta = 'select top 1 id + 1 as proximo_id from ' + QUOTENAME(@nombre_tabla) + ' order by id desc'
  EXEC sp_executesql @consulta
END
