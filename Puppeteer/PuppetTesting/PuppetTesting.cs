using NUnit.Framework;
using Puppetry.Puppeteer;
using Puppetry.Puppeteer.Puppet;
using Puppetry.Puppeteer.Conditions;

namespace PuppetTesting
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class PuppetTesting
    {
        [SetUp]
        public void Init()
        {
            Configuration.Set(Settings.TimeoutMs, 45000);
            Editor.StartPlayMode();
        }

        [Test]
        public void CheckAllMethods()
        {
            var gameObject = new GameObject("Main Camera", "Name", "Parent");
            //gameObject.Click();
            //gameObject.SendKeys("qwerty");
            var isExist = gameObject.Exists();
            //Assert.IsTrue(isExist);
            var isActive = gameObject.IsActive();
            //Assert.IsTrue(isActive);
            gameObject.ShouldNot(Be.Present);
            gameObject.ShouldNot(Be.Active);
            //Editor.MakeScreenshot(TestContext.CurrentContext.Test.FullName, "FailedTests");
         }

        [Test]
        public void CheckAllMethods2()
        {
            var gameObject = new GameObject("Root", "Name", "Parent");
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

        [Test]
        public void CheckGPLogin()
        {
            var LoginGamePointButton = new GameObject("Main Canvas", "gamepointButton");
            LoginGamePointButton.Should(Be.Active);
            LoginGamePointButton.Click();
            
            var UserNameInput = new GameObject("Main Canvas", "loginTextFiled");
            UserNameInput.Should(Be.Active);
            UserNameInput.Click();
            UserNameInput.SendKeys("Yevhen");
            
            var PasswordInput = new GameObject("Main Canvas", "passTextFiled");
            PasswordInput.Should(Be.Active);
            PasswordInput.Click();
            PasswordInput.SendKeys("defaultPassword");
            
            var LoginButton = new GameObject("Main Canvas", "button_login");
            LoginButton.Should(Be.Active);
            LoginButton.Click();
            
            var ExperienceBar = new GameObject("Main Canvas", "fillarea_playerxp", "TopBar(Clone)");
            ExperienceBar.Should(Be.Present);
            ExperienceBar.Should(Be.Active);
            
        }
        
        [Test]
        public void CheckGPLogin2()
        {
            var LoginGamePointButton = new GameObject("Main Canvas", "gamepointButton");
            LoginGamePointButton.Should(Be.Active);
            LoginGamePointButton.Click();
            
            var UserNameInput = new GameObject("Main Canvas", "loginTextFiled");
            UserNameInput.Should(Be.Active);
            UserNameInput.Click();
            UserNameInput.SendKeys("Yevhen");
            
            var PasswordInput = new GameObject("Main Canvas", "passTextFiled");
            PasswordInput.Should(Be.Active);
            PasswordInput.Click();
            PasswordInput.SendKeys("defaultPassword");
            
            var LoginButton = new GameObject("Main Canvas", "button_login");
            LoginButton.Should(Be.Active);
            LoginButton.Click();
            
            var ExperienceBar = new GameObject("Main Canvas", "fillarea_playerxp", "TopBar(Clone)");
            ExperienceBar.Should(Be.Present);
            ExperienceBar.Should(Be.Active);
            
        }

        [TearDown]
        public void CleanUp()
        {
            Editor.StopPlayMode();
            Editor.KillSession();
        }
    }
}
