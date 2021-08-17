using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Producer;
using Application.Producer.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/producers")]
    public class ProducerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProducerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<ProducerResource>>> GetAllAsync()
        {
            return await _mediator.Send(new GetAllProducers.Query());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProducerResource>> GetAsync(Guid id)
        {
            return await _mediator.Send(new GetProducer.Query{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateProducer.Command data)
        {
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(Guid id, UpdateProducer.Command data)
        {
            data.SetId(id);
            await _mediator.Send(data);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _mediator.Send(new DeleteProducer.Command { Id = id });
            return NoContent();
        }
    }
}