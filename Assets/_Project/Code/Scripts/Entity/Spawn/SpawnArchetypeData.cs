using Project.ScriptableObjects;

namespace Project.Entities
{
    [System.Serializable]
    public sealed class SpawnArchetypeData
    {
        public EntityArchetypeConfig Archetype;
        public int StartPoolCount;
        public int MaxAliveCount;
        public float MinSpawnY;
        public float MaxSpawnY;
    }
}