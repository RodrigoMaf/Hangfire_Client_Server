using System.Threading.Tasks;
using POC.Domain.Contracts;

namespace POC.Domain.Application
{
    public interface IFeatureApplication
    {
        Task Execute(DataContractSample value);
    }
}
