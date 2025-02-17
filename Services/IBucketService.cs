
using ProvisionAPI.Models;
using ProvisionAPI.Models.BucketController;

namespace ProvisionAPI.Services
{
	public interface IBucketService
	{
		Task<bool> InsertIntoBucket(AddBucket bucket, int uid);
		Task<List<GetBucketByRange>> GetBucketByDateRange(DateOnly from, DateOnly to , int uid);
		Task<bool> UpdateBucket(List<UpdateBucket> updateBuckets, int uid);
	}
}