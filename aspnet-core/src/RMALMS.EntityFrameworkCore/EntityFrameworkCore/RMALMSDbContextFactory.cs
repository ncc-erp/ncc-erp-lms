using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RMALMS.Configuration;
using RMALMS.Web;

namespace RMALMS.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class RMALMSDbContextFactory : IDesignTimeDbContextFactory<RMALMSDbContext>
    {
        public RMALMSDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<RMALMSDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            RMALMSDbContextConfigurer.Configure(builder, configuration.GetConnectionString(RMALMSConsts.ConnectionStringName));

            return new RMALMSDbContext(builder.Options);
        }
    }
}
