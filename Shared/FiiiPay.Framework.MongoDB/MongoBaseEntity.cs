using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FiiiPay.Framework.MongoDB
{
    /// <summary>
    /// 基类实体
    /// </summary>
    public class MongoBaseEntity
    {
        [BsonId]
        public ObjectId _id { get; set; }

        ///// <summary>
        ///// Gets or sets the identifier.
        ///// </summary>
        ///// <value>
        ///// The identifier.
        ///// </value>
        //[BsonId]
        //public ObjectId MongoObjectId { get; set; }

        /// <summary>
        /// Gets or sets the create time.
        /// </summary>
        /// <value>
        /// The create time.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the update time.
        /// </summary>
        /// <value>
        /// The update time.
        /// </value>
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTime { get; set; }
    }
}
