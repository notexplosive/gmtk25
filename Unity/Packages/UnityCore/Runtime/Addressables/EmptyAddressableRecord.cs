namespace SecretPlan.Core
{
    internal class EmptyAddressableRecord : IAddressableManagerRecord
    {
        public void IncrementCount()
        {
            
        }

        public void DecrementCount()
        {
            
        }

        public bool IsEmpty()
        {
            return true;
        }

        public bool IsReady()
        {
            return true;
        }

        public void Release()
        {
            
        }

        public void ForceLoadNow()
        {
            
        }
    }
}