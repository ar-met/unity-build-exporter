using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace armet.BuildExporter.Tests
{
    internal class PlayerSettingsVersionerTests
    {
        private string m_CurrentVersion;
        private int    m_CurrentVersionCodeAndroid;
        private string m_CurrentVersionCodeIOS;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_CurrentVersion            = PlayerSettings.bundleVersion;
            m_CurrentVersionCodeAndroid = PlayerSettings.Android.bundleVersionCode;
            m_CurrentVersionCodeIOS     = PlayerSettings.iOS.buildNumber;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PlayerSettings.bundleVersion             = m_CurrentVersion;
            PlayerSettings.Android.bundleVersionCode = m_CurrentVersionCodeAndroid;
            PlayerSettings.iOS.buildNumber           = m_CurrentVersionCodeIOS;

            // saving changes
            AssetDatabase.SaveAssets();
        }

        [SetUp]
        public void SetUp()
        {
            PlayerSettings.bundleVersion             = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 0;
            PlayerSettings.iOS.buildNumber           = "0";
        }

        private void AssertInitialVersionsVersionCodes()
        {
            Assert.AreEqual("1.0.0", PlayerSettings.bundleVersion);
            AssertInitialVersionCodes();
        }

        private void AssertInitialVersionCodes()
        {
            Assert.AreEqual(0, PlayerSettings.Android.bundleVersionCode);
            Assert.AreEqual("0", PlayerSettings.iOS.buildNumber);
        }

        private void AssertVersionCodesEqualTo(int code)
        {
            Assert.AreEqual(code, PlayerSettings.Android.bundleVersionCode);
            Assert.AreEqual(code.ToString(), PlayerSettings.iOS.buildNumber);
        }

        [Test]
        public void TryParse_01()
        {
            Assert.True(PlayerSettingsVersioner.TryParse(string.Empty, string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
        }

        [Test]
        public void TryParse_02()
        {
            Assert.True(PlayerSettingsVersioner.TryParse(null, null, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
        }

        [Test]
        public void TryParse_03()
        {
            Assert.True(PlayerSettingsVersioner.TryParse(string.Empty, null, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
        }

        [Test]
        public void TryParse_04()
        {
            Assert.True(PlayerSettingsVersioner.TryParse(string.Empty, null, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
        }

        [Test]
        public void TryParse_05()
        {
            string newVersion = "1.1";
            // expecting two errors
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            Assert.False(PlayerSettingsVersioner.TryParse(newVersion, string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void TryParse_06()
        {
            string newVersionCode = "-1";
            LogAssert.Expect(LogType.Error, new Regex(@".*"));
            Assert.False(
                PlayerSettingsVersioner.TryParse(
                    string.Empty, newVersionCode,
                    out var playerSettingsVersioner));

            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionsVersionCodes();
            LogAssert.NoUnexpectedReceived();
        }

        [Test]
        public void Version_01()
        {
            string newVersion = "1.1.1";
            Assert.True(PlayerSettingsVersioner.TryParse(newVersion, string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionCodes();
            Assert.AreEqual(newVersion, PlayerSettings.bundleVersion);
        }

        [Test]
        public void Version_Major_01()
        {
            Assert.True(PlayerSettingsVersioner.TryParse("major", string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionCodes();
            Assert.AreEqual("2.0.0", PlayerSettings.bundleVersion);
        }

        [Test]
        public void Version_Minor_01()
        {
            Assert.True(PlayerSettingsVersioner.TryParse("minor", string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionCodes();
            Assert.AreEqual("1.1.0", PlayerSettings.bundleVersion);
        }

        [Test]
        public void Version_Patch_01()
        {
            Assert.True(PlayerSettingsVersioner.TryParse("patch", string.Empty, out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertInitialVersionCodes();
            Assert.AreEqual("1.0.1", PlayerSettings.bundleVersion);
        }

        [Test]
        public void VersionCode_01()
        {
            int newVersionCode = 42;
            Assert.True(
                PlayerSettingsVersioner.TryParse(
                    string.Empty, newVersionCode.ToString(),
                    out var playerSettingsVersioner));

            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertVersionCodesEqualTo(newVersionCode);
        }

        [Test]
        public void VersionCode_Increment_01()
        {
            Assert.True(PlayerSettingsVersioner.TryParse(string.Empty, "increment", out var playerSettingsVersioner));
            Assert.NotNull(playerSettingsVersioner);

            playerSettingsVersioner.Apply();
            AssertVersionCodesEqualTo(1);
        }
    }
}
