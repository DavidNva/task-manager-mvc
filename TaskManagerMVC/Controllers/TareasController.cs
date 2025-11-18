using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Services;

namespace TaskManagerMVC.Controllers
{
    [Route("api/[controller]")]
    public class TareasController : ControllerBase//Usamos ControllerBase en lugar de Controller cuando queremos crear una API sin vistas.
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;

        public TareasController(ApplicationDBContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpGet("Listar")]
        public async Task<List<TaskItem> > ObtenerTareas()
        {
            return await _context.Tasks.ToListAsync();
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
                Description = "Descripcion: " + titulo,
                UserCreatorId = usuarioId,
                CreatedAt = DateTime.UtcNow,
                Order = ordenMayor + 1
            };
            _context.Add(tarea);
            await _context.SaveChangesAsync();

            return tarea;
        }

    }
}
