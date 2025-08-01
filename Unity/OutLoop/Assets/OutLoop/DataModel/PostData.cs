using System;
using YamlDotNet.Serialization;

namespace OutLoop.Data
{
    [Serializable]
    [YamlSerializable]
    public class PostData
    {
        [YamlMember(Alias = "post_id")]
        public string? PostId { get; set; }

        [YamlMember(Alias = "author_username")]
        public string Author { get; set; } = "";
        
        [YamlMember(Alias = "text")]
        public string Text { get; set; } = "";

        [YamlMember(Alias = "likes")]
        public int? Likes { get; set; } = null;
        
        [YamlMember(Alias = "reposts")]
        public int? Reposts { get; set; } = null;
        
        [YamlMember(Alias = "image")]
        public string? ImagePath { get; set; } = "";
        
        [YamlMember(Alias = "linked_post_id")]
        public string? LinkedPostId { get; set; } = "";
    }
}