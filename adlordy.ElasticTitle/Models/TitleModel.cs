using System;

namespace adlordy.ElasticTitle.Models
{
    public class TitleModel
    {
        public TitleModel()
        {
        }
        public TitleModel(DateTime timestamp, string title)
        {
            Timestamp = timestamp;
            Title = title;
        }

        public long Id
        {
            get
            {
                return Timestamp.Ticks;
            }
            set
            {
                Timestamp = new DateTime(value);
            }
        }
        public DateTime Timestamp { get; set; }
        public string Title { get; set; }
    }
}
