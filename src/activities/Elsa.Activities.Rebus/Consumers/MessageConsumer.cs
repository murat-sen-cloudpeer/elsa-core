using System.Threading.Tasks;
using Elsa.Activities.Rebus.Bookmarks;
using Elsa.Dispatch;
using Elsa.Services;
using MediatR;
using Rebus.Extensions;
using Rebus.Handlers;
using Rebus.Messages;
using Rebus.Pipeline;

namespace Elsa.Activities.Rebus.Consumers
{
    public class MessageConsumer<T> : IHandleMessages<T> where T : notnull
    {
        // TODO: Design multi-tenancy. 
        private const string? TenantId = default;

        private readonly IWorkflowLaunchpad _workflowLaunchpad;

        public MessageConsumer(IWorkflowLaunchpad workflowLaunchpad)
        {
            _workflowLaunchpad = workflowLaunchpad;
        }

        public async Task Handle(T message)
        {
            var correlationId = MessageContext.Current.TransportMessage.Headers.GetValueOrNull(Headers.CorrelationId);
            await _workflowLaunchpad.CollectAndExecuteWorkflowsAsync(new CollectWorkflowsContext(
                nameof(RebusMessageReceived),
                new MessageReceivedBookmark { MessageType = message.GetType().Name, CorrelationId = correlationId },
                new MessageReceivedBookmark { MessageType = message.GetType().Name },
                correlationId,
                default,
                TenantId));
        }
    }
}