using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample_Redis.Data;
using Sample_Redis.Models;
using Sample_Redis.Services;

namespace Sample_Redis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ICacheService cacheService;
        private readonly AppDbContext context;

        public DriverController(
            ICacheService cacheService,
            AppDbContext context)
        {
            this.cacheService = cacheService;
            this.context = context;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cacheData = await cacheService.GetAsync(
                "drivers",
                async () =>
                {
                    return await context.Drivers.ToListAsync();
                });

            return Ok(cacheData);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var cacheData = await cacheService.GetAsync(
                $"driver{id}",
                async () =>
                {
                    return await context.Drivers
                        .FirstOrDefaultAsync(x => x.Id == id);
                });

            return Ok(cacheData);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Driver driver)
        {
            var addedObj = await context.Drivers.AddAsync(driver);

            await cacheService.SetAsync<Driver>($"driver{addedObj.Entity.Id}", addedObj.Entity);
            await cacheService.RemoveByPrefixAsync("drivers");

            await context.SaveChangesAsync();

            return Ok(addedObj.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var exist = await context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

            if (exist == null)
                return NotFound();

            context.Remove(exist);

            await cacheService.RemoveAsync($"driver{id}");
            await cacheService.RemoveByPrefixAsync("drivers");

            await context.SaveChangesAsync();

            return NoContent();

        }
    }
}
