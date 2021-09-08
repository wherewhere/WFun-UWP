namespace WFunUWP.Models
{
    internal interface ICanChangeLikModel
    {
        string LikeNum { get; set; }
        bool Liked { get; set; }
        string ID { get; }
    }

    internal interface ICanChangeReplyNum
    {
        string ReplyNum { get; set; }
    }

    internal interface ICanCopy
    {
        bool IsCopyEnabled { get; set; }
    }
}
