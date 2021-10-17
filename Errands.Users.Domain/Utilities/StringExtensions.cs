using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Errands.Users.Domain.Utilities
{
	public static class StringExtensions
	{
		public static string PluralizeWord(this string singularWord, int count)
		{
			return PluralizeWord(singularWord, singularWord + "s", count);
		}
		public static string PluralizeWord(this string singularWord, string plural, int count)
		{
			if (count == 1)
				return singularWord;
			return plural;
		}
		public static string Pluralize(this string singular, string plural, int count)
		{
			return $"{count} {PluralizeWord(singular, plural, count)}";
		}
		public static string Pluralize(this string singular, int count)
		{
			return $"{count} {PluralizeWord(singular, count)}";
		}

		public static string ToTitleCase(this string text)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
		}

		public static string Stringify(this IEnumerable<string> items)
		{
			var saneList = items.Distinct().Where(i => !string.IsNullOrWhiteSpace(i))
				.Select(i => i.Trim().ToLower());
			return string.Join(',', saneList);
		}

		public static string Sanitize(this string text)
		{
			var trimmed = text.Trim().ToLower().Replace(" ", "_");
			return Regex.Replace(trimmed, @"[^0-9a-zA-Z_]+", "");
		}
	}
}
