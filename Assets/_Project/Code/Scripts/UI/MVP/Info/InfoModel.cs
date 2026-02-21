using Project.Entities;
using Project.ScriptableObjects;

namespace Project.UI.MVP
{
    public interface IInfoModel
    {
        void UpdateCounter(ArchetypeData data, out int killedCount);
    }
    
    public sealed class InfoModel : IInfoModel
    {
        public int KilledAnimalsCount { get; private set; }
        public int KilledHuntersCount { get; private set; }

        public void UpdateCounter(ArchetypeData data, out int killedCount)
        {
            switch (data.Kind)
            {
                case EntityKind.Hunter:
                    KilledHuntersCount++;
                    break;
                case EntityKind.Frog:
                case EntityKind.Snake:
                    KilledAnimalsCount++;
                    break;
            }
            
            killedCount = GetKilledCountByKind(data.Kind);
        }

        private int GetKilledCountByKind(EntityKind kind)
        {
            int killedCount = 0;
            switch (kind)
            {
                case EntityKind.Hunter:
                    killedCount = KilledHuntersCount;
                    break;
                case EntityKind.Frog:
                case EntityKind.Snake:
                    killedCount = KilledAnimalsCount;
                    break;
            }
            
            return killedCount;
        }
    }
}