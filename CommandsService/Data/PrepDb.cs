using CommandsService.Models;
using CommandsService.Repositories;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
            }
        }

        private static void SeedData(ICommandRepo commandRepo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("--> Seeding new Platforms ...");
            foreach(var platform in platforms)
            {
                if (!commandRepo.PlatformExist(platform.ExternalId))
                {
                    commandRepo.CreatePlatform(platform);
                }
            }
            commandRepo.SaveChanges();
        }
    }
}
