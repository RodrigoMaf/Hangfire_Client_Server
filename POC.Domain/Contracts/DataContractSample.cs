using System;

namespace POC.Domain.Contracts
{
    /// <summary>Dados de contrato pensando em uma pessoa</summary>
    public class DataContractSample
    {
        /// <summary>Nome</summary>
        public string Nome { get; set; }

        /// <summary>Data</summary>
        public DateTime Nascimento { get; set; }
    }
}
