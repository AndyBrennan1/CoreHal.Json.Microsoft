namespace CoreHal.Json.Tests.Utilities
{
    public static class ResourceReader
    {
        public static string Get(byte[] resourceContentByteArray)
        {
            var contentsAsString = System.Text.Encoding.Default.GetString(resourceContentByteArray);

            return contentsAsString;
        }
    }
}