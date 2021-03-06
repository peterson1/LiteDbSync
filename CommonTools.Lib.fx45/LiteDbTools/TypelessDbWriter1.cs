﻿using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace CommonTools.Lib.fx45.LiteDbTools
{
    public class TypelessDbWriter1 : TypelessDbReader1
    {
        public void Insert(string dbFilepath, string collectionName, List<string> records)
        {
            using (var db = CreateConnection(dbFilepath))
            {
                var coll = db.GetCollection(collectionName);
                var docs = records.Select(json => Deserialize(json));
                coll.InsertBulk(docs);
            }
        }

        private BsonDocument Deserialize (string json)
        {
            var bVal = JsonSerializer.Deserialize(json);
            return bVal.AsDocument;
        }


        protected override string GetConnectionString(string filepath)
        {
            return $"Filename={filepath};Mode=Shared;";
        }
    }
}
