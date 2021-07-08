// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackTeamsChanges
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Teams> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=CSMM1\WIN19SQL17CI;Database=TrackTeamsChanges;Trusted_Connection=True;");
        }
    }
}
