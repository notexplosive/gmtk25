using SecretPlan.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageSpriteSwapButtonDecorator : ButtonDecoratorPerState<Sprite>
    {
        private readonly CachedComponent<Image> _image = new();
        
        public override void OnState(NavigationState previousState, NavigationState newState, bool isInstant,
            ButtonSkin? skin)
        {
            _image.Get(this).sprite = DataPerState.GetDataFromState(newState);
        }
    }
}
