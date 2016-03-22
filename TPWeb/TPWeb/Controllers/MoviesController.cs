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
                // NC récuppérer le input hidden qui contient les movie ID
                String[] MoviesIdItems = Request["Items"].Split(',');
                // NC
                foreach (String movieId in MoviesIdItems)
                {
                    if (!String.IsNullOrEmpty(movieId))
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
        // GET: /Movies/Edit/5
        public ActionResult Edit(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Movie movieToEdit = ((Movies)HttpRuntime.Cache["Movies"]).Get(int.Parse(id));

                //Make sure the contact exist
                if (movieToEdit != null)
                {
                    // Retreive from the Session dictionary the reference of the ContactsView instance
                    ActorsView ActorsView = (ActorsView)Session["ActorsView"];
                    MoviesView MoviesView = (MoviesView)Session["ContactsView"];

                    // Retreive from the Application dictionary the reference of the Friends instance
                    Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

                    // Store in ViewBag the friend list of the contact to edit
                    ViewBag.ActorsList = movieToEdit.GetActorsList(Parutions, ActorsView);

                    // Store in ViewBag the "not yet friend" contact list  of the contact to edit
                    ViewBag.NotYetActorsList = movieToEdit.GetNotYetActorsList(Parutions, ActorsView);

                    // Pass the contact to edit reference to this action view
                    return View(movieToEdit);
                }
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
            Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

             //Reset the contact friend list
            movie.ClearActorList(Parutions);

             //Extract the friends Id list from the hidden input 
             //FriendsListItems embedded in the Http post request
            String[] ActorsListItems = Request["Items"].Split(',');

             //Add friends to the Friends collection
            foreach (String actorId in ActorsListItems)
            {
                if (!String.IsNullOrEmpty(actorId))
                {
                    Parutions.Add(new Parution(movie.id, int.Parse(actorId)));
                }
            }
             //Return the Index action of this controller
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
                    ViewBag.ParutionsList = movieToView.GetActorsList((Parutions)HttpRuntime.Cache["Parutions"],
                                                                       (ActorsView)Session["ActorsView"]);
                    // Pass the reference of the contact to view to this action view
                    return View(movieToView);
                }
            }

            // Return the Index action of this controller
            return RedirectToAction("Index");
        }
    }
}
