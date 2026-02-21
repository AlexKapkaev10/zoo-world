using Project.ScriptableObjects;

namespace Project.UI.MVP
{
    public interface IInfoModel
    {
        InfoModelData CalculateInfoData(ArchetypeData data);
    }
}