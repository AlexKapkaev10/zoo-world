using Project.Entities;
using Project.ScriptableObjects;

namespace Project.UI.MVP
{
    public sealed class InfoModel : IInfoModel
    {
        private int _killedHuntersCount;
        private int _killedAnimalsCount;

        InfoModelData IInfoModel.CalculateInfoData(ArchetypeData archetypeData)
        {
            InfoCounterKind infoKind;
            
            if (archetypeData.Kind == EntityKind.Hunter)
            {
                _killedHuntersCount++;
                infoKind = InfoCounterKind.Hunters;
            }
            else
            {
                _killedAnimalsCount++;
                infoKind = InfoCounterKind.Animals;
            }
            
            return GetData(infoKind, 
                infoKind == InfoCounterKind.Animals 
                    ? _killedAnimalsCount 
                    : _killedHuntersCount);
        }

        private InfoModelData GetData(InfoCounterKind infoKind, int count)
        {
            return new InfoModelData(infoKind, count);
        }
    }
}