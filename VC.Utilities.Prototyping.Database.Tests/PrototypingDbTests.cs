using System;
using System.Linq;
using LiteDB;
using Xunit;

namespace VC.Utilities.Prototyping.Database.Tests
{
    public class PrototypingDbTests
    {
        private static PrototypingDb database;

        public PrototypingDbTests()
        {
            var options = new DbOptions
            {
                DbFilePath = @"C:\Temp\LiteDbTests\litedb.db"
            };

            database = new PrototypingDb(options);
            database.ResetDb();
        }

        [Fact]
        public void CreatingTwoMessages_BothHaveTheCorrectId()
        {
            var message1 = database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 1" });
            var message2 = database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 2" });

            Assert.Equal(1, message1.Id);
            Assert.Equal(2, message2.Id);
        }

        [Fact]
        public void NewMessage_CallingUpdate_ThrowsAnException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                database.Update(new Message { Reference = Guid.NewGuid(), Text = "Message 1" }));
        }

        [Fact]
        public void UpdateMessage_HasUpdatedField()
        {
            var oldGuid = Guid.NewGuid();
            var oldText = "Message 1";

            var newGuid = Guid.NewGuid();
            var newText = "Message 2";

            var message = database.Create(new Message { Reference = oldGuid, Text = oldText });
            message.Reference = newGuid;
            message.Text = newText;
            database.Update(message);

            Assert.Equal(newGuid, message.Reference);
            Assert.Equal(newText, message.Text);
        }

        [Fact]
        public void Creating3MessagesAndReturningTheSecondMessage_RetrieveTheSecondMessage_ReturnsCorrectMessage()
        {
            const string message = "Message 2";

            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 1" });
            var message2 = database.Create(new Message { Reference = Guid.NewGuid(), Text = message });
            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 3" });

            var result = database.Query<Message>(x => x.Reference == message2.Reference );
            Assert.NotNull(result);
            Assert.Equal(message, message2.Text);
        }

        [Fact]
        public void Creating3Messages_GettingAll_Returns3Messages()
        {
            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 1" });
            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 2" });
            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 3" });

            var result = database.Query<Message>(Query.All());
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void CreatingAMessage_ReadsMessage_ReadsCorrectly()
        {
            var guid = Guid.NewGuid();
            var message = database.Create(new Message { Reference = guid, Text = "Message 1" });
            var result = database.Read<Message>(message.Id);

            Assert.Equal(guid, message.Reference);
        }

        [Fact]
        public void Creating3Messages_Delete2nd_Returns2Messages()
        {
            var message = "Message 2";

            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 1" });
            var message2 = database.Create(new Message { Reference = Guid.NewGuid(), Text = message });
            database.Create(new Message { Reference = Guid.NewGuid(), Text = "Message 3" });

            database.Delete(message2);

            var result = database.Query<Message>(Query.All());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void CallingCreateWithAnEntityWithAndId_ThrowsAnException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                database.Create(new Message {Id = 1, Reference = Guid.NewGuid(), Text = "Message 1"}));
        }

    }
}