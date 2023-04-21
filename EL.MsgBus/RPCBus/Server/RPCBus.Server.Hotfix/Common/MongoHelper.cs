using ET;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace RPCBus.Server
{
    public class IdentityFactory
    {
        public string Id { get; set; }
        public long Value { get; set; }
    }

    public static partial class MongoHelper
    {
        private static FindOneAndUpdateOptions<IdentityFactory> options =
                new FindOneAndUpdateOptions<IdentityFactory>() { ReturnDocument = ReturnDocument.After, IsUpsert = true };

        public static async ETVoid TryCreateCollection<T>(this DBComponent self, long defaultValue)
        {
            var collection = self.GetCollection<IdentityFactory>();
            await collection.FindOneAndUpdateAsync(
                Builders<IdentityFactory>.Filter.Eq(f => f.Id, typeof(T).Name),
                Builders<IdentityFactory>.Update.SetOnInsert(f => f.Value, 1), options);
            
        }
        public static async ETTask CollectionExists<T>(this DBComponent self, string name, int defalut = 10000)
        {
            var filter = new BsonDocument("name", typeof(T).Name);
            var options = new ListCollectionNamesOptions { Filter = filter };
            var isExist = await self.database.ListCollectionNames(options).AnyAsync();
            var collection = self.GetCollection<IdentityFactory>();
            if (!isExist)
                await collection.InsertOneAsync(new IdentityFactory() { Id = name, Value = defalut });
        }
        public static async ETTask<long> GetIncrementId<T>(this DBComponent self)
        {
            var collection = self.GetCollection<IdentityFactory>();
            IdentityFactory id = await collection.FindOneAndUpdateAsync(
                Builders<IdentityFactory>.Filter.Eq(f => f.Id, typeof(T).Name),
                Builders<IdentityFactory>.Update.Inc(f => f.Value, 1), options);
            return id.Value;
        }

        public static long GetTokenId<T>(this DBComponent self, int defalut = 2000)
        {
            //return IdWorker.IdGen.nextId();
            var collection = self.GetCollection<IdentityFactory>();
            var identity = collection.Find(x => x.Id == typeof(T).Name).FirstOrDefault();
            if (identity == null || identity.Value < 1)
                collection.InsertOne(new IdentityFactory() { Id = typeof(T).Name, Value = defalut });
            IdentityFactory id = collection.FindOneAndUpdate(
                Builders<IdentityFactory>.Filter.Eq(f => f.Id, typeof(T).Name),
                Builders<IdentityFactory>.Update.Inc(f => f.Value, 1), options);
            return id.Value;
        }

        public static async ETTask<List<T>> Query<T>(this DBComponent self, Expression<Func<T, bool>> filter, string collection = null)
        //where T : Entity
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, RandomHelper.RandInt64() % DBComponent.TaskCount))
            {
                IAsyncCursor<T> cursor = await self.GetCollection<T>(collection).FindAsync(filter);

                return await cursor.ToListAsync();
            }
        }

        public static async ETTask<List<T>> Query<T>(this DBComponent self, long taskId, Expression<Func<T, bool>> filter, string collection = null)
        //where T : Entity
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, taskId % DBComponent.TaskCount))
            {
                IAsyncCursor<T> cursor = await self.GetCollection<T>(collection).FindAsync(filter);

                return await cursor.ToListAsync();
            }
        }

        public static async ETTask<T> QueryOne<T>(this DBComponent self, Expression<Func<T, bool>> filter, string collection = null)
        //where T : Entity
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, RandomHelper.RandInt64() % DBComponent.TaskCount))
            {
                IAsyncCursor<T> cursor = await self.GetCollection<T>(collection).FindAsync(filter);

                return await cursor.FirstOrDefaultAsync();
            }
        }

        public static async ETTask<T> QueryOne<T>(this DBComponent self, long taskId, Expression<Func<T, bool>> filter, string collection = null)
        //where T : Entity
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, taskId % DBComponent.TaskCount))
            {
                IAsyncCursor<T> cursor = await self.GetCollection<T>(collection).FindAsync(filter);

                return await cursor.FirstOrDefaultAsync();
            }
        }

        //public static async ETTask SaveTransaction(this DBComponent self, long id, List<Entity> entities)
        //{
        //    if (entities == null)
        //    {
        //        Log.Error($"save entity is null");
        //        return;
        //    }

        //    var session = self.mongoClient.StartSession();

        //    session.StartTransaction(new TransactionOptions(
        //        readConcern: ReadConcern.Snapshot,
        //        writeConcern: WriteConcern.WMajority));

        //    using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.DB, id % DBComponent.TaskCount))
        //    {
        //        try
        //        {
        //            foreach (Entity entity in entities)
        //            {
        //                if (entity == null)
        //                {
        //                    continue;
        //                }

        //                await self.GetCollection(entity.GetType().Name)
        //                        .ReplaceOneAsync(session, d => d.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = true });
        //            }
        //        }
        //        catch(Exception e)
        //        {
        //            session.AbortTransaction();
        //            throw;
        //        }

        //    }

        //    while (true)
        //    {
        //        try
        //        {
        //            session.CommitTransaction();
        //            break;
        //        }
        //        catch (MongoException e)
        //        {
        //            if (e.HasErrorLabel("UnknownTransactionCommitResult"))
        //            {
        //                Log.Info("UnknownTransactionCommitResult, retrying commit operation.");
        //                continue;
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //    }
        //}
    }
}
