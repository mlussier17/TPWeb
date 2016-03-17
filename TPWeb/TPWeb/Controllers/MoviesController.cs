using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPWeb.Models;

namespace TPWeb.Controllers
{
    public class MoviesController : Controller
    {
        //
        // GET: /Movies/
        public ActionResult Index()
        {
            return View(((MoviesView)Session["MoviesView"]).ToList());
        }

        //
        // GET: /Movies/Create
        public ActionResult Create()
        {
            ViewBag.Movies = ((MoviesView)Session["MoviesView"]).ToList();
            ViewBag.Actors = ((ActorsView)Session["ActorsView"]).ToList();

            // Create a new instance of contact and pass it to this action view
            Movie movie = new Movie();
            return View(movie);
        }

        //
        // POST: /Movies/Create
        [HttpPost]
        public ActionResult Create(Movie movie)
        {
            try
            {
                Movies movies = (Movies)HttpRuntime.Cache["Movies"];

                movie.UploadPoster(Request);

                int newActorId = movies.Add(movie);

                Parutions parutions = (Parutions)HttpRuntime.Cache["Parutions"];

                String[] ParutionsList = Request["ParutionsList"].Split(',');

                foreach(String parutionId in ParutionsList)
                {
                    if (!String.IsNullOrEmpty(parutionId))
                    {
                        parutions.Add(new Parution(newActorId, int.Parse(parutionId)));
                    }
                }
                int newContactId = movies.Add(movie);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Movies/Edit/5
        public ActionResult Edit(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Movie movieToEdit = ((Movies)HttpRuntime.Cache["Movies"]).Get(int.Parse(id));

                // Make sure the contact exist
                //if (movieToEdit != null)
                //{
                //    // Retreive from the Session dictionary the reference of the ContactsView instance
                //    MoviesView MoviesView = (MoviesView)Session["MoviesView"];

                //    // Retreive from the Application dictionary the reference of the Friends instance
                //    Playeds playeds = (Playeds)HttpRuntime.Cache["Playeds"];

                //    // Store in ViewBag the friend list of the contact to edit
                //    ViewBag.PlayedsList = movieToEdit.GetPlayedsList(playeds, MoviesView);

                //    // Store in ViewBag the "not yet friend" contact list  of the contact to edit
                //    ViewBag.NotYetFriendsList = contactToEdit.GetNotYetFriendsList(Friends, ContactsView);

                //    // Pass the contact to edit reference to this action view
                //    return View(movieToEdit);
                //}
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Movies/Edit/5
        [HttpPost]
        public ActionResult Edit(Movie movie)
        {
            movie.UploadPoster(Request);

            // Update the contact
            ((Movies)HttpRuntime.Cache["Movies"]).Update(movie);

            // Retreive from the Application dictionary the reference of the Friends instance
            //Playeds playeds = (playeds)HttpRuntime.Cache["Friends"];

            // Reset the contact friend list
            //contact.ClearFriendList(Friends);

            // Extract the friends Id list from the hidden input 
            // FriendsListItems embedded in the Http post request
            //String[] FriendsListItems = Request["FriendsListItems"].Split(',');

            // Add friends to the Friends collection
            //foreach (String friendId in FriendsListItems)
            //{
            //    if (!String.IsNullOrEmpty(friendId))
            //    {
            //        Friends.Add(new Friend(contact.Id, int.Parse(friendId)));
            //    }
            //}
            // Return the Index action of this controller
            return RedirectToAction("Index");
        }

        //
        // GET: /Movies/Delete/5
        public ActionResult Delete(String id)
        {
            // Make sure that this action is called with an id
            if (!String.IsNullOrEmpty(id))
            {
                ((Movies)HttpRuntime.Cache["Movies"]).Delete(id);
            }
            // Return the Index action of this controller
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Details(String id)
        {
            // Make sure that this action is called with an id
            if (!String.IsNullOrEmpty(id))
            {
                Movie movieToView = ((Movies)HttpRuntime.Cache["Movies"]).Get(int.Parse(id));
                if (movieToView != null)
                {
                    // Pass the reference of the contact to view to this action view
                    return View(movieToView);
                }
            }

            // Return the Index action of this controller
            return RedirectToAction("Index");
        }
    }
}
