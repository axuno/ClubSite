// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClubSite.Data.Poco
{
    /*
CREATE TABLE [Club_Member] (
   [Id] INT,
   [Guid] NVARCHAR(50),
   [UserName] NVARCHAR(20),
   [PasswordHash] NVARCHAR(70),
   [LastLoginOn] DATETIME,
   [AccessFailedCount] INT,
   [LockoutEndDateUtc] DATETIME,
   [MembershipNo] NVARCHAR(14),
   [Gender] NVARCHAR(1),
   [Title] NVARCHAR(40),
   [FirstName] NVARCHAR(40),
   [MiddleName] NVARCHAR(40),
   [LastName] NVARCHAR(40),
   [Nickname] NVARCHAR(40),
   [Street] NVARCHAR(50),
   [PostalCode] NVARCHAR(8),
   [City] NVARCHAR(50),
   [CountryCode] NVARCHAR(3),
   [PhoneNumber] NVARCHAR(25),
   [PhoneNumber2] NVARCHAR(25),
   [Mobile] NVARCHAR(25),
   [Fax] NVARCHAR(25),
   [Email] NVARCHAR(100),
   [Email2] NVARCHAR(100),
   [PhotoFilename] NVARCHAR(255),
   [JoinedOn] DATETIME,
   [SeparatedOn] DATETIME,
   [Birthday] DATETIME,
   [Status] NVARCHAR(1),
   [HeadOfFamily] INT,
   [Payer] INT,
   [IBAN] NVARCHAR(30),
   [BIC] NVARCHAR(30),
   [AccountHolder] NVARCHAR(70),
   [BankAccount] NVARCHAR(15),
   [BankCode] NVARCHAR(10),
   [BankName] NVARCHAR(50),
   [MandateId] NVARCHAR(30),
   [MandateDate] DATETIME,
   [SequenceType] INT,
   [PaymentMethod] INT,
   [PaymentTerm] NVARCHAR(1),
   [CreatedOn] DATETIME,
   [CreatedBy] NVARCHAR(50),
   [ModifiedOn] DATETIME,
   [ModifiedBy] NVARCHAR(50)
)
    */
    public class ClubMember
    {
        [Key] public int? Id { get; set; }
        [MaxLength(50)] public string Guid { get; set; } = string.Empty;
        [MaxLength(20)] public string UserName { get; set; } = string.Empty;
        [MaxLength(70)] public string PasswordHash { get; set; } = string.Empty;
        [Column(TypeName = "datetime")] public DateTime? LastLoginOn { get; set; }
        public int? AccessFailedCount { get; set; }
        [Column(TypeName = "datetime")] public DateTime? LockoutEndDateUtc { get; set; }
        [MaxLength(14)] public string MembershipNo { get; set; } = string.Empty;
        [MaxLength(1)] public string Gender { get; set; } = string.Empty;
        [MaxLength(40)] public string Title { get; set; } = string.Empty;
        [MaxLength(40)] public string FirstName { get; set; } = string.Empty;
        [MaxLength(40)] public string MiddleName { get; set; } = string.Empty;
        [MaxLength(40)] public string LastName { get; set; } = string.Empty;
        [MaxLength(40)] public string Nickname { get; set; } = string.Empty;
        [MaxLength(50)] public string Street { get; set; } = string.Empty;
        [MaxLength(8)] public string PostalCode { get; set; } = string.Empty;
        [MaxLength(50)] public string City { get; set; } = string.Empty;
        [MaxLength(3)] public string CountryCode { get; set; } = string.Empty;
        [MaxLength(25)] public string PhoneNumber { get; set; } = string.Empty;
        [MaxLength(25)] public string PhoneNumber2 { get; set; } = string.Empty;
        [MaxLength(25)] public string Mobile { get; set; } = string.Empty;
        [MaxLength(25)] public string Fax { get; set; } = string.Empty;
        [MaxLength(100)] public string Email { get; set; } = string.Empty;
        [MaxLength(100)] public string Email2 { get; set; } = string.Empty;
        [MaxLength(255)] public string PhotoFilename { get; set; } = string.Empty;
        [Column(TypeName = "datetime")] public DateTime? JoinedOn { get; set; }
        [Column(TypeName = "datetime")] public DateTime? SeparatedOn { get; set; }
        [Column(TypeName = "datetime")] public DateTime? Birthday { get; set; }
        [MaxLength(1)] public string Status { get; set; } = string.Empty;
        public int? HeadOfFamily { get; set; }
        public int? Payer { get; set; }
        [MaxLength(30)] public string IBAN { get; set; } = string.Empty;
        [MaxLength(30)] public string BIC { get; set; } = string.Empty;
        [MaxLength(70)] public string AccountHolder { get; set; } = string.Empty;
        [MaxLength(15)] public string BankAccount { get; set; } = string.Empty;
        [MaxLength(10)] public string BankCode { get; set; } = string.Empty;
        [MaxLength(50)] public string BankName { get; set; } = string.Empty;
        [MaxLength(30)] public string MandateId { get; set; } = string.Empty;
        [Column(TypeName = "datetime")] public DateTime? MandateDate { get; set; }
        public int? SequenceType { get; set; }
        public int? PaymentMethod { get; set; }
        [MaxLength(1)] public string PaymentTerm { get; set; } = string.Empty;
        [Column(TypeName = "datetime")] public DateTime? CreatedOn { get; set; }
        [MaxLength(50)] public string CreatedBy { get; set; } = string.Empty;
        [Column(TypeName = "datetime")] public DateTime? ModifiedOn { get; set; }
        [MaxLength(50)] public string ModifiedBy { get; set; } = string.Empty;
    }
}