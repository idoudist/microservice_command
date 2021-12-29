using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Enum;
using CommandsService.Models;
using CommandsService.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, 
            IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    //To Do
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event)
            {
                case "platform_Published":
                    Console.WriteLine("Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine event Type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            //we use this scope , because we cannot inject scopedservice into a singleton service
            //so this replace classic dependency injection
            using(var scope = _scopeFactory.CreateScope())
            {
                //get repo instance
                var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                //deserialise message
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                //add to database
                try
                {
                    var platform = _mapper.Map<Platform>(platformPublishedDto);
                    if (!commandRepo.PlatformExist(platform.ExternalId))
                    {
                        commandRepo.CreatePlatform(platform);
                        commandRepo.SaveChanges();
                    } else
                    {
                        Console.WriteLine("--> Platform Already Exist");
                    }
                } 
                catch (Exception e)
                {
                    Console.WriteLine($"--> Could not add platform to database: {e.Message}");
                }
            }
        }
    }
}
