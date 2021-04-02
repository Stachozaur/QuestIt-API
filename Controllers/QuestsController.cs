using AutoMapper;
using Job.it_classes.Data.DTO;
using Job.it_classes.Data.Entities;
using Job.it_ClassLib.Data.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.it_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public QuestsController(IQuestRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<QuestDTO[]>> Get()
        {
            try
            {
                var results = await _repository.GetAllQuestsAsync();

                return _mapper.Map<QuestDTO[]>(results);

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<QuestDTO>> Get(int id)
        {
            try
            {
                var result = await _repository.GetQuestAsync(id);

                if (result == null) return NotFound("There is not Quest with such ID");

                return _mapper.Map<QuestDTO>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("category/{categoryId:int}")]
        public async Task<ActionResult<QuestDTO[]>> SearchByCategory(int categoryId)
        {
            try
            {
                var results = await _repository.GetQuestsByCategoryAsync(categoryId);

                if (results == null) return NotFound("There is not Quest with such ID");

                return Ok(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
        [HttpPost]
        public async Task<ActionResult<QuestDTO>> Post(QuestDTO model)
        {
            try
            {
                var existing = await _repository.GetQuestAsync(model.QuestId);
                if (existing != null)
                {
                    return BadRequest("ID in use");
                }

                var location = _linkGenerator.GetPathByAction("Get",
                    "Quests",
                    new { id = model.QuestId });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                var quest = _mapper.Map<Quest>(model);
                _repository.Add(quest);
                if (await _repository.SaveChangesAsync())
                {
                    return Created($"/api/camps/{quest.QuestId}", _mapper.Map<QuestDTO>(quest));
                }

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<QuestDTO>> Put(int id, QuestDTO model)
        {
            try
            {
                var toChange = await _repository.GetQuestAsync(id);
                if (toChange== null) return NotFound($"Could not find quest with id of {id}");

                _mapper.Map(model, toChange);

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<QuestDTO>(toChange);
                }
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var toDelete = await _repository.GetQuestAsync(id);
                if (toDelete == null) return NotFound();

                _repository.Delete(toDelete);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest("failed to delete the quest");
        }
    }


}

