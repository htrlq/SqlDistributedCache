using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlDistributedCache.Model
{
    public class EntityItem
    {
        public long Id { get; set; }

        public string Key { get; set; }

        [NotMapped]
        public bool IsLock => EndTime < DateTimeOffset.Now;

        [NotMapped]
        public bool IsDelete => DateTimeOffset.Now.Subtract(EndTime).Days > 0;

        public byte[] Value { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        [Timestamp]
        public byte[] Timestamp { get; set; }
    }
}
