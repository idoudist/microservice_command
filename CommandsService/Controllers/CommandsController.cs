using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;
        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");
            if(!_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var commands = _commandRepo.GetCommandsForPlatform(platformId);
            var models = _mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Ok(models);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");
            if (!_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var command = _commandRepo.GetCommand(platformId, commandId);
            if(command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatorm([FromRoute] int platformId, [FromBody] CommandCreateDto command)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatorm: {platformId} ");
            if (!_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var commandItem = _mapper.Map<Command>(command);

            _commandRepo.CreateCommand(platformId, commandItem);
            _commandRepo.SaveChanges();
            var commandDto = _mapper.Map<CommandReadDto>(commandItem);
            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandDto.Id}, commandDto);
        }
    }
}
