// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackTeamsChanges
{
    public class DbOperations
    {
        public static async Task AddTeamsToTable(IList<Teams> teams)
        {
            using (var context = new DbContext())
            {
                context.Teams.AddRange(teams);
                await context.SaveChangesAsync();
            }
        }
        public static IList<Teams> GetTeams(int limit)
        {
            using (var context = new DbContext())
            {
                return context.Teams.OrderBy(o => o.CreatedOn).Take(limit).ToList();
            }
        }
        
        internal static async Task ClearTeamsTable()
        {
            using (var context = new DbContext())
            {
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Teams");
            }
        }
    }
}
