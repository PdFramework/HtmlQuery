namespace PeinearyDevelopment.Web.HtmlQueryUnitTests
{
    using System.IO;

    internal static class HtmlUnitTestHelpers
    {
        internal static void AdvanceReaderToTestCharStart(this TextReader reader, int linesToAdvance)
        {
            for (var i = 0; i < linesToAdvance; i++)
            {
                reader.Read();
            }
        }

        internal static void AdvanceReaderToTestLine(this TextReader reader, int linesToAdvance)
        {
            for (var i = 0; i < linesToAdvance; i++)
            {
                reader.ReadLine();
            }
        }
    }
}
