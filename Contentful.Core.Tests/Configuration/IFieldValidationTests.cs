using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests.Configuration
{
    public class IFieldValidationTests
    {
        [Fact]
        public void DeserializingCustomValidatorShouldReturnCorrectType()
        {
            //Arrange

            //Act
            var customValidator = JsonConvert.DeserializeObject<CustomValidator>("{}");
            //Assert
            Assert.IsType<CustomValidator>(customValidator);
        }

        [Fact]
        public void DeserializingCustomValidatorPropertyShouldReturnCorrectType()
        {
            //Arrange

            //Act
            var customClass = JsonConvert.DeserializeObject<CustomClassWithValidator>("{ \"Validator\": {}}");
            //Assert
            Assert.IsType<CustomValidator>(customClass.Validator);

        }

    }

    public class CustomClassWithValidator
    {
        public CustomValidator Validator { get; set; }
    }

    public class CustomValidator : IFieldValidator
    {
        public string Thingy => "something";

        public object CreateValidator()
        {
            return new { Test = "test-string" };
        }
    }
}
