using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPWeb.Models;

namespace TPWeb.Controllers
{
    public class ActorsController : Controller
    {
        public IEnumerable<SelectListItem> GetCountries()
        {
            Countries cts = (Countries)HttpRuntime.Cache["Countries"];
            var allFlavors = cts.ToList().Select(f => new SelectListItem
                                                    {
                                                        Value = f.Id.ToString(),
                                                        Text = f.Name           
                                                    });
            return allFlavors;         
        }
        //
        // GET: /Actors/
        public ActionResult Index()
        {
            string sortfield = this.Request.QueryString["sortField"];

            if (sortfield != null)
            {
                ActorsView av = (ActorsView)Session["ActorsView"];
                av.Sort(sortfield);
                return RedirectToAction("Index");
            }

            return View(((ActorsView)Session["ActorsView"]).ToList());
        }

        //
        // GET: /Actors/Create
        public ActionResult Create()
        {
            ViewBag.Actors = ((ActorsView)Session["ActorsView"]).ToList();
            ViewBag.Movies = ((MoviesView)Session["MoviesView"]).ToList();
            ViewBag.CountryList = GetCountries();

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
                // NC récuppérer le input hidden qui contient les movie ID
                String[] MoviesIdItems = Request["Items"].Split(',');
                // NC
                foreach (String movieId in MoviesIdItems)
                {
                    if (!String.IsNullOrEmpty(movieId))
                    {
                        parutions.Add(new Parution(int.Parse(movieId), newActorId));
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
        // GET: /Actors/Edit/{id}
        public ActionResult Edit(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                Actor actorToEdit = ((Actors)HttpRuntime.Cache["Actors"]).Get(int.Parse(id));

                // Make sure the contact exist
                if (actorToEdit != null)
                {
                    // Retreive from the Session dictionary the reference of the ContactsView instance
                    MoviesView MoviesView = (MoviesView)Session["MoviesView"];

                    // Retreive from the Application dictionary the reference of the Friends instance
                    Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

                    // Store in ViewBag the friend list of the contact to edit
                    ViewBag.MoviesList = actorToEdit.GetMoviesList(Parutions, MoviesView);

                    // Store in ViewBag the "not yet friend" contact list  of the contact to edit
                    ViewBag.NotYetMoviesList = actorToEdit.GetNotYetMoviesList(Parutions, MoviesView);

                    // Store in ViewBag the countries
                    ViewBag.CountryList = GetCountries();

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

            // Update the actor
            ((Actors)HttpRuntime.Cache["Actors"]).Update(actor);

            // Retreive from the Application dictionary the reference of the Friends instance
            Parutions Parutions = (Parutions)HttpRuntime.Cache["Parutions"];

            // Reset the contact friend list
            actor.ClearMovieList(Parutions);

            // Extract the friends Id list from the hidden input 
            // FriendsListItems embedded in the Http post request
            String[] MoviesListItems = Request["Items"].Split(',');

            // Add friends to the Friends collection
            foreach (String movieId in MoviesListItems)
            {
                if (!String.IsNullOrEmpty(movieId))
                {
                    Parutions.Add(new Parution(int.Parse(movieId), actor.id));
                }
            }
            // Return the Index action of this controller
            return RedirectToAction("Details", new { id = actor.id });
        }

        //
        // GET: /Actors/Delete/5
        public ActionResult Delete(String id)
        {
            // Make sure that this action is called with an id
            if (!String.IsNullOrEmpty(id))
            {
                Actor actorToDelete = ((Actors)HttpRuntime.Cache["Actors"]).Get(int.Parse(id));
                // Retreive from the Application dictionary the reference of the Friends instance
                Parutions parutions = (Parutions)HttpRuntime.Cache["Parutions"];
                // Clear the contact friend list
                actorToDelete.ClearMovieList(parutions);
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
                    ViewBag.ParutionsList = actorToView.GetMoviesList((Parutions)HttpRuntime.Cache["Parutions"],
                                                                       (MoviesView)Session["MoviesView"]);
                    // Pass the reference of the contact to view to this action view
                    return View(actorToView);
                }
            }

            // Return the Index action of this controller
            return RedirectToAction("Index");
        }
    }
}
