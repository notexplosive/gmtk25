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
        public Post OriginalPost { get; set; } = new();
        
        [YamlMember(Alias = "thread")]
        public List<Post> ThreadEntries { get; set; } = new();
        
        [YamlMember(Alias = "comments")]
        public List<Post> NormalComments { get; set; } = new();
    }
}