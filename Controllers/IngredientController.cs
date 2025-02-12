using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using ProvisionAPI.Models;
using ProvisionAPI.Models.IngredientController;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IngredientController : ControllerBase
	{
		private IJwtAuthenticationService _jwtAuthenticationService;
		private IIngredientServices _ingredientServices;

		public IngredientController(IJwtAuthenticationService jwtAuthenticationService, IIngredientServices services)
		{
			_jwtAuthenticationService = jwtAuthenticationService;
			this._ingredientServices = services;
		}

		[Authorize]
		[HttpPost("AddIngredient")]
		public async Task<IActionResult> AddIngredient(List<AddNewIngredient> ingredients)
		{
			try
			{
				if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
				{
					throw new Exception("Token not found");
				}
				var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());
				foreach(var ingredient in ingredients)
				{
					await this._ingredientServices.InsertIngredient(ingredient, uid);
				}
				return Ok("Done inserting new ingredients");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpPost("UpdateIngredient")]
		public async Task<IActionResult> UpdateIngredient(UpdateIngredient ingredient)
		{
			try
			{
				if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
				{
					throw new Exception("Token not found");
				}
				var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());
				await this._ingredientServices.UpdateIngredient(ingredient);
				return Ok("Done updating ingredient");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("GetIngredient")]
		public async Task<List<Ingredient>> GetIngredients()
		{
			var result = await this._ingredientServices.GetIngredients();
			return result == null ? new List<Ingredient>() : result;
		}
	}
}
