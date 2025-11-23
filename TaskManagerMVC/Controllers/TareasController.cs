using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Models;
using TaskManagerMVC.Services;

namespace TaskManagerMVC.Controllers
{
    [Route("api/[controller]")]
    public class TareasController : ControllerBase//Usamos ControllerBase en lugar de Controller cuando queremos crear una API sin vistas.
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IMapper _mapper;
        public TareasController(ApplicationDBContext context, IServicioUsuarios servicioUsuarios, IMapper mapper)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
            _mapper = mapper;
        }

        [HttpGet("Listar")]
        public async Task<List<TaskDTO>> ObtenerTareas()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tareas = await _context.Tasks
                .Where(t => t.UserCreatorId == usuarioId)
                .OrderBy(t=>t.Order)
                .ProjectTo<TaskDTO>(_mapper.ConfigurationProvider)//Con esto le decimos a automapper que mapee de TaskItem a TaskDTO usando la configuracion que tenemos en el profile.
                .ToListAsync();//A la expresion => se le llama operador lambda. Para entenderlo mejor, es como una funcion anonima que recibe un parametro t y devuelve t.UserCreatorId == usuarioId
            return tareas;

        }

        [HttpPost("Crear")]
        public async Task<TaskItem> CrearTarea([FromBody] string titulo)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var existeTareas = await _context.Tasks.AnyAsync(t => t.UserCreatorId == usuarioId);

            var ordenMayor = 0;
            if (existeTareas)
            {
                ordenMayor = await _context.Tasks.Where(t => t.UserCreatorId == usuarioId)
                    .Select(t => t.Order).MaxAsync();
            }
            var tarea = new TaskItem
            {
                Title = titulo,
                Description = "description: " + titulo,
                UserCreatorId = usuarioId,
                CreatedAt = DateTime.UtcNow,
                Order = ordenMayor + 1
            };
            _context.Add(tarea);
            await _context.SaveChangesAsync();

            return tarea;
        }

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var tareas = await _context.Tasks
                .Where(t => t.UserCreatorId == usuarioId).ToListAsync();

            var tareasId = tareas.Select(t => t.Id);

            var idsTareasNoPertenecenAlUsuario = ids.Except(tareasId).ToList();

            if (idsTareasNoPertenecenAlUsuario.Any())
            {
                return Forbid();
            }
            var tareasDiccionario = tareas.ToDictionary(t => t.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tarea = tareasDiccionario[id];
                tarea.Order = i + 1;
            }
            await _context.SaveChangesAsync();
            return Ok();

        }


        [HttpGet("ObtenerTareaPorId/{id:int}")]
        public async Task<ActionResult<TaskItem>>ObtenerTareaPorId(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tasks
                .Include(t =>t.Steps)
                .FirstOrDefaultAsync(t => t.Id == id &&
            t.UserCreatorId == usuarioId);

            if(tarea is null)
            {
                return NotFound();
            }

            return tarea;
        }


        [HttpPut("EditarTarea/{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, [FromBody] TaskEditarDTO taskEditarDTO)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tarea = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserCreatorId == usuarioId);

            if(tarea is null)
            {
                return NotFound();
            }

            tarea.Title = taskEditarDTO.Title;
            tarea.Description = taskEditarDTO.description;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("BorrarTarea/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserCreatorId == usuarioId);

            if(tarea is null)
            {
                return NotFound();
            }

            _context.Remove(tarea);

            await _context.SaveChangesAsync();

            return Ok();
        }
    
    }
}
