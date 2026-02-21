using TMPro;
using UnityEngine;

namespace Project.UI
{
    public class CustomWorldView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textHeader;

        public void SetHeader(string header)
        {
            _textHeader.SetText(header);
        }
    }
}