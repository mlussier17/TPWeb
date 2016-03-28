using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace TPWeb.Models
{
    public class Country
    {
        public int Id { get; set; }
        public String Name { get; set; }
        private const char SEPERATOR = '|';
        public static Country FromStreamTextLine(String streamTextLine)
        {
            Country country = new Country();
            String[] Tokens = streamTextLine.Split(SEPERATOR);
            country.Id = int.Parse(Tokens[0]);
            country.Name = Tokens[1];
            return country;
        }

        public string ToString()
        {
            return Name;
        }
    }

    public class Countries
    {
        private List<Country> List = new List<Country>();
        private String FilePath;
        public Countries(String filePath)
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
                List.Add(Country.FromStreamTextLine(sr.ReadLine()));
            }
            sr.Close();
            List.Sort(new CountryComparer());
        }
        public List<Country> ToList()
        {
            return List;
        }
        public Country Get(String Id)
        {
            return Get(int.Parse(Id));
        }
        public Country Get(int Id)
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
    class CountryComparer : IComparer<Country>
    {
        public int Compare(Country x, Country y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
}