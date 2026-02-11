using FKTV_BLL;
using FKTV.Models.Items;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FKTV_DAL;

namespace FKTV.Controllers.Item
{
    public class ItemController : Controller
    {
        // GET: ItemController
        public ActionResult Index()
        {
            ItemsRepository itemsRepository = new ItemsRepository();
            //store items from the html database
            List<StoreItem> storeItems = itemsRepository.GetItems(); //get items from html database
            List<Models.Items.Item> items = new List<Models.Items.Item>();
            //create a model item with the data of each html database item
            foreach (var item in storeItems)
            {
                Models.Items.Item modelItem = new Models.Items.Item();
                modelItem.ProductID = item.ProductID;
                modelItem.Title = item.Title;
                modelItem.Type = item.Type;
                modelItem.Amount = item.Amount;
                items.Add(modelItem);
            }
            return View(items);
        }

        // GET: ItemController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ItemController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemController/Edit/5
        public ActionResult Edit(string id)
        {
            //get item info and display in editor
            ItemsRepository itemsRepository = new ItemsRepository();
            StoreItem storeItem = itemsRepository.GetItem(id);
            Models.Items.Item modelItem = new Models.Items.Item();
            modelItem.ProductID = storeItem.ProductID;
            modelItem.Title = storeItem.Title;
            modelItem.Type = storeItem.Type;
            modelItem.Amount = storeItem.Amount;
            return View(modelItem);
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Models.Items.Item collection)
        {
            try
            {
                ItemsRepository itemsRepository = new ItemsRepository();
                itemsRepository.UpdateAmount(collection.ProductID, collection.Amount, false); //update the item amount in html database

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ItemController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ItemController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
