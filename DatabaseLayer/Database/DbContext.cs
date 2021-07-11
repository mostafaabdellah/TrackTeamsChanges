// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Data.Entity;

namespace TrackTeamsChanges
{
    public class DbCtxt : DbContext
    {
        public DbSet<Teams> Teams { get; set; }

        public DbCtxt():base(@"Server=CSMM1\WIN19SQL17CI;Database=TrackTeamsChanges;Trusted_Connection=True;")
        {
            _ =
                System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
