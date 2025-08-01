using SecretPlan.UI;
using UnityEngine;

namespace OutLoop.UI
{
    public class AppButtonRow : MonoBehaviour
    {
        [SerializeField]
        private SecretButton? _homeButton;

        [SerializeField]
        private SecretButton? _searchButton;

        [SerializeField]
        private SecretButton? _messagesButton;

        [SerializeField]
        private SecretButton? _favoritesButton;

        public SecretButton? HomeButton => _homeButton;

        public SecretButton? SearchButton => _searchButton;

        public SecretButton? MessagesButton => _messagesButton;

        public SecretButton? FavoritesButton => _favoritesButton;
    }
}