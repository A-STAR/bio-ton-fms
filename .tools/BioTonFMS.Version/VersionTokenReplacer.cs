namespace BioTonFMS.Version
{
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1401 // Fields must be private
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Replaces tokens in a string with basic project versioning information.
    /// </summary>
    public class VersionTokenReplacer
    {
        private readonly IList<Token> _tokens;
        
        public VersionTokenReplacer()
        {
            _tokens = new List<Token>();

            AddToken("YEAR", () => DateTime.Now.ToString("yyyy"));
            AddToken("MONTH", () => DateTime.Now.ToString("MM"));
            AddToken("DAY", () => DateTime.Now.ToString("dd"));
            AddToken("DATE", () => DateTime.Now.ToString("yyyy-MM-dd"));
            AddToken("DATETIME", () => DateTime.Now.ToString("s"));

            AddToken("UTCYEAR", () => DateTime.UtcNow.ToString("yyyy"));
            AddToken("UTCMONTH", () => DateTime.UtcNow.ToString("MM"));
            AddToken("UTCDAY", () => DateTime.UtcNow.ToString("dd"));
            AddToken("UTCDATE", () => DateTime.UtcNow.ToString("yyyy-MM-dd"));
            AddToken("UTCDATETIME", () => DateTime.UtcNow.ToString("s"));

            AddToken("USER", () => Environment.UserName);
            AddToken("MACHINE", () => Environment.MachineName);
            AddToken("ENVIRONMENT", GetEnvironmentValue);
        }

        public SourceControlInfoProvider SourceControlInfoProvider { get; set; }
        
        public virtual string Replace(string content)
        {
            foreach (Token token in _tokens)
            {
                content = token.Replace(content);
            }

            return content;
        }

        protected void AddToken(string tokenName, TokenFunction function)
        {
            _tokens.Add(new NoArgsToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<int> function)
        {
            _tokens.Add(new IntArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<string> function)
        {
            _tokens.Add(new StringArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        protected void AddToken(string tokenName, TokenFunction<string, string> function)
        {
            _tokens.Add(new TwoStringArgToken
            {
                tokenName = tokenName,
                function = function
            });
        }

        private string GetEnvironmentValue(string name, string defaultValue)
        {
            string returnValue = Environment.GetEnvironmentVariable(name);
            return returnValue ?? defaultValue;
        }

        private abstract class Token
        {
            public string tokenName;

            public abstract string Replace(string str);
        }

        private class NoArgsToken : Token
        {
            public TokenFunction function;

            public override string Replace(string str)
            {
                string token = "$" + tokenName + "$";
                if (str.Contains(token))
                {
                    str = str.Replace(token, function());
                }

                return str;
            }
        }

        private class IntArgToken : Token
        {
            public TokenFunction<int> function;

            public override string Replace(string str)
            {
                foreach (Match match in Regex.Matches(str, @"\$" + tokenName + @"\((\d+)\)\$"))
                {
                    string token = match.Groups[0].Value;
                    int arg = int.Parse(match.Groups[1].Value);
                    str = str.Replace(token, function(arg));
                }

                return str;
            }
        }

        private class StringArgToken : Token
        {
            public TokenFunction<string> function;

            public override string Replace(string str)
            {
                foreach (Match match in Regex.Matches(str, @"\$" + tokenName + @"\(""(.+?)""\)\$"))
                {
                    string token = match.Groups[0].Value;
                    string arg = match.Groups[1].Value;
                    str = str.Replace(token, function(arg));
                }

                return str;
            }
        }

        private class TwoStringArgToken : Token
        {
            public TokenFunction<string, string> function;

            public override string Replace(string str)
            {
                foreach (Match match in Regex.Matches(str, @"\$" + tokenName + @"\(""(.+?)"",""(.*?)""\)\$"))
                {
                    string token = match.Groups[0].Value;
                    string arg1 = match.Groups[1].Value;
                    string arg2 = match.Groups[2].Value;
                    str = str.Replace(token, function(arg1, arg2));
                }

                return str;
            }
        }
    }
#pragma warning restore SA1401 // Fields must be private
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
}