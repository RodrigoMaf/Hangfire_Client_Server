using System.Threading.Tasks;
using POC.Domain.Application;
using POC.Domain.Contracts;
using POC.Domain.Infra;

namespace POC.Application
{
    public class FeatureApplication : IFeatureApplication
    {
        #region Properties

        /// <summary>Feature de infra</summary>
        public IFeatureInfra FeatureInfra { get; set; }
        
        #endregion

        public FeatureApplication(IFeatureInfra featureInfra)
        {
            FeatureInfra = featureInfra;
        }


        public async Task Execute(DataContractSample dataContract)
        {
            await FeatureInfra.Execute(dataContract);
        }
    }
}
