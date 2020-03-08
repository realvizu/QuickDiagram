using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Codartis.Util.UI.Wpf.Resources
{
    public struct PackUri
    {
        private static readonly Regex Regex = new Regex(@"^(?<base>pack://application:,,,/[\w.]*;component)[\w.-/]*$", RegexOptions.Compiled);

        private readonly string _uriString;

        public PackUri([CanBeNull] string baseUri, [NotNull] string relativeUri)
        {
            _uriString = baseUri == null
                ? relativeUri
                : Regex.Replace(baseUri, $"${{base}}{relativeUri}");
        }

        [NotNull]
        public static implicit operator Uri(PackUri packUri)
        {
            return new Uri(packUri._uriString);
        }

        public override string ToString()
        {
            return _uriString;
        }
    }
}