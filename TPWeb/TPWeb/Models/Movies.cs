using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public String GetAvaterURL()
        {
            return defaultPoster.GetImageURL(poster);
        }

        public void UploadPicture(HttpRequestBase request)
        {
            poster = defaultPoster.UpLoadImage(request, poster);
        }

        public void RemovePicture()
        {
            defaultPoster.Remove(poster);
        }

        #endregion

    }
}