using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Lindhart.Analyser.MissingAwaitWarning.Test.Helpers
{
    internal static class EmbeddedFilesLoader
    {
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly HashSet<string> embeddedFiles = new(assembly.GetManifestResourceNames());

        public static async Task<string> LoadFile(string name)
        {
            var fullFilename = embeddedFiles.First(x => x.Contains(name));
            var stream = assembly.GetManifestResourceStream(fullFilename);
            using var reader = new StreamReader(stream!);
            return await reader.ReadToEndAsync();
        }
    }
}
