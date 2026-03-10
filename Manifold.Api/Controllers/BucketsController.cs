using Manifold.Api.Data;
using Manifold.Api.Data.DTO;
using Manifold.Api.Data.Entities;
using Manifold.Api.Services.Buckets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manifold.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BucketsController : ControllerBase
{
    private readonly BucketService _bucketService;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BucketsController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    
    public BucketsController(
        BucketService bucketService, 
        ApplicationDbContext dbContext, 
        ILogger<BucketsController> logger, 
        UserManager<IdentityUser> userManager)
    {
        _bucketService = bucketService;
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllBuckets()
    {
        var buckets = await _dbContext.Buckets.ToListAsync();
        var partials = buckets.Select(PartialBucket.From);
        return Ok(new ApiResponse<List<PartialBucket>>()
        {
            Success = true,
            Data = partials.ToList()
        });
    }

    [Authorize]
    [HttpGet("{bucketId}")]
    public async Task<IActionResult> GetBucket(Guid bucketId)
    {
        var query = 
            from bucket in _dbContext.Buckets
            where bucket.Id == bucketId
            select bucket;

        var result = await query.FirstOrDefaultAsync();
        
        return Ok(new ApiResponse<PartialBucket>()
        {
            Success = result != null,
            Data = result == null ? null : PartialBucket.From(result)
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBucket([FromBody] CreateBucketBody body)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest();
        }
        

        var entry = await _dbContext.Buckets.AddAsync(new Bucket()
        {
            DriverType = body.DriverType,
            Name = body.Name,
            Description = body.Description,
            ConnectionString = body.ConnectionString,
        });

        var bucket = entry.Entity;
        _bucketService.UpdateDriver(bucket);
        await _dbContext.SaveChangesAsync();

        return Ok(new ApiResponse<PartialBucket>()
        {
            Success = true,
            Data = PartialBucket.From(bucket)
        });
    }
}