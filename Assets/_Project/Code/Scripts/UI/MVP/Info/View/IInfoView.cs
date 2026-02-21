namespace Project.UI.MVP
{
    public interface IInfoView
    {
        void UpdateAnimalsInfo(string killedAnimalsText);
        void UpdateHuntersInfo(string killedHuntersText);
        void Destroy();
    }
}