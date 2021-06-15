namespace AzureSamples.StatusStateMachine
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Stateless;

    public static class StatusStateMachine
    {
        [FunctionName("StatusStateMachine")]
        public static async Task RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "StateMachine")] HttpRequest req, ILogger log)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"C# HTTP trigger function processed: {requestBody}");

            var dto = JsonConvert.DeserializeObject<Dto>(requestBody);

            var currentState = (State)Enum.Parse(typeof(State), new DbClient().GetStateOfDto(dto.Email));
            var stateMachine = new StateMachine<State, Trigger>(currentState);

            var toActiveTrigger = stateMachine.SetTriggerParameters<string, string>(Trigger.Approve);
            var toPendingTrigger = stateMachine.SetTriggerParameters<string, string>(Trigger.ToPending);
            var toRejectTrigger = stateMachine.SetTriggerParameters<string, string>(Trigger.Reject);

            ConfigureStateMachineWithProperTriggers(stateMachine, toActiveTrigger, toRejectTrigger, toPendingTrigger);

            switch (dto.Trigger)
            {
                case Trigger.Reject:
                    await stateMachine.FireAsync(toRejectTrigger, dto.Email, State.Rejected.ToString());
                    break;
                case Trigger.Approve:
                    await stateMachine.FireAsync(toActiveTrigger, dto.Email, State.Active.ToString());
                    break;
                case Trigger.ToPending:
                    await stateMachine.FireAsync(toPendingTrigger, dto.Email, State.Pending.ToString());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ConfigureStateMachineWithProperTriggers(
            StateMachine<State, Trigger> stateMachine,
            StateMachine<State, Trigger>.TriggerWithParameters<string, string> toActiveTrigger,
            StateMachine<State, Trigger>.TriggerWithParameters<string, string> toRejectTrigger,
            StateMachine<State, Trigger>.TriggerWithParameters<string, string> toPendingTrigger)
        {
            stateMachine.Configure(State.New)
                .OnEntryFrom(toPendingTrigger, ChangeStatusInDatabase);

            stateMachine.Configure(State.Pending)
                .OnEntryFrom(toActiveTrigger, ChangeStatusInDatabase)
                .OnEntryFrom(toRejectTrigger, ChangeStatusInDatabase);

            stateMachine.Configure(State.Active)
                .OnEntryFrom(toRejectTrigger, ChangeStatusInDatabase);

            stateMachine.Configure(State.Rejected)
                .OnEntryFrom(toActiveTrigger, ChangeStatusInDatabase);
        }

        private static void ChangeStatusInDatabase(string email, string newState)
        {
            new DbClient().ChangeStatusInDatabase(email, newState);
        }
    }
}

