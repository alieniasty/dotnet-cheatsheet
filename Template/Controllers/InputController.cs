namespace MassTransitSample.Template.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using DomainModels;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using QueryResults;
    using Serilog;
    using Services;
    using ViewModels;

    [ApiController]
    [Route("[controller]")]
    public class InputController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly ITestService _service;

        public InputController(ILogger logger, IMapper mapper, ITestService service)
        {
            _logger = logger;
            _mapper = mapper;
            _service = service;
        }

        // Please notice, in this particular case, WE COULD create commands, events and queries right away in the Controller's action but in most real-life cases, services
        // below the controller will have something more to do than just invoking send/publish/query. That's why we stick with viewmodel and model separation.

        [HttpPost]
        [Route("ImportantProcessing")]
        public async Task<IActionResult> ImportantProcessing(ImportantProcessingViewModel vm)
        {
            var model = _mapper.Map<ImportantProcessingModel>(vm);
            await _service.DoStuffAndPublish(model);

            // Ok thanks, I will take it from here. It's a fully async process so everything a client needs to know is that its' request is being processed.
            return Accepted();
        }

        [HttpPost]
        [Route("CommandSomething")]
        public async Task<IActionResult> CommandSomething(CommandSomethingViewModel vm)
        {
            var model = _mapper.Map<CommandSomethingModel>(vm);
            await _service.DoStuffAndSend(model);

            // Ok thanks, I will take it from here. It's a fully async process so everything a client needs to know is that its' request is being processed.
            return Accepted();
        }

        [HttpGet]
        [Route("GetSomething")]
        public async Task<IActionResult> GetSomething([FromRoute] string id)
        {
            Response<GetSomethingQueryResult> result = await _service.DoStuffAndQuery(id);

            // This one is actually returning a 'classic' result.
            return Ok(result.Message);
        }
    }
}
