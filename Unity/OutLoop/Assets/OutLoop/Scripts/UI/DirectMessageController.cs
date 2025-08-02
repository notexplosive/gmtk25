using OutLoop.Core;
using UnityEngine;
using UnityEngine.UI;

namespace OutLoop.UI
{
    public class DirectMessageController : MonoBehaviour
    {
        [SerializeField]
        private Image? _backer;

        [SerializeField]
        private TextMeshWithHyperlinks? _messageTextMesh;

        public void Populate(DirectMessage message)
        {
            if (_messageTextMesh != null)
            {
                _messageTextMesh.SetText(message.Message);
            }
        }
    }
}