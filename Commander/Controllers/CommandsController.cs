using AutoMapper;
using Commander.Data;
using Commander.Dtos;
using Commander.Models;
using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commander.Controllers
{
    //api/commands    //this can be dangerous, cause it will map the class name ; if it chagnes the route will also change
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase   //controllerBase => no view , controller => views 
    {
        private readonly IDataAccess _data;
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public CommandsController(IDataAccess data, IConfiguration config, IMapper mapper)
        {
            _data = data;
            _connectionString = config.GetConnectionString("default");
            _mapper = mapper;
        }

        [HttpGet] //api/commands
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAllCommands()

        {
            var sql = "select * from commands";
            var commandItems = await _data.LoadData<Command, dynamic>(sql, new { }, _connectionString);


            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }
        [HttpGet("{id}", Name = "GetCommandById")]  //api/commands/{id}
        public async Task<ActionResult<CommandReadDto>> GetCommandById(int id)
        {
            var sql = $"select * from commands where id={id} limit 1";
            var commandItem = await _data.LoadData<Command, dynamic>(sql, new { }, _connectionString);
            if (commandItem.Count == 0)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem[0]));

        }
        [HttpPost]
        public async Task<ActionResult<Command>> CreateCommand(CommandReadDto cmd)

        {
            if (cmd.Description == null || cmd.Name == null)
            {
                return BadRequest();
            }
            var sql = "insert into commands (name,description) values(@Name,@Description)";
            var inserted = await _data.SaveData(sql, new { Name = cmd.Name, Description = cmd.Description }, _connectionString);
            if (inserted == 1)
            {
                return CreatedAtRoute(nameof(GetCommandById), new { Inserted = true });
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Command>> ReplaceCommand(int? id, CommandReadDto cmd)

        {
            if (cmd.Description == null || cmd.Name == null || id == null)
            {
                return BadRequest();
            }
            var sql = "update commands Set name = @Name, description = @Description where Id=@id";
            var inserted = await _data.SaveData(sql, new { Name = cmd.Name, Description = cmd.Description, Id=id }, _connectionString);
            if (inserted == 1)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Command>> DeleteCommand(int? id)

        {
            if (id == null)
            {
                return BadRequest();
            }
            var sql = "delete from commands where Id=@id";
            var inserted = await _data.SaveData(sql, new { Id = id }, _connectionString);
            if (inserted == 1)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
