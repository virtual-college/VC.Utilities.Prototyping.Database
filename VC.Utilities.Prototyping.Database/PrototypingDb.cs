using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using LiteDB;

namespace VC.Utilities.Prototyping.Database
{
    public class PrototypingDb : IPrototypingDb
    {
        private readonly DbOptions options;

        public PrototypingDb(DbOptions options)
        {
            this.options = options;
        }

        public T Create<T>(T entity) where T : IEntity
        {
            using (var db = CreateDb())
            {
                if (entity.Id != 0)
                {
                    throw new InvalidOperationException("Cannot call Create on an entity whose Id has already been assigned");
                }

                GetCollection<T>(db).Insert(entity);
                return entity;
            }
        }

        public T Read<T>(int id) where T : IEntity
        {
            using (var db = CreateDb())
            {
                return GetCollection<T>(db).Find(x => x.Id == id).First();
            }
        }

        public T Update<T>(T entity) where T : IEntity
        {
            using (var db = CreateDb())
            {
                if (entity.Id == 0)
                {
                    throw new InvalidOperationException("Cannot call Update on an entity whose Id has not been assigned");
                }

                GetCollection<T>(db).Update(entity);
                return entity;
            }
        }

        public void Delete<T>(T entity) where T : IEntity
        {
            using (var db = new LiteDatabase(options.DbFilePath))
            {
                GetCollection<T>(db).Delete(x => x.Id == entity.Id);
            }
        }

        public void ResetDb()
        {
            var fileInfo = new FileInfo(options.DbFilePath);

            if (fileInfo.Exists)
            {
                while (fileInfo.Exists)
                {
                    try
                    {
                        fileInfo.Delete();
                        Thread.Sleep(100);
                        fileInfo.Refresh();
                    }
                    catch 
                    {
                    }
                }
            }
        }

        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : IEntity
        {
            using (var db = CreateDb())
            {
                return GetCollection<T>(db).Find(expression);
            }
        }

        public IEnumerable<T> Query<T>(Query query) where T : IEntity
        {
            using (var db = CreateDb())
            {
                return GetCollection<T>(db).Find(query);
            }
        }

        private LiteDatabase CreateDb()
        {
            return new LiteDatabase(options.DbFilePath);
        }

        private static LiteCollection<T> GetCollection<T>(LiteDatabase db) where T : IEntity
        {
            return db.GetCollection<T>(typeof(T).Name);
        }
    }
}
