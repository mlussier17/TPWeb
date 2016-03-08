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

        [Display(Name = "Titre")]
        [StringLength(50), Required]
        public String title { get; set; }

        [Display(Name = "Description")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(100), Required]
        public String description { get; set; }

        [Display(Name = "Pays")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(50), Required]
        public String country { get; set; }

        [Display(Name = "Année")]
        [RegularExpression(@"^([0-9])",ErrorMessage = "Année invalide")]
        public int year { get; set; }

        [Display(Name = "Catégorie")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(50), Required]
        public String categorie { get; set; }

        [Display(Name = "Affiche")]
        public String poster { get; set; }

        [Display(Name = "Directeurs")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
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
            clone.description = this.description;
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
            movie.description = tokens[2];
            movie.country = tokens[3];
            movie.year = int.Parse(tokens[4]);
            movie.categorie = tokens[5];
            movie.poster = tokens[6];
            movie.directors = tokens[7];
            return movie;
        }

        public String toText()
        {
            return id.ToString() + SEPARATOR +
                   title + SEPARATOR +
                   description + SEPARATOR +
                   country + SEPARATOR +
                   year + SEPARATOR +
                   categorie + SEPARATOR +
                   poster + SEPARATOR +
                   directors+ SEPARATOR;
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
            while(!sr.EndOfStream)
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
            foreach(Movie m in List)
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

        public void Delete (int id)
        {
            int index = -1;
            for (int i = 0; i < List.Count; ++i)
            {
                if (List[i].id == id) index = id;
            }
            if (index < -1)
            {
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
                List[index].description = movie.description;
                List[index].year = movie.year;
                List[index].poster = movie.poster;
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
}