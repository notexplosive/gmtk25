using System;
using System.Collections.Generic;
using UnityEngine;

namespace SecretPlan.UI
{
    public abstract class ButtonDecoratorPerSkin<TData> : ButtonDecorator
    {
        [SerializeField]
        private List<DataAndSkin> _entries = new();

        private DataAndSkin GetEntryForSkin(ButtonSkin? skin)
        {
            foreach (var entry in _entries)
            {
                if (entry.Skin == skin)
                {
                    return entry;
                }
            }

            // Return an empty entry if none is found
            return new DataAndSkin(default, skin);
        }

        private bool HasSkin(ButtonSkin? skin)
        {
            foreach (var entry in _entries)
            {
                if (entry.Skin == skin)
                {
                    return true;
                }
            }

            return false;
        }

        public override void OnState(NavigationState previousState, NavigationState newState, bool isInstant,
            ButtonSkin? skin)
        {
            if (HasSkin(skin))
            {
                ActOn(GetEntryForSkin(skin).Data);
            }
            else
            {
                var noSkin = GetEntryForSkin(null);
                ActOn(noSkin.Data);
            }
        }

        protected abstract void ActOn(TData? data);

        /// <summary>
        ///     Generates and returns a Data object representing the state the button is currently in (used for saving)
        /// </summary>
        protected abstract TData? GetDataForCurrentState();

        [Serializable]
        private class DataAndSkin
        {
            [SerializeField]
            private TData? _data;

            [SerializeField]
            private ButtonSkin? _skin;

            public DataAndSkin()
            {
                // needed for serializer
            }

            public DataAndSkin(TData? data, ButtonSkin? skin)
            {
                _data = data;
                _skin = skin;
            }

            public ButtonSkin? Skin => _skin;

            public TData? Data
            {
                get => _data;
                set => _data = value;
            }
        }
    }
}
