using System;
using LiteDB;

namespace VC.Utilities.Prototyping.Database
{
    public interface IEntity
    {
        int Id { get; set; }
        Guid Reference { get; set; }
    }
}