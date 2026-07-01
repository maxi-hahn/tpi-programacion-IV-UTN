using Application.Dtos.Request.Admin;
using Application.Dtos.Responses;
using Application.Interfaces;
using Application.Mapper;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Presentation.Controller;

namespace Presentation.Presentation.Controller
{

    [Authorize(Policy = Policies.AdminOSysAdmin)]
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

        
        [HttpPut("UpdatePlan")]
        public async Task<ActionResult> UpdatePlan(Guid id, CreatePlanAdminRequest request)
        {
            var result = await _AdminService.UpdatePlan(id, request);
            return Ok(new
            {
                Message = "Clase actualizada correctamente",
                Class = result?.Name
            });
        }

        
        [HttpPost("CreatePlan")]
        public async Task<ActionResult> CreatePlan(CreatePlanAdminRequest request)
        {
            var result = await _AdminService.CreatePlan(request);
            return Ok(new
           {
               Message = "Clase creada correctamente",
                Class = result?.Name
           });
        }


        [HttpDelete("DeletePlan")]
        public async Task<ActionResult> DeletePlan(Guid id)
        {
            var result = await _AdminService.DeletePlan(id);
            if (result == null)
                return NotFound("Plan no encontrado");
            return Ok(result.ToPlanResponse());
        }


        [HttpGet("GetPlan")]
        public async Task<ActionResult<IEnumerable<PlanResponse>>> GetPlan()
        {
            var result = await _AdminService.GetPlan();
            return Ok(result.Select(p => p.ToPlanResponse()));
        }



        //-------------------- Class controler-----------------------------



        [HttpPost("CreteClass")]
        public async Task<ActionResult> CreteClass([FromBody] CreateClassWithSchedulesRequest request)
        {
            var result = await _AdminService.CreteClass(request.ClassRequest, request.ScheduleRequests);

            return Ok(new
            {
                Message = "Clase creada correctamente",
                Class = result?.Name
            });

        }


        [HttpGet("getClassDetail/{id}")]
        public async Task<ActionResult> GetClassDetail(Guid id)
        {
            var result = await _AdminService.GetClassDetail(id);
            return Ok(result);
        }


        [HttpPut("updateClass/{id}")]
        public async Task<ActionResult> UpdateClass(Guid id, [FromBody] CreateClassWithSchedulesRequest request)
        {

            var result = await _AdminService.UpdateClass(id, request.ClassRequest, request.ScheduleRequests);

            return Ok(result.Select(c => c.ToClassResponse()));
        }


        //-------------------- Client Inscriptions-----------------------------

        [HttpGet("clientInscriptions/{clientId}")]
        public async Task<ActionResult> GetClientInscriptions(Guid clientId)
        {
            var result = await _AdminService.GetClientInscriptions(clientId);
            return Ok(result);
        }
    }
}
