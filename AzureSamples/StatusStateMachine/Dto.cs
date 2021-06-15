namespace AzureSamples.StatusStateMachine
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class Dto
    {
        public string Email { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Trigger Trigger { get; set; }

    }
}