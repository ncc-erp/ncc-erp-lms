using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace RMALMS.EntityFrameworkCore
{
    public static class RMALMSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<RMALMSDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<RMALMSDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
