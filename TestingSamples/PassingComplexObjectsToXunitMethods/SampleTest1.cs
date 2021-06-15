using AutoFixture;
using System.Collections.Generic;
using Xunit;

namespace TestingSamples.PassingComplexObjectsToXunitMethods
{
    using FluentAssertions;

    public class SampleTest1
    {
        private readonly Fixture _fixture;

        public SampleTest1()
        {
            _fixture = new Fixture();
        }

        public static IEnumerable<object[]> FixtureData =>
            new List<object[]>
            {
                new object[]
                {
                    new Fixture().Build<TestObject1>()
                        .With(dto => dto.FirstName, "Firstname")
                        .With(dto => dto.LastName, "Lastname")
                        .With(dto => dto.Country, "Poland")
                        .With(dto => dto.State, "Kujpom")
                        .Create(),

                    new Fixture().Build<TestObject2>()
                        .With(dto => dto.FirstName, "Firstname")
                        .With(dto => dto.LastName, "Lastname")
                        .With(dto => dto.Country, "Poland")
                        .With(dto => dto.State, "Kujpom")
                        .Create(),

                    false
                },

                new object[]
                {
                    new Fixture().Create<TestObject1>(),
                    new Fixture().Create<TestObject2>(),
                    true
                }
            };

        [Theory]
        [MemberData(nameof(FixtureData))]
        public void When_TwoObjects_Are_Equal_Should_Return_Bool(TestObject1 testObject1, TestObject2 testObject2, bool expectedResponse)
        {
            //Arrange

            var systemUnderTest = new SystemUnderTest();

            //Act
            var result = systemUnderTest.CompareObjects(testObject1, testObject2);

            //Assert
            result.Should().Be(expectedResponse);
        }
    }

    #region FakeLogic

    public class SystemUnderTest
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
