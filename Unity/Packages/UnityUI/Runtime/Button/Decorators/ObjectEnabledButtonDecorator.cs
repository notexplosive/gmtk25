namespace SecretPlan.UI
{
    public class ObjectEnabledButtonDecorator : ButtonDecoratorPerState<bool>
    {
        public override void OnState(NavigationState previousState, NavigationState newState, bool isInstant,
            ButtonSkin? skin)
        {
            gameObject.SetActive(DataPerState.GetDataFromState(newState));
        }
    }
}
