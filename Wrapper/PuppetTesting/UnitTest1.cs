using NUnit.Framework;
using Puppet;
using Puppet.Conditions;

namespace PuppetTesting
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void Init()
        {
            Configuration.Set(Settings.TimeoutMs, 10000);
            Editor.StartPlayMode();
        }

        [Test]
        public void Test1()
        {
            var gameObject = new GameObject("Name", "Parent");
            gameObject.Click();
            gameObject.SendKeys("qwerty");
            var isExist = gameObject.Exists();
            var isActive = gameObject.IsActive();
            gameObject.Should(Be.Present);
            gameObject.ShouldNot(Be.Active);
            Editor.MakeScreenshot(TestContext.CurrentContext.Test.FullName, "FailedTests");
        }

        [TearDown]
        public void CleanUp()
        {
            Editor.StopPlayMode();
            Editor.KillSession();
        }
    }
}
