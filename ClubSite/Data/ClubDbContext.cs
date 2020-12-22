// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://https://github.com/axuno/ClubSite

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ClubSite.Data
{
    public class ClubDbContext : DbContext
    {
        public ClubDbContext(DbContextOptions<ClubDbContext> options) : base(options)
        {
        }

        public DbSet<Poco.ClubMember>? ClubMember { get; set; }
        public DbSet<Poco.TournamentRegistration>? TournamentRegistration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Poco.ClubMember>().ToTable("Club_Member");
            modelBuilder.Entity<Poco.TournamentRegistration>().ToTable("Club_TournamentRegistration");
        }
    }
}