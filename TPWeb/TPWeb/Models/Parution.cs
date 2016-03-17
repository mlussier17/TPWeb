using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace TPWeb.Models
{
    public class Parution
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        public Parution()
        {
            Id = 0;
            MovieId = 0;
            ActorId = 0;
        }

        public Parution(int ContactId, int FriendId)
        {
            Id = 0;
            this.MovieId = ContactId;
            this.ActorId = FriendId;
        }

        public Parution Clone()
        {
            Parution clone = new Parution();
            clone.Id = this.Id;
            clone.MovieId = this.MovieId;
            clone.ActorId = this.ActorId;
            return clone;
        }

        private const char SEPERATOR = '|';

        public static Parution FromStreamTextLine(String streamTextLine)
        {
            Parution parution = new Parution();
            String[] Tokens = streamTextLine.Split(SEPERATOR);

            parution.Id = int.Parse(Tokens[0]);
            parution.MovieId = int.Parse(Tokens[1]);
            parution.ActorId = int.Parse(Tokens[2]);

            return parution;
        }

        public String ToStreamTextLine()
        {
            return Id.ToString() + SEPERATOR +
                    MovieId.ToString() + SEPERATOR +
                    ActorId.ToString() + SEPERATOR;
        }
    }

    public class Parutions
    {
        public DateTime LastUpdate { get { return _LastUpdate; } }

        #region Construction
        public Parutions(String filePath)
        {
            FilePath = filePath;
            Read();
        }
        #endregion

        #region Locking handlers
        private bool locked = false;

        private void WaitForUnlocked()
        {
            while (locked) { /* do nothing */}
        }

        private void Lock()
        {
            WaitForUnlocked();
            locked = true;
        }

        private void UnLock()
        {
            locked = false;
        }
        #endregion

        #region File IO handlers

        String FilePath;
        private DateTime _LastUpdate = DateTime.Now;

        private void Read()
        {
            WaitForUnlocked();
            StreamReader sr = new StreamReader(FilePath, Encoding.Unicode);

            List.Clear();
            while (!sr.EndOfStream)
            {
                List.Add(Parution.FromStreamTextLine(sr.ReadLine()));
            }
            sr.Close();
            _LastUpdate = DateTime.Now;
        }

        private void Save()
        {
            StreamWriter sw = new StreamWriter(FilePath, false /*erase*/, Encoding.Unicode);

            foreach (Parution Parution in List)
            {
                sw.WriteLine(Parution.ToStreamTextLine());
            }
            sw.Close();
            _LastUpdate = DateTime.Now;
        }
        #endregion

        #region Friends list handlers
        private List<Parution> List = new List<Parution>();

        public List<Parution> ToList()
        {
            WaitForUnlocked();
            return List;
        }

        public List<Parution> CloneList()
        {
            WaitForUnlocked();
            List<Parution> clone = new List<Parution>();
            foreach (Parution Friend in List)
            {
                clone.Add(Friend.Clone());
            }
            return clone;
        }
        #endregion

        #region CRUD queries
        public void Add(Parution Parution)
        {
            Lock();
            int maxId = 0;

            foreach (Parution p in List)
            {
                if (p.Id > maxId)
                    maxId = p.Id;
            }
            Parution.Id = maxId + 1;
            List.Add(Parution);
            Save();
            UnLock();
        }

        public void Delete(String id)
        {
            Delete(int.Parse(id));
        }

        public void Delete(int Id)
        {
            Lock();
            int index = -1;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Id == Id)
                    index = i;
            }
            if (index > -1)
            {
                List.RemoveAt(index);
                Save();
            }
            UnLock();
        }

        public Parution Get(int Id)
        {
            Lock();
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].Id == Id)
                {
                    UnLock();
                    return List[i];
                }
            }
            UnLock();
            return null;
        }

        #endregion
    }
}