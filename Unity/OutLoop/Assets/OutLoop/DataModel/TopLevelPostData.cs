using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace OutLoop.Data
{
    [Serializable]
    [YamlSerializable]
    public class TopLevelPostData
    {
        [YamlMember(Alias = "original_post")]
        public PostData OriginalPost { get; set; } = new();
        
        [YamlMember(Alias = "thread")]
        public List<PostData> ThreadEntries { get; set; } = new();
        
        [YamlMember(Alias = "comments")]
        public List<PostData> NormalComments { get; set; } = new();
    }
}