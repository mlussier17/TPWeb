using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPWeb.Models;

namespace TPWeb.Controllers
{
    public class ActorsController : Controller
    {
        //
        // GET: /Actors/
        public ActionResult Index()
        {
            return View(((ActorsView)Session["ActorsView"]).ToList());
        }

        //
        // GET: /Actors/Create
        public ActionResult Create()
        {
            ViewBag.Actors = ((ActorsView)Session["ActorsView"]).ToList();
            ViewBag.Movies = ((MoviesView)Session["MoviesView"]).ToList();

            // Create a new instance of contact and pass it to this action view
            Actor Actor = new Actor();
            return View(Actor);
        }

        //
        // POST: /Actors/Create
        [HttpPost]
        public ActionResult Create(Actor actor)
        {
            try
            {
                Actors actors = (Actors)HttpRuntime.Cache["Actors"];
                Movies movies = (Movies)HttpRuntime.Cache["Movies"];

                actor.UploadPicture(Request);

                int newActorId = actors.Add(actor);

                Parutions parutions = (Parutions)HttpRuntime.Cache["Parutions"];

                String[] MoviesListItems = Request["FriendsListItems"].Split(',');

                foreach(String movieId in MoviesListItems)
                {
                    if(!String.IsNullOrEmpty(movieId))
                    {
                        parutions.Add(new Parution(newActorId, int.Parse(movieId)));
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Actors/Edit/5
        public ActionResult Edit(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Actor actorToEdit = ((Actors)HttpRuntime.Cache["Actors"]).Get(int.Parse(id));

                // Make sure the contact exist
                if (actorToEdit != null)
                {
                    // Retreive from the Session dictionary the reference of the ContactsView instance
                    ActorsView ActorsView = (ActorsView)Session["ActorsView"];
                    MoviesView MoviesView = (MoviesView)Session["ContactsView"];

                    // Retreive from the Application dictionary the reference of the Friends instance
                    Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

                    // Store in ViewBag the friend list of the contact to edit
                    ViewBag.MoviesList = actorToEdit.GetMoviesList(Parutions, MoviesView);

                    // Store in ViewBag the "not yet friend" contact list  of the contact to edit
                    ViewBag.NotYetFriendsList = actorToEdit.GetNotYetMoviesList(Parutions, MoviesView);

                    // Pass the contact to edit reference to this action view
                    return View(actorToEdit);
                }
            }
            return RedirectToAction("Index");
        }
        

        //
        // POST: /Actors/Edit/5
        [HttpPost]
        public ActionResult Edit(Actor actor)
        {
            actor.UploadPicture(Request);

            // Update the contact
            ((Actors)HttpRuntime.Cache["Actors"]).Update(actor);

            // Retreive from the Application dictionary the reference of the Friends instance
            Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

            // Reset the contact friend list
            actor.ClearMovieList(Parutions);

            // Extract the friends Id list from the hidden input 
            // FriendsListItems embedded in the Http post request
            String[] MoviesListItems = Request["MoviesListItems"].Split(',');

            // Add friends to the Friends collection
            foreach (String movieId in MoviesListItems)
            {
                if (!String.IsNullOrEmpty(movieId))
                {
                    Parutions.Add(new Parution(actor.id, int.Parse(movieId)));
                }
            }
            // Return the Index action of this controller
            return RedirectToAction("Index");
        }

        //
        // GET: /Actors/Delete/5
        public ActionResult Delete(String id)
        {
            // Make sure that this action is called with an id
            if (!String.IsNullOrEmpty(id))
            {
                ((Actors)HttpRuntime.Cache["Actors"]).Delete(id);
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
                Actor actorToView = ((Actors)HttpRuntime.Cache["Actors"]).Get(int.Parse(id));
                if (actorToView != null)
                {
                    // Pass the reference of the contact to view to this action view
                    return View(actorToView);
                }
            }

            // Return the Index action of this controller
            return RedirectToAction("Index");
        }
    }
}
