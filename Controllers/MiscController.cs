using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProvisionAPI.Models;
using ProvisionAPI.Models.MiscController;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MiscController : ControllerBase
	{
		private IMiscServices _miscService;

		public MiscController(IMiscServices services) {
			this._miscService = services;
		}

		[HttpGet("GetUnits")]
		[Authorize]
		public async Task<IActionResult> GetUnits()
		{
			try
			{
				var units = await _miscService.GetUnits();
				return Ok(units == null ? new List<Unit>() : units);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("InsertNewUnit")]
		[Authorize]
		public async Task<IActionResult> InsertNewUnit(InsertUnit unit)
		{
			try
			{
				var units = await _miscService.GetUnits();
				if(units.Any(x => x.Title == unit.Title))
				{
					return BadRequest("Unit already exists");
				}

				var result = await _miscService.InsertNewUnit(unit.Title);
				return Ok(result ? "Successfully inserted" : "Failed to insert");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("UpdateUnit")]
		[Authorize]
		public async Task<IActionResult> UpdateUnit(Unit unit)
		{
			try
			{
				var result = await _miscService.UpdateUnit(unit);
				return Ok(result ? "Successfully updated" : "Failed to update");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost("RemoveUnit")]
		[Authorize]
		public async Task<IActionResult> RemoveUnit(RemoveUnit unit)
		{
			try
			{
				var result = await _miscService.RemoveUnit(unit.UnitId);
				return Ok(result ? "Succesfully remove" : "Failed to remove");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
