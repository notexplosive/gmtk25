using UnityEngine;

namespace SecretPlan.Core
{
    public abstract class GlobalService<TService> : IService where TService : GlobalService<TService>, new()
    {
        private static TService? _instance;

        public static TService Instance
        {
            get
            {
                if (!Application.isPlaying)
                {
                    // If we're in edit mode, just create an instance and use it once
                    return new TService();
                }

                if (_instance != null)
                {
                    return _instance;
                }

                var serviceContainer = new GameObject($"{typeof(TService).Name}_GlobalServiceInstance");
                Object.DontDestroyOnLoad(serviceContainer);
                _instance = new TService();
                serviceContainer.AddComponent<ServiceContainer>().Bind(_instance);
                return _instance;
            }
        }

        public abstract void OnUpdate();
    }
}
