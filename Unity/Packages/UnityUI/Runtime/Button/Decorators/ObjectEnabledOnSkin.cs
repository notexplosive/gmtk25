namespace SecretPlan.UI
{
    public class ObjectEnabledOnSkin : ButtonDecoratorPerSkin<bool>
    {
        protected override void ActOn(bool data)
        {
            gameObject.SetActive(data);
        }

        protected override bool GetDataForCurrentState()
        {
            return gameObject.activeSelf;
        }
    }
}
