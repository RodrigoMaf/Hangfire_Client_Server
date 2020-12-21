using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace POC.Infra.Hangfire
{
    /// <summary>Classe de eventos do hangfire</summary>
    public class HangFireLogStepsAttribute : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        /// <summary>Logger da classe</summary>
        private ILogger<HangFireLogStepsAttribute> Logger { get; }

        /// <summary>Inicia uma nova instância da classe <see cref="HangFireLogStepsAttribute" />.</summary>
        /// <param name="logger">Logger da classe</param>
        public HangFireLogStepsAttribute(ILogger<HangFireLogStepsAttribute> logger)
        {
            Logger = logger;
        }

        /// <summary>Evento antes de registrar uma ação(client)</summary>
        /// <param name="context">Contexto do evento</param>
        public void OnCreating(CreatingContext context)
        {
            Logger.LogInformation($"Creating a job based on method `{context.Job.Method.Name}`...");
        }

        /// <summary>Evento após o registro de uma ação(client)</summary>
        /// <param name="context">Contexto do evento</param>
        public void OnCreated(CreatedContext context)
        {
            Logger.LogInformation($"Job that is based on method `{context.Job.Method.Name}` has been created with id `{context.BackgroundJob?.Id}`");
        }

        /// <summary>Evento quando o servidor inicia a execução de uma ação(server)</summary>
        /// <param name="context">Contexto do evento</param>
        public void OnPerforming(PerformingContext context)
        {
            Logger.LogInformation($"Starting to perform job `{context.BackgroundJob.Id}`");
        }

        /// <summary>Evento quando o servidor termina a execução de uma ação(server)</summary>
        /// <param name="context">Contexto do evento</param>
        public void OnPerformed(PerformedContext context)
        {
            Logger.LogInformation($"Job `{context.BackgroundJob.Id}` has been performed");
        }

        /// <summary>
        /// Chamado quando um status da ação está sendo setado.
        /// Esta mudança de estado pode ser interceptada e o estado final pode ser alterado
        /// através da definição do estado diferente no contexto em uma implementação deste método.
        /// </summary>
        /// <param name="context">Contexto do evento</param>
        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                var innerException = failedState.Exception.InnerException;
                Logger.LogWarning($"Job `{context.BackgroundJob.Id}` has been failed due to an exception `{JsonConvert.SerializeObject(innerException)}`");
                failedState.Reason = innerException.Message;
            }
        }

        /// <summary>Evento chamado quando foi aplicado um estado da transação relacionada a ação</summary>
        /// <param name="context">Contexto do evento</param>
        /// <param name="transaction">Objeto de transação</param>
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        { 
            Logger.LogInformation($"Job `{context.BackgroundJob.Id}` state was changed from `{context.OldStateName}` to `{context.NewState.Name}`");
        }

        /// <summary>Evento quando um estado não foi aplicado a uma ação</summary>
        /// <param name="context">Contexto do evento</param>
        /// <param name="transaction">Objeto de transação</param>
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.LogInformation($"Job `{context.BackgroundJob.Id}` state `{context.OldStateName}` was unapplied.");
        }
    }
}
