namespace WFunUWP.Models
{
    internal interface ICanLike
    {
        string LikeNum { get; set; }
        bool Liked { get; set; }
        string ID { get; }
    }

    internal interface ICanReply
    {
        string ReplyNum { get; set; }
    }

    internal interface ICanCopy
    {
        bool IsCopyEnabled { get; set; }
    }
}
