using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkAsync()
        {
            //Fetch data from database
            var WalkDomain = await _walkRepository.GetAllAsync();

            //Convert domain Walks to DTO Walks
            var WalkDTO = _mapper.Map<List<Models.DTO.Walk>>(WalkDomain);

            //Return Response
            return Ok(WalkDTO);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            //Get Walk Domian object from database
            var WalkDomain = await _walkRepository.GetAsync(id);

            //Convert Domain object to DTO
            var WalkDTO = _mapper.Map<Models.DTO.Walk>(WalkDomain);

            //Return response
            return Ok(WalkDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync(Models.DTO.AddWalkRequest addWalkRequest)
        {
            //TO convert DTO to Domain Object
            var walkDomain = new Models.Domain.Walk
            {
                Length = addWalkRequest.Length,
                Name = addWalkRequest.Name,
                RegionId = addWalkRequest.RegionId,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId
            };

            //Pass Domain Object to Repository to persist this
            walkDomain = await _walkRepository.AddAsync(walkDomain);

            //Convert the Domain object back to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id = walkDomain.Id,
                Length = walkDomain.Length,
                Name = walkDomain.Name,
                RegionId = walkDomain.RegionId,
                WalkDifficultyId = walkDomain.WalkDifficultyId
            };

            //Return Response.
            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDTO.Id }, walkDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync([FromRoute] Guid id, [FromBody] Models.DTO.UpdateWalkRequest updateWalkRequest)
        {
            //Convert DTO to Domain object
            var WalkDomain = new Models.Domain.Walk
            {
                Length = updateWalkRequest.Length,
                Name = updateWalkRequest.Name,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId
            };
            //Pass details to Repository - Get Domain object in response or null

            WalkDomain = await _walkRepository.UpdateAsync(id, WalkDomain);
            
            //Handle Null(Not found)
            if (WalkDomain == null)
            {
                return NotFound();
            }

            //Convert back Domain to DTO
            var walkDTO = new Models.DTO.Walk
            {
                Id = WalkDomain.Id,
                Length = WalkDomain.Length,
                Name = WalkDomain.Name,
                RegionId = WalkDomain.RegionId,
                WalkDifficultyId = WalkDomain.WalkDifficultyId
            };

            //Return Response.
            return Ok(walkDTO);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteWalkAsync(Guid id)
        {
            //call repository to delete walk
            var walkDomain =await _walkRepository.DeleteAsync(id);

            if(walkDomain == null)
            {
                return NotFound();
            }

            var walkDTO = _mapper.Map<Models.DTO.Walk>(walkDomain);

            return Ok(walkDTO);
        }
    }
}
