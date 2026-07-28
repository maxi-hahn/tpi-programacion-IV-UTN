using Application.Dtos.Request.Admin;
using Application.Dtos.Responses;
using Application.Interfaces;
using Application.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;

namespace Presentation.Presentation.Controller
{

  
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {

        private readonly IAdminService _AdminService;
        public PlanController( IAdminService adminService)
        {
            _AdminService = adminService;
        }
        // -------------PLAN controller -------------------

        [HttpGet("GetPlan")]
        public async Task<ActionResult<IEnumerable<PlanResponse>>> GetPlan()
        {
            var result = await _AdminService.GetPlan();
            return Ok(result.Select(p => p.ToPlanResponse()));
        }

        [Authorize(Policy = Policies.AdminOSysAdmin)]
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

        [Authorize(Policy = Policies.AdminOSysAdmin)]
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

        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpDelete("DeletePlan")]
        public async Task<ActionResult> DeletePlan(Guid id)
        {
            var result = await _AdminService.DeletePlan(id);
            if (result == null)
                return NotFound("Plan no encontrado");
            return Ok(result.ToPlanResponse());
        }





        [Authorize(Policy = Policies.AdminOSysAdmin)]
        [HttpGet("getClassDetail/{id}")]
        public async Task<ActionResult> GetClassDetail(Guid id)
        {
            var result = await _AdminService.GetClassDetail(id);
            return Ok(result);
        }

 //       [Authorize(Policy = Policies.AdminOSysAdmin)]
  //      [HttpPut("updateClass/{id}")]
    //    public async Task<ActionResult> UpdateClass(Guid id, [FromBody] CreateClassWithSchedulesRequest request)
      //  {

        //    var result = await _AdminService.UpdateClass(id, request.ClassRequest, request.ScheduleRequests);

          //  return Ok(result.Select(c => c.ToClassResponse()));
        //}

    }
}
