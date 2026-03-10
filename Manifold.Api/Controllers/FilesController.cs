using Manifold.Api.Data;
using Manifold.Api.Data.DTO;
using Manifold.Api.Data.Entities;
using Manifold.Api.Services.Buckets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Manifold.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController : ControllerBase
{
    private readonly ILogger<FilesController> _logger;
    private readonly BucketService _bucketService;
    private readonly ApplicationDbContext _dbContext;

    public FilesController(
        ILogger<FilesController> logger,
        BucketService bucketService,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _bucketService = bucketService;
        _dbContext = dbContext;
    }
    
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] UploadFileForm form)
    {
        if (form.File.Length == 0)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Message = "File is empty"
            });
        }
        
        var driver = _bucketService.GetDriver(form.DestinationBucket);
        var bucket = await _dbContext.Buckets.Where(v => v.Id == form.DestinationBucket).FirstOrDefaultAsync();
        if (driver == null || bucket == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Message = "Driver not found"
            });
        }
        
        var metaEntry = await _dbContext.Metas.AddAsync(new FileMeta()
        {
            Filename = form.File.Name,
            ContentType = form.File.ContentType,
            ContentLength = form.File.Length,
            Bucket = bucket
        });
        
        var meta = metaEntry.Entity;
        
        await using var stream = form.File.OpenReadStream();
        await driver.UploadAsync(meta, stream);
        return Ok(new ApiResponse<FileMeta>()
        {
            Success = true,
            Message = "File successfully uploaded",
            Data = meta
        });
    }

    [HttpGet("bucket/{bucket}")]
    public async Task<IActionResult> GetFiles(Guid bucket)
    {
        var metas = await _dbContext.Metas.Where(v => v.Bucket.Id == bucket).ToListAsync();
        return Ok(new ApiResponse<List<FileMeta>>()
        {
            Success = true,
            Data = metas
        });
    }

    [HttpGet("download/{fileId}")]
    public async Task DownloadFile(Guid fileId)
    {
        var query =
            from meta in _dbContext.Metas
            where meta.Id == fileId
            select meta.Bucket;

        var bucket = await query.FirstOrDefaultAsync();
        if (bucket == null)
        {
            Response.StatusCode = 404;
            return;
        }
        
        var driver = _bucketService.GetDriver(bucket.Id);
        if (driver == null)
        {
            // how the fuck
            Response.StatusCode = 500;
            return;
        }
        
        await driver.DownloadAsync(fileId.ToString(), Response.Body);
    }
}