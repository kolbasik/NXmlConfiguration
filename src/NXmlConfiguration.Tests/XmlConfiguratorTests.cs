using System;
using System.Linq;
using Xunit;

namespace kolbasik.NXmlConfiguration.Tests
{
    public sealed class XmlConfiguratorTests
    {
        public sealed class CreateInstance
        {
            [Fact]
            public void Should_create_an_instance_using_value_from_type_attribute()
            {
                // arrange
                var xml = $"<object type='{typeof(TestObject).AssemblyQualifiedName}' />";
                var xmlElement = XmlConfiguration.ToXmlElement(xml);

                // act
                var actual = XmlConfigurator.CreateInstance(xmlElement, Activator.CreateInstance);

                // assert
                Assert.NotNull(actual);
                Assert.IsType<TestObject>(actual);
            }

            public sealed class TestObject
            {
            }
        }

        public sealed class SetProperties
        {
            [Fact]
            public void Should_set_scalar_property_using_value_from_attribute()
            {
                // arrange
                var expected = Guid.NewGuid();
                var instance = new TestObject();
                var xml = $"<object unique='{expected.ToString("D")}' />";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Unique;

                // assert
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Should_set_scalar_property_using_value_from_child_node()
            {
                // arrange
                var expected = Guid.NewGuid();
                var instance = new TestObject();
                var xml = $"<object><unique value='{expected.ToString("D")}' /></object>";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Unique;

                // assert
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Should_not_set_any_value_to_scalar_property_if_not_defined_in_config()
            {
                // arrange
                var expected = Guid.NewGuid();
                var instance = new TestObject { Unique = expected };
                var xml = "<object />";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Unique;

                // assert
                Assert.Equal(expected, actual);
            }

            [Theory]
            [InlineData(",")]
            [InlineData(";")]
            [InlineData(" ")]
            public void Should_set_array_property_using_value_from_attribute(string separator)
            {
                // arrange
                var expected = Enumerable.Range(0, 3).Select(x => Guid.NewGuid()).ToArray();
                var instance = new TestObject();
                var xml = $"<object uniques='{string.Join(separator, expected)}' />";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Uniques;

                // assert
                Assert.Equal(expected, actual);
            }

            [InlineData(",")]
            [InlineData(";")]
            [InlineData(" ")]
            public void Should_set_array_property_using_value_attribute_from_child_node(string separator)
            {
                // arrange
                var expected = Enumerable.Range(0, 3).Select(x => Guid.NewGuid()).ToArray();
                var instance = new TestObject();
                var xml = $"<object><uniques value='{string.Join(separator, expected)}' /></object>";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Uniques;

                // assert
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Should_set_array_property_using_values_from_child_nodes()
            {
                // arrange
                var expected = Enumerable.Range(0, 3).Select(x => Guid.NewGuid()).ToArray();
                var instance = new TestObject();
                var xml = $"<object><uniques>{string.Concat(expected.Select(x => $"<item value='{x}' />"))}</uniques></object>";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Uniques;

                // assert
                Assert.Equal(expected, actual);
            }

            [Fact]
            public void Should_not_set_any_values_to_array_property_if_not_defined_in_config()
            {
                // arrange
                var expected = Enumerable.Range(0, 3).Select(x => Guid.NewGuid()).ToArray();
                var instance = new TestObject { Uniques = expected };
                var xml = "<object />";

                // act
                XmlConfigurator.SetProperties(XmlConfiguration.ToXmlElement(xml), instance);
                var actual = instance.Uniques;

                // assert
                Assert.Equal(expected, actual);
            }

            public sealed class TestObject
            {
                public Guid Unique { get; set; }
                public Guid[] Uniques { get; set; }
            }
        }
    }
}