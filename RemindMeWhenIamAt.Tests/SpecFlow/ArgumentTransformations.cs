using System.Globalization;
using RemindMeWhenIamAt.SharedCode;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.SpecFlow
{
    [Binding]
    public static class ArgumentTransformations
    {
        [StepArgumentTransformation(@"([\d\.]+),\s*([\d\.]+)")]
        public static GeoLocation ParseLocation(string latitude, string longitude) =>
            new GeoLocation(double.Parse(latitude, CultureInfo.InvariantCulture), double.Parse(longitude, CultureInfo.InvariantCulture));
    }
}