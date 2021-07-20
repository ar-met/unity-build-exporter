using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityExporter.Tests
{
    internal class SemanticVersionTests
    {
        [Test]
        public void TryParse_01()
        {
            Assert.True(SemanticVersion.TryParse("1.2.3", out SemanticVersion semanticVersion));
            Assert.AreEqual(1, semanticVersion.major);
            Assert.AreEqual(2, semanticVersion.minor);
            Assert.AreEqual(3, semanticVersion.patch);
        }

        [Test]
        public void TryParse_02()
        {
            Assert.True(
                SemanticVersion.TryParse(
                    $"{uint.MaxValue}.{uint.MaxValue}.{uint.MaxValue}",
                    out SemanticVersion semanticVersion));

            Assert.AreEqual(uint.MaxValue, semanticVersion.major);
            Assert.AreEqual(uint.MaxValue, semanticVersion.minor);
            Assert.AreEqual(uint.MaxValue, semanticVersion.patch);
        }

        [Test]
        public void TryParse_03()
        {
            Assert.True(SemanticVersion.TryParse("0.0.0", out SemanticVersion semanticVersion));
            Assert.AreEqual(0, semanticVersion.major);
            Assert.AreEqual(0, semanticVersion.minor);
            Assert.AreEqual(0, semanticVersion.patch);
        }

        [Test]
        public void TryParse_04()
        {
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            Assert.False(SemanticVersion.TryParse("0.0.-1", out _));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void TryParse_05()
        {
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            Assert.False(SemanticVersion.TryParse("0.-1.0", out _));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void TryParse_06()
        {
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            Assert.False(SemanticVersion.TryParse("-1.0.0", out _));
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void TryParse_07()
        {
            string versionString = "5.6.7";
            Assert.True(SemanticVersion.TryParse(versionString, out SemanticVersion semanticVersion));
            Assert.AreEqual(versionString, semanticVersion.ToString());
        }
    }
}
