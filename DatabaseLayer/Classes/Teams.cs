﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using System.ComponentModel.DataAnnotations;

namespace TrackTeamsChanges
{
    public class Teams
    {
        [Key]
        public string TeamId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string DriveId { get; set; }
        public string DisplayName { get; set; }
        public string SiteUrl { get; set; }
        public string SiteId { get; set; }
        public string ListId { get; set; }
    }
}
