using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Producer;
using Application.Producer.Params;
using Application.Producer.Resources;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/producers")]
    public class ProducerController : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync([FromQuery] ProducerParams queryParams)
        {
            var producers = await Mediator.Send(new GetAllProducers.Query{ QueryParams = queryParams });
            return HandlePaginationHeader(producers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProducerResource>> GetAsync(Guid id)
        {
            return await Mediator.Send(new GetProducer.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateProducer.Command data)
        {
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateProducer.Command data)
        {
            data.SetId(id);
            await Mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await Mediator.Send(new DeleteProducer.Command { Id = id });
            return NoContent();
        }
    }
}