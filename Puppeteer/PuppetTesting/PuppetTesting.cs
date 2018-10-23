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
            previousButton.Should(Be.Interactable("Button"));
            previousButton.Click();

            titleLabel.Should(Be.ActiveInHierarchy);
            titleLabel.Should(Have.Component("Text"));
            titleLabel.Should(Have.Text("Text","Flare"));
            var text = titleLabel.GetComponent("Text");
            Assert.IsTrue(text.Contains("Flare"));
        }

        [TearDown]
        public void CleanUp()
        {
            Editor.MakeScreenshot("D:\\ScreenShots\\Test.png");
            Editor.StopPlayMode();
            Editor.KillSession();
        }
    }
}
