
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegionController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
        }

        //Get Request   
        [HttpGet]
        [Authorize(Roles = "Reader,Writer")]
        public async Task<IActionResult> GetAll()
        {
            //REM: We never return the Domain model to client, We only return DTOs to Client

            //Get Data from Database (Domain Model)
            var regionsDomain = await regionRepository.GetAllAsync();

            //Map Domain Model to DTO
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);


            //Return DTOs
            return Ok(regionsDto);
        }

        //Get One Request
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader,Writer")]

        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            var regionDomain = await regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            //Map

            var regionDto = mapper.Map<RegionDto>(regionDomain);


            //Return DTO back to client
            return Ok(regionDto);
        }

        //Post Request
        [HttpPost]
        [Authorize(Roles = "Writer")]

        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            if (ModelState.IsValid)
            {

                //Mapping DTO from body to Domain Model 
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                //Adding Domain model in data base

                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

                //Creating a DTO to return it to client 
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);

            }
            else
            {
                return BadRequest(ModelState);
            }

        }

        //Put Request

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]

        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            if (ModelState.IsValid)
            {
                var RegionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                RegionDomainModel = await regionRepository.UpdateAsync(id, RegionDomainModel);

                if (RegionDomainModel == null)
                {
                    return NotFound();
                }

                RegionDomainModel.Name = updateRegionRequestDto.Name;
                RegionDomainModel.Code = updateRegionRequestDto.Code;
                RegionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

                await dbContext.SaveChangesAsync();

                var RegionsDto = mapper.Map<RegionDto>(RegionDomainModel);

                return Ok(RegionsDto);
            }
            else { return BadRequest(ModelState); }
        }

        //Delete Request

        [HttpDelete]

        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if (regionDomainModel == null)
            {

                return NotFound();
            }

            var RegionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(RegionDto);
        }
    }
}
