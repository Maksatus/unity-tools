namespace UnityTools.Editor.MaterialCleaner.Data
{
    public readonly struct MaterialPropertyInfo
    {
        public string Name { get; }
        public MaterialPropertyStatus Status { get; }

        public MaterialPropertyInfo(string name, MaterialPropertyStatus status)
        {
            Name = name;
            Status = status;
        }
    }
}