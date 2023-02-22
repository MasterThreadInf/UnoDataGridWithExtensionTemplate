using Windows.ApplicationModel.Resources;

namespace sample.Helpers
{
    public static class ErrorMessageHelper
    {
        private static ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse("ErrorMessages");

        public static string GetErrorMessageResource(string name)
        {
            return _resourceLoader.GetString(name);
        }
    }
}
