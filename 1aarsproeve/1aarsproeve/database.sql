﻿CREATE TABLE [dbo].[Stillinger] (
    [StillingId] INT          IDENTITY (1, 1) NOT NULL,
    [Stilling]   VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([StillingId] ASC)
);
INSERT INTO Stillinger VALUES ('Butikschef');
INSERT INTO Stillinger VALUES ('Ungarbejder');
INSERT INTO Stillinger VALUES ('Nøglebærer');
INSERT INTO Stillinger VALUES ('Flaskedreng');

CREATE TABLE [dbo].[Ugedage] (
    [UgedagId] INT         IDENTITY (1, 1) NOT NULL,
    [Ugedag]   VARCHAR (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([UgedagId] ASC)
);
INSERT INTO Ugedage VALUES ('Mandag');
INSERT INTO Ugedage VALUES ('Tirsdag');
INSERT INTO Ugedage VALUES ('Onsdag');
INSERT INTO Ugedage VALUES ('Torsdag');
INSERT INTO Ugedage VALUES ('Fredag');
INSERT INTO Ugedage VALUES ('Lørdag');
INSERT INTO Ugedage VALUES ('Søndag');

CREATE TABLE [dbo].[Ansatte] (
    [Brugernavn] VARCHAR (50) NOT NULL,
    [Navn]       VARCHAR (50) NULL,
    [Password]   VARCHAR (50) NOT NULL,
    [Email]      VARCHAR (50) NULL,
    [Mobil]      VARCHAR (50) NULL,
    [Adresse]    VARCHAR (50) NULL,
    [Postnummer] VARCHAR (50) NULL,
    [StillingId] INT          DEFAULT ((2)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Brugernavn] ASC),
    CONSTRAINT [FK_Ansatte_Roller] FOREIGN KEY ([StillingId]) REFERENCES [dbo].[Stillinger] ([StillingId])
);
INSERT INTO Ugedage VALUES ('Daniel', 'Daniel Winther', 'daniel', 'daniel@hotmail.dk', '88888888', 'Degnestavnen 99', '2400', 1);

CREATE TABLE [dbo].[Vagter] (
    [VagtId]         INT          IDENTITY (1, 1) NOT NULL,
    [Starttidspunkt] TIME (0)     NOT NULL,
    [Sluttidspunkt]  TIME (0)     NOT NULL,
    [Ugenummer]      INT          NOT NULL,
    [UgedagId]       INT          NOT NULL,
    [Brugernavn]     VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([VagtId] ASC),
    CONSTRAINT [FK_Vagter_Ugedage] FOREIGN KEY ([UgedagId]) REFERENCES [dbo].[Ugedage] ([UgedagId]),
    CONSTRAINT [FK_Vagter_Ansatte] FOREIGN KEY ([Brugernavn]) REFERENCES [dbo].[Ansatte] ([Brugernavn])
);
INSERT INTO Vagter VALUES ('10:00', '16:00', 18, 1, 'Daniel');

CREATE TABLE [dbo].[Beskeder] (
    [BeskedId]    INT          IDENTITY (1, 1) NOT NULL,
    [Overskrift]  VARCHAR (50) NULL,
    [Dato]        DATE         NOT NULL,
    [Beskrivelse] TEXT         NULL,
    [Udloebsdato] DATE         NOT NULL,
    [Brugernavn]  VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([BeskedId] ASC),
    CONSTRAINT [FK_Beskeder_Ansatte] FOREIGN KEY ([Brugernavn]) REFERENCES [dbo].[Ansatte] ([Brugernavn])
);
INSERT INTO Beskeder VALUES ('MUS-samtaler', '2015-04-04', 'Så er der MUS-samtaler!', '2015-05-15', 'Daniel');