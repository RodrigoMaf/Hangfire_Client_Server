using System.Threading.Tasks;
using POC.Domain.Contracts;

namespace POC.Domain.Infra
{
    /// <summary>Serviço de infraestrutura de exemplo 1</summary>
    public interface IFeatureInfra
    {
        public Task Execute(DataContractSample value);
    }
}
