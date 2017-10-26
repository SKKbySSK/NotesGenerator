using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace NotesPlayer.Ranking
{
    public static class Saved
    {
        static Lazy<ResultCollection> res = new Lazy<ResultCollection>(() =>
        {
            try
            {
                if (File.Exists(Constants.RankindFileName))
                {
                    using (StreamReader sr = new StreamReader(Constants.RankindFileName))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(ResultCollection));
                        return (ResultCollection)ser.Deserialize(sr);
                    }
                }
                else
                    return new ResultCollection();
            }
            catch (Exception)
            {
                return new ResultCollection();
            }
        });

        public static ResultCollection Results
        {
            get { return res.Value; }
        }

        public static void Save()
        {
            using(StreamWriter sw = new StreamWriter(Constants.RankindFileName))
            {
                XmlSerializer ser = new XmlSerializer(typeof(ResultCollection));
                ser.Serialize(sw, Results);
            }
        }
    }
}
