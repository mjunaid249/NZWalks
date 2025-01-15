using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repositories
{
    public class SQLRegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLRegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Region> CreateAsync(Region region)
        {
            await dbContext.Regions.AddAsync(region);

            await dbContext.SaveChangesAsync();
            return region;
        }

        public async Task<Region?> DeleteAsync(Guid id)
        {
            var regionExist = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionExist == null)
            {

                return null;
            }

            dbContext.Regions.Remove(regionExist);
            await dbContext.SaveChangesAsync();

            return regionExist;
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await dbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid id)
        {
            return await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Region?> UpdateAsync(Guid id, Region region)
        {
            var regionExist = await dbContext.Regions.FirstOrDefaultAsync(x => x.Id == id);

            if (regionExist == null)
            {
                return null;
            }

            regionExist.Name = region.Name;
            regionExist.Code = region.Code;
            regionExist.RegionImageUrl = region.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            return regionExist;
        }
    }
}
