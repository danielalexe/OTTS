--OTTS - Teacher Priorities #32

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TEACHERS]') 
         AND name = 'iPRIORITY'
)
BEGIN
ALTER TABLE dbo.TEACHERS
ADD iPRIORITY int NULL
END
GO

UPDATE dbo.TEACHERS 
SET iPRIORITY=0 WHERE iPRIORITY IS NULL
GO

ALTER TABLE dbo.TEACHERS 
ALTER COLUMN iPRIORITY int NOT NULL
GO

--OTTS - Multi Semester System #31
/****** Object:  Table [dbo].[SEMESTERS]    Script Date: 30-Aug-19 11:02:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'SEMESTERS')
BEGIN
    --Do Stuff
	CREATE TABLE [dbo].[SEMESTERS](
	[iID_SEMESTER] [int] IDENTITY(1,1) NOT NULL,
	[nvNAME] [varchar](50) NOT NULL,
	[bACTIVE] [bit] NOT NULL,
	[dtCREATE_DATE] [datetime] NOT NULL,
	[dtLASTMODIFIED_DATE] [datetime] NULL,
	[iCREATE_USER] [int] NOT NULL,
	[iLASTMODIFIED_USER] [int] NULL,
 CONSTRAINT [PK_SEMESTERS] PRIMARY KEY CLUSTERED 
(
	[iID_SEMESTER] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET IDENTITY_INSERT [dbo].[SEMESTERS] ON 

INSERT [dbo].[SEMESTERS] ([iID_SEMESTER], [nvNAME], [bACTIVE], [dtCREATE_DATE], [dtLASTMODIFIED_DATE], [iCREATE_USER], [iLASTMODIFIED_USER]) VALUES (1, N'Sem1', 1, CAST(N'2019-08-30T00:00:00.000' AS DateTime), NULL, 1, NULL)

INSERT [dbo].[SEMESTERS] ([iID_SEMESTER], [nvNAME], [bACTIVE], [dtCREATE_DATE], [dtLASTMODIFIED_DATE], [iCREATE_USER], [iLASTMODIFIED_USER]) VALUES (2, N'Sem2', 1, CAST(N'2019-08-30T00:00:00.000' AS DateTime), NULL, 1, NULL)

SET IDENTITY_INSERT [dbo].[SEMESTERS] OFF

END
GO

--IF NOT EXISTS (
--  SELECT * 
--  FROM   sys.columns 
--  WHERE  object_id = OBJECT_ID(N'[dbo].[TEACHERS_LECTURES_LINK]') 
--         AND name = 'iID_SEMESTER'
--)
--BEGIN
--END
--GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TEACHERS_LECTURES_LINK]') 
         AND name = 'iID_SEMESTER'
)
BEGIN
ALTER TABLE dbo.TEACHERS_LECTURES_LINK
ADD iID_SEMESTER int null
END
GO

UPDATE dbo.TEACHERS_LECTURES_LINK 
SET iID_SEMESTER=1 WHERE iID_SEMESTER IS NULL
GO

ALTER TABLE dbo.TEACHERS_LECTURES_LINK 
ALTER COLUMN iID_SEMESTER int NOT NULL
GO

ALTER TABLE [dbo].[TEACHERS_LECTURES_LINK]  WITH CHECK ADD  CONSTRAINT [FK_TEACHERS_LECTURES_LINK_SEMESTERS] FOREIGN KEY([iID_SEMESTER])
REFERENCES [dbo].[SEMESTERS] ([iID_SEMESTER])
GO

ALTER TABLE [dbo].[TEACHERS_LECTURES_LINK] CHECK CONSTRAINT [FK_TEACHERS_LECTURES_LINK_SEMESTERS]
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[LECTURES]') 
         AND name = 'iID_SEMESTER'
)
BEGIN
ALTER TABLE dbo.LECTURES
ADD iID_SEMESTER int null
END
GO

UPDATE dbo.LECTURES 
SET iID_SEMESTER=1 WHERE iID_SEMESTER IS NULL
GO

ALTER TABLE dbo.LECTURES 
ALTER COLUMN iID_SEMESTER int NOT NULL
GO

ALTER TABLE [dbo].[LECTURES]  WITH CHECK ADD  CONSTRAINT [FK_LECTURES_SEMESTERS] FOREIGN KEY([iID_SEMESTER])
REFERENCES [dbo].[SEMESTERS] ([iID_SEMESTER])
GO
ALTER TABLE [dbo].[LECTURES] CHECK CONSTRAINT [FK_LECTURES_SEMESTERS]
GO
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[GROUPS_LECTURES_LINK]') 
         AND name = 'iID_SEMESTER'
)
BEGIN
ALTER TABLE dbo.GROUPS_LECTURES_LINK
ADD iID_SEMESTER int null
END
GO

UPDATE dbo.GROUPS_LECTURES_LINK 
SET iID_SEMESTER=1 WHERE iID_SEMESTER IS NULL
GO

ALTER TABLE dbo.GROUPS_LECTURES_LINK 
ALTER COLUMN iID_SEMESTER int NOT NULL
GO

ALTER TABLE [dbo].[GROUPS_LECTURES_LINK]  WITH CHECK ADD  CONSTRAINT [FK_GROUPS_LECTURES_LINK_SEMESTERS] FOREIGN KEY([iID_SEMESTER])
REFERENCES [dbo].[SEMESTERS] ([iID_SEMESTER])
GO
ALTER TABLE [dbo].[GROUPS_LECTURES_LINK] CHECK CONSTRAINT [FK_GROUPS_LECTURES_LINK_SEMESTERS]
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[TIMETABLE_PLANNING]') 
         AND name = 'iID_SEMESTER'
)
BEGIN
ALTER TABLE dbo.TIMETABLE_PLANNING
ADD iID_SEMESTER int null
END
GO

UPDATE dbo.TIMETABLE_PLANNING 
SET iID_SEMESTER=1 WHERE iID_SEMESTER IS NULL
GO

ALTER TABLE dbo.TIMETABLE_PLANNING 
ALTER COLUMN iID_SEMESTER int NOT NULL
GO


DELETE FROM dbo.SETTINGS
WHERE iKEY = 1337
GO

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[dbo].[SEMESTERS]') 
         AND name = 'iGENERATION_NUMBER'
)
BEGIN
ALTER TABLE dbo.SEMESTERS
ADD iGENERATION_NUMBER int null
END
GO

UPDATE dbo.SEMESTERS 
SET iGENERATION_NUMBER=1 WHERE iGENERATION_NUMBER IS NULL
GO

ALTER TABLE dbo.SEMESTERS 
ALTER COLUMN iGENERATION_NUMBER int NOT NULL
GO

UPDATE dbo.SETTINGS
SET iVALUE=2,nvVALUE=N'v0.2.0',iLASTMODIFIED_USER=1,dtLASTMODIFIED_DATE = GETDATE()
WHERE iKEY=1001
GO