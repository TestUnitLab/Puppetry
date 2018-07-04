using NUnit.Framework;
using Puppeteer;
using Puppeteer.Puppet;
using Puppeteer.Conditions;

namespace PuppetTesting
{
    [TestFixture]
    //[Parallelizable(ParallelScope.All)]
    public class PuppetTesting
    {
        [SetUp]
        public void Init()
        {
            Configuration.Set(Settings.TimeoutMs, 10000);
            Editor.StartPlayMode();
        }

        [Test]
        public void CheckAllMethods()
        {
            var gameObject = new GameObject("Name", "Parent");
            //gameObject.Click();
            //gameObject.SendKeys("qwerty");
            var isExist = gameObject.Exists();
            //Assert.IsTrue(isExist);
            var isActive = gameObject.IsActive();
            //Assert.IsTrue(isActive);
            //gameObject.Should(Be.Present);
            //gameObject.Should(Be.Active);
            //Editor.MakeScreenshot(TestContext.CurrentContext.Test.FullName, "FailedTests");
         }

        [Test]
        public void CheckAllMethods2()
        {
            var gameObject = new GameObject("Name", "Parent");
            gameObject.Click();
            gameObject.SendKeys("qwerty");
            var isExist = gameObject.Exists();
            Assert.IsTrue(isExist);
            var isActive = gameObject.IsActive();
            Assert.IsTrue(isActive);
            gameObject.Should(Be.Present);
            gameObject.Should(Be.Active);
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
