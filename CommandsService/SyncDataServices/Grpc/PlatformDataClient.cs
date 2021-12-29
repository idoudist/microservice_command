using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling Grpc Service {_configuration["GrpcPlatform"]}");
            var grpcChannel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
            var grpcClient = new GrpcPlatform.GrpcPlatformClient(grpcChannel);
            //GetAllRequest() from platforms.proto
            var request = new GetAllRequest();

            try
            {
                var reply = grpcClient.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            } catch (Exception e)
            {
                Console.WriteLine($"--> Could Not call Grpc Server : {e.Message}");
                return null;
            }
        }
    }
}
