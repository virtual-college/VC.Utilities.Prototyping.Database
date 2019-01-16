namespace VC.Utilities.Prototyping.Database
{
    public interface IMessage : IEntity
    {
        string Text { get; set; }
    }
}