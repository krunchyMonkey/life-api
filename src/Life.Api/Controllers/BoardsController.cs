using Life.Application.Board.Commands;
using Life.Application.Board.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Life.Api.Controllers
{
    [ApiController]
    [Route("api/v1/boards")]
    public sealed class BoardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BoardsController(IMediator mediator) => _mediator = mediator;

        [HttpPost("upload")]
        public Task<string> Upload([FromBody] UploadRequest request) => _mediator.Send(new UploadBoard(request));

        [HttpPost("next")]
        public Task<BoardDto> Next([FromBody] NextRequest request) => _mediator.Send(new NextState(request));

        [HttpPost("n-ahead")]
        public Task<BoardDto> NAhead([FromBody] NAheadRequest request) => _mediator.Send(new NAhead(request));

        [HttpPost("final")]
        public Task<FinalDto> Final([FromBody] FinalRequest request) => _mediator.Send(new FinalState(request));
    }
}
