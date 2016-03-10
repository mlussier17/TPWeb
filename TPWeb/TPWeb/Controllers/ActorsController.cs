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
        // GET: /Actors/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Actors/Create
        public ActionResult Create()
        {
            ViewBag.Actors = ((ActorsView)Session["ActorsView"]).ToList();

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

                actor.UploadPicture(Request);
                int newContactId = actors.Add(actor);

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
                    ActorsView ContactsView = (ActorsView)Session["ContactsView"];

                    // Retreive from the Application dictionary the reference of the Friends instance
                    //Friends Friends = (Friends)HttpRuntime.Cache["Friends"];

                    // Store in ViewBag the friend list of the contact to edit
                    //ViewBag.FriendsList = contactToEdit.GetFriendsList(Friends, ContactsView);

                    // Store in ViewBag the "not yet friend" contact list  of the contact to edit
                    //ViewBag.NotYetFriendsList = contactToEdit.GetNotYetFriendsList(Friends, ContactsView);

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
            //Friends Friends = (Friends)HttpRuntime.Cache["Friends"];

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
        // GET: /Actors/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Actors/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
