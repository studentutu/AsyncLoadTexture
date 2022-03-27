using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityTextureLoader.Extensions
{
	public static class UrlExtensions
	{
		private static Dictionary<string, Regex> _listRegexTokens = new Dictionary<string, Regex>()
		{
			{"?AWSAccess", new Regex(".+(AWS[Aa]ccess)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
			{"token", new Regex(".+([Tt]oken)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
			{"expire", new Regex(".+([Ee]xpire)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
			{"access", new Regex(".+([Aa]ccess)", RegexOptions.CultureInvariant | RegexOptions.Compiled)},
		};

		public static string RemoveAllTokens(string fullUri)
		{
			if (string.IsNullOrEmpty(fullUri))
			{
				return fullUri;
			}

			var next = true;
			while (next)
			{
				next = false;
				foreach (var regexForTokens in _listRegexTokens)
				{
					var match = regexForTokens.Value.Match(fullUri);
					if (match.Success)
					{
						int length = match.Value.Length;
						next = true;
						fullUri = match.Value.Substring(0, length - regexForTokens.Key.Length);
					}
				}
			}

			return fullUri;
		}
	}
}