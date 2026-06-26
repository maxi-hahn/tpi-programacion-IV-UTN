using Application.Dtos.Request.Admin;
using Application.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controller;

namespace Presentation.Presentation.Controller
{


    public class AdminController : UsersController<Admin>
    {

        private readonly IAdminService _AdminService;
        public AdminController(IUserService service, IAuthService authService, IAdminService adminService) : base(service, authService)
        {
            _AdminService = adminService;
        }

        //Borrar horarios de clases

        //Modificar horarios de clases

        //Crear una clase con horaris deseados


        // -------------PLAN controller --------------------

        [Authorize]
        [HttpPost("UpdatePlan")]
        public async Task<ActionResult> UpdatePlan(Guid id, CreatePlanAdminRequest request)
        {
            var result = await _AdminService.UpdatePlan(id, request);
            if (result == null)
            {
                return BadRequest("Datos incorrectos");
            }
            return Ok(new
            {
                Message = "Clase creada correctamente",
                Class = result?.Name
            });
        }

        [Authorize]
        [HttpPost("CreatePlan")]
        public async Task<ActionResult> CreatePlan(CreatePlanAdminRequest request)
        {
            var result = await _AdminService.CreatePlan(request);
            if (result == null)
            {
              return BadRequest("Datos incorrectos");
          }
            return Ok(new
           {
               Message = "Clase creada correctamente",
                Class = result?.Name
           });
        }

        [Authorize]
        [HttpPost("DeletePlan")]
        public async Task<ActionResult> DeletePlan(Guid id)
        {
            var result = await _AdminService.DeletePlan(id);
            if (result == null)
            {
                return NotFound("Plan no encontrado");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetPlan")]
        public async Task<ActionResult<IEnumerable<Plan>>> GetPlan()
        {
            var result = await _AdminService.GetPlan();
            if (result == null)
            {
                return NotFound("Planes no encontrados");
            }
            return Ok(result);
        }

        //-----------------Schedule Controller ----------------------------
        [Authorize]
        [HttpPost("DeleteSchedule")]
        public async Task<ActionResult> DeleteSchedule(Guid id)
        {
            var result = await _AdminService.DeleteSchedule(id);
            if (result == null)
            {
                return NotFound("Horario no encontrado");
            }
            return Ok(result);
        }


        //-------------------- Class controler-----------------------------


        [Authorize]
        [HttpPost("CreteClass")]
        public async Task<ActionResult> CreteClass([FromBody] CreateClassWithSchedulesRequest request)

        {

            var result = await _AdminService.CreteClass(request.ClassRequest, request.ScheduleRequests);
            if (result == null)
            {
                return BadRequest("Datos incorrectos");
            }

            return Ok(new
            {
                Message = "Clase creada correctamente",
                Class = result?.Name
            });

        }

        [Authorize]
        [HttpGet("getClass")]
        public async Task<ActionResult> GetClass()
        {
            var result = await _AdminService.GetClass();
            if (result == null)
            {
                return NotFound("Clase no encontrada");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut("updateClass/{id}")]
        public async Task<ActionResult> UpdateClass(Guid id, [FromBody] CreateClassWithSchedulesRequest request)
        {

            var result = await _AdminService.UpdateClass(id, request.ClassRequest, request.ScheduleRequests);
            if (result == null)
            {
                return NotFound("Clase no encontrada");
            }

            return Ok(result);
        }

        [Authorize()]
        [HttpDelete("deleteClass/{id}")]
        public async Task<ActionResult<IEnumerable<Class?>>> DeleteClass(Guid id)
        {
            var result = await _AdminService.DeleteClass(id);
            if (result == null)
            {
                return NotFound("Clase no encontrada");
            }

            return Ok(result);
        }
    }
}
