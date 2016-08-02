using System.Threading.Tasks;
using PokeGoBot.Core.Logging;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Networking.Envelopes;

namespace PokeGoBot.Core.Logic.Handlers
{
    public class ApiStrategyHandler : IApiFailureStrategy
    {
        private const int RetriesCount = 5;
        private readonly ILogger _logger;
        private RequestEnvelope _lastRequestEnvelope;

        private int _retrieAttempts;

        public ApiStrategyHandler(ILogger logger)
        {
            _logger = logger;
        }


        public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            var operation = ApiOperation.Abort;

            if (_lastRequestEnvelope != null && !_lastRequestEnvelope.Equals(request))
                _retrieAttempts = 0;

            _logger.Write("Api failure", LogLevel.DEBUG);
            _retrieAttempts++;

            if (_retrieAttempts <= RetriesCount)
            {
                _logger.Write($"Retrying again in 1 second. Attempt number:{_retrieAttempts}", LogLevel.DEBUG);
                await Task.Delay(1000);
                operation = ApiOperation.Retry;
            }

            _lastRequestEnvelope = request;
            return operation;
        }

        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            //TODO: Maybe we can catch some information here, but i dont think we should do much here
        }
    }
}
