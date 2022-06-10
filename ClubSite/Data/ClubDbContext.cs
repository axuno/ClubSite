﻿// Copyright (C) axuno gGmbH and Contributors.
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
// https://github.com/axuno/ClubSite

using Microsoft.EntityFrameworkCore;

namespace ClubSite.Data;

public class ClubDbContext : DbContext
{
    public ClubDbContext(DbContextOptions<ClubDbContext> options) : base(options)
    {
    }

    public DbSet<Poco.TournamentRegistration>? TournamentRegistration { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Poco.TournamentRegistration>().ToTable("Club_TournamentRegistration");
    }
}