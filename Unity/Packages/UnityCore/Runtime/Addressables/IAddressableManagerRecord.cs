namespace SecretPlan.Core
{
    internal interface IAddressableManagerRecord
    {
        void IncrementCount();
        void DecrementCount();
        bool IsEmpty();
        bool IsReady();
        void Release();
    }
}