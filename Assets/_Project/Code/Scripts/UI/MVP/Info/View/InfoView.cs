using TMPro;
using UnityEngine;

namespace Project.UI.MVP
{
    public sealed class InfoView : MonoBehaviour, IInfoView
    {
        [SerializeField] private TMP_Text _killedAnimalsText;
        [SerializeField] private TMP_Text _killedHuntersText;

        void IInfoView.UpdateAnimalsInfo(string killedAnimalsText)
        {
            _killedAnimalsText.SetText(killedAnimalsText);
        }

        void IInfoView.UpdateHuntersInfo(string killedHuntersText)
        {
            _killedHuntersText.SetText(killedHuntersText);
        }

        void IInfoView.Destroy()
        {
            Destroy(gameObject);
        }
    }
}