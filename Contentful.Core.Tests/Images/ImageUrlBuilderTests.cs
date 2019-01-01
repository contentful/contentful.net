using Contentful.Core.Images;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Contentful.Core.Tests.Images
{
    public class ImageUrlBuilderTests
    {
        [Fact]
        public void NewUnusedImageBuilderShouldReturnEmptyString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AddingMultipleQueryParametersShouldCorrectlyAddQueryStringValues()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetHeight(23).SetWidth(34).Build();
            //Assert
            Assert.Equal("?h=23&w=34", result);
        }

        [Theory]
        [InlineData(ImageFormat.Jpg, "?fm=jpg")]
        [InlineData(ImageFormat.Png, "?fm=png")]
        [InlineData(ImageFormat.Webp, "?fm=webp")]
        [InlineData(ImageFormat.Default, "")]
        public void AddingImageFormatShouldSetCorrectQueryString(ImageFormat format, string expected)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetFormat(format).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(24)]
        [InlineData(45)]
        [InlineData(888)]
        public void SettingJpgQualityShouldSetCorrectQueryString(int quality)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetJpgQuality(quality).Build();
            //Assert
            Assert.Equal($"?q={quality}", result);
        }

        [Fact]
        public void Setting8BitPngShouldSetCorrectQueryString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.Set8BitPng().Build();
            //Assert
            Assert.Equal($"?fm=png&fl=png8", result);
        }

        [Fact]
        public void Setting8BitPngAndFormatShouldSetCorrectQueryString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetFormat(ImageFormat.Png).Set8BitPng().Build();
            //Assert
            Assert.Equal($"?fm=png&fl=png8", result);
        }

        [Fact]
        public void Setting8BitPngAndFormatInDifferentOrderShouldSetCorrectQueryString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.Set8BitPng().SetFormat(ImageFormat.Png).Build();
            //Assert
            Assert.Equal($"?fm=png&fl=png8", result);
        }

        [Fact]
        public void Setting8BitPngWithIncorrectFormatShouldThrow()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var ex = Assert.Throws<ArgumentException>(() => builder.SetFormat(ImageFormat.Jpg).Set8BitPng().Build());
            //Assert
            Assert.Equal("The format must be set to png when using the 8 bit png color depth.", ex.Message);
        }

        [Fact]
        public void SettingProgressiveJpgShouldSetCorrectQueryString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.UseProgressiveJpg().Build();
            //Assert
            Assert.Equal($"?fl=progressive", result);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(546)]
        [InlineData(94)]
        public void SettingWidthShouldSetCorrectQueryString(int width)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetWidth(width).Build();
            //Assert
            Assert.Equal($"?w={width}", result);
        }

        [Theory]
        [InlineData(734)]
        [InlineData(7848)]
        [InlineData(3267)]
        public void SettingHeightShouldSetCorrectQueryString(int height)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetHeight(height).Build();
            //Assert
            Assert.Equal($"?h={height}", result);
        }

        [Theory]
        [InlineData(ImageResizeBehaviour.Crop, "?fit=crop")]
        [InlineData(ImageResizeBehaviour.Fill, "?fit=fill")]
        [InlineData(ImageResizeBehaviour.Thumb, "?fit=thumb")]
        [InlineData(ImageResizeBehaviour.Pad, "?fit=pad")]
        [InlineData(ImageResizeBehaviour.Scale, "?fit=scale")]
        [InlineData(ImageResizeBehaviour.Default, "")]
        public void AddingResizeBehaviourShouldAddCorrectQueryString(ImageResizeBehaviour behaviour, string expected)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetResizingBehaviour(behaviour).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddingResizeBehaviourDefaulShouldNotAddAnyQueryString()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetResizingBehaviour(ImageResizeBehaviour.Default).Build();
            //Assert
            Assert.Equal(string.Empty, result);
        }

        [Theory]
        [InlineData(ImageFocusArea.Bottom, "?f=bottom")]
        [InlineData(ImageFocusArea.Bottom_Left, "?f=bottom_left")]
        [InlineData(ImageFocusArea.Face, "?f=face")]
        [InlineData(ImageFocusArea.Faces, "?f=faces")]
        [InlineData(ImageFocusArea.Top, "?f=top")]
        [InlineData(ImageFocusArea.Default, "")]
        public void AddingFocusAreaShouldSetCorrectQueryString(ImageFocusArea focusArea, string expected)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetFocusArea(focusArea).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(234)]
        [InlineData(763)]
        [InlineData(894)]
        public void SettingCornerRadiusShouldSetCorrectQueryString(int radius)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetCornerRadius(radius).Build();
            //Assert
            Assert.Equal($"?r={radius}", result);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("#333", "?bg=rgb:333")]
        [InlineData("#cc0000", "?bg=rgb:cc0000")]
        [InlineData("rgb:ccc", "?bg=rgb:ccc")]
        [InlineData("rgb:ff0000", "?bg=rgb:ff0000")]
        public void SettingBackgroundShouldAddCorrectQueryString(string bg, string expected)
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetBackgroundColor(bg).Build();
            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SettingFormatToPngShouldNotAllowForJpgSpecificValues()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetFormat(ImageFormat.Png).SetJpgQuality(34).UseProgressiveJpg().Build();
            //Assert
            Assert.Equal("?fm=png", result);
        }

        [Fact]
        public void SettingFormatToJpgShouldAllowForJpgSpecificValues()
        {
            //Arrange
            var builder = new ImageUrlBuilder();
            //Act
            var result = builder.SetFormat(ImageFormat.Jpg).SetJpgQuality(34).UseProgressiveJpg().Build();
            //Assert
            Assert.Equal("?fm=jpg&q=34&fl=progressive", result);
        }
    }
}
