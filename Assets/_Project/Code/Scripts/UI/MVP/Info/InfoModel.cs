using Project.Entities;
using Project.ScriptableObjects;

namespace Project.UI.MVP
{
    public enum InfoCounterKind
    {
        Hunters = 0,
        Animals = 1,
    }

    public readonly struct InfoCounterUpdate
    {
        public readonly InfoCounterKind Kind;
        public readonly int Value;

        public InfoCounterUpdate(InfoCounterKind kind, int value)
        {
            Kind = kind;
            Value = value;
        }
    }

    public interface IInfoModel
    {
        InfoCounterUpdate UpdateCounter(ArchetypeData data);
    }
    
    public sealed class InfoModel : IInfoModel
    {
        public int KilledAnimalsCount { get; private set; }
        public int KilledHuntersCount { get; private set; }

        public InfoCounterUpdate UpdateCounter(ArchetypeData data)
        {
            if (data.Kind == EntityKind.Hunter)
            {
                KilledHuntersCount++;
                return new InfoCounterUpdate(InfoCounterKind.Hunters, KilledHuntersCount);
            }

            KilledAnimalsCount++;
            return new InfoCounterUpdate(InfoCounterKind.Animals, KilledAnimalsCount);
        }
    }
}