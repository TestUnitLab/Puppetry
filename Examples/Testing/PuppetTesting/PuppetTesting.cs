using NUnit.Framework;
using Puppetry.Puppeteer;
using GameObjectConditins = Puppetry.Puppeteer.Conditions.GameObject;
using GameConditions = Puppetry.Puppeteer.Conditions.Game;

namespace PuppetTesting
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class PuppetTesting
    {
        [OneTimeSetUp]
        public void TestRunInit()
        {
            PuppetryDriver.ReleaseAllSessions();
            Configuration.Set(Settings.TimeoutMs, 45000);
            Configuration.Set(Settings.StartPlayModeTimeoutMs, 45000);
        }

        [SetUp]
        public void Init()
        {
            Editor.StartPlayMode();
            Game.Should(GameConditions.Have.SceneName("Particles"));
        }

        [Test]
        public void OpenMenu_Positive_MenuOpened()
        {
            var menuButton = new GameObject().FindByUPath("/MainMenuUI(Clone)//OpenMenuButton");
            var menu = new GameObject().FindByUPath("/MainMenuUI(Clone)//MenuParent");

            menuButton.Should(GameObjectConditins.Be.Present);
            //Game.Should(GameConditions.Have.SceneName("Name"));
            menuButton.Should(GameObjectConditins.Be.ActiveInHierarchy);

            menuButton.Click();

            menu.Should(GameObjectConditins.Be.Present);
            menu.Should(GameObjectConditins.Be.ActiveInHierarchy);
        }

        [Test]
        public void SwitchParticle_Previous_FlareIsActive()
        {
            var nextButton = new GameObject().FindByUPath("/UI//Next");
            var previousButton = new GameObject().FindByUPath("/UI//Previous");
            var titleLabel = new GameObject().FindByUPath("/UI//TitleText");

            previousButton.Should(GameObjectConditins.Be.Present);
            previousButton.Should(GameObjectConditins.Be.ActiveInHierarchy);
            previousButton.Should(GameObjectConditins.Have.ComponentWithPropertyAndValue("Button", "m_Interactable", "true"));
            var onScreen = previousButton.IsOnScreen;
            var clickable = previousButton.IsHitByGraphicRaycast;
            previousButton.Click();

            titleLabel.Should(GameObjectConditins.Be.ActiveInHierarchy);
            titleLabel.Should(GameObjectConditins.Have.Component("Text"));
            titleLabel.Should(GameObjectConditins.Have.ComponentWithPropertyAndValue("Text", "m_Text", "\"Flare\""));
            var text = titleLabel.GetComponent("Text");
            Assert.IsTrue(text.Contains("Flare"));
        }

        [Test]
        public void TestSceneFunctionality()
        {
            Game.OpenScene("Car");
            Game.Should(GameConditions.Have.SceneName("Car"));
        }

        [TearDown]
        public void CleanUp()
        {
            //Game.MakeScreenshot("D:\\ScreenShots\\Test.png");
            Editor.StopPlayMode();
        }

        [OneTimeTearDown]
        public void TestRunCleanUp()
        {
            PuppetryDriver.ReleaseAllSessions();
        }
    }
}
