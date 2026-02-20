using Project.ScriptableObjects;

namespace Project.Services.SpawnEntity
{
    [System.Serializable]
    public sealed class SpawnArchetypeData
    {
        public EntityArchetypeConfig Archetype;
        public int PrewarmCount;
        public int MaxAliveCount;
    }
}