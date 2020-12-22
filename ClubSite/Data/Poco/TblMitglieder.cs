﻿//
// Copyright (C) axuno gGmbH and other contributors.
// Licensed under the MIT license.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubSite.Data.Poco
{
    /*
CREATE TABLE [dbo].[tblMitglieder](
	[AId] [int] NOT NULL,
	[Id] [int] NULL,
	[MitglNr] [nvarchar](15) NULL,
	[MitglGrp] [nvarchar](30) NULL,
	[Anrede] [nvarchar](20) NULL,
	[Titel] [nvarchar](40) NULL,
	[Nachname] [nvarchar](40) NULL,
	[Vorname] [nvarchar](40) NULL,
	[Name] [nvarchar](100) NULL,
	[Zusatz] [nvarchar](50) NULL,
	[Strasse] [nvarchar](50) NULL,
	[Land] [nvarchar](3) NULL,
	[Plz] [nvarchar](8) NULL,
	[Ort] [nvarchar](50) NULL,
	[Briefanrede] [nvarchar](80) NULL,
	[Telefon1] [nvarchar](25) NULL,
	[Telefon2] [nvarchar](25) NULL,
	[Mobil] [nvarchar](25) NULL,
	[Fax] [nvarchar](25) NULL,
	[EMail] [nvarchar](100) NULL,
	[Internet] [nvarchar](100) NULL,
	[ZAnrede] [nvarchar](20) NULL,
	[ZTitel] [nvarchar](40) NULL,
	[ZNachname] [nvarchar](40) NULL,
	[ZVorname] [nvarchar](40) NULL,
	[ZName] [nvarchar](100) NULL,
	[ZZusatz] [nvarchar](50) NULL,
	[ZStrasse] [nvarchar](50) NULL,
	[ZLand] [nvarchar](3) NULL,
	[ZPlz] [nvarchar](8) NULL,
	[ZOrt] [nvarchar](50) NULL,
	[ZAnschrift] [bit] NOT NULL,
	[Foto] [nvarchar](255) NULL,
	[Eintritt] [nvarchar](8) NULL,
	[Austritt] [nvarchar](8) NULL,
	[GebDatum] [nvarchar](8) NULL,
	[Geschlecht] [nvarchar](1) NULL,
	[AP] [nvarchar](1) NULL,
	[Betreuer] [nvarchar](50) NULL,
	[Funktion] [nvarchar](50) NULL,
	[SuchKrit] [nvarchar](50) NULL,
	[Familie] [int] NULL,
	[EMailNews] [nvarchar](255) NULL,
	[VereinNews] [bit] NOT NULL,
	[BuchDat] [nvarchar](8) NULL,
	[ZArt] [int] NULL,
	[ZW] [nvarchar](1) NULL,
	[FTag] [int] NULL,
	[VBank] [int] NULL,
	[Zahler] [int] NULL,
	[IBAN] [nvarchar](30) NULL,
	[BIC] [nvarchar](30) NULL,
	[Blz] [nvarchar](10) NULL,
	[KtoNr] [nvarchar](15) NULL,
	[KtoInh] [nvarchar](70) NULL,
	[MndtId] [nvarchar](35) NULL,
	[MndtDat] [nvarchar](8) NULL,
	[SeqType] [int] NULL,
	[Bankname] [nvarchar](50) NULL,
	[Sonst1] [nvarchar](80) NULL,
	[Sonst2] [nvarchar](80) NULL,
	[Sonst3] [nvarchar](80) NULL,
	[Sonst4] [nvarchar](80) NULL,
	[Sonst5] [nvarchar](80) NULL,
	[Sonst6] [nvarchar](80) NULL,
	[Sonst7] [nvarchar](80) NULL,
	[Memo] [nvarchar](max) NULL,
	[EMailInfo] [bit] NOT NULL,
	[FaxInfo] [bit] NOT NULL,
	[Serienbrief] [bit] NOT NULL,
	[Markiert] [bit] NOT NULL,
	[Abteilung] [nvarchar](255) NULL,
	[BKenn] [nvarchar](2) NULL,
	[GebTag] [nvarchar](4) NULL,
	[bSpende] [bit] NOT NULL,
	[MStufe] [int] NULL,
	[Mahndatum] [nvarchar](8) NULL,
	[Recalc] [bit] NOT NULL,
	[GesOffen] [float] NULL,
	[OffenJB] [float] NULL,
	[OffenMG] [float] NULL,
	[OffenBG] [float] NULL,
	[OffenZus] [float] NULL,
	[OffenRS] [float] NULL,
	[GesBeitrag] [float] NULL,
	[GesZuZahlen] [float] NULL,
	[GesGezahlt] [float] NULL,
	[NoMeldung] [bit] NOT NULL,
	[SoundEx] [nvarchar](40) NULL,
	[ZusZuZahlen] [float] NULL,
	[ZusGezahlt] [float] NULL,
	[ZusGesamt] [float] NULL,
	[JBZuZahlen] [float] NULL,
	[JBGezahlt] [float] NULL,
	[JBGesamt] [float] NULL,
	[OffenAG] [float] NULL,
	[Guthaben] [float] NULL,
	[SStrasse] [nvarchar](50) NULL,
	[SHsNr] [int] NULL,
	[UserId] [int] NULL,
	[UserName] [nvarchar](50) NULL,
	[UserDateTime] [nvarchar](15) NULL,
	[UserChangeId] [int] NULL,
	[UserChangeName] [nvarchar](50) NULL,
	[UserChangeDateTime] [nvarchar](15) NULL,
	[DFBMeldung] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
     */

    public class TblMitglieder
    {
        [Key] public int AId { get; set; } = 0;
        public int? Id { get; set; }
        [MaxLength(15)] public string MitglNr { get; set; } = string.Empty;
        [MaxLength(30)] public string MitglGrp { get; set; } = string.Empty;
        [MaxLength(20)] public string Anrede { get; set; } = string.Empty;
        [MaxLength(40)] public string Titel { get; set; } = string.Empty;
        [MaxLength(40)] public string Nachname { get; set; } = string.Empty;
        [MaxLength(40)] public string Vorname { get; set; } = string.Empty;
        [MaxLength(100)] public string Name { get; set; } = string.Empty;
        [MaxLength(50)] public string Zusatz { get; set; } = string.Empty;
        [MaxLength(50)] public string Strasse { get; set; } = string.Empty;
        [MaxLength(3)] public string Land { get; set; } = string.Empty;
        [MaxLength(8)] public string Plz { get; set; } = string.Empty;
        [MaxLength(50)] public string Ort { get; set; } = string.Empty;
        [MaxLength(80)] public string Briefanrede { get; set; } = string.Empty;
        [MaxLength(25)] public string Telefon1 { get; set; } = string.Empty;
        [MaxLength(25)] public string Telefon2 { get; set; } = string.Empty;
        [MaxLength(25)] public string Mobil { get; set; } = string.Empty;
        [MaxLength(25)] public string Fax { get; set; } = string.Empty;
        [MaxLength(100)] public string EMail { get; set; } = string.Empty;
        [MaxLength(100)] public string Internet { get; set; } = string.Empty;
        [MaxLength(20)] public string ZAnrede { get; set; } = string.Empty;
        [MaxLength(40)] public string ZTitel { get; set; } = string.Empty;
        [MaxLength(40)] public string ZNachname { get; set; } = string.Empty;
        [MaxLength(40)] public string ZVorname { get; set; } = string.Empty;
        [MaxLength(100)] public string ZName { get; set; } = string.Empty;
        [MaxLength(50)] public string ZZusatz { get; set; } = string.Empty;
        [MaxLength(50)] public string ZStrasse { get; set; } = string.Empty;
        [MaxLength(3)] public string ZLand { get; set; } = string.Empty;
        [MaxLength(8)] public string ZPlz { get; set; } = string.Empty;
        [MaxLength(50)] public string ZOrt { get; set; } = string.Empty;
        [Required] public bool ZAnschrift { get; set; } = false;
        [MaxLength(255)] public string Foto { get; set; } = string.Empty;
        [MaxLength(8)] public string Eintritt { get; set; } = string.Empty;
        [MaxLength(8)] public string Austritt { get; set; } = string.Empty;
        [MaxLength(8)] public string GebDatum { get; set; } = string.Empty;
        [MaxLength(1)] public string Geschlecht { get; set; } = string.Empty;
        [MaxLength(1)] public string AP { get; set; } = string.Empty;
        [MaxLength(50)] public string Betreuer { get; set; } = string.Empty;
        [MaxLength(50)] public string Funktion { get; set; } = string.Empty;
        [MaxLength(50)] public string SuchKrit { get; set; } = string.Empty;
        public int? Familie { get; set; }
        [MaxLength(255)] public string EMailNews { get; set; } = string.Empty;
        [Required] public bool VereinNews { get; set; } = false;
        [MaxLength(8)] public string BuchDat { get; set; } = string.Empty;
        public int? ZArt { get; set; }
        [MaxLength(1)] public string ZW { get; set; } = string.Empty;
        public int? FTag { get; set; }
        public int? VBank { get; set; }
        public int? Zahler { get; set; }
        [MaxLength(30)] public string IBAN { get; set; } = string.Empty;
        [MaxLength(30)] public string BIC { get; set; } = string.Empty;
        [MaxLength(10)] public string Blz { get; set; } = string.Empty;
        [MaxLength(15)] public string KtoNr { get; set; } = string.Empty;
        [MaxLength(70)] public string KtoInh { get; set; } = string.Empty;
        [MaxLength(35)] public string MndtId { get; set; } = string.Empty;
        [MaxLength(8)] public string MndtDat { get; set; } = string.Empty;
        public int? SeqType { get; set; }
        [MaxLength(50)] public string Bankname { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst1 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst2 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst3 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst4 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst5 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst6 { get; set; } = string.Empty;
        [MaxLength(80)] public string Sonst7 { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        [Required] public bool EMailInfo { get; set; } = false;
        [Required] public bool FaxInfo { get; set; } = false;
        [Required] public bool Serienbrief { get; set; } = false;
        [Required] public bool Markiert { get; set; } = false;
        [MaxLength(255)] public string Abteilung { get; set; } = string.Empty;
        [MaxLength(2)] public string BKenn { get; set; } = string.Empty;
        [MaxLength(4)] public string GebTag { get; set; } = string.Empty;
        [Required] public bool bSpende { get; set; } = false;
        public int? MStufe { get; set; }
        [MaxLength(8)] public string Mahndatum { get; set; } = string.Empty;
        [Required] public bool Recalc { get; set; } = false;
        public string? GesOffen { get; set; }
        public string? OffenJB { get; set; }
        public string? OffenMG { get; set; }
        public string? OffenBG { get; set; }
        public string? OffenZus { get; set; }
        public string? OffenRS { get; set; }
        public string? GesBeitrag { get; set; }
        public string? GesZuZahlen { get; set; }
        public string? GesGezahlt { get; set; }
        [Required] public bool NoMeldung { get; set; } = true;
        [MaxLength(40)] public string SoundEx { get; set; } = string.Empty;
        public string? ZusZuZahlen { get; set; }
        public string? ZusGezahlt { get; set; }
        public string? ZusGesamt { get; set; }
        public string? JBZuZahlen { get; set; }
        public string? JBGezahlt { get; set; }
        public string? JBGesamt { get; set; }
        public string? OffenAG { get; set; }
        public string? Guthaben { get; set; }
        [MaxLength(50)] public string SStrasse { get; set; } = string.Empty;
        public int? SHsNr { get; set; }
        public int? UserId { get; set; }
        [MaxLength(50)] public string UserName { get; set; } = string.Empty;
        [MaxLength(15)] public string UserDateTime { get; set; } = string.Empty;
        public int? UserChangeId { get; set; }
        [MaxLength(50)] public string UserChangeName { get; set; } = string.Empty;
        [MaxLength(15)] public string UserChangeDateTime { get; set; } = string.Empty;
        public int? DFBMeldung { get; set; }
    }
}
