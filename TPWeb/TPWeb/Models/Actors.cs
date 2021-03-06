﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TPWeb.Models
{
    public class Actor
    {
        #region Variables
        private const char SEPARATOR = '|';

        private int countryid;

        public int id { get; set; }

        [Display(Name = "Nom")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9 àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(50), Required]
        public String name { get; set; }

        [Display(Name = "Pays")]
        public String country 
        { 
            get 
            {
                Countries cts = (Countries)HttpRuntime.Cache["Countries"];
                return cts.GetName(countryid);
            }
            set
            {
                Countries cts = (Countries)HttpRuntime.Cache["Countries"];
                countryid = cts.GetId(value);
            }
        }


        [Display(Name = "Pays")]
        public int countryID
        {
            get
            {
                return countryid;
            }
            set
            {
                countryid = value;
            }
        }

        [Display(Name = "Naissance")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Photo")]
        public String pictureId { get; set; }

        
        #endregion

        #region Construction
        public Actor()
        {
            defaultPicture = new ImageGUIDReference(@"~/Images/Pictures/", @"Anonymous.png");
            BirthDay = DateTime.Now;
        }
        public Actor Clone()
        {
            Actor clone = new Actor();
            clone.id = this.id;
            clone.name = this.name;
            clone.country = this.country;
            clone.BirthDay = new DateTime(this.BirthDay.Ticks);
            clone.pictureId = this.pictureId;
            return clone;
        }

        #endregion

        #region TextModifiers
        public static Actor fromText(String textLine)
        {
            Actor actor = new Actor();
            String[] tokens = textLine.Split(SEPARATOR);

            actor.id = int.Parse(tokens[0]);
            actor.name = tokens[1];
            actor.countryid = int.Parse(tokens[2]);
            if (!String.IsNullOrEmpty(tokens[3]))
                try
                {
                    actor.BirthDay = DateTime.Parse(tokens[3]);
                }
                catch (Exception e)
                {
                    actor.BirthDay = DateTime.Now;
                }
            else
                actor.BirthDay = DateTime.Now;

            actor.pictureId = tokens[4];
            
            return actor;
        }

        public String toText()
        {
            return id.ToString() + SEPARATOR +
                   name + SEPARATOR +
                   countryid + SEPARATOR +
                   BirthDay + SEPARATOR +
                   pictureId + SEPARATOR;
        }
        #endregion

        #region Picture
        private ImageGUIDReference defaultPicture;

        public String GetAvatarURL()
        {
            return defaultPicture.GetImageURL(pictureId);
        }

        public void UploadPicture(HttpRequestBase request)
        {
            pictureId = defaultPicture.UpLoadImage(request, pictureId);
        }

        public void RemovePicture()
        {
            defaultPicture.Remove(pictureId);
        }
        #endregion

        #region actors/movies
        public List<Movie> GetMoviesList(Parutions parutions, MoviesView movies)
        {
            List<Movie> ActorMoviesList = new List<Movie>();
            List<Parution> ParutionsList = parutions.ToList();

            foreach (Parution Parution in ParutionsList)
            {
                if (this.id == Parution.ActorId)
                    ActorMoviesList.Add(movies.Get(Parution.MovieId).Clone());
            }

            return ActorMoviesList;
        }

        public List<Movie> GetNotYetMoviesList(Parutions parutions, MoviesView movies)
        {
            List<Movie> ContactNotYetActorList = new List<Movie>();
            List<Movie> ParutionsList = GetMoviesList(parutions, movies);
            List<Movie> MovieList = movies.ToList();

            foreach (Movie Movie in MovieList)
            {
                bool played = false;
                foreach (Movie movie in ParutionsList)
                {
                    if (Movie.id == movie.id)
                    {
                        played = true;
                        break;
                    }
                }
                if (!played)
                    ContactNotYetActorList.Add(movies.Get(Movie.id).Clone());
            }

            return ContactNotYetActorList;
        }

        public void ClearMovieList(Parutions parutions)
        {
            bool checkForMore;
            do
            {
                List<Parution> ParutionList = parutions.ToList();
                checkForMore = false;
                foreach (Parution Parution in ParutionList)
                {
                    if (this.id == Parution.ActorId)
                    {
                        parutions.Delete(Parution.Id);
                        checkForMore = true;
                        break;
                    }
                }
            } while (checkForMore);
        }
        #endregion

    }

    class ActorComparer : IComparer<Actor>
    {
        #region Sort parameters
        private String sortField = "";
        private bool ascending = true;
        #endregion

        #region Construction
        public ActorComparer(String sortField, bool ascending = true)
        {
            this.sortField = sortField;
            this.ascending = ascending;
        }
        #endregion

        #region Typed comparers
        private int IntCompare(int x, int y)
        {
            if (x > y) return 1;
            if (x < y) return -1;
            return 0;
        }

        private int DateCompare(DateTime x, DateTime y)
        {
            if (x > y) return 1;
            if (x < y) return -1;
            return 0;
        }
        #endregion

        #region Comparer selector
        public int Compare(Actor x, Actor y)
        {
            switch (sortField)
            {
                case "name":
                    if (ascending)
                        return string.Compare(x.name, y.name);
                    else
                        return string.Compare(y.name, x.name);

                case "country":
                    if (ascending)
                        return string.Compare(x.country, y.country);
                    else
                        return string.Compare(y.country, x.country);

                case "birthday":
                    if (ascending)
                        return DateCompare(x.BirthDay, y.BirthDay);
                    else
                        return DateCompare(y.BirthDay, x.BirthDay);
            }

            return 0;
        }
        #endregion
    }

    public class Actors
    {
        public DateTime LastUpdate { get { return _LastUpdate; } }

        #region Construction
        public Actors(String filePath)
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
                List.Add(Actor.fromText(sr.ReadLine()));
            }
            sr.Close();
            _LastUpdate = DateTime.Now;
        }

        private void Save()
        {
            StreamWriter sw = new StreamWriter(FilePath, false /*erase*/, Encoding.Unicode);

            foreach (Actor contact in List)
            {
                sw.WriteLine(contact.toText());
            }
            sw.Close();
            _LastUpdate = DateTime.Now;
        }
        #endregion

        #region Contact list handlers
        private List<Actor> List = new List<Actor>();

        public List<Actor> ToList()
        {
            WaitForUnlocked();
            return List;
        }

        public List<Actor> CloneList()
        {
            WaitForUnlocked();
            List<Actor> clone = new List<Actor>();
            foreach (Actor actor in List)
            {
                clone.Add(actor.Clone());
            }
            return clone;
        }
        #endregion

        #region CRUD queries
        public int Add(Actor actor)
        {
            Lock();
            int maxId = 0;

            foreach (Actor a in List)
            {
                if (a.id > maxId)
                    maxId = a.id;
            }
            actor.id = maxId + 1;
            List.Add(actor);
            Save();
            UnLock();
            return actor.id;
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
                if (List[i].id == Id)
                    index = i;
            }
            if (index > -1)
            {
                Parutions currentParution = (Parutions)HttpRuntime.Cache["Parutions"];
                currentParution.DeleteActor(List[index].id);
                List[index].RemovePicture();
                List.RemoveAt(index);
                Save();
            }
            UnLock();
        }

        public void Update(Actor actor)
        {
            Lock();
            int index = -1;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].id == actor.id)
                    index = i;
            }
            if (index > -1)
            {
                List[index].name = actor.name;
                List[index].country = actor.country;
                if (actor.pictureId != null)
                {
                    if (List[index].pictureId != actor.pictureId) List[index].RemovePicture(); // Delete old
                    List[index].pictureId = actor.pictureId; 
                }
                List[index].BirthDay = new DateTime(actor.BirthDay.Ticks);

                Save();
            }
            UnLock();
        }

        public Actor Get(int Id)
        {
            Lock();
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].id == Id)
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

    public class ActorsView
    {
        private List<Actor> List = new List<Actor>();
        private DateTime LastUpdate = new DateTime(0);
        private bool ascending = true;
        private String sortField = "";
        private Actors actors = null;
        private bool toggleSortDirection = true;

        public ActorsView(Actors actors, String FieldToSort = "")
        {
            this.actors = actors;
            sortField = FieldToSort;
            UpdateList();
        }

        public List<Actor> ToList()
        {
            UpdateList();
            return List;
        }

        private void UpdateList()
        {
            if (actors.LastUpdate > LastUpdate)
            {
                List.Clear();
                List = actors.CloneList();
                LastUpdate = actors.LastUpdate;
                toggleSortDirection = false;
                Sort(sortField);
            }
        }

        public void Sort(String FieldToSort = "")
        {
            UpdateList();
            if (!String.IsNullOrEmpty(FieldToSort))
            {
                if (sortField != FieldToSort)
                {
                    sortField = FieldToSort;
                    ascending = true;
                    toggleSortDirection = true;
                }
                else
                {
                    if (toggleSortDirection)
                        ascending = !ascending;
                    toggleSortDirection = true;
                }
                List.Sort(new ActorComparer(sortField, ascending));
            }
        }
        public Actor Get(int Id)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].id == Id)
                {
                    return List[i];
                }
            }
            return null;
        }
    }
    
}