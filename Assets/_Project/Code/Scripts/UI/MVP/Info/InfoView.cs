using TMPro;
using UnityEngine;

namespace Project.UI.MVP
{
    public interface IInfoView
    {
        void UpdateAnimalsInfo(string killedAnimalsText);
        void UpdateHuntersInfo(string killedHuntersText);
        void Destroy();
    }
    
    public sealed class InfoView : MonoBehaviour, IInfoView
    {
        [SerializeField] private TMP_Text _killedAnimalsText;
        [SerializeField] private TMP_Text _killedHuntersText;
        public void UpdateAnimalsInfo(string killedAnimalsText)
        {
            _killedAnimalsText.SetText(killedAnimalsText);
        }

        public void UpdateHuntersInfo(string killedHuntersText)
        {
            _killedHuntersText.SetText(killedHuntersText);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}