namespace UnityTools.Editor.RenameObjects.Data
{
    public readonly struct RenamePreviewItem
    {
        public UnityEngine.Object Target { get; }
        public string CurrentName { get; }
        public string NewName { get; }

        public bool IsChanged => CurrentName != NewName && !string.IsNullOrEmpty(NewName);

        public RenamePreviewItem(UnityEngine.Object target, string currentName, string newName)
        {
            Target = target;
            CurrentName = currentName;
            NewName = newName;
        }
    }
}
