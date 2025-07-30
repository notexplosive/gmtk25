using SecretPlan.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(Canvas))]
    public class PopupCanvas : MonoBehaviour
    {
        [SerializeField]
        private PopupRelay? _popupRelay;

        private PopupState _boundPopupState = new();
        private CachedComponent<Canvas> _canvas = new();

        public void Awake()
        {
            if (_popupRelay != null)
            {
                _boundPopupState = _popupRelay.State();
            }

            _boundPopupState.PopupRequested += OpenPopup;
        }

        private void OpenPopup(Popup popupPrefab)
        {
            var cluster = new GameObject($"{popupPrefab.name} Cluster");
            cluster.transform.SetParent(transform);
            cluster.GetOrAddComponent<RectTransform>().StretchToFill();

            var popupInstance = SpawnUtility.Spawn(popupPrefab, new InstantiateParameters {parent = cluster.transform});
            if (popupPrefab.ScrimPrefab != null)
            {
                var graphicalScrim = SpawnUtility.Spawn(popupPrefab.ScrimPrefab,
                    new InstantiateParameters {parent = cluster.transform});
                StretchAndMoveToBackground(graphicalScrim);
            }

            var invisibleScrim = new GameObject("InvisibleScrim");
            invisibleScrim.transform.SetParent(cluster.transform);
            var invisibleScrimImage = invisibleScrim.AddComponent<Image>();
            invisibleScrimImage.color = new Color(0, 0, 0, 0);
            StretchAndMoveToBackground(invisibleScrim);

            popupInstance.RequestClose += () =>
            {
                Destroy(cluster);
            };
        }

        private static void StretchAndMoveToBackground(GameObject? scrim)
        {
            if (scrim != null)
            {
                var rectTransform = scrim.GetOrAddComponent<RectTransform>();
                rectTransform.StretchToFill();
                rectTransform.SetAsFirstSibling();
            }
        }
    }
}
