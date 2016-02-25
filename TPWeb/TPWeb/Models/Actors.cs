using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPWeb.Models
{
    public class Actor
    {
        #region Variables
        private const char SEPARATOR = '|';

        public int id { get; set; }

        [Display(Name = "Nom")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-Z0-9àâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(50), Required]
        public String name { get; set; }

        [Display(Name = "Pays")]
        [RegularExpression(@"^((?!^Name$)[-a-zA-ZàâäçèêëéìîïòôöùûüÿñÀÂÄÇÈÊËÉÌÎÏÒÔÖÙÛÜ_'])+$", ErrorMessage = "Caractères illégaux.")]
        [StringLength(50), Required]
        public String country { get; set; }

        [Display(Name = "Naissance")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime birthday { get; set; }

        [Display(Name = "Photo")]
        public String pictureId { get; set; }

        public ImageGUIDReference defaultPicture;
        #endregion

        #region Construction
        public Actor()
        {
            defaultPicture = new ImageGUIDReference(@"~/Images/Pictures/", @"Default.png");
            birthday = DateTime.Now;
        }

        #endregion

        #region TextModifiers
        public static Actor fromText(String textLine)
        {
            Actor actor = new Actor();
            String[] tokens = textLine.Split(SEPARATOR);

            actor.id = int.Parse(tokens[0]);
            actor.name = tokens[1];
            actor.country = tokens[2];
            if (!String.IsNullOrEmpty(tokens[3]))
                try
                {
                    actor.birthday = DateTime.Parse(tokens[3]);
                }
                catch (Exception e)
                {
                    actor.birthday = DateTime.Now;
                }
            else
                actor.birthday = DateTime.Now;

            actor.pictureId = tokens[4];
            
            return actor;
        }

        public String toText()
        {
            return id.ToString() + SEPARATOR +
                   name + SEPARATOR +
                   country + SEPARATOR +
                   birthday + SEPARATOR +
                   pictureId + SEPARATOR;
        }
        #endregion

        #region Picture

        public String GetAvaterURL()
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
    }
    
}