using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace VC.Utilities.Prototyping.Database
{
    public interface IPrototypingDb
    {
        T Create<T>(T entity) where T : IEntity;
        T Read<T>(int id) where T : IEntity;
        T Update<T>(T entity) where T : IEntity;
        void Delete<T>(T entity) where T : IEntity;
        void ResetDb();
        IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : IEntity;
        IEnumerable<T> Query<T>(Query query) where T : IEntity;
    }
}