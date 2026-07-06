using UnityTools.Editor.RenameObjects.Data;
using System;
using System.Text;

namespace UnityTools.Editor.RenameObjects.Services
{
    public class RenameService : IRenameService
    {
        private static readonly char[] WordSeparators = { ',', '.', ' ', '_' };

        public string Apply(string name, RenameRules rules)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            var output = new StringBuilder();
            output.Append(rules.Prefix);
            output.Append(name);
            output.Append(rules.Suffix);

            if (!string.IsNullOrEmpty(rules.Replace))
                output.Replace(rules.Replace, rules.ReplaceBy ?? string.Empty);

            InsertText(output, rules.InsertBeforeAnchor, rules.InsertBeforeText, after: false);
            InsertText(output, rules.InsertAfterAnchor, rules.InsertAfterText, after: true);

            var result = output.ToString();

            if (rules.ToLowercase)
                result = result.ToLowerInvariant();

            if (rules.TrimLastWord)
                result = TrimLastWord(result);

            return result;
        }

        private static void InsertText(StringBuilder output, string anchor, string text, bool after)
        {
            if (string.IsNullOrEmpty(anchor) || string.IsNullOrEmpty(text))
                return;

            var index = output.ToString().IndexOf(anchor, StringComparison.Ordinal);
            if (index < 0)
                return;

            if (after)
                index += anchor.Length;

            output.Insert(index, text);
        }

        private static string TrimLastWord(string value)
        {
            var trimmed = value.TrimEnd(WordSeparators);
            var separatorIndex = trimmed.LastIndexOfAny(WordSeparators);
            return separatorIndex < 0 ? trimmed : trimmed[..separatorIndex].TrimEnd(WordSeparators);
        }
    }
}
