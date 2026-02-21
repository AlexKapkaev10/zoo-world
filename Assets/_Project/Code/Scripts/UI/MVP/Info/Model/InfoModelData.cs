namespace Project.UI.MVP
{
    public readonly struct InfoModelData
    {
        public readonly InfoCounterKind Kind;
        public readonly int Value;

        public InfoModelData(InfoCounterKind kind, int value)
        {
            Kind = kind;
            Value = value;
        }
    }
}