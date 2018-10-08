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
            var isExist = gameObject.Exists;
            //Assert.IsTrue(isExist);
            var isActive = gameObject.IsActiveInHierarchy;
            //Assert.IsTrue(isActive);
            gameObject.ShouldNot(Be.Present);
            gameObject.ShouldNot(Be.ActiveInHierarchy);
            //Editor.MakeScreenshot(TestContext.CurrentContext.Test.FullName, "FailedTests");
         }

        [Test]
        public void CheckAllMethods2()
        {
            var gameObject = new GameObject("Root", "Name", "Parent");
            gameObject.Click();
            gameObject.SendKeys("qwerty");
            var isExist = gameObject.Exists;
            Assert.IsTrue(isExist);
            var isActive = gameObject.IsActiveInHierarchy;
            Assert.IsTrue(isActive);
            gameObject.Should(Be.Present);
            gameObject.Should(Be.ActiveInHierarchy);
            Editor.MakeScreenshot(TestContext.CurrentContext.Test.FullName, "FailedTests");
        }

        [Test]
        public void CheckGPLogin()
        {
            var LoginGamePointButton = new GameObject("Main Canvas", "gamepointButton");
            LoginGamePointButton.Should(Be.ActiveInHierarchy);
            LoginGamePointButton.Click();
            
            var UserNameInput = new GameObject("Main Canvas", "loginTextFiled");
            UserNameInput.Should(Be.ActiveInHierarchy);
            UserNameInput.Click();
            UserNameInput.SendKeys("Yevhen");
            
            var PasswordInput = new GameObject("Main Canvas", "passTextFiled");
            PasswordInput.Should(Be.ActiveInHierarchy);
            PasswordInput.Click();
            PasswordInput.SendKeys("defaultPassword");
            
            var LoginButton = new GameObject("Main Canvas", "button_login");
            LoginButton.Should(Be.ActiveInHierarchy);
            LoginButton.Click();
            
            var ExperienceBar = new GameObject("Main Canvas", "fillarea_playerxp", "TopBar(Clone)");
            ExperienceBar.Should(Be.Present);
            ExperienceBar.Should(Be.ActiveInHierarchy);
            
        }
        
        [Test]
        public void CheckGPLogin2()
        {
            var LoginGamePointButton = new GameObject("Main Canvas", "gamepointButton");
            LoginGamePointButton.Should(Be.ActiveInHierarchy);
            LoginGamePointButton.Click();
            
            var UserNameInput = new GameObject("Main Canvas", "loginTextFiled");
            UserNameInput.Should(Be.ActiveInHierarchy);
            UserNameInput.Click();
            UserNameInput.SendKeys("Yevhen");
            
            var PasswordInput = new GameObject("Main Canvas", "passTextFiled");
            PasswordInput.Should(Be.ActiveInHierarchy);
            PasswordInput.Click();
            PasswordInput.SendKeys("defaultPassword");
            
            var LoginButton = new GameObject("Main Canvas", "button_login");
            LoginButton.Should(Be.ActiveInHierarchy);
            LoginButton.Click();
            
            var ExperienceBar = new GameObject("Main Canvas", "fillarea_playerxp", "TopBar(Clone)");
            ExperienceBar.Should(Be.Present);
            ExperienceBar.Should(Be.ActiveInHierarchy);
            
        }

        [Test]
        public void StandartAsset_OpenMenu_Positive()
        {
            var menuButton = new GameObject().FindByUPath("/MainMenuUI(Clone)//OpenMenuButton");
            var menu = new GameObject().FindByUPath("/MainMenuUI(Clone)//MenuParent");
            //var menuButton = new GameObject().FindByName("MainMenuUI(Clone)", "OpenMenuButton");
            //var menu = new GameObject().FindByName("MainMenuUI(Clone)", "MenuParent");

            menuButton.Should(Be.Present);
            menuButton.Should(Be.ActiveInHierarchy);

            menuButton.Click();

            menu.Should(Be.Present);
            menu.Should(Be.ActiveInHierarchy);
        }

        [TearDown]
        public void CleanUp()
        {
            Editor.StopPlayMode();
            Editor.KillSession();
        }
    }
}
