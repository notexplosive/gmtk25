using UnityEngine;

namespace SecretPlan.Core
{
    /// <summary>
    ///     Binds to a Service so the service can use lifetime functions like update
    /// </summary>
    public class ServiceContainer : MonoBehaviour
    {
        private IService? _serviceInstance;

        private void Update()
        {
            if (_serviceInstance != null)
            {
                _serviceInstance.OnUpdate();
            }
        }

        public void Bind(IService serviceInstance)
        {
            if (_serviceInstance != null)
            {
                Debug.LogError("Attempted to reuse a ServiceContainer, should have created a new one");
                return;
            }

            _serviceInstance = serviceInstance;
        }
    }
}
