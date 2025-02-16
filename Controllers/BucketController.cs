using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ProvisionAPI.Models;
using ProvisionAPI.Models.BucketController;
using ProvisionAPI.Services;

namespace ProvisionAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BucketController : ControllerBase
	{
		private IBucketService _bucketService;
		private IJwtAuthenticationService _jwtAuthenticationService;

		public BucketController(IBucketService bucketService, IJwtAuthenticationService jwtAuthenticationService)
		{
			this._bucketService = bucketService;
			this._jwtAuthenticationService = jwtAuthenticationService;

		}


		[Authorize]
		[HttpPost("AddBucket")]
		public async Task<IActionResult> AddBucket(List<AddBucket> buckets)
		{
			try
			{
				// AddBucket is a model that contains the properties of the bucket
				// that we want to add to the database
				// We will implement this method in the next step
				if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
				{
					throw new Exception("Token not found");
				}
				var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());

				foreach (var bucket in buckets)
				{
					await this._bucketService.InsertIntoBucket(bucket, uid);
				}

				return Ok("Bucket added successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpGet("GetBucketByDateRange")]
		public async Task<IActionResult> GetBucketByDateRange([FromQuery] DateOnly from, [FromQuery] DateOnly to)
		{
			try
			{
				List<Bucket> buckets = new List<Bucket>();
				if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
				{
					throw new Exception("Token not found");
				}
				var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());
				var GetBucketByRangeresult = await this._bucketService.GetBucketByDateRange(from, to, uid);
				if(GetBucketByRangeresult != null)
				{
					var PerBucketgroup = GetBucketByRangeresult.GroupBy(x => x.PurchaseDate).ToList();
					foreach (var group in PerBucketgroup)
					{
						var ingredientGroup = group.GroupBy(x => x.IngredientID).ToList();

						var bucket = new Bucket()
						{
							PurchaseDate = group.First().PurchaseDate,
							ingredients = ingredientGroup.Select(x => new IngredientRequired
							{
								//BucketId = x.ID,
								IngredientId = x.First().IngredientID,
								Required = x.Sum(s => s.UnitRequired),
								ImagePath = x.First().IngredientImage,
								Purchased = x.Sum(s => s.UnitPurchased),
								Title = x.First().IngredientTitle,
								Details = x.First().IngredientDetails,
								PricePerUnit = x.First().PricePerUnit,
								UnitTitle = x.First().UnitTitle
							}).ToList()
						};
						buckets.Add(bucket);
					}
					buckets = buckets.OrderBy(buckets => buckets.PurchaseDate).ToList();
				}
				return Ok(buckets);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		//[Authorize]
		//[HttpPost("UpdateBucket")]
		//public async Task<IActionResult> UpdateBucket(UpdateBucket updateBucket)
		//{
		//	try
		//	{
		//		if (!Request.Headers.TryGetValue("Authorization", out StringValues token))
		//		{
		//			throw new Exception("Token not found");
		//		}
		//		var uid = _jwtAuthenticationService.GetUserIDFromToken(token.ToString().Replace("Bearer", "").Trim());

		//		return Ok(bucket);
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest(ex.Message);
		//	}

	}
}
