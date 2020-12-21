using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POC.Domain.Contracts;
using POC.Domain.Infra;

namespace POC.Infra
{
    public class FeatureInfra : IFeatureInfra
    {

        /// <summary>Feature de log no sistema</summary>
        public ILogger<FeatureInfra> Logger { get; set; }


        public FeatureInfra(ILogger<FeatureInfra> logger)
        {
            Logger = logger;
        }

        public Task Execute(DataContractSample value)
        {
            Logger.LogInformation($"Processamento da infra com os dados {JsonConvert.SerializeObject(value)}''");
            return Task.CompletedTask;
        }
    }
}
