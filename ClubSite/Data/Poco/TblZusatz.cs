//
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
CREATE TABLE [dbo].[tblZusatz] (
[AId] int NOT NULL,
[Id] int,
[Zusatz1] nvarchar(80),
[Zusatz2] nvarchar(80),
[Zusatz3] nvarchar(80),
[Zusatz4] nvarchar(80),
[Zusatz5] nvarchar(80),
[Zusatz6] nvarchar(80),
[Zusatz7] nvarchar(80),
[Zusatz8] nvarchar(80),
[Zusatz9] nvarchar(80),
[Zusatz10] nvarchar(80),
[Zusatz11] nvarchar(80),
[Zusatz12] nvarchar(80),
[Zusatz13] nvarchar(80),
[Zusatz14] nvarchar(80),
[Zusatz15] nvarchar(80),
[Zusatz16] nvarchar(80),
[Zusatz17] nvarchar(80),
[Zusatz18] nvarchar(80),
[Zusatz19] nvarchar(80),
[Zusatz20] nvarchar(80),
[Zusatz21] nvarchar(80),
[Zusatz22] nvarchar(80),
[Zusatz23] nvarchar(80),
[Zusatz24] nvarchar(80),
[Zusatz25] nvarchar(80),
[Zusatz26] nvarchar(80),
[Zusatz27] nvarchar(80),
[Zusatz28] nvarchar(80),
[Zusatz29] nvarchar(80),
[Zusatz30] nvarchar(80),
[Zusatz31] nvarchar(80),
[Zusatz32] nvarchar(80),
[Zusatz33] nvarchar(80),
[Zusatz34] nvarchar(80),
[Zusatz35] nvarchar(80),
[Zusatz36] nvarchar(80),
[Zusatz37] nvarchar(80),
[Zusatz38] nvarchar(80),
[Zusatz39] nvarchar(80),
[Zusatz40] nvarchar(80)
)
    */
    public class TblZusatz
    {
        [Key] public int AId { get; set; }
        public int? Id { get; set; }
        [MaxLength(80)] public string Zusatz1 { get; set; } = string.Empty;  // Die Unberechenbaren
        [MaxLength(80)] public string Zusatz2 { get; set; } = string.Empty;  // Unbelehrbare
        [MaxLength(80)] public string Zusatz3 { get; set; } = string.Empty;  // Unersättliche
        [MaxLength(80)] public string Zusatz4 { get; set; } = string.Empty;  // Unglaubliche
        [MaxLength(80)] public string Zusatz5 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz6 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz7 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz8 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz9 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz10 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz11 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz12 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz13 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz14 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz15 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz16 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz17 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz18 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz19 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz20 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz21 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz22 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz23 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz24 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz25 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz26 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz27 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz28 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz29 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz30 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz31 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz32 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz33 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz34 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz35 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz36 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz37 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz38 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz39 { get; set; } = string.Empty;
        [MaxLength(80)] public string Zusatz40 { get; set; } = string.Empty;
    }
}
