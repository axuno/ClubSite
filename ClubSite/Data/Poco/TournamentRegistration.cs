// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubSite.Data.Poco
{
    /*
CREATE TABLE dbo.Club_TournamentRegistration (
  Id bigint NOT NULL,
  Guid nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT '' NULL,
  TournamentDate date NOT NULL,
  Rank int NULL,
  TeamName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT '' NOT NULL,
  ClubName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  Gender nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT 'u' NOT NULL,
  FirstName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT '' NOT NULL,
  LastName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT '' NOT NULL,
  Nickname nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  Fone nvarchar(40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  Email nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  Message nvarchar(4000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  IsStandByRegistration bit NOT NULL,
  RegisteredOn datetime NULL,
  RegCanceledOn datetime NULL,
  CreatedOn datetime DEFAULT getdate() NOT NULL,
  ModifiedOn datetime DEFAULT getdate() NOT NULL,
  CONSTRAINT Club_TournamentRegistration_pk PRIMARY KEY CLUSTERED (Id)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO
    */

    /*
    * Export aus MySQL Web Datenbank am 04.09.2020

INSERT INTO dbo.Club_TournamentRegistration (Id, Guid, TournamentDate, Rank, TeamName, ClubName, Gender, FirstName, LastName, Nickname, Fone, Email, Message, IsStandByRegistration, RegisteredOn, RegCancelledOn, CreatedOn, ModifiedOn)
VALUES 
  (1, NULL, N'1999-03-28', 1, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (2, NULL, N'1999-03-28', 2, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (3, NULL, N'1999-03-28', 3, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (4, NULL, N'1999-03-28', 4, N'Grufties', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (5, NULL, N'1999-03-28', 5, N'Internet', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (6, NULL, N'1999-03-28', 6, N'TSV Steppach', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (7, NULL, N'1999-03-28', 7, N'Lechtal Schnaufer', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (8, NULL, N'1999-03-28', 8, N'Just for Fun', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (9, NULL, N'1999-03-28', 9, N'Schdäragugger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (10, NULL, N'1999-03-28', 10, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (11, NULL, N'1999-03-28', 11, N'Die kleinen Erdlinge', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (12, NULL, N'1999-03-28', 12, N'Hobby Deuringen', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '19990328', '20120218 23:17:28'),
  (13, NULL, N'2000-04-02', 1, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (14, NULL, N'2000-04-02', 2, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (15, NULL, N'2000-04-02', 3, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (16, NULL, N'2000-04-02', 4, N'Die Sieben Chlorleichen', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (17, NULL, N'2000-04-02', 5, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (18, NULL, N'2000-04-02', 6, N'Spätzünder', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (19, NULL, N'2000-04-02', 7, N'Der Krater bebt', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (20, NULL, N'2000-04-02', 8, N'Lechtal Schnaufer', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (21, NULL, N'2000-04-02', 9, N'Hobby Ehekirchen', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (22, NULL, N'2000-04-02', 10, N'Grufties', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (23, NULL, N'2000-04-02', 11, N'Schdäragugger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (24, NULL, N'2000-04-02', 12, N'Just for Fun', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (25, NULL, N'2000-04-02', 13, N'Hobby Hiltenfingen', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (26, NULL, N'2000-04-02', 14, N'Hobby Deuringen', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (27, NULL, N'2000-04-02', 15, N'Catweazle', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (28, NULL, N'2000-04-02', 16, N'Wickie und die starken Männer', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20000402', '20120218 23:17:28'),
  (29, NULL, N'2001-04-01', 1, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (30, NULL, N'2001-04-01', 2, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (31, NULL, N'2001-04-01', 3, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (32, NULL, N'2001-04-01', 4, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (33, NULL, N'2001-04-01', 5, N'Sch... egal', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (34, NULL, N'2001-04-01', 6, N'Spätzünder', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (35, NULL, N'2001-04-01', 7, N'Los Locotos', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (36, NULL, N'2001-04-01', 8, N'Seven up', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (37, NULL, N'2001-04-01', 9, N'Just for fun', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (38, NULL, N'2001-04-01', 10, N'Vorsicht Stufe', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (39, NULL, N'2001-04-01', 11, N'Grufties', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (40, NULL, N'2001-04-01', 12, N'Der Krater bebt', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20010401', '20120218 23:17:28'),
  (41, NULL, N'2002-03-24', 1, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (42, NULL, N'2002-03-24', 2, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (43, NULL, N'2002-03-24', 3, N'Schwimmteam', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (44, NULL, N'2002-03-24', 4, N'Sch... egal', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (45, NULL, N'2002-03-24', 5, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (46, NULL, N'2002-03-24', 6, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (47, NULL, N'2002-03-24', 7, N'Haifische', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (48, NULL, N'2002-03-24', 8, N'Cappuccino', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (49, NULL, N'2002-03-24', 9, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (50, NULL, N'2002-03-24', 10, N'Spätzünder', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (51, NULL, N'2002-03-24', 11, N'Grufties', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (52, NULL, N'2002-03-24', 12, N'Superfreunde', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20020324', '20120218 23:17:28'),
  (53, NULL, N'2003-03-23', 1, N'Die Zuagroastn', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (54, NULL, N'2003-03-23', 2, N'Jumping Ducks', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (55, NULL, N'2003-03-23', 3, N'Smashin'' Pumpkins', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (56, NULL, N'2003-03-23', 4, N'L` Alibi', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (57, NULL, N'2003-03-23', 5, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (58, NULL, N'2003-03-23', 6, N'Schwimmteam Neusäß', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (59, NULL, N'2003-03-23', 7, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (60, NULL, N'2003-03-23', 8, N'Schdäraguggr', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (61, NULL, N'2003-03-23', 9, N'Spätzünder', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (62, NULL, N'2003-03-23', 10, N'Los Locotos', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (63, NULL, N'2003-03-23', 11, N'Haifisch', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (64, NULL, N'2003-03-23', 12, N'Vorsicht Stufe', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (65, NULL, N'2003-03-23', 13, N'Fortuna Fehlschlag', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (66, NULL, N'2003-03-23', 14, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (67, NULL, N'2003-03-23', 15, N'Internet', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (68, NULL, N'2003-03-23', 16, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (69, NULL, N'2003-03-23', 17, N'Just for fun', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (70, NULL, N'2003-03-23', 18, N'Die Knieschoner', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20030323', '20120218 23:17:28'),
  (71, NULL, N'2004-03-28', 1, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (72, NULL, N'2004-03-28', 2, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (73, NULL, N'2004-03-28', 3, N'Haifisch', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (74, NULL, N'2004-03-28', 4, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (75, NULL, N'2004-03-28', 5, N'Oldies und Endorfine', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (76, NULL, N'2004-03-28', 6, N'Schdäraguggr', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (77, NULL, N'2004-03-28', 7, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (78, NULL, N'2004-03-28', 8, N'Vorsicht Stufe', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (79, NULL, N'2004-03-28', 9, N'Die Zuagroastn', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (80, NULL, N'2004-03-28', 10, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (81, NULL, N'2004-03-28', 11, N'Affendennis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (82, NULL, N'2004-03-28', 12, N'Grufties', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20040328', '20120218 23:17:28'),
  (83, NULL, N'2005-04-03', 1, N'L'' Alibi', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (84, NULL, N'2005-04-03', 2, N'Sch... egal', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (85, NULL, N'2005-04-03', 3, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (86, NULL, N'2005-04-03', 4, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (87, NULL, N'2005-04-03', 5, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (88, NULL, N'2005-04-03', 6, N'Cappucino', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (89, NULL, N'2005-04-03', 7, N'KO''s Pritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (90, NULL, N'2005-04-03', 8, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (91, NULL, N'2005-04-03', 9, N'He Duda', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (92, NULL, N'2005-04-03', 10, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (93, NULL, N'2005-04-03', 11, N'Nobody''s perfect', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (94, NULL, N'2005-04-03', 12, N'Die Unbelehrbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20050403', '20120218 23:17:28'),
  (95, NULL, N'2006-04-02', 1, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (96, NULL, N'2006-04-02', 2, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (97, NULL, N'2006-04-02', 3, N'Internet', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (98, NULL, N'2006-04-02', 4, N'Sch...egal', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (99, NULL, N'2006-04-02', 5, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (100, NULL, N'2006-04-02', 6, N'He Duda', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (101, NULL, N'2006-04-02', 7, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (102, NULL, N'2006-04-02', 8, N'Happy Hippos', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (103, NULL, N'2006-04-02', 9, N'KO''s Pritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (104, NULL, N'2006-04-02', 10, N'Vorsicht Stufe', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (105, NULL, N'2006-04-02', 11, N'Schdäraguggr', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (106, NULL, N'2006-04-02', 12, N'Die Unbelehrbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20060402', '20120218 23:17:28'),
  (107, NULL, N'2007-03-25', 1, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (108, NULL, N'2007-03-25', 2, N'SC Eching II', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (109, NULL, N'2007-03-25', 3, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (110, NULL, N'2007-03-25', 4, N'L'' Alibi', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (111, NULL, N'2007-03-25', 5, N'He Duda', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (112, NULL, N'2007-03-25', 6, N'Vorsicht Stufe', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (113, NULL, N'2007-03-25', 7, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (114, NULL, N'2007-03-25', 8, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (115, NULL, N'2007-03-25', 9, N'Sch...egal', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (116, NULL, N'2007-03-25', 10, N'Die Unbelehrbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (117, NULL, N'2007-03-25', 11, N'Blockbusters', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (118, NULL, N'2007-03-25', 12, N'The Hornies', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20070325', '20120218 23:17:28'),
  (119, NULL, N'2008-04-13', 1, N'Löschzwerge', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (120, NULL, N'2008-04-13', 2, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (121, NULL, N'2008-04-13', 3, N'Hurrikan', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (122, NULL, N'2008-04-13', 4, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (123, NULL, N'2008-04-13', 5, N'Super Burschis', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (124, NULL, N'2008-04-13', 6, N'He Duda', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (125, NULL, N'2008-04-13', 7, N'Rubber Baby Buggy Bumpers', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (126, NULL, N'2008-04-13', 8, N'Die Knieschoner', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (127, NULL, N'2008-04-13', 9, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (128, NULL, N'2008-04-13', 10, N'Nimm''n du', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (129, NULL, N'2008-04-13', 11, N'Die Unbelehrbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (130, NULL, N'2008-04-13', 12, N'Not und Elend', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20080413', '20120218 23:17:28'),
  (131, NULL, N'2009-03-29', 1, N'Querschläger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (132, NULL, N'2009-03-29', 2, N'Die Unberechenbaren', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (133, NULL, N'2009-03-29', 3, N'Volleytigger', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (134, NULL, N'2009-03-29', 4, N'Team & Struppi', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (135, NULL, N'2009-03-29', 5, N'Talfestpritscher', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (136, NULL, N'2009-03-29', 6, N'He Duda', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (137, NULL, N'2009-03-29', 7, N'Unbelehrbare', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (138, NULL, N'2009-03-29', 8, N'Die Knieschoner', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (139, NULL, N'2009-03-29', 9, N'Pizza 16', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (140, NULL, N'2009-03-29', 10, N'Haifisch', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (141, NULL, N'2009-03-29', 11, N'Rubber Baby Buggy Bumpers', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (142, NULL, N'2009-03-29', 12, N'Lass''n mir', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (143, NULL, N'2009-03-29', 13, N'Turnbeutelvergesser', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (144, NULL, N'2009-03-29', 14, N'Unglaubliche', NULL, N'', N'', N'', NULL, NULL, NULL, NULL, 0, NULL, NULL, '20090329', '20120218 23:17:28'),
  (145, N'edcdf820-ffac-48d1-aab0-e7277943269c', N'2010-03-21', 12, N'Nimm''n Du', NULL, N'f', N'Susanne', N'Aigner', NULL, N'0821/606831', N'amelie.franka@t-online.de', NULL, 0, '20091110 15:25:58', NULL, '20091110 15:25:58', '20120218 23:17:28'),
  (146, N'7008c907-a066-4c30-ad41-2f7f6843a569', N'2010-03-21', 4, N'Die PANGalaktischen Donnerknödl', NULL, N'm', N'Heiner', N'Stepen', NULL, N'1718236963', N'heiner@pgdk.de', NULL, 0, '20091117 16:19:21', NULL, '20091117 16:19:21', '20120218 23:17:28'),
  (147, N'391838e7-09cd-4e25-a72d-6f79c705b69f', N'2010-03-21', 5, N'Kohl Hasa', NULL, N'f', N'Laura', N'Heilrath', NULL, N'0881- 6707', N'laura.heilrath@go4more.de', NULL, 0, '20091130 21:18:23', NULL, '20091130 21:18:23', '20120218 23:17:28'),
  (148, N'8a4cb606-0ba4-4ca1-ad76-53fde7d20d7b', N'2010-03-21', 2, N'Team & Struppi', NULL, N'f', N'Beate', N'Rüping', NULL, N'0821/5082434', N'bluecat99@gmx.de', NULL, 0, '20091201 17:52:44', NULL, '20091201 17:52:44', '20120218 23:17:28'),
  (149, N'118efd5c-0804-4cf9-af1e-ac7c21afcf96', N'2010-03-21', 1, N'Die Unberechenbaren', NULL, N'f', N'Hilde', N'Bietsch', NULL, N'0821 469996', N'hb@bietsch.net', NULL, 0, '20091206 17:28:26', NULL, '20091206 17:28:26', '20120218 23:17:28'),
  (150, N'8a731b80-8676-4c30-a017-763356625506', N'2010-03-21', 15, N'Turnbeutelvergesser', NULL, N'm', N'Thiemo', N'Frosch', NULL, N'08232 / 99 68 32', N'thiemo-frosch@t-online.de', NULL, 0, '20091221 13:11:35', NULL, '20091221 13:11:35', '20120218 23:17:28'),
  (151, N'86412fe8-e176-49d7-809b-034b817afc06', N'2010-03-21', 7, N'He Duda', NULL, N'm', N'Rupert', N'Schöttler', NULL, N'0821/9983338', N'rupert.schoettler@gmx.de', NULL, 0, '20100104 18:27:56', NULL, '20100104 18:27:56', '20120218 23:17:28'),
  (152, N'6eabe007-1cfc-4650-9ccf-59b622b605fb', N'2010-03-21', 13, N'Lass''n mir', NULL, N'm', N'Rolf', N'Ocklenburg', NULL, N'821667408', N'rolf.ocklenburg@t-online.de', NULL, 0, '20100104 21:19:19', NULL, '20100104 21:19:19', '20120218 23:17:28'),
  (153, N'70b4c378-b376-45d8-87fb-168f6999c96d', N'2010-03-21', 3, N'Unbelehrbare', NULL, N'm', N'Harro', N'Wolf', NULL, N'0821 4861896', N'harro-wolf@t-online.de', NULL, 0, '20100110 17:37:37', NULL, '20100110 17:37:37', '20120218 23:17:28'),
  (154, N'269150ad-04c0-4ff1-9af6-41c3d53376aa', N'2010-03-21', 6, N'Freecheese & Nutella Company', NULL, N'm', N'Wolfgang', N'Huber', NULL, N'08233 849474', N'gnagflow.huber@arcor.de', NULL, 0, '20100112 09:12:15', NULL, '20100112 09:12:15', '20120218 23:17:28'),
  (155, N'ce004240-8350-4bd0-b4d9-1cbeed27c927', N'2010-03-21', 11, N'Die Knieschoner', NULL, N'f', N'Monika', N'Diepold', NULL, N'0821/60999887', N'schnurpsl8482@aol.com', NULL, 0, '20100113 19:58:20', NULL, '20100113 19:58:20', '20120218 23:17:28'),
  (156, N'03aceb32-a881-4c63-b9e7-0c90ac46b67e', N'2010-03-21', 10, N'Querschläger', NULL, N'm', N'Tommi', N'Nieborowsky', NULL, N'(0821) 782804', N'nieborowsky@gmx.de', NULL, 0, '20100118 19:23:32', NULL, '20100118 19:23:32', '20120218 23:17:28'),
  (157, N'a66ecd8f-e801-46fd-9318-35ed70bc8491', N'2010-03-21', 14, N'Talfestpritscher', NULL, N'm', N'Marcus', N'Grasmann', NULL, N'0821-467941', N'marcus.grasmann@vr-web.de', NULL, 0, '20100119 18:01:13', NULL, '20100119 18:01:13', '20120218 23:17:28'),
  (158, N'88857c8b-80cc-407b-975e-68c9bfd3f3b2', N'2010-03-21', 16, N'Los Locotos', NULL, N'f', N'Petra', N'Schlehhuber', NULL, N'08233-4498', N'petra@schlehhuber.de', NULL, 0, '20100126 15:53:23', NULL, '20100126 15:53:23', '20120218 23:17:28'),
  (159, N'8fd2ef4c-b8db-4fac-8a61-99768a082bea', N'2010-03-21', 8, N'Generation X', NULL, N'm', N'Walther', N'Bednarz', NULL, N'08431/538405', N'info@wblt.de', NULL, 0, '20100208 21:24:08', NULL, '20100208 21:24:08', '20120218 23:17:28'),
  (160, N'f99ce08f-dbd9-4762-b4e1-72632ebc1a5a', N'2010-03-21', 9, N'Einer fehlt', NULL, N'f', N'Erika', N'Geffner', NULL, N'08272/3404', N'egeffner@bndlg.de', N'MANNSCHAFTSNAME FOLGT! ehem. Superburschis', 0, '20100210 22:07:48', NULL, '20100210 22:07:48', '20120218 23:17:28'),
  (161, N'128efd5c-0804-4cf9-af1e-ac7c21afcf96', N'2011-03-20', 2, N'Die Unberechenbaren', NULL, N'f', N'Hilde', N'Bietsch', NULL, N'0821 469996', N'hb@bietsch.net', NULL, 0, '20101114 09:54:45', NULL, '20101114 09:54:45', '20120218 23:17:28'),
  (162, N'12b4c378-b376-45d8-87fb-168f6999c96d', N'2011-03-20', 5, N'Unbelehrbare', NULL, N'm', N'Harro', N'Wolf', NULL, N'0821 4861896', N'harro-wolf@t-online.de', NULL, 0, '20101114 09:55:30', NULL, '20101114 09:55:30', '20120218 23:17:28'),
  (163, N'9113f20d-608e-4736-887a-461f99a42cc5', N'2011-03-20', 1, N'Die PANGalaktischen Donnerknödl', NULL, N'm', N'Heiner', N'Stepen', NULL, N'1718236963', N'heiner@pgdk.de', N'Schönen Gruß aus Pang/Rosenheim. Wir knödeln gerne wieder mit!', 0, '20101208 09:23:14', NULL, '20101208 09:23:14', '20120218 23:17:28'),
  (164, N'c399c141-5519-459f-a9f4-3031caca0986', N'2011-03-20', 3, N'InTeam sind wir besser', NULL, N'm', N'Markus', N'Schwab', NULL, N'1791481872', N'markus-schwab@gmx.de', NULL, 0, '20110111 09:00:19', NULL, '20110111 09:00:19', '20120218 23:17:28'),
  (165, N'b1595b41-79ba-4a94-8561-bb6e511c1c90', N'2011-03-20', 13, N'Unglaubliche', NULL, N'f', N'Uta', N'Wolf', NULL, N'(0821) 4861896', N'harro-wolf@t-online.de', NULL, 0, '20110116 11:37:47', NULL, '20110116 11:37:47', '20120218 23:17:28'),
  (166, N'13aa0237-9cd8-4550-a87a-03e3a3eb5d52', N'2011-03-20', 16, N'Super-TBV', NULL, N'm', N'Thiemo', N'Frosch', NULL, N'082 32 / 99 68 32', N'thiemo-frosch@t-online.de', NULL, 0, '20110117 10:00:53', NULL, '20110117 10:00:53', '20120218 23:17:28'),
  (167, N'b7ac918c-c2ae-4218-a036-6029c503879a', N'2011-03-20', 14, N'HopfaZupfa', N'', N'm', N'Thomas', N'Wetzel', N'Thomas', N'08442-5492', N'wetzel-t@t-online.de', N'danke für die Einladung, sehen uns dann zu Eurem Turnier Tom', 0, '20110117 20:56:25', NULL, '20110117 20:56:25', '20110117 20:56:25'),
  (168, N'44075cd7-7811-4d6d-bc38-d34fe5ccd4fa', N'2011-03-20', 4, N'Superburschis', NULL, N'f', N'Erika', N'Geffner', NULL, N'08272/3404', N'egeffner@bndlg.de', NULL, 0, '20110118 19:21:28', NULL, '20110118 19:21:28', '20120218 23:17:47'),
  (169, N'8033e2c3-a7ab-4c15-8a9e-c59313363163', N'2011-03-20', 11, N'Augustiners 12', NULL, N'm', N'Gerhard', N'Eberl', NULL, N'825283475', N'eberl@haw-ingolstadt.de', NULL, 0, '20110122 11:20:09', NULL, '20110122 11:20:09', '20120218 23:17:47'),
  (170, N'52c3bc68-8a6e-4351-a251-6110e55c18f5', N'2011-03-20', 12, N'Nimm´n Du', NULL, N'm', N'Richard', N'Strobl', NULL, N'08257 928651', N'richardstrobl@web.de', N'TSV Friedberg', 0, '20110123 09:16:22', NULL, '20110123 09:16:22', '20120218 23:17:47'),
  (171, N'e602173f-a7d4-47db-a5a5-1293ca5157b2', N'2011-03-20', 8, N'Querschläger', NULL, N'm', N'Thomas', N'Nieborowsky', NULL, N'0821-782804', N'Nieborowsky@gmx.de', NULL, 0, '20110125 20:18:44', NULL, '20110125 20:18:44', '20120218 23:17:47'),
  (172, N'39770d93-21a2-4a9f-bf69-9255bd47586f', N'2011-03-20', 6, N'Baggerfahrer', NULL, N'm', N'Ernst', N'Häupler', NULL, N'1633272705', N'ernst.haeupler@gmx.de', NULL, 0, '20110127 08:06:42', NULL, '20110127 08:06:42', '20120218 23:17:47'),
  (173, N'c27e2c47-8387-42b9-b886-3bddbd9e6503', N'2011-03-20', 10, N'Freecheese & Nutella Company', NULL, N'm', N'Wolfgang', N'Huber', NULL, N'(08233) 84 94 74', N'gnagflow.huber@arcor.de', NULL, 0, '20110201 00:58:31', NULL, '20110201 00:58:31', '20120218 23:17:47'),
  (174, N'3881ec78-f922-49cc-b06c-a7359d940139', N'2011-03-20', 7, N'Talfestpritscher', NULL, N'm', N'Marcus', N'Grasmann', NULL, N'0821/467941', N'marcus.grasmann@vr-web.de', NULL, 0, '20110201 07:51:01', NULL, '20110201 07:51:01', '20120218 23:17:47'),
  (175, N'e2714c13-33e1-44bd-98dc-2a6c6de507bc', N'2011-03-20', 15, N'Rubber Baby Buggy Bumpers', NULL, N'm', N'Tim', N'Roser', NULL, N'(0179) 9745766', N'troser@gmx.de', NULL, 0, '20110208 22:06:49', NULL, '20110208 22:06:49', '20120218 23:17:47'),
  (176, N'0883ae0d-c3dc-4678-9bc3-a4a3cf0d4dce', N'2011-03-20', 9, N'Die Knieschoner', NULL, N'f', N'Monika', N'Diepold', NULL, N'0821/60999887', N'schnurpsl8482@aol.com', NULL, 0, '20110209 19:18:12', NULL, '20110209 19:18:12', '20120218 23:17:47'),
  (177, N'fa2311ad5dc5418badc78366c46b0f7a', N'2012-03-18', 16, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'Erster ;-)', 0, '20111204 23:44:52', NULL, '20111204 23:44:52', '20120218 23:17:47'),
  (178, N'c1ebd6aa4ce04164bf0563cd335fa794', N'2012-03-18', 4, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'(0821) 4861896', N'harro-wolf@t-online.de', NULL, 0, '20120115 19:42:17', NULL, '20120115 19:42:17', '20120218 23:17:47'),
  (179, N'e826247912e64fb8ac5346d3ea4d07f3', N'2012-03-18', 1, N'InTeam sind wir besser', N'TSV Nymphenburg', N'm', N'Markus', N'Schwab', N'Markus', N'(0179) 1481872', N'markus-schwab@gmx.de', NULL, 0, '20120118 14:18:34', NULL, '20120118 14:18:34', '20120218 23:17:47'),
  (180, N'2d54f643af5c4384b51ba2c7ccc69484', N'2012-03-18', 5, N'Ups, Tschuldigung', N'TBM München', N'f', N'Helga', N'Grünig', N'Helga', N'(089) 6908615', N'helga.gruenig@gmx.de', NULL, 0, '20120120 15:26:56', NULL, '20120120 15:26:56', '20120218 23:17:47'),
  (181, N'4e8dc4ea87054336b7df0e4dc25382df', N'2012-03-18', 9, N'Superburschis', N'freie Spielgemeinschaft', N'f', N'Erika', N'Geffner', N'Erika', N'(08272) 3404', N'egeffner@bndlg.de', NULL, 0, '20120126 22:46:16', NULL, '20120126 22:46:16', '20120218 23:17:47'),
  (182, N'cf384a4fec5f4880be652099d16c44e8', N'2012-03-18', 15, N'Lass''n mir', N'TSV Friedberg', N'f', N'Christine', N'Bruger', N'Christine', N'(0821) 668771', N'christine.bruger@web.de', NULL, 0, '20120128 13:07:44', NULL, '20120128 13:07:44', '20120218 23:17:47'),
  (183, N'4f3f7a6dad2e448196bf4e1168e9d884', N'2012-03-18', 13, N'HopfaZupfa', N'TSV Wolnzach', N'm', N'Thomas', N'Wetzel', N'Thomas', NULL, N't-wetzel@web.de', NULL, 0, '20120129 19:57:00', NULL, '20120129 19:57:00', '20120218 23:17:47'),
  (184, N'4b036420d4354f538273d3907dda3724', N'2012-03-18', 8, N'Augustiners 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'(08252) 83475', N'eberl@haw-ingolstadt.de', NULL, 0, '20120131 15:10:35', NULL, '20120131 15:10:35', '20120218 23:17:47'),
  (185, N'8a0b33ac97ac4df6887f542f44be79e5', N'2012-03-18', 2, N'Baggerfahrer', N'VC Ottobrunn', N'm', N'Ernst', N'Häupler', N'Ernst', NULL, N'ernst.haeupler@gmx.de', NULL, 0, '20120202 07:01:14', NULL, '20120202 07:01:14', '20120218 23:17:47'),
  (186, N'a912d7465bf444968fc43ec5147c1e1d', N'2012-03-18', 10, N'Longliners', N'I.Q.2001 Friedberg', N'm', N'Christoph', N'Döring', N'Christoph Döring', N'(08207) 382', N'christoph-doering@gmx.de', NULL, 0, '20120202 21:36:22', NULL, '20120202 21:36:22', '20120218 23:17:47'),
  (187, N'960eeb34f40f4939be156484909cefcc', N'2012-03-18', 6, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'(0821) 9983338', N'rupert.schoettler@gmx.de', N'Freu! :-)', 0, '20120203 17:42:17', NULL, '20120203 17:42:17', '20120218 23:17:47'),
  (188, N'8b143457967c4b8b9fc091811bd46154', N'2012-03-18', 3, N'Winnie Puh', N'DJK Pfersee', N'f', N'Ilka', N'Quotschalla', N'Ilka', N'0821551026', N'ilka.quotschalla@googlemail.com', N'Ich gehe mal davon aus, dass wir erst am Turniertag zahlen brauchen.', 0, '20120215 17:12:24', NULL, '20120215 17:12:24', '20120215 17:12:24'),
  (189, N'e9c59220d08a47da8d1707764b563db1', N'2012-03-18', 7, N'Hobby Haunstetten', N'Hobby Haunstetten', N'm', N'Sebastian', N'Roeber', N'Sebastian', N'', N'sebastian_roeber@gmx.de', N'', 0, '20120222 10:28:29', NULL, '20120222 10:28:29', '20120222 10:28:29'),
  (190, N'dee7357044a9472f82e4b0ff4663b5d6', N'2012-03-18', 14, N'Talfest-Chaoten', N'SSV Anhausen', N'm', N'Bernd', N'Höfer', N'Bernd', N'', N'bernhard.hoefer@gmx.de', N'', 0, '20120222 20:08:16', NULL, '20120222 20:08:16', '20120222 20:08:16'),
  (191, N'ff38eeb45e4d40e3952f632aecd8e8ad', N'2012-03-18', 11, N'Unglaubliche', N'SpVgg Unglaublich Unersättlich', N'm', N'Norbert', N'Bietsch', N'Norbert', N'', N'nb@bietsch.net', N'', 0, '20120229 20:00:17', NULL, '20120229 20:00:17', '20120229 20:00:17'),
  (192, N'28c5878fb3524f2881c12ef0be196f4e', N'2012-03-18', 12, N'AllStars', N'VC Neusäß', N'm', N'Norbert', N'Bietsch', N'Norbert', N'', N'b@bietsch.net', N'', 0, '20120308 00:07:56', NULL, '20120308 00:07:56', '20120308 00:07:56'),
  (193, N'bad36a77d8a94af39f6a82df87744019', N'2013-03-10', 3, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'Erster :-)', 0, '20130102 19:28:33', NULL, '20130102 19:28:33', '20130102 19:28:33'),
  (194, N'6529db72b14e46f589cda235a3ad15b7', N'2013-03-10', 13, N'Augustiner 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'0825283475', N'eberl@haw-ingolstadt.de', N'', 0, '20130106 21:39:03', NULL, '20130106 21:39:03', '20130106 21:39:03'),
  (195, N'7799db72b14e46f589cda235a3ad15b7', N'2013-03-10', 4, N'Die flotten Karotten', N'', N'm', N'Thomas', N'Woller', N'Tom', N'(0911) 9699503', N'silvia.mundt@stadt.nuernberg.de', N'Info zu E-Mail-Adr. folgt, Überweisung 30 € folgt. Tel. gemeldet am 07.01.2013', 0, '20130107 19:06:16', NULL, '20130107 19:06:16', '20130107 19:06:16'),
  (196, N'e9ff1ce6004e47dc918beeb2793089cd', N'2013-03-10', 10, N'KampfKeksGeschwader', N'TSV Friedberg', N'm', N'Richard', N'Strobl', N'Richard', N'(08257) 928651', N'richardstrobl@web.de', N'', 0, '20130108 17:53:11', NULL, '20130108 17:53:11', '20130108 17:53:11'),
  (197, N'04236c8f250e48668041ef2eaa352c91', N'2013-03-10', 16, N'Lassnmir', N'TSV Friedberg', N'f', N'Christine', N'Bruger', N'Christine', N'(0821) 668771', N'christine.bruger@web.de', N'', 0, '20130112 18:33:15', NULL, '20130112 18:33:15', '20130112 18:33:15'),
  (198, N'f06bedee130c42ffb058b9325d8fc7aa', N'2013-03-10', 6, N'Rubber Baby Buggy Bumpers', N'', N'm', N'Uli', N'Eibl', N'Uli', N'0906245797', N'eibl@eiblverlag.de', N'', 0, '20130113 16:22:52', NULL, '20130113 16:22:52', '20130113 16:22:52'),
  (199, N'98c8f6b949d744b9b313d5c6d6e83764', N'2013-03-10', 1, N'In your Face', N'TC Hochzoll', N'm', N'Alexander', N'Sprang', N'Alexander', N'(0179) 4532473', N'alexander.sprang@web.de', N'', 0, '20130114 15:12:50', NULL, '20130114 15:12:50', '20130114 15:12:50'),
  (200, N'4316d7c352c34ec28f78bea1d0287506', N'2013-03-10', 9, N'Hobby Haunstetten', N'Hobby Haunstetten', N'm', N'Sebastian', N'Röber', N'Sebastian', N'', N'sebastian_roeber@gmx.de', N'Wir freuen uns schon! :-)', 0, '20130114 20:36:59', NULL, '20130114 20:36:59', '20130114 20:36:59'),
  (201, N'4466288b8cf64dd1a9867d4de5fe5345', N'2013-03-10', 8, N'HopfaZupfa', N'TSV Wolnzach', N'm', N'Thomas', N'Wetzel', N'Thomas', N'(08442) 5492', N't-wetzel@web.de', N'Hallo,
habe schon vor der Anmeldung überwiesen, danke nochmal für den Hinweis von Norbert, dachte mit der Überweisung ist man automatisch angemeldet...
TOM', 0, '20130120 18:31:50', NULL, '20130120 18:31:50', '20130120 18:31:50'),
  (202, N'd124658e9e554a6da17db313ef979e0c', N'2013-03-10', 14, N'Longliners', N'IQ Friedberg', N'm', N'Christoph', N'Döring', N'Christoph', N'(08207) 382', N'christoph-doering@gmx.de', N'', 0, '20130122 19:36:41', NULL, '20130122 19:36:41', '20130122 19:36:41'),
  (203, N'621b25d3012643dba1c5a7324d61a3e6', N'2013-03-10', 15, N'Flying Frogs', N'Kissinger SC', N'm', N'Wolfgang', N'Huber', N'Wolfgang', N'(08233) 849474', N'gnagflow.huber@arcor.de', N'', 0, '20130123 09:48:56', NULL, '20130123 09:48:56', '20130123 09:48:56'),
  (204, N'5993bfd777cc4c9493f380c61d5a524a', N'2013-03-10', 2, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20130127 17:17:37', NULL, '20130127 17:17:37', '20130127 17:17:37'),
  (205, N'8564d900f52b487f844b02bb3d9a68a8', N'2013-03-10', 12, N'Die Unglaublichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20130127 17:33:20', NULL, '20130127 17:33:20', '20130127 17:33:20'),
  (206, N'4b0ff284273d4cd5b40366dae298cc8d', N'2013-03-10', 5, N'Superschurken', N'TSV Schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20130128 23:01:58', NULL, '20130128 23:01:58', '20130128 23:01:58'),
  (207, N'36a75332a62d435c86614ba511527211', N'2013-03-10', 7, N'Die Gögginger', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'', N'rupert.schoettler@gmx.de', N'', 0, '20130130 18:15:42', NULL, '20130130 18:15:42', '20130130 18:15:42'),
  (208, N'4b1775aeff554ba7ba337a5970a784f0', N'2013-03-10', 11, N'Planet', N'TSV Bobingen', N'f', N'Gertraud', N'Reichert', N'Gertraud', N'(08234) 2854', N'fam.reichert@online.de', N'', 0, '20130130 21:23:28', NULL, '20130130 21:23:28', '20130130 21:23:28'),
  (209, N'8d24cf7c3d184a53aa2a27923a116b01', N'2013-03-10', NULL, N'Die Knieschoner', N'TSG Hochzoll', N'f', N'Monika', N'Diepold', N'Monika', N'(0821) 60999887', N'schnurpsl8482@aol.com', N'', 1, '20130206 13:21:51', NULL, '20130206 13:21:51', '20130206 13:21:51'),
  (210, N'938bc797b2fa4c32bb83bf37b7d8599e', N'2013-03-10', NULL, N'Remix Gersthofen', N'TSV Gersthofen', N'f', N'Helga', N'Fell', N'Helga', N'(0821) 71058494', N'helga.fell@gmx.de', N'Bitte nehmt uns auf die Nachrückliste. Wie viele stehen schon vor uns?', 1, '20130206 20:45:26', NULL, '20130206 20:45:26', '20130206 20:45:26'),
  (211, N'17a628307cfb415e80211e0d07cd9e17', N'2013-03-10', NULL, N'Los Locotos', N'TV Mering', N'f', N'Petra', N'Schlehhuber', N'Petra', N'(08233) 4498', N'petra@schlehhuber.de', N'', 1, '20130222 22:32:02', NULL, '20130222 22:32:02', '20130222 22:32:02'),
  (212, N'b7af2a9caf2f46cc820f12796f255f4c', N'2014-03-23', 3, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'Erster!!', 0, '20131227 23:23:16', NULL, '20131227 23:23:16', '20131227 23:23:16'),
  (213, N'45b97f5d893141488b0782e7f86017e7', N'2014-03-23', 9, N'Superschurken', N'TSV Schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20140102 13:23:17', NULL, '20140102 13:23:17', '20140102 13:23:17'),
  (214, N'750612f9bb8b4aaa85d8603e9ff70539', N'2014-03-23', 7, N'Rubber Baby Buggy Bumpers', N'', N'm', N'Uli', N'Eibl', N'Uli', N'0906245797', N'eibl@eiblverlag.de', N'', 0, '20140104 17:55:50', NULL, '20140104 17:55:50', '20140104 17:55:50'),
  (215, N'be1ed1cfbadc4f988d14423894fcf6bf', N'2014-03-23', 11, N'Augustiner 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'', N'gerhard.eberl@thi.de', N'', 0, '20140106 21:32:03', NULL, '20140106 21:32:03', '20140106 21:32:03'),
  (216, N'e909034bfdba41e49a37b8bc62419a49', N'2014-03-23', 12, N'KampfKeksGeschwader', N'TSV Friedberg', N'm', N'Richard', N'Strobl', N'Richard', N'(08257) 928651', N'richardstrobl@web.de', N'', 0, '20140108 07:52:29', NULL, '20140108 07:52:29', '20140108 07:52:29'),
  (217, N'6662f010227e453390d0ed1c56d237c0', N'2014-03-23', 13, N'Los Locotos', N'TV Mering', N'f', N'Petra', N'Schlehhuber', N'Petra', N'(08233) 4498', N'petraschlehhuber@gmail.com', N'', 0, '20140109 14:31:42', NULL, '20140109 14:31:42', '20140109 14:31:42'),
  (218, N'2c95f4476f7647e4830babd4e1a29488', N'2014-03-23', 1, N'In your Face', N'TSV Haunstetten', N'm', N'Alexander', N'Sprang', N'Alexander', N'', N'alexander.sprang@web.de', N'', 0, '20140113 13:40:26', NULL, '20140113 13:40:26', '20140113 13:40:26'),
  (219, N'a489f28c8e9347de82f9e30c091597ce', N'2014-03-23', 2, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'(0821) 151011', N'harro-wolf@t-online.de', N'', 0, '20140115 11:18:28', NULL, '20140115 11:18:28', '20140115 11:18:28'),
  (220, N'd7f72c648ce44365b58f36dd38654b8a', N'2014-03-23', 8, N'Planet', N'TSV Bobingen', N'm', N'Franz', N'Donig', N'Franz', N'(08203) 959698', N'franzdonig@aol.com', N'', 0, '20140116 21:53:15', NULL, '20140116 21:53:15', '20140116 21:53:15'),
  (221, N'9065945bc1ee44e59954c7c6b4256c02', N'2014-03-23', 4, N'HopfaZupfa', N'TSV Wolnzach', N'm', N'Thomas', N'Wetzel', N'Thomas', N'', N't-wetzel@web.de', N'', 0, '20140117 23:53:37', NULL, '20140117 23:53:37', '20140117 23:53:37'),
  (222, N'0ada4e3e2cf94026bb1a3dc1bf34800d', N'2014-03-23', NULL, N'Lassnmir', N'TSV Friedberg', N'm', N'Rolf', N'Ocklenburg', N'Rolf', N'(0821) 667420', N'rolf.ocklenburg@t-online.de', N'Per E-Mail abgesagt am 15.03.2014', 0, '20140119 12:26:48', '20140315', '20140119 12:26:48', '20140119 12:26:48'),
  (223, N'f65f7c3d7c5d4e7bbc06964ff6a5af01', N'2014-03-23', 5, N'Unersättliche', N'VC Neusäß', N'm', N'Gregor', N'Pahl', N'Gregor', N'(0821) 5439247', N'grpahl@gmx.de', N'', 0, '20140119 12:26:48', NULL, '20140129 21:29:46', '20140129 21:29:46'),
  (224, N'f9ab024c647949c4ad829029295dc71c', N'2014-03-23', 10, N'Sixxxpack', N'TSV Göggingen', N'm', N'Alf', N'Hunger', N'Alf', N'(0821) 709514', N'alf.hunger@t-online.de', N'-----Original Message-----
From: Alf Hunger <alfhunger@gmx.de>
Sent: Fri Jan 31 2014 11:47:20 GMT+0100
To: Volleyballclub Neusäß <nb@volleyballclub.de>
Subject: Re: Einladung zum Mixed-Turnier am 23. März 2014
> Hallo Norbert,
> das Team Sixxxpack meldet sich zu eurem Turnier am 23.3. an.
> Die Überweisung regle ich so schnell wie möglich.
> LG Alf Hunger', 0, '20140131 17:08:46', NULL, '20140131 17:08:46', '20140131 17:08:46'),
  (225, N'b19a6a63dcce418ba49855b533905da4', N'2014-03-23', 14, N'Hobby Haunstetten', N'TSV Haunstetten', N'f', N'Katja', N'Haag', N'Katja', N'01778643889', N'haagkatja@yahoo.de', N'', 0, '20140205 19:03:01', NULL, '20140205 19:03:01', '20140205 19:03:01'),
  (226, N'bfa7dea46a07433da9d9e406f7bf95c8', N'2014-03-23', NULL, N'E(y)-Plus-kein-Netz', N'', N'm', N'Ulrich', N'Sauter', N'Ulrich', N'', N'sauter.ulrich@web.de', N'Per E-Mail abgesagt am 17.03.2014', 0, '20140311 11:02:41', '20140317', '20140311 11:02:41', '20140311 11:02:41'),
  (227, N'bfa7dea46a07433da9d9e406f7bf95c6', N'2014-03-23', 6, N'Unglaubliche', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'', N'harro-wolf@t-online.de', NULL, 0, '20140322', NULL, '20140322 12:00:00', '20140322 12:00:00'),
  (228, N'c2bb90049577427fa38a57d215b58495', N'2015-03-22', 12, N'Superschurken', N'TSV Schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20150102 11:46:07', NULL, '20150102 11:46:07', '20150102 11:46:07'),
  (229, N'43735028006a439e801c40c2603879a4', N'2015-03-22', 3, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'', N'hb@bietsch.net', N'', 0, '20150102 12:59:17', NULL, '20150102 12:59:17', '20150102 12:59:17'),
  (230, N'1d0393ed7cc14bbd8d2469bddb34d59f', N'2015-03-22', 1, N'In your Face', N'DJK Hochzoll', N'm', N'Alexander', N'Sprang', N'Alexander', N'', N'alexander.sprang@web.de', N'', 0, '20150104 14:54:24', NULL, '20150104 14:54:24', '20150104 14:54:24'),
  (231, N'4eb9e7c1938e450a907a456836921c14', N'2015-03-22', 16, N'Spvgg Scheuring/Landsberg', N'FC Scheuring BSG Landsberg', N'm', N'Manuel', N'Jahn', N'Manuel', N'(08232) 905902', N'bankerimpp@web.de', N'Erstteilnehmer, wir bilden uns aus einer Spielvereinigung der Damen aus Scheuring und der Betriebssportgemeinschaft der JVA Landsberg', 0, '20150105 06:39:32', NULL, '20150105 06:39:32', '20150105 06:39:32'),
  (232, N'a103441e2f014e14a7ab128b9d6af7c0', N'2015-03-22', 7, N'Murphy''s Law', N'TSV Haunstetten Hobby', N'f', N'Elisabeth', N'Morgenstern', N'Elisabeth', N'', N'morgenstern.eli@gmail.com', N'', 0, '20150106 17:25:04', NULL, '20150106 17:25:04', '20150106 17:25:04'),
  (233, N'e14fc3ae18f34a9495fa91af6555a753', N'2015-03-22', 13, N'Rubber Baby Buggy Bumpers', N'VSC Donauwörth', N'm', N'Uli', N'Eibl', N'Uli', N'0906245797', N'eibl@eiblverlag.de', N'', 0, '20150109 09:27:26', NULL, '20150109 09:27:26', '20150109 09:27:26'),
  (234, N'19d95b94648e433996654b0af15d4655', N'2015-03-22', 5, N'Kampfkeksgeschwader', N'IQ2001', N'm', N'Peter', N'Schamberger', N'Peter', N'', N'mayr.schamberger@web.de', N'', 0, '20150111 20:51:05', NULL, '20150111 20:51:05', '20150111 20:51:05'),
  (235, N'f78bc88755ed4d7cb2915399ec1cbead', N'2015-03-22', 11, N'Sixxxpack', N'TSV Göggingen', N'm', N'Alf', N'Hunger', N'Alf', N'', N'alf.hunger@t-online.de', N'', 0, '20150114 16:02:10', NULL, '20150114 16:02:10', '20150114 16:02:10'),
  (236, N'2ee7b541160743039e638940a5f8398e', N'2015-03-22', 4, N'Augustiner 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'', N'gerhard.eberl@thi.de', N'', 0, '20150117 14:04:56', NULL, '20150117 14:04:56', '20150117 14:04:56'),
  (237, N'2d4e1621cad6464392239e62ccbd6b5f', N'2015-03-22', 14, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20150117 14:18:37', NULL, '20150117 14:18:37', '20150117 14:18:37'),
  (238, N'987ae21f5b1b4775a5296abee9a72c3a', N'2015-03-22', 9, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20150117 14:20:45', NULL, '20150117 14:20:45', '20150117 14:20:45'),
  (239, N'2a7290413a724143bb9295d68b0f8ffd', N'2015-03-22', 15, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'(0821) 9983338', N'rupert.schoettler@gmx.de', N'Nach Jahren endlich mal wieder dabei!  :-)', 0, '20150120 21:03:34', NULL, '20150120 21:03:34', '20150120 21:03:34'),
  (240, N'cedf9566f136409eaf88979f4bfc9d02', N'2015-03-22', 6, N'Kannnix', N'TV Kempten', N'm', N'Manuel', N'Roll', N'Manuel', N'01795152501', N'manuel.roll@gmx.de', N'Den Teamnamen passen wir evtl. noch an.', 0, '20150127 22:18:34', NULL, '20150127 22:18:34', '20150127 22:18:34'),
  (241, N'46d09784d3164b3aa9b142609d52c11e', N'2015-03-22', 10, N'Talentbefreit', N'TV Kempten', N'm', N'Manuel', N'Roll', N'Manuel', N'01795152501', N'manuel.roll@gmx.de', N'Den Teamnamen passen wir vielleicht noch an.', 0, '20150127 22:19:49', NULL, '20150127 22:19:49', '20150127 22:19:49'),
  (242, N'09ecca66e1524dba9f3b1782697e5060', N'2015-03-22', 2, N'HopfaZupfa', N'TSV Wolnzach', N'm', N'Thomas', N'Wetzel', N'Thomas', N'', N't-wetzel@web.de', N'Hallo Norbert,
konnte uns leider nicht früher anmelden, wie sind die Chancen noch teilzunehmen?

Tom', 0, '20150127 22:44:38', NULL, '20150127 22:44:38', '20150127 22:44:38'),
  (243, N'5e1ec0b66d114419bbee607ba0f6d31c', N'2015-03-22', 8, N'Sechs mit drei Frauen', N'', N'm', N'Stefan', N'Ampenberger', N'Stefan', N'(0821) 442266', N'stefan_ampi@gmx.de', N'', 0, '20150128 07:21:04', NULL, '20150128 07:21:04', '20150128 07:21:04'),
  (244, N'388460d25f8a4099917ca35a3fe551fd', N'2015-03-22', 10, N'Suht!', N'SV Fortuna', N'm', N'Jonas', N'Pillhatsch', N'Jonas', N'', N'jojo_p@gmx.net', N'ist noch ein plätzchen frei?', 1, '20150217 08:57:03', NULL, '20150217 08:57:03', '20150217 08:57:03'),
  (245, N'6ec5b6bf530a4f1dbc6bc0e8f92ab413', N'2016-03-13', 4, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'Wird auch so langsam Zeit !!!', 0, '20160103 21:53:53', NULL, '20160103 21:53:53', '20160103 21:53:53'),
  (246, N'b2647d9578094619af5218352802a8cf', N'2016-03-13', 16, N'Freibier auf Feld 2', N'TSV Milbertshofen', N'f', N'Annett', N'Klaschwitz', N'Annett', N'', N'a.klaschwitz@gmx.de', N'Eigentlicher Mannschaftsname ist Hahn im Korb', 0, '20160103 23:58:48', NULL, '20160103 23:58:48', '20160103 23:58:48'),
  (247, N'bd000689b635414ea1ef8aac37a5e339', N'2016-03-13', 1, N'HAARsträubend schlecht', N'TSV Haar', N'f', N'Katja', N'Mildner', N'Katja', N'', N'katja.mildner@gmx.net', N'', 0, '20160105 11:21:04', NULL, '20160105 11:21:04', '20160105 11:21:04'),
  (248, N'018a8eeafad04461a65e112c0c7e2150', N'2016-03-13', 2, N'In your face', N'', N'f', N'Sabine', N'Kraemer', N'Sabine', N'', N'sabine.kraemer@net-zone.de', N'', 0, '20160106 10:37:45', NULL, '20160106 10:37:45', '20160106 10:37:45'),
  (249, N'f8094e2f58954f63843e5ece87014329', N'2016-03-13', 11, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Harro', N'Wolf', N'Harro', N'(0821) 151011', N'harro-wolf@t-online.de', N'', 0, '20160113 08:43:04', NULL, '20160113 08:43:04', '20160113 08:43:04'),
  (250, N'1ac275f04f184ad3996fc71b812cade2', N'2016-03-13', 7, N'Sixxxpack', N'TSV Göggingen', N'm', N'Alf', N'Hunger', N'Alf', N'', N'alf.hunger@t-online.de', N'', 0, '20160114 11:34:42', NULL, '20160114 11:34:42', '20160114 11:34:42'),
  (251, N'647f204584ad4c17aed10d25e8118f6a', N'2016-03-13', 13, N'Los Locotos', N'TV Mering', N'f', N'Christine', N'Lampl', N'Christine', N'(0826) 6245', N'c.lampl@web.de', N'', 0, '20160116 20:24:32', NULL, '20160116 20:24:32', '20160116 20:24:32'),
  (252, N'4a7780bc1aa647d5a315292ccb8e59be', N'2016-03-13', 3, N'Sechs mit drei Frauen', N'TSV Deuringen', N'm', N'Stefan', N'Ampenberger', N'Stefan', N'', N'stefan_ampi@gmx.de', N'', 0, '20160117 14:25:56', NULL, '20160117 14:25:56', '20160117 14:25:56'),
  (253, N'773763b042904d52bee6b0edbe9e7711', N'2016-03-13', 8, N'Kampfkeksgeschwader', N'TSV Friedberg', N'm', N'Peter', N'Schamberger', N'Peter', N'', N'mayr.schamberger@web.de', N'', 0, '20160117 21:18:17', NULL, '20160117 21:18:17', '20160117 21:18:17'),
  (254, N'fcd02fd84abb4b7781fa4b095d95b56a', N'2016-03-13', 10, N'Suht!', N'SV Fortuna Regensburg', N'm', N'Jonas', N'Pillhatsch', N'Jonas', N'', N'jojo_p@gmx.net', N'', 0, '20160118 12:25:21', NULL, '20160118 12:25:21', '20160118 12:25:21'),
  (255, N'a38cc55c491a496d9127152671c09daf', N'2016-03-13', 5, N'Superschurken', N'TSV Schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'Servus, würde wie die letzten Jahre gern wieder a Mannschaft melden. Lg Stefan', 0, '20160118 18:03:07', NULL, '20160118 18:03:07', '20160118 18:03:07'),
  (256, N'd60013ca15714f5792e1807338d32979', N'2016-03-13', 9, N'Augustiner 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'(08252) 83475', N'gerhard.eberl@thi.de', N'', 0, '20160119 08:52:40', NULL, '20160119 08:52:40', '20160119 08:52:40'),
  (257, N'480fec58d77640d885468a6fdc44e0e3', N'2016-03-13', 14, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20160120 14:31:01', NULL, '20160120 14:31:01', '20160120 14:31:01'),
  (258, N'5fa969bc135f436d9c419f08c4e5efbd', N'2016-03-13', 12, N'VSC on Tour', N'VSC Donauwörth', N'm', N'Uli', N'Eibl', N'Uli', N'090670011373', N'eibl@eiblverlag.de', N'', 0, '20160120 14:42:43', NULL, '20160120 14:42:43', '20160120 14:42:43'),
  (259, N'dd0745e4c1e14ff88bb7cef31a885ac3', N'2016-03-13', 15, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'(0821) 9983338', N'rupert.schoettler@gmx.de', N':-)', 0, '20160122 11:01:06', NULL, '20160122 11:01:06', '20160122 11:01:06'),
  (260, N'955ae45fe0b341d4bf7ed0a460526906', N'2016-03-13', 6, N'6 mit 4 Frauen', N'SC Eching', N'm', N'Markus', N'Welm', N'Markus', N'', N'markus.welm@gmx.de', N'', 0, '20160124 16:12:04', NULL, '20160124 16:12:04', '20160124 16:12:04'),
  (261, N'43d11060635c4a71b16dc5689fba1bf9', N'2016-03-13', NULL, N'Murphy''s Law', N'HSV Haunstetten', N'f', N'Elisabeth', N'Morgenstern', N'Elisabeth', N'', N'morgenstern.eli@gmail.com', N'', 1, '20160126 18:03:57', NULL, '20160126 18:03:57', '20160126 18:03:57'),
  (262, N'b1922e5d734d40c2ad30686879a4d1a3', N'2016-03-13', NULL, N'die Sprungwunder', N'TSG Lechbruck Waltershofen', N'm', N'Marcus', N'Schröder', N'Marcus', N'', N'marcus-schroeder@freenet.de', N'', 1, '20160214 21:22:40', NULL, '20160214 21:22:40', '20160214 21:22:40'),
  (263, N'e2a1291655da4386920f62832a3b3b33', N'2017-03-19', NULL, N'Auusgezeichnet TSV 1871', N'TSV 1871 Augsburg e.V.', N'f', N'Karina', N'Ludwig', N'Karina', N'', N'karina-ludwig@gmx.de', N'', 0, '20170111 18:44:03', NULL, '20170111 18:44:03', '20170111 18:44:03'),
  (264, N'0c2c920e8f964aa7834d8dae6cafea6f', N'2017-03-19', NULL, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'', 0, '20170115 00:23:36', NULL, '20170115 00:23:36', '20170115 00:23:36'),
  (265, N'748e64c0f38f400b821e5c070fdec761', N'2017-03-19', NULL, N'Schwanensee', N'TSV Haunstetten', N'f', N'Elisabeth', N'Morgenstern', N'Elisabeth', N'', N'morgenstern.eli@gmail.com', N'', 0, '20170115 12:54:24', NULL, '20170115 12:54:24', '20170115 12:54:24'),
  (266, N'd1f8bc5f11104a3c83590c258352c6aa', N'2017-03-19', NULL, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20170115 17:19:40', NULL, '20170115 17:19:40', '20170115 17:19:40'),
  (267, N'6c6718d3ec5e4141a0c0b4d18acfbab0', N'2017-03-19', NULL, N'Sixxxpack', N'TSV Göggingen', N'm', N'Alf', N'Hunger', N'Alf', N'', N'alf.hunger@t-online.de', N'-----Original Message-----
From: Alf Hunger [mailto:alf.hunger@t-online.de] 
Sent: Tuesday, January 10, 2017 11:56 AM
To: info@volleyball-mixedliga.de
Subject: Hobbyturnier

Nachricht an volleyball-mixedliga.de
über das Kontaktformular

Vor-/Nachname:
   Herr Alf Hunger
E-Mail:
   alf.hunger@t-online.de
----------------------------------------
Hallo Norbert, das Team Sixxxpack, TSV Göggingen, würde an eurem Turnier gerne Teilnehmen. LG, Alf Hunger
----------------------------------------', 0, '20170116 19:00:18', NULL, '20170116 19:00:18', '20170116 19:00:18'),
  (268, N'e091f0ae2f24467988e31fc895316291', N'2017-03-19', NULL, N'Die Unbelehrbaren', N'VC Neusäß', N'm', N'Andreas', N'Frank', N'Andreas', N'', N'frank.andreas87@googlemail.com', N'', 0, '20170117 08:43:49', NULL, '20170117 08:43:49', '20170117 08:43:49'),
  (269, N'c0913ddc9dfa44608e8da01b6f52a35e', N'2017-03-19', NULL, N'Mach was schlaues', N'SG Stern Sindelfingen', N'm', N'Florian', N'Hintze', N'Florian', N'', N'hintze@spinmo.de', N'Bitte nicht, wenn möglich, für das erste Spiel einplanen, da einige einen längeren Anfahrtsweg aus Sindelfingen/ Stuttgart haben. Auch wenn man am Sonntag Morgen nicht unbedingt mit einem Stau auf der A8 rechnen muss. ;)', 0, '20170123 23:25:07', NULL, '20170123 23:25:07', '20170123 23:25:07'),
  (270, N'424806721dcd48e79586b8390b9e2c11', N'2017-03-19', NULL, N'Suht!', N'SV Fortuna Regensburg', N'm', N'Jonas', N'Pillhatsch', N'Jonas', N'', N'jojo_p@gmx.net', N'', 0, '20170203 11:52:53', NULL, '20170203 11:52:53', '20170203 11:52:53'),
  (271, N'299dde63d07a4cecbfab83b6578d1912', N'2017-03-19', NULL, N'Superschurken', N'TSV Schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20170204 19:27:07', NULL, '20170204 19:27:07', '20170204 19:27:07'),
  (272, N'e81d3c5b7e45401daaf58ceb61c7b9d8', N'2017-03-19', NULL, N'Augustiner 12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'(08252) 83475', N'gerhard.eberl@thi.de', N'', 0, '20170208 10:36:06', NULL, '20170208 10:36:06', '20170208 10:36:06'),
  (273, N'581c7e9ecd3d4e38b467d6574997f00a', N'2017-03-19', NULL, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'(0821) 9983338', N'rupert.schoettler@gmx.de', N'Überweisung auch für Sixxxpäck kommt gleich!', 0, '20170215 21:42:02', NULL, '20170215 21:42:02', '20170215 21:42:02'),
  (274, N'397620c89a904c6aa21cf5e88129adbd', N'2017-03-19', NULL, N'In your face', N'-', N'f', N'Sabine', N'Kraemer', N'Sabine', N'', N'sabine.kraemer@net-zone.de', N'', 0, '20170216 10:04:34', NULL, '20170216 10:04:34', '20170216 10:04:34'),
  (275, N'002015c874334baf962ccb90eec27931', N'2017-03-19', NULL, N'6 mit 3 Frauen', N'Eching', N'm', N'Markus', N'Welm', N'Markus', N'', N'markus.welm@gmx.de', N'', 0, '20170218 09:11:47', NULL, '20170218 09:11:47', '20170218 09:11:47'),
  (276, N'576b8bc8e0b4483f9562dac988e890bc', N'2017-03-19', NULL, N'dazwischenRufKühe', N'', N'm', N'Moritz', N'Schächterle', N'Moritz', N'', N'blub.indikator@gmail.com', N'', 0, '20170221 18:21:28', NULL, '20170221 18:21:28', '20170221 18:21:28'),
  (277, N'5268d00cdd6e479ab96c73f7de491b1f', N'2017-03-19', NULL, N'7 Kurze', N'', N'm', N'Moritz', N'Schächterle', N'Moritz', N'', N'blub.indikator@gmail.com', N'', 0, '20170221 18:24:41', NULL, '20170221 18:24:41', '20170221 18:24:41'),
  (278, N'703f7052d18a424a999eaf36d691eadb', N'2017-03-19', NULL, N'Die Unsterblichen', N'VC Neusäß', N'm', N'Jakob', N'Baldauf', N'Jakob', N'', N'derkoenigderwelt9395@gmail.com', N'', 0, '20170317 10:16:20', NULL, '20170317 10:16:20', '20170317 10:16:20'),
  (279, N'8c3e97cd4e744f0f9ac6bf730c97d0b6', N'2018-03-18', NULL, N'Mein Lieblingsteam', N'VC Neusäß', N'm', N'Andreas', N'Frank', N'Andreas', N'', N'frank.andreas87@googlemail.com', N'', 0, '20171027 15:48:08', NULL, '20171027 15:48:08', '20171027 15:48:08'),
  (280, N'c31f6dd47780487ea85f5ed828507307', N'2018-03-18', NULL, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'', 0, '20171028 20:26:07', NULL, '20171028 20:26:07', '20171028 20:26:07'),
  (281, N'f214a12a4dd548e8b947b6210ebbd360', N'2018-03-18', NULL, N'Nonames', N'gemischt', N'f', N'Stephanie', N'Lehmann', N'Stephanie', N'', N'steffelchen@gmx.de', N'', 0, '20171029 23:23:56', NULL, '20171029 23:23:56', '20171029 23:23:56'),
  (282, N'306b2877795345478a68c9b0e658fdaf', N'2018-03-18', NULL, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20171102 19:42:47', NULL, '20171102 19:42:47', '20171102 19:42:47'),
  (284, N'380eb43d539145a8b78c1c9575834325', N'2018-03-18', NULL, N'Superschurken', N'Tsv schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20180105 14:32:17', NULL, '20180105 14:32:17', '20180105 14:32:17'),
  (285, N'e9783212773940df944a371ba636fafc', N'2018-03-18', NULL, N'QuEST München', N'', N'm', N'Ruslan', N'Ponezhda', N'Ruslan', N'017620473462', N'pultist@gmail.com', N'Bitte um kurze bestätigung ob noch Plätze vorhanden sind.', 0, '20180119 19:17:10', NULL, '20180119 19:17:10', '20180119 19:17:10'),
  (286, N'8abebcc57eaa47d7a7c590b216d7e304', N'2018-03-18', NULL, N'StuSta', N'SV Studentenstadt Freimann', N'm', N'Tsvetan', N'Budinov', N'Tsvetan', N'', N'tsvetan.budinov@gmail.com', N'', 0, '20180123 08:58:26', NULL, '20180123 08:58:26', '20180123 08:58:26'),
  (287, N'0827baef44d74367b6156c5932f13661', N'2018-03-18', NULL, N'Augustiner12', N'SSV Schrobenhausen', N'm', N'Gerhard', N'Eberl', N'Gerhard', N'', N'eberlsg@gmail.com', N'', 0, '20180201 16:33:23', NULL, '20180201 16:33:23', '20180201 16:33:23'),
  (289, N'044610669d4a4c13840b493577c8e4dc', N'2018-03-18', NULL, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'', N'rupert.schoettler@gmx.de', N'', 0, '20180204 16:51:57', NULL, '20180204 16:51:57', '20180204 16:51:57'),
  (290, N'0f5bb7a6be964d1cb4128a026add6407', N'2018-03-18', NULL, N'Hahn im Korb', N'TSV Milbertshofen', N'f', N'Annett', N'Klaschwitz', N'Annett', N'', N'a.klaschwitz@gmx.de', N'', 0, '20180205 09:39:59', NULL, '20180205 09:39:59', '20180205 09:39:59'),
  (291, N'8a93ea7c8c3e4310b5609aac8f02ae3f', N'2018-03-18', NULL, N'Balisto', N'', N'f', N'Mira', N'Rauh', N'Mira', N'(0951) 500544', N'mirarauh1997@gmail.con', N'', 0, '20180205 15:49:11', NULL, '20180205 15:49:11', '20180205 15:49:11'),
  (292, N'f793deac81ff4b4d870113c21be111d9', N'2018-03-18', NULL, N'In your face', N'', N'f', N'Sabine', N'Sprang', N'Sabine', N'', N'sabine.sprang@net-zone.de', N'', 0, '20180223 20:41:49', NULL, '20180223 20:41:49', '20180223 20:41:49'),
  (293, N'ba62ec70e5dc4791b1391974090333cc', N'2018-03-18', NULL, N'Die Unsterblichen', N'VC Neusäß', N'm', N'Jakob', N'Baldauf', N'Jakob', N'08214863243', N'jakob@jbhost.de', N'', 0, '20180228 00:57:53', NULL, '20180228 00:57:53', '20180228 00:57:53'),
  (294, N'd9cf0a1b588544d1813f9cb7b792e12c', N'2018-03-18', NULL, N'Beamtenmikado', N'', N'm', N'Michael', N'Lucas', N'Michael', N'', N'lucas.michael@me.com', N'', 0, '20180303 18:24:18', NULL, '20180303 18:24:18', '20180303 18:24:18'),
  (295, N'7866f2472b1f4ffe9c9058f82e14b245', N'2019-03-24', NULL, N'Die Unverwüstlichen', N'VC Neusäß', N'm', N'Andreas', N'Frank', N'Andreas', N'', N'killa-opa@web.de', N'', 0, '20190107 20:42:29', NULL, '20190107 20:42:29', '20190107 20:42:29'),
  (296, N'10d10a7a07214e75963f8bbdb0a08f5c', N'2019-03-24', NULL, N'Die Unberechenbaren', N'VC Neusäß', N'f', N'Hilde', N'Bietsch', N'Hilde', N'(0821) 469996', N'hb@bietsch.net', N'', 0, '20190112 16:04:22', NULL, '20190112 16:04:22', '20190112 16:04:22'),
  (297, N'4883480bc20a4e919e979e44b26f2081', N'2019-03-24', NULL, N'Gummibierbande', N'Post SV Nürnberg', N'm', N'Thomas', N'Krieg', N'Thomas', N'', N'thomas-krieg@gmx.net', N'', 0, '20190115 19:16:25', NULL, '20190115 19:16:25', '20190115 19:16:25'),
  (298, N'd71b6f4e3f2e42a38d54ec8f5b78f397', N'2019-03-24', NULL, N'QuEST - Banda', N'StuSta München', N'm', N'Ruslan', N'Ponezhda', N'Ruslan', N'017620473462', N'ruslan.ponezhda@quest-global.de', N'', 0, '20190116 19:47:00', NULL, '20190116 19:47:00', '20190116 19:47:00'),
  (299, N'3430d0b0223b47c78915688913eeb738', N'2019-03-24', NULL, N'Superschurken', N'Tsv schongau', N'm', N'Stefan', N'Walter', N'Stefan', N'', N'walterstefan1@gmx.de', N'', 0, '20190120 18:32:24', NULL, '20190120 18:32:24', '20190120 18:32:24'),
  (300, N'c377470fdf6249c684f59979d01ef9bc', N'2019-03-24', NULL, N'Power Booster', N'', N'm', N'Arthur', N'Farkas', N'Arthur', N'', N'afa13@gmx.de', N'', 0, '20190122 16:42:04', NULL, '20190122 16:42:04', '20190122 16:42:04'),
  (301, N'b40ee00d4b0041aabd02a0ccd7dd61a4', N'2019-03-24', NULL, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'(0821) 4861896', N'harro-wolf@t-online.de', N'', 0, '20190124 19:45:55', NULL, '20190124 19:45:55', '20190124 19:45:55'),
  (302, N'905d34cc2f1640d3856b1a827273e2d9', N'2019-03-24', NULL, N'Zwiderwurzen', N'', N'f', N'Sabine', N'Sprang', N'Sabine', N'01604478880', N'sabine.sprang@net-zone.de', N'', 0, '20190127 14:08:10', NULL, '20190127 14:08:10', '20190127 14:08:10'),
  (303, N'5c117abd8a0a48ebbca018d82f4c1bc7', N'2019-03-24', NULL, N'Mein persönlicher Favorit', N'TSV Deuringen', N'f', N'Stephanie', N'Lehmann', N'Stephanie', N'', N'steffelchen@gmx.de', N'', 0, '20190129 22:58:32', NULL, '20190129 22:58:32', '20190129 22:58:32'),
  (305, N'1780cebbb8f44ea49ceefc4fc2097985', N'2019-03-24', NULL, N'Team Heftig', N'DJK Ingolstadt', N'm', N'Florian', N'Markert', N'Florian', N'', N'markertfloo@web.de', N'', 0, '20190206 22:48:53', NULL, '20190206 22:48:53', '20190206 22:48:53'),
  (306, N'ea3aab1549d34cd492896bbfdcd4d41e', N'2019-03-24', NULL, N'Schredders', N'TSV Turnerbund München eV', N'm', N'Markus', N'Schwab', N'Markus', N'', N'markus-schwab@gmx.de', N'', 0, '20190208 20:12:04', NULL, '20190208 20:12:04', '20190208 20:12:04'),
  (307, N'834b2fa133484628b3f98db0ef9bd16d', N'2019-03-24', NULL, N'Die 7 Zwerge', N'TVA', N'm', N'Marius', N'Craig', N'Marius', N'', N'marius.craig@gmx.de', N'', 0, '20190209 10:39:33', NULL, '20190209 10:39:33', '20190209 10:39:33'),
  (308, N'5ae7d9c5af00456b9d0a01905736eb19', N'2019-03-24', NULL, N'Genauuu sooo!', N'Holzbein Kiel', N'm', N'Markus', N'Heimbrock', N'Markus', N'017630366380', N'markus.heimbrock@gmx.net', N'Keine Vereinszugehörigkeit, daher das Pseudonym', 0, '20190227 17:42:36', NULL, '20190227 17:42:36', '20190227 17:42:36'),
  (309, N'f0831815d51f4befbe083947aff176c9', N'2020-03-15', NULL, N'QuEST Banda', N'ohne....', N'm', N'Ruslan', N'Ponezhda', N'Ruslan', N'', N'pultist@gmail.com', N'', 0, '20191108 09:42:33', NULL, '20191108 09:42:33', '20191108 09:42:33'),
  (310, N'd8786f8937fe48fa821516ea84b39545', N'2020-03-15', NULL, N'Die Unersättlichen', N'VC Neusäß', N'f', N'Uta', N'Wolf', N'Uta', N'', N'harro-wolf@t-online.de', N'', 0, '20191112 22:09:16', NULL, '20191112 22:09:16', '20191112 22:09:16'),
  (311, N'635bf8b18a7444a1b7bbcb8ea3eccd58', N'2020-03-15', NULL, N'Unverwüstlichen', N'VC Neusäß', N'f', N'Angelika', N'Walz', N'Angelika', N'', N'sa30197@yahoo.de', N'', 0, '20191117 15:45:21', NULL, '20191117 15:45:21', '20191117 15:45:21'),
  (312, N'd54d194857164b9bac09b5fe4f56a89c', N'2020-03-15', NULL, N'Die Unberechenbaren', N'VC Neusäß', N'm', N'Andreas', N'Frank', N'Andreas', N'', N'frank.andreas87@googlemail.com', N'', 0, '20191122 22:33:35', NULL, '20191122 22:33:35', '20191122 22:33:35'),
  (313, N'84d7fe261f024cd9aaf67f62caabccab', N'2020-03-15', NULL, N'Der klügere kippt nach...!!!', N'TV Dillingen', N'm', N'Sandro', N'Petzoldt', N'Sandro', N'', N'sandropetzoldt@gmx.de', N'', 0, '20191209 13:38:58', NULL, '20191209 13:38:58', '20191209 13:38:58'),
  (314, N'e6a9ccc105d343dbbef1bfc6a7cd6300', N'2020-03-15', NULL, N'Schönes Ding', N'Post SV Nürnberg', N'm', N'Thomas', N'Krieg', N'Thomas', N'', N'thomas-krieg@gmx.net', N'', 0, '20191210 18:10:01', NULL, '20191210 18:10:01', '20191210 18:10:01'),
  (315, N'18d038d42e6c43ffb4e84c7841ed09c1', N'2020-03-15', NULL, N'Black Volley', N'Pfennigfuchser', N'f', N'Diana', N'Rogoll', N'Diana', N'', N'diana.klaude@gmx.de', N'', 0, '20191220 00:16:09', NULL, '20191220 00:16:09', '20191220 00:16:09'),
  (316, N'9ff508953e064da5963364a092c351b7', N'2020-03-15', NULL, N'Sensationell', N'', N'm', N'Yorick', N'Gehrlicher', N'Yorick', N'', N'yorickgehrlicher@web.de', N'', 0, '20200103 18:57:00', NULL, '20200103 18:57:00', '20200103 18:57:00'),
  (318, N'2e52f4c5a209434a9eb32824ba2f9409', N'2020-03-15', NULL, N'In your Face', N'', N'f', N'Sabine', N'Sprang', N'Sabine', N'', N'sabine.sprang@net-zone.de', N'', 0, '20200113 08:35:29', NULL, '20200113 08:35:29', '20200113 08:35:29'),
  (320, N'd2fda897944a44a5b6b48a0048925eb8', N'2020-03-15', NULL, N'Die Knieschoner', N'TSG Augsburg-Hochzoll', N'f', N'Monika', N'Diepold', N'Monika', N'', N'schnurpsl8482@aol.com', N'', 0, '20200120 06:32:41', NULL, '20200120 06:32:41', '20200120 06:32:41'),
  (321, N'40f9b37bfb84471fa37f094181883a7c', N'2020-03-15', NULL, N'Die glorreichen 6', N'TSV Neufahrn', N'm', N'Michael', N'Riedel', N'Michael', N'', N'michi60@web.de', N'', 0, '20200121 08:22:23', NULL, '20200121 08:22:23', '20200121 08:22:23'),
  (322, N'95aa1b78a7294597a8a00c8d740ee5ed', N'2020-03-15', NULL, N'Decke Boden', N'', N'f', N'Aleksandra', N'Dokic', N'Aleksandra', N'4915202530289', N'dokic.aleksandra@gmail.com', N'Hallo, gibt es noch Platz bei diese Turnier? Ich will ein Mannschaft von München gern anmelden. Freue mich auf Rückmeldung. :)', 0, '20200121 10:55:52', NULL, '20200121 10:55:52', '20200121 10:55:52'),
  (324, N'88ff6640f08b4a22b5f186f957f2f865', N'2020-03-15', NULL, N'Pritsch Perfect', N'TV Erlangen', N'm', N'Kim', N'Sauer', N'Kim', N'', N'kimsauer@gmx.de', N'', 0, '20200125 00:44:53', NULL, '20200125 00:44:53', '20200125 00:44:53'),
  (325, N'4f37fffd992f48ad87fc98c97072b5cf', N'2020-03-15', NULL, N'Der alte Sauhaufen', N'TV Jahn 1895 Schweinfurt', N'm', N'Stefan', N'Ternus', N'Stefan', N'08217958230', N'stefan.ternus@web.de', N'Hallo Norbert, wäre klasse, wenn es noch klappt. Dann müssten die anderen nicht umsonst aus Schweinfurt und München anfahren.', 0, '20200127 14:07:57', NULL, '20200127 14:07:57', '20200127 14:07:57'),
  (326, N'161b4dd69e0d4572883a4dba00e9e479', N'2020-03-15', NULL, N'Kunterbunter Haufen', N'Verschiedene (SC Lehr, TSV Pfuhl, VfB Ulm, TSV Berghülen, SV Jedesheim)', N'f', N'Evelyn', N'Jooß', N'Evelyn', N'', N'evelynjooss@gmx.de', N'Startgeld in Höhe von 30 EUR ist bereits überwiesen und müsste im Laufe der Woche bei euch eingehen. Über kurze Bestätigungs-Mail/Anruf würde ich mich freuen.', 0, '20200202 16:00:20', NULL, '20200202 16:00:20', '20200202 16:00:20'),
  (327, N'a26739c3f44a46e895c5970e73c0d748', N'2020-03-15', NULL, N'Baby Lauch und Lukas der Lokomotivführer', N'Augsburg', N'm', N'Moritz', N'Schächterle', N'Moritz', N'', N'm.schaechterle@djk-augsburg-hochzoll.de', N'Bitte frühzeitig mitteilen, ob wir fest dabei sind oder nur Nachrücker sind.', 0, '20200204 23:18:19', NULL, '20200204 23:18:19', '20200204 23:18:19'),
  (328, N'c68e7faffe974a498e424082339544be', N'2020-03-15', NULL, N'He Duda', N'TSV Göggingen', N'm', N'Rupert', N'Schöttler', N'Rupert', N'08219983338', N'rupert.schoettler@gmx.de', N'', 0, '20200210 22:33:24', NULL, '20200210 22:33:24', '20200210 22:33:24'),
  (329, N'd95e05125d6448998b9b7d57e985d25a', N'2020-03-15', NULL, N'Weihnachtsmann & Co KG', N'DJK Ingolstadt', N'm', N'Thomas', N'Muhr', N'Thomas', N'', N'muhrthomasthi@gmx.de', N'Hi,
wenn wir noch mitmachen könnten oder nachrücken wäre dies hervorragend, Bis wann würde man dies denn erfahren?
Gruß
Thomas Muhr', 0, '20200211 20:47:12', NULL, '20200211 20:47:12', '20200211 20:47:12'),
  (330, N'3339e39e919d496d8c99c15a16002606', N'2020-03-15', NULL, N'Suht!', N'Sv Fortuna Regensburg', N'm', N'Jonas', N'Pillhatsch', N'Jonas', N'094138225442', N'jonas@mixedvolleyball.net', N'', 1, '20200217 08:34:18', NULL, '20200217 08:34:18', '20200217 08:34:18')
GO
    */
    public class TournamentRegistration
    {
        [Key] public long Id { get; set; }
        [MaxLength(255)] public string Guid { get; set; } = string.Empty;
        [Column(TypeName = "date")] public DateTime TournamentDate { get; set; }
        public int? Rank { get; set; }

        [MaxLength(255)]
        [Display(Name = "Teamname")]
        public string TeamName { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Vereinsname")]
        public string ClubName { get; set; } = string.Empty;

        [MaxLength(1)]
        [Display(Name = "Anrede")]
        public string Gender { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Vorname")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Familienname")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Spitzname")]
        public string Nickname { get; set; } = string.Empty;

        [MaxLength(40)]
        [Display(Name = "Telefon")]
        public string Fone { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "E-Mail")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(4000)]
        [Display(Name = "Nachricht/Hinweis")]
        public string Message { get; set; } = string.Empty;

        public bool IsStandByRegistration { get; set; }
        [Column(TypeName = "datetime")] public DateTime? RegisteredOn { get; set; }
        [Column(TypeName = "datetime")] public DateTime? RegCanceledOn { get; set; }
        [Column(TypeName = "datetime")] public DateTime? CreatedOn { get; set; }
        [Column(TypeName = "datetime")] public DateTime? ModifiedOn { get; set; }

        public string GetTeamNameWithClub()
        {
            return string.Join(' ', TeamName.Trim(),
                !string.IsNullOrWhiteSpace(ClubName) ? "(" + ClubName.Trim() + ")" : string.Empty);
        }

        public string GetCompleteName()
        {
            return !string.IsNullOrWhiteSpace(Nickname) && !Nickname.Trim().Equals(FirstName.Trim())
                ? string.Join(' ', FirstName.Trim(), "(" + Nickname.Trim() + ")", LastName.Trim())
                : string.Join(' ', FirstName.Trim(), LastName.Trim());
        }
    }
}