using Api.Interfaces;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace Api.Base
{ 
    public abstract class BaseService<T> : IBaseService<T> where T : IBaseModel
    {
        protected readonly IMongoCollection<T> _collection;

        protected BaseService(BaseDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _collection  = database.GetCollection<T>(settings.CollectionName);
        }

        public List<T> Get() => 
            _collection.Find(entity => true).ToList();

        public T Get(string id) =>
             _collection.Find<T>(entity => entity.Id == id).FirstOrDefault();

        public T Create(T entity)
        {
            _collection.InsertOne(entity);
            return entity;
        }

        public void Update(string id, T entityIn) =>
            _collection.ReplaceOne(entity => entity.Id == id, entityIn);

        public void Remove(T entityIn) =>
            _collection.DeleteOne(entity => entity.Id == entityIn.Id);

        public void Remove(string id) =>
            _collection.DeleteOne(entity => entity.Id == id);

    }

    public interface IBaseService<T>
    {
        List<T> Get();

        T Get(string id);

        T Create(T entity);

        void Update(string id, T entityIn);

        void Remove(T entityIn);

        void Remove(string id);
    }

}
