namespace PwaHomeScreenUtilities
{
/// <summary>
/// Utility to add/remove a Prograssive Web Application to Home Screeen
/// Ideially, the functionality from this assembly should have been located in RemindMeWhenIamAt.Tests project,
/// but it currently can not be done. PwaHomeScreenUtilities uses Appium NuGet package that currently
/// can not work with Selenium 4 package that is used in RemindMeWhenIamAt.Tests.
/// </summary>
    public static class Program
    {
/// <summary>Main entry point.</summary>
/// <param name="pwaName">The name of the PWA that should be added to/removed from Home Screen. </param>
/// <param name="add">Add PWA to Home Screen.</param>
/// <param name="remove">Remove PWA from Home Screen.</param>
        public static void Main(string pwaName, bool add = false, bool remove = false)
        {
            var pwa = new Pwa(pwaName);

            if (add)
            {
                pwa.AddToHomeScreen();
            }

            if (remove)
            {
                pwa.RemoveFromHomeScreen();
            }
        }
    }
}
