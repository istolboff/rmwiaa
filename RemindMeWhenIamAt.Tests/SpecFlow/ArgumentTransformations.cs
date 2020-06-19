using System.Globalization;
using RemindMeWhenIamAt.SharedCode;
using TechTalk.SpecFlow;

namespace RemindMeWhenIamAt.Tests.SpecFlow
{
    [Binding]
    public static class ArgumentTransformations
    {
        [StepArgumentTransformation(@"([\d\.]+),\s*([\d\.]+)")]
        public static GeoLocation ParseLocation(string x, string y) =>
            new GeoLocation(decimal.Parse(x, CultureInfo.InvariantCulture), decimal.Parse(y, CultureInfo.InvariantCulture));
    }
}