using RemindMeWhenIamAt.SharedCode;

namespace RemindMeWhenIamAt.Tests.Sut.GuiTestDriverExtensions
{
    internal interface IBrowserDeveloperTools
    {
        void SetCurrentLocation(GeoLocation value);
    }
}