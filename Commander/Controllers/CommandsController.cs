using Commander.Data;
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

        public CommandsController(IDataAccess data, IConfiguration config)
        {
            _data = data;
            _connectionString = config.GetConnectionString("default");
        }
        
        [HttpGet] //api/commands
        public ActionResult <IEnumerable<Command>> GetAllCommands()

        {
            var sql = "select * from commands";
            var commandItems = _data.LoadData<Command,dynamic>(sql, new { }, _connectionString);

            return Ok(commandItems);
        }
        [HttpGet("{id}")]  //api/commands/{id}
        public ActionResult<Command> GetCommandById(int id)
        {
            var sql = $"select * from commands where id={id} limit 1";
            var commandItem = _data.LoadData<Command, dynamic>(sql, new { }, _connectionString);
            return Ok(commandItem);

        }

    }
}
