namespace MassTransitSample.Template.ViewModels
{
    public class CommandSomethingViewModel
    {
        public string StringField { get; set; }
        public int IntField { get; set; }
        public SomeCommandObjectViewModel SomeObject { get; set; }
    }

    public class SomeCommandObjectViewModel
    {
        public string TestField { get; set; }
    }
}
