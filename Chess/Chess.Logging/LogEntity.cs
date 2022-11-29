using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Logging
{
    public class LogEntity
    {
        public int Id { get; set; } = 0;

        public DateTime DateTimeStamp { get; } = DateTime.Now;

        public LogEntity(int id, DateTime dateTimeStamp)
        {
            Id = id;
            DateTimeStamp = dateTimeStamp;
        }
    }
}
