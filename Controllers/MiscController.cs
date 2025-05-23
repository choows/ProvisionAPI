﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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
		private readonly IAssetProcessing _assetProcessing;
		private readonly IJwtAuthenticationService _jwtAuthenticationService;

		public MiscController(IMiscServices services, IAssetProcessing assetProcessing, IJwtAuthenticationService jwtAuthenticationService) {
			this._miscService = services;
			this._assetProcessing = assetProcessing;
			this._jwtAuthenticationService = jwtAuthenticationService;
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

		[HttpPost("UploadImage")]
		[Authorize]
		public async Task<IActionResult> UploadImage(IFormFile file)
		{
			try
			{
				if(file == null)
				{
					throw new Exception("file not found");
				}
				var stream = file.OpenReadStream();
				var name = file.FileName;
				if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
				{
					throw new Exception("Token not found");
				}

				var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());
				var resp = await this._assetProcessing.UploadImage(stream, name, uid);
				return Ok(new { Status = "OK" , Url = resp.Item1, id= resp.Item2 });
			}
			catch(Exception ex)
			{
				return BadRequest(ex);
			}
		}


	}
}
