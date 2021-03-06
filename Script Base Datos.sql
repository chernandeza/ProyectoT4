USE [master]
GO
/****** Object:  Database [T3SJTest1]    Script Date: 8/7/2016 9:43:33 PM ******/
CREATE DATABASE [T3SJTest1]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'T3SJTest1', FILENAME = N'F:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\T3SJTest1.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'T3SJTest1_log', FILENAME = N'F:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\T3SJTest1_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [T3SJTest1] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [T3SJTest1].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [T3SJTest1] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [T3SJTest1] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [T3SJTest1] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [T3SJTest1] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [T3SJTest1] SET ARITHABORT OFF 
GO
ALTER DATABASE [T3SJTest1] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [T3SJTest1] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [T3SJTest1] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [T3SJTest1] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [T3SJTest1] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [T3SJTest1] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [T3SJTest1] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [T3SJTest1] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [T3SJTest1] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [T3SJTest1] SET  DISABLE_BROKER 
GO
ALTER DATABASE [T3SJTest1] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [T3SJTest1] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [T3SJTest1] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [T3SJTest1] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [T3SJTest1] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [T3SJTest1] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [T3SJTest1] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [T3SJTest1] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [T3SJTest1] SET  MULTI_USER 
GO
ALTER DATABASE [T3SJTest1] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [T3SJTest1] SET DB_CHAINING OFF 
GO
ALTER DATABASE [T3SJTest1] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [T3SJTest1] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [T3SJTest1] SET DELAYED_DURABILITY = DISABLED 
GO
USE [T3SJTest1]
GO
/****** Object:  Table [dbo].[Comentario]    Script Date: 8/7/2016 9:43:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Comentario](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[comentario] [nvarchar](max) NOT NULL,
	[cedula_persona] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Comentario] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Persona]    Script Date: 8/7/2016 9:43:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Persona](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cedula] [nvarchar](50) NOT NULL,
	[nombre] [nvarchar](50) NOT NULL,
	[apellido1] [nvarchar](50) NOT NULL,
	[apellido2] [nvarchar](50) NULL,
	[provincia] [smallint] NOT NULL,
	[genero] [nvarchar](50) NOT NULL,
	[autorizada] [bit] NULL,
 CONSTRAINT [PK_Persona] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Comentario] ON 

INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (1, N'Hola 1', N'05-2500-3500')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (2, N'Hola 2', N'05-2500-3500')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (3, N'Hola 3', N'05-2500-3500')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (4, N'Comentario 1', N'01-1000-2000')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (5, N'Comentario 2', N'01-1000-2000')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (7, N'Hola 4', N'05-2500-3500')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (8, N'Comentario 3', N'01-1000-2000')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (9, N'Hola amigos', N'02-0333-5555')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (10, N'Silencio', N'04-5984-1542')
INSERT [dbo].[Comentario] ([id], [comentario], [cedula_persona]) VALUES (11, N'Prueba 1', N'06-3333-4444')
SET IDENTITY_INSERT [dbo].[Comentario] OFF
SET IDENTITY_INSERT [dbo].[Persona] ON 

INSERT [dbo].[Persona] ([id], [cedula], [nombre], [apellido1], [apellido2], [provincia], [genero], [autorizada]) VALUES (6, N'01-1000-2000', N'Carlos', N'Hernandez', N'Alvarado', 3, N'Masculino', 1)
INSERT [dbo].[Persona] ([id], [cedula], [nombre], [apellido1], [apellido2], [provincia], [genero], [autorizada]) VALUES (7, N'02-0333-5555', N'Luis', N'Montero', N'Solano', 0, N'Otro', 1)
INSERT [dbo].[Persona] ([id], [cedula], [nombre], [apellido1], [apellido2], [provincia], [genero], [autorizada]) VALUES (8, N'04-5984-1542', N'Mayra', N'Bustos', N'Camacho', 4, N'Femenino', 0)
INSERT [dbo].[Persona] ([id], [cedula], [nombre], [apellido1], [apellido2], [provincia], [genero], [autorizada]) VALUES (9, N'06-3333-4444', N'LKAJLDJ', N'LAKJDLKDJ', N'LJLKJLKJLKJ', 0, N'NoIndica', 0)
INSERT [dbo].[Persona] ([id], [cedula], [nombre], [apellido1], [apellido2], [provincia], [genero], [autorizada]) VALUES (10, N'05-2500-3500', N'Pedro', N'Juarez', N'Arroyo', 2, N'Masculino', 0)
SET IDENTITY_INSERT [dbo].[Persona] OFF
ALTER TABLE [dbo].[Comentario]  WITH CHECK ADD  CONSTRAINT [FK_Comentario_Comentario] FOREIGN KEY([id])
REFERENCES [dbo].[Comentario] ([id])
GO
ALTER TABLE [dbo].[Comentario] CHECK CONSTRAINT [FK_Comentario_Comentario]
GO
/****** Object:  StoredProcedure [dbo].[sp_GuardarComentario]    Script Date: 8/7/2016 9:43:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GuardarComentario] 
	@cedula nvarchar(50) = '',
	@texto nvarchar(MAX) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO Comentario(cedula_persona, comentario)
	VALUES (@cedula, @texto)
END

GO
/****** Object:  StoredProcedure [dbo].[sp_ObtenerCedulas]    Script Date: 8/7/2016 9:43:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ObtenerCedulas]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT cedula FROM Persona
END

GO
/****** Object:  StoredProcedure [dbo].[sp_retornarComentariosPersona]    Script Date: 8/7/2016 9:43:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Carlos Hernandez>
-- Create date: <7 de agosto de 2016>
-- Description:	<Retorna los comentarios de una persona>
-- =============================================
CREATE PROCEDURE [dbo].[sp_retornarComentariosPersona] 
	-- Add the parameters for the stored procedure here
	@cedPersona nvarchar(50) = ''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT id,comentario FROM Comentario as C WHERE C.cedula_persona = @cedPersona
END

GO
USE [master]
GO
ALTER DATABASE [T3SJTest1] SET  READ_WRITE 
GO
