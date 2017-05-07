using System;
using System.Globalization;
using System.IO;

namespace adlordy.ElasticTitle.Extensions
{
    public static class IndexExtensions
    {
        private const string pattern = "yyyy-MM-dd";
        private const string prefix = "title-";

        public static string IndexName(this string file)
        {
            var filename = Path.GetFileNameWithoutExtension(file);
            if (DateTime.TryParseExact(filename, pattern, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
                return IndexName(result);
            throw new FormatException($"Cannot parse filename: {filename}");
        }

        public static string IndexName(this DateTime result)
        {
            return prefix + result.ToString(pattern);
        }
    }
}
