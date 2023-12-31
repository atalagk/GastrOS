CREATE TABLE [Endoscopy_Type](
	[Type_ID] [int] NOT NULL PRIMARY KEY,
	[Type_Name] [nvarchar](100) NULL,
	[Op_Template] [nvarchar](100) NULL
)

CREATE TABLE [Endoscopist](
	[ID] [int] NOT NULL PRIMARY KEY,
	[Signout_Name] [nvarchar](120) NULL
)

CREATE TABLE [Device](
	[Device_ID] [int] NOT NULL PRIMARY KEY,
	[Device_Name] [nvarchar](100) NULL
)

CREATE TABLE [Department](
	[Dept_ID] [int] NOT NULL PRIMARY KEY,
	[Dept_Name] [nvarchar](50) NULL
)

CREATE TABLE [Insurer](
	[Ins_ID] [int] NOT NULL PRIMARY KEY,
	[Ins_Name] [nvarchar](50) NULL
)

CREATE TABLE [ArchIdMapping](
	[ID] [int] NOT NULL PRIMARY KEY,
	[Term] [nvarchar](100) NULL,
	[Archetype_Id] [nvarchar](255) NULL
)

CREATE TABLE [Endoscopy_Type_Organs](
	[Endoscopy_Type_Id] [int] NOT NULL,
	[Organ_Id] [int] NOT NULL,
	[Ordinal] [int] NULL,
	CONSTRAINT [PK_Endoscopy_Type_Organs] PRIMARY KEY ([Endoscopy_Type_Id],	[Organ_Id])
)

CREATE TABLE [Patient](
	[PID] [int] NOT NULL PRIMARY KEY,
	[Patient_No] [int] NOT NULL,
	[Surname] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Midname] [nvarchar](50) NULL,
	[DoB] [datetime] NULL,
	[Gender] [nvarchar](1) NULL,
	[Ethnicity] [nvarchar](50) NULL,
	[Insurer] [int] NULL
)

CREATE TABLE [Clinical](
	[Clinical_ID] [int] NOT NULL PRIMARY KEY,
	[CRF] [bit] NOT NULL,
	[HBV] [bit] NOT NULL,
	[HCV] [bit] NOT NULL,
	[HDV] [bit] NOT NULL,
	[HIV] [bit] NOT NULL,
	[details] [nvarchar](max) NULL
)

CREATE TABLE [Examination](
	[Report_ID] [int] NOT NULL PRIMARY KEY,
	[Patient_ID] [int] NOT NULL,
	[Endoscopy_Date] [datetime] NULL,
	[Report_Date] [datetime] NULL,
	[Doctor] [nvarchar](100) NULL,
	[Department] [int] NULL,
	[Endoscopy_Type] [int] NULL,
	[Device] [int] NULL,
	[Premedication] [nvarchar](50) NULL,
	[Report_Text] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[Exam_Info_Text] [nvarchar](max) NULL,
	[Findings_Text] [nvarchar](max) NULL,
	[Interventions_Text] [nvarchar](max) NULL,
	[Diagnoses_Text] [nvarchar](max) NULL,
	[Signout1] [int] NULL,
	[Signout2] [int] NULL,
	[Signout3] [int] NULL,
	[Signout4] [int] NULL,
	[Log] [nvarchar](max) NULL
)

CREATE TABLE [Exam_Report](
	[Exam_Id] [int] NOT NULL,
	[Term_Id] [int] NOT NULL,
	[XmlText] [nvarchar](max) NULL,
	CONSTRAINT [PK_Exam_Report] PRIMARY KEY ([Exam_Id],	[Term_Id])
)

ALTER TABLE [Clinical]  WITH CHECK ADD  CONSTRAINT [FK_Clinical_Patient] FOREIGN KEY([Clinical_ID])
REFERENCES [Patient] ([PID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Clinical] CHECK CONSTRAINT [FK_Clinical_Patient]

ALTER TABLE [Endoscopy_Type_Organs]  WITH CHECK ADD  CONSTRAINT [FK_Endoscopy_Type_Organs_ArchIdMapping] FOREIGN KEY([Organ_Id])
REFERENCES [ArchIdMapping] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Endoscopy_Type_Organs] CHECK CONSTRAINT [FK_Endoscopy_Type_Organs_ArchIdMapping]

ALTER TABLE [Endoscopy_Type_Organs]  WITH CHECK ADD  CONSTRAINT [FK_Endoscopy_Type_Organs_Endoscopy_Type] FOREIGN KEY([Endoscopy_Type_Id])
REFERENCES [Endoscopy_Type] ([Type_ID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Endoscopy_Type_Organs] CHECK CONSTRAINT [FK_Endoscopy_Type_Organs_Endoscopy_Type]

ALTER TABLE [Exam_Report]  WITH CHECK ADD  CONSTRAINT [FK_Exam_Report_ArchIdMapping] FOREIGN KEY([Term_Id])
REFERENCES [ArchIdMapping] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Exam_Report] CHECK CONSTRAINT [FK_Exam_Report_ArchIdMapping]

ALTER TABLE [Exam_Report]  WITH CHECK ADD  CONSTRAINT [FK_Exam_Report_Examination] FOREIGN KEY([Exam_Id])
REFERENCES [Examination] ([Report_ID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Exam_Report] CHECK CONSTRAINT [FK_Exam_Report_Examination]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Department] FOREIGN KEY([Department])
REFERENCES [Department] ([Dept_ID])
ON UPDATE CASCADE
ON DELETE SET NULL

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Department]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Device] FOREIGN KEY([Device])
REFERENCES [Device] ([Device_ID])
ON UPDATE CASCADE
ON DELETE SET NULL

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Device]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Endoscopy_Type] FOREIGN KEY([Endoscopy_Type])
REFERENCES [Endoscopy_Type] ([Type_ID])
ON UPDATE CASCADE
ON DELETE SET NULL

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Endoscopy_Type]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Patient] FOREIGN KEY([Patient_ID])
REFERENCES [Patient] ([PID])
ON UPDATE CASCADE
ON DELETE CASCADE

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Patient]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Signout1] FOREIGN KEY([Signout1])
REFERENCES [Endoscopist] ([ID])

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Signout1]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Signout2] FOREIGN KEY([Signout2])
REFERENCES [Endoscopist] ([ID])

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Signout2]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Signout3] FOREIGN KEY([Signout3])
REFERENCES [Endoscopist] ([ID])

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Signout3]

ALTER TABLE [Examination]  WITH CHECK ADD  CONSTRAINT [FK_Examination_Signout4] FOREIGN KEY([Signout4])
REFERENCES [Endoscopist] ([ID])

ALTER TABLE [Examination] CHECK CONSTRAINT [FK_Examination_Signout4]

ALTER TABLE [Patient]  WITH CHECK ADD  CONSTRAINT [FK_Patient_Patient] FOREIGN KEY([Insurer])
REFERENCES [Insurer] ([Ins_ID])
ON UPDATE CASCADE
ON DELETE SET NULL

ALTER TABLE [Patient] CHECK CONSTRAINT [FK_Patient_Patient]

