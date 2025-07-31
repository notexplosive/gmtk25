using System;

namespace OutLoop.Data
{
    [Serializable]
    public class Account
    {
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";

        public string UserNameWithAt()
        {
            return $"@{UserName}";
        }
    }
}