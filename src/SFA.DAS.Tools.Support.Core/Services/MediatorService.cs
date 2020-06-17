using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Core.Services
{
    public interface IMediatorService
    {
        Task Publish<T>(T message);
        Task Send<T>(T message);
    }

    public class MediatorService : IMediatorService
    {
        private readonly IMediator _mediator;

        public MediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish<T>(T message)
        {
            await _mediator.Publish(message);
        }

        public async Task Send<T>(T message)
        {
            await _mediator.Send(message);
        }

    }

}
