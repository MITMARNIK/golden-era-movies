using System;

namespace GoldenEraMovies.Services
{
    public static class SyncState
    {
        public static DateTime LastSyncTime { get; set; } = DateTime.MinValue;
    }
}
