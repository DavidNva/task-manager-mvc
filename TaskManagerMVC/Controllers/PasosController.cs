using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Models;
using TaskManagerMVC.Services;

namespace TaskManagerMVC.Controllers
{
    [Route("api/steps")]
    public class stepsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IServicioUsuarios _servicioUsuarios;
        public stepsController(ApplicationDBContext context, IServicioUsuarios servicioUsuarios)
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

            var existensteps = await _context.Steps.AnyAsync(p => p.TaskItemId == tareaId);

            var ordenMayor = 0;
            if (existensteps)
            {
                ordenMayor = await _context.Steps
                    .Where(p => p.TaskItemId == tareaId).Select(p => p.Order).MaxAsync();
            }

            var paso = new Step();

            paso.TaskItemId = tareaId;
            paso.Order = ordenMayor + 1;
            paso.Description = pasoCrearDTO.Description;
            paso.IsCompleted = pasoCrearDTO.IsCompleted;

            _context.Add(paso);
            await _context.SaveChangesAsync();
            return paso;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var paso = await _context.Steps.Include(p => p.TaskItem).FirstOrDefaultAsync(p => p.Id == id);
            if(paso is null)
            {
                return NotFound();
            }
            if(paso.TaskItem.UserCreatorId != usuarioId)
            {
                return Forbid();
            }

            paso.Description = pasoCrearDTO.Description;
            paso.IsCompleted = pasoCrearDTO.IsCompleted;
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
