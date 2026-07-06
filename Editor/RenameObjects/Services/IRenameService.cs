using UnityTools.Editor.RenameObjects.Data;
namespace UnityTools.Editor.RenameObjects.Services
{
    public interface IRenameService
    {
        string Apply(string name, RenameRules rules);
    }
}
