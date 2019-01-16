using System;

namespace VC.Utilities.Prototyping.Database.Tests
{
    public class Message : IMessage
    {
        public int Id { get; set; }
        public Guid Reference { get; set; }
        public string Text { get; set; }
    }
}