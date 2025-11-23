using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Models;
using TaskManagerMVC.Services;

namespace TaskManagerMVC.Controllers
{
    [Route("api/pasos")]
    public class PasosController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        public PasosController(ApplicationDBContext context, IServicioUsuarios servicioUsuarios)
        {
            _context = context;
            _servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("CrearPaso/{tareaId:int}")]
        public async Task<ActionResult<Step>> CrearPaso(int tareaId, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var tarea = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == tareaId);

            if(tarea is null)
            {
                return NotFound();
            }
            if (tarea.UserCreatorId != usuarioId)
            {
                return Forbid();
            }

            var existenPasos = await _context.Steps.AnyAsync(p => p.TaskItemId == tareaId);

            var ordenMayor = 0;
            if (existenPasos)
            {
                ordenMayor = await _context.Steps
                    .Where(p => p.TaskItemId == tareaId).Select(p => p.Order).MaxAsync();
            }

            var paso = new Step();

            paso.TaskItemId = tareaId;
            paso.Order = ordenMayor + 1;
            paso.Description = pasoCrearDTO.Description;
            paso.IsCompleted = pasoCrearDTO.Realizado;

            _context.Add(paso);
            await _context.SaveChangesAsync();
            return paso;
        }
    }
}
