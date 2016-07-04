using embeddeddata.logic.configuration;

using NUnit.Framework;

namespace embeddeddata.logic.tests.configuration
{
    [TestFixture]
    public sealed class ConfigurationReaderTests
    {
        [Test]
        public void ReadEmptyJson()
        {
            const string Json = "{}";

            var sut = new ConfigurationReader();

            var result = sut.Read(Json);

            Assert.That(result.UseResharperAnnotations.HasValue, Is.False);
        }

        [Test]
        public void ReadConfiguredProperty()
        {
            const string Json = "{" +
                                "\"UseResharperAnnotations\": true" +
                                "}";

            var sut = new ConfigurationReader();

            var result = sut.Read(Json);

            Assert.That(result.UseResharperAnnotations.HasValue, Is.True);
            Assert.That(result.UseResharperAnnotations.Value, Is.True);
        }
    }
}
