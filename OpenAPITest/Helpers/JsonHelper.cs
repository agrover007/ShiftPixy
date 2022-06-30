using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenAPITest
{
	public static class JsonHelper
	{
		public static void ObscureMatchingValues(JToken token, IEnumerable<string> jsonPaths)
		{
			foreach (string path in jsonPaths)
			{
				foreach (JToken match in token.SelectTokens(path))
				{
					match.Replace(new JValue(Obscure(match.ToString())));
				}
			}
		}

		public static string Obscure(string s)
		{
			if (string.IsNullOrEmpty(s)) return s;

			int len = s.Length;
			int leftLen = len > 4 ? 1 : 0;
			int rightLen = len > 6 ? Math.Min((len - 6) / 2, 4) : 0;
			return s.Substring(0, leftLen) +
				new string('*', len - leftLen - rightLen) +
				s.Substring(len - rightLen);
		}

		public static string MaskJson(string src)
		{
			JToken token = JToken.Parse(src);
			string[] jsonPaths = ExampleConfigSettings.GetJsonPathsToObscure();
			JsonHelper.ObscureMatchingValues(token, jsonPaths);
			ExampleLogger.Log(token.ToString(Formatting.None));
			return token.ToString(Formatting.None);
		}
	}


	public static class ExampleConfigSettings
	{
		public static string[] GetJsonPathsToObscure()
		{
			// In real code this would read the paths from a config file
			//return new string[] { "$..Password", "$..credit_card.number" };
			return new string[] { "$..account_id" };
		}
	}

	public static class ExampleLogger
	{
		public static void Log(string s)
		{
			// In real code this would write to a file or SQL table
			Console.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + " - " + s);
		}
	}
}