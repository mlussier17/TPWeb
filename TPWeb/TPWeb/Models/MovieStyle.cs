using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace TPWeb.Models
{
    public class MovieStyle
    {
        public int Id { get; set; }
        public String Name { get; set; }
        private const char SEPERATOR = '|';
        public static MovieStyle FromStreamTextLine(String streamTextLine)
        {
            MovieStyle movieStyle = new MovieStyle();
            String[] Tokens = streamTextLine.Split(SEPERATOR);
            movieStyle.Id = int.Parse(Tokens[0]);
            movieStyle.Name = Tokens[1];
            return movieStyle;
        }
    }
    public class MovieStyles
    {
        private List<MovieStyle> List = new List<MovieStyle>();
        private String FilePath;
        public MovieStyles(String filePath)
        {
            FilePath = filePath;
            Read();
        }
        private void Read()
        {
            StreamReader sr = new StreamReader(FilePath, Encoding.Unicode);
            List.Clear();
            while (!sr.EndOfStream)
            {
                List.Add(MovieStyle.FromStreamTextLine(sr.ReadLine()));
            }
            sr.Close();
            List.Sort(new MovieStyleComparer());
        }
        public List<MovieStyle> ToList()
        {
            return List;
        }
        public int GetId(String Name)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Name == Name)
                {
                    return List[i].Id;
                }
            }
            return 0;
        }

        public MovieStyle Get(String Id)
        {
            return Get(int.Parse(Id));
        }
        public MovieStyle Get(int Id)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Id == Id)
                {
                    return List[i];
                }
            }
            return null;
        }
        public String GetName(String Id)
        {
            return GetName(int.Parse(Id));
        }
        public String GetName(int Id)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Id == Id)
                {
                    return List[i].Name;
                }
            }
            return "";
        }
    }
    class MovieStyleComparer : IComparer<MovieStyle>
    {
        public int Compare(MovieStyle x, MovieStyle y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
}