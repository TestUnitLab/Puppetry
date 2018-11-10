using NUnit.Framework;
using Puppetry.Puppeteer;
using Puppetry.Puppeteer.Conditions;
using Puppetry.Puppeteer.PuppetDriver;

namespace PuppetTesting
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class PuppetTesting
    {
        [OneTimeSetUp]
        public void TestRunInit()
        {
            Driver.ReleaseAllSessions();
            Configuration.Set(Settings.TimeoutMs, 45000);
        }

        [SetUp]
        public void Init()
        {
            Editor.StartPlayMode();
        }

        [Test]
        public void OpenMenu_Positive_MenuOpened()
        {
            var menuButton = new GameObject().FindByUPath("/MainMenuUI(Clone)//OpenMenuButton");
            var menu = new GameObject().FindByUPath("/MainMenuUI(Clone)//MenuParent");

            menuButton.Should(Be.Present);
            menuButton.Should(Be.ActiveInHierarchy);

            menuButton.Click();

            menu.Should(Be.Present);
            menu.Should(Be.ActiveInHierarchy);
        }

        [Test]
        public void SwitchParticle_Previous_FlareIsActive()
        {
            var nextButton = new GameObject().FindByUPath("/UI//Next");
            var previousButton = new GameObject().FindByUPath("/UI//Previous");
            var titleLabel = new GameObject().FindByUPath("/UI//TitleText");

            previousButton.Should(Be.Present);
            previousButton.Should(Be.ActiveInHierarchy);
            previousButton.Should(Have.ComponentWithPropertyAndValue("Button", "m_Interactable", "true"));
            var onScreen = previousButton.IsOnScreen;
            var clickable = previousButton.IsGraphicClickable;
            previousButton.Click();

            titleLabel.Should(Be.ActiveInHierarchy);
            titleLabel.Should(Have.Component("Text"));
            titleLabel.Should(Have.ComponentWithPropertyAndValue("Text", "m_Text", "\"Flare\""));
            var text = titleLabel.GetComponent("Text");
            Assert.IsTrue(text.Contains("Flare"));
        }

        [TearDown]
        public void CleanUp()
        {
            Game.MakeScreenshot("D:\\ScreenShots\\Test.png");
            Editor.StopPlayMode();
        }

        [OneTimeTearDown]
        public void TestRunCleanUp()
        {
            Driver.ReleaseSession();
        }
    }
}
