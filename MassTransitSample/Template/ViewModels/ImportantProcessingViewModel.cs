namespace MassTransitSample.Template.ViewModels
{
    public class ImportantProcessingViewModel
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public SomeObjectVm SomeObject { get; set; }
    }

    public class SomeObjectVm
    {
        public string TestField { get; set; }
    }
}
