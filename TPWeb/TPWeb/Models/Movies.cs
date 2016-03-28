using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace TPWeb.Models
{
    public class Movie
    {
        #region Variables
        private const char SEPARATOR = '|';

        public int id { get; set; }
        private int countryid;
        private int categorieid;

        [Display(Name = "Titre")]
        [StringLength(50), Required]
        public String title { get; set; }

        [Display(Name = "Synopsis")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9 àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(1024), Required]
        public String synopsis { get; set; }

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

        [Display(Name = "Année")]
        public int year { get; set; }

        [Display(Name = "Genre")]
        public String categorie
        {
            get
            {
                MovieStyles cts = (MovieStyles)HttpRuntime.Cache["MovieStyles"];
                return cts.GetName(categorieid);
            }
            set
            {
                MovieStyles cts = (MovieStyles)HttpRuntime.Cache["MovieStyles"];
                countryid = cts.GetId(value);
            }
        }


        [Display(Name = "Genre")]
        public int categorieID
        {
            get
            {
                return categorieid;
            }
            set
            {
                categorieid = value;
            }
        }

        [Display(Name = "Affiche")]
        public String poster { get; set; }

        [Display(Name = "Realisateur")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9 àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(100), Required]
        public String directors { get; set; }

        public ImageGUIDReference defaultPoster;
        #endregion

        #region Construction
        public Movie()
        {
            defaultPoster = new ImageGUIDReference(@"~/Images/Posters/", @"DefaultPoster.jpg");
        }

        public Movie Clone()
        {
            Movie clone = new Movie();
            clone.id = this.id;
            clone.title = this.title;
            clone.synopsis = this.synopsis;
            clone.country = this.country;
            clone.year = this.year;
            clone.categorie = this.categorie;
            clone.poster = this.poster;
            clone.directors = this.directors;
            return clone;
        }
        #endregion

        #region TextModifiers
        public static Movie fromText(String textLine)
        {
            Movie movie = new Movie();
            String[] tokens = textLine.Split(SEPARATOR);

            movie.id = int.Parse(tokens[0]);
            movie.title = tokens[1];
            movie.synopsis = tokens[2];
            movie.countryid = int.Parse(tokens[3]);
            movie.year = int.Parse(tokens[4]);
            movie.categorieid = int.Parse(tokens[5]);
            movie.poster = tokens[6];
            movie.directors = tokens[7];
            return movie;
        }

        public String toText()
        {
            return id.ToString() + SEPARATOR +
                   title + SEPARATOR +
                   synopsis + SEPARATOR +
                   countryid + SEPARATOR +
                   year + SEPARATOR +
                   categorieid + SEPARATOR +
                   poster + SEPARATOR +
                   directors + SEPARATOR;
        }
        #endregion

        #region Poster

        public String GetPosterURL()
        {
            return defaultPoster.GetImageURL(poster);
        }

        public void UploadPoster(HttpRequestBase request)
        {
            poster = defaultPoster.UpLoadImage(request, poster);
        }

        public void RemovePoster()
        {
            defaultPoster.Remove(poster);
        }

        #endregion

        #region actors/movies
        public List<Actor> GetActorsList(Parutions parutions, ActorsView actors)
        {
            List<Actor> ActorMoviesList = new List<Actor>();
            List<Parution> ParutionsList = parutions.ToList();

            foreach (Parution Parution in ParutionsList)
            {
                if (this.id == Parution.MovieId)
                    ActorMoviesList.Add(actors.Get(Parution.ActorId).Clone());
            }

            return ActorMoviesList;
        }

        public List<Actor> GetNotYetActorsList(Parutions parutions, ActorsView actors)
        {
            List<Actor> ContactNotYetActorList = new List<Actor>();
            List<Actor> ParutionsList = GetActorsList(parutions, actors);
            List<Actor> ActorList = actors.ToList();

            foreach (Actor Actor in ActorList)
            {
                bool played = false;
                foreach (Actor actor in ParutionsList)
                {
                    if (Actor.id == actor.id)
                    {
                        played = true;
                        break;
                    }
                }
                if (!played)
                    ContactNotYetActorList.Add(actors.Get(Actor.id).Clone());
            }

            return ContactNotYetActorList;
        }

        public void ClearActorList(Parutions parutions)
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

    public class Movies
    {
        public DateTime lastUpdate { get { return _lastUpdate; } }
        private DateTime _lastUpdate = DateTime.Now;
        public String filePath;
        private List<Movie> List = new List<Movie>();

        public Movies(String oFilePath)
        {
            filePath = oFilePath;
            Read();
        }

        #region Read & Save
        private void Read()
        {
            StreamReader sr = new StreamReader(filePath, Encoding.Unicode);

            List.Clear();
            while (!sr.EndOfStream)
            {
                List.Add(Movie.fromText(sr.ReadLine()));
            }
            sr.Close();
            _lastUpdate = DateTime.Now;
        }

        private void Save()
        {
            StreamWriter sw = new StreamWriter(filePath, false, Encoding.Unicode);

            foreach (Movie movie in List)
            {
                sw.WriteLine(movie.toText());
            }
            sw.Close();
            _lastUpdate = DateTime.Now;
        }
        #endregion

        #region List
        public List<Movie> ToList()
        {
            return List;
        }
        public List<Movie> CloneList()
        {
            List<Movie> clone = new List<Movie>();
            foreach (Movie movie in List)
            {
                clone.Add(movie.Clone());
            }
            return clone;
        }
        #endregion

        #region CRUD

        public int Add(Movie movie)
        {
            int maxId = 0;
            foreach (Movie m in List)
            {
                if (m.id > maxId) maxId = m.id;
            }
            movie.id = maxId + 1;
            List.Add(movie);
            Save();
            return movie.id;
        }

        public void Delete(String id)
        {
            Delete(int.Parse(id));
        }

        public void Delete(int Id)
        {
            int index = -1;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].id == Id)
                    index = i;
            }
            if (index > -1)
            {
                Parutions currentParution = (Parutions)HttpRuntime.Cache["Parutions"];
                currentParution.DeleteMovie(List[index].id);
                List[index].RemovePoster();
                List.RemoveAt(index);
                Save();
            }
        }

        public void Update(Movie movie)
        {
            int index = -1;
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].id == movie.id)
                    index = i;
            }
            if (index > -1)
            {
                List[index].title = movie.title;
                List[index].country = movie.country;
                List[index].synopsis = movie.synopsis;
                List[index].year = movie.year;
                if (movie.poster != null) List[index].poster = movie.poster;
                List[index].categorie = movie.categorie;
                List[index].directors = movie.directors;

                Save();
            }
        }

        public Movie Get(int Id)
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

        #endregion
    }

    class MovieComparer : IComparer<Movie>
    {
        #region Sort parameters
        private String sortField = "";
        private bool ascending = true;
        #endregion

        #region Construction
        public MovieComparer(String sortField, bool ascending = true)
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
        public int Compare(Movie x, Movie y)
        {
            switch (sortField)
            {
                case "title":
                    if (ascending)
                        return string.Compare(x.title, y.title);
                    else
                        return string.Compare(y.title, x.title);

                case "description":
                    if (ascending)
                        return string.Compare(x.synopsis, y.synopsis);
                    else
                        return string.Compare(y.synopsis, x.synopsis);

                case "country":
                    if (ascending)
                        return string.Compare(x.country, y.country);
                    else
                        return string.Compare(y.country, x.country);
                case "year":
                    if (ascending)
                        return IntCompare(x.year, y.year);
                    else
                        return IntCompare(y.year, x.year);

                case "categorie":
                    if (ascending)
                        return string.Compare(x.categorie, y.categorie);
                    else
                        return string.Compare(y.categorie, x.categorie);

                case "directors":
                    if (ascending)
                        return string.Compare(x.directors, y.directors);
                    else
                        return string.Compare(y.directors, x.directors);
            }

            return 0;
        }
        #endregion
    }

    public class MoviesView
    {
        private List<Movie> List = new List<Movie>();
        private DateTime LastUpdate = new DateTime(0);
        private bool ascending = true;
        private String sortField = "";
        private Movies movies = null;
        private bool toggleSortDirection = true;

        public MoviesView(Movies movies, String FieldToSort = "")
        {
            this.movies = movies;
            sortField = FieldToSort;
            UpdateList();
        }

        public List<Movie> ToList()
        {
            UpdateList();
            return List;
        }

        private void UpdateList()
        {
            if (movies.lastUpdate > LastUpdate)
            {
                List.Clear();
                List = movies.CloneList();
                LastUpdate = movies.lastUpdate;
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
                List.Sort(new MovieComparer(sortField, ascending));
            }
        }
        public Movie Get(int Id)
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