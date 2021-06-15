namespace TestingSamples.CapturingPassedParamsToMockedObjects
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class SampleTest2
    {
        [Fact]
        public void TestMethod()
        {
            //Arrange
            var systemUnderTestMock = new Mock<ISystemUnderTest>();

            var dtoToVerify = new List<TestObject1>();

            systemUnderTestMock.Setup(d => d.CompareObjects(Capture.In(dtoToVerify), It.IsAny<TestObject2>()));

            //Act
            var testObject1 = new TestObject1{Country = "SE"};
            var testObject2 = new TestObject2 { Country = "PL" };

            systemUnderTestMock.Object.CompareObjects(testObject1, testObject2);

            dtoToVerify.First().Should().BeEquivalentTo(testObject1);
        }
    }

    #region FakeLogic
    public class SystemUnderTest : ISystemUnderTest
    {
        public bool CompareObjects(TestObject1 testObject1, TestObject2 testObject2)
        {
            return
                testObject1.FirstName.Trim() != testObject2.FirstName.Trim() ||
                testObject1.LastName.Trim() != testObject2.LastName.Trim() ||
                testObject1.Country.Trim().ToLowerInvariant() != testObject2.Country.Trim().ToLowerInvariant() ||
                testObject1.State?.Trim().ToLowerInvariant() != testObject2.State?.Trim().ToLowerInvariant();
        }
    }

    public interface ISystemUnderTest
    {
        bool CompareObjects(TestObject1 testObject1, TestObject2 testObject2);
    }

    public class TestObject1
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }

    public class TestObject2
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
    }

    #endregion

}
