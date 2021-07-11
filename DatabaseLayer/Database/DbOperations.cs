// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrackTeamsChanges
{
    public class DbOperations
    {
        public static void AddTeamsToTable(IList<Teams> teams)
        {
            using (var context = new DbCtxt())
            {
                context.Teams.AddRange(teams);
                context.SaveChanges();
            }
        }
        public static IList<Teams> GetTeams(int limit)
        {
            using (var context = new DbCtxt())
            {
                return context.Teams.OrderBy(o => o.CreatedOn).Take(limit).ToList();
            }
        }

        internal static void ClearTeamsTable()
        {
            using (var context = new DbCtxt())
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE Teams");
            }
        }
    }
}
