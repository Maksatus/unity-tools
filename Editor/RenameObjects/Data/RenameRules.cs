namespace UnityTools.Editor.RenameObjects.Data
{
    public readonly struct RenameRules
    {
        public string Prefix { get; }
        public string Suffix { get; }
        public string Replace { get; }
        public string ReplaceBy { get; }
        public string InsertBeforeAnchor { get; }
        public string InsertBeforeText { get; }
        public string InsertAfterAnchor { get; }
        public string InsertAfterText { get; }
        public bool ToLowercase { get; }
        public bool TrimLastWord { get; }

        public RenameRules(string prefix, string suffix, string replace, string replaceBy,
            string insertBeforeAnchor, string insertBeforeText, string insertAfterAnchor, string insertAfterText,
            bool toLowercase, bool trimLastWord)
        {
            Prefix = prefix;
            Suffix = suffix;
            Replace = replace;
            ReplaceBy = replaceBy;
            InsertBeforeAnchor = insertBeforeAnchor;
            InsertBeforeText = insertBeforeText;
            InsertAfterAnchor = insertAfterAnchor;
            InsertAfterText = insertAfterText;
            ToLowercase = toLowercase;
            TrimLastWord = trimLastWord;
        }
    }
}
