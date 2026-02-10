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
            List<StoreItem> storeItems = itemsRepository.GetItems();
            List<Models.Items.Item> items = new List<Models.Items.Item>();
            foreach (var item in storeItems)
            {
                Models.Items.Item I = new Models.Items.Item();
                I.ProductID = item.ProductID;
                I.Title = item.Title;
                I.Type = item.Type;
                I.Amount = item.Amount;
                items.Add(I);
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
            ItemsRepository itemsRepository = new ItemsRepository();
            StoreItem storeItem = itemsRepository.GetItem(id);
            Models.Items.Item I = new Models.Items.Item();
            I.ProductID = storeItem.ProductID;
            I.Title = storeItem.Title;
            I.Type = storeItem.Type;
            I.Amount = storeItem.Amount;
            return View(I);
        }

        // POST: ItemController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Models.Items.Item collection)
        {
            try
            {
                ItemsRepository itemsRepository = new ItemsRepository();
                itemsRepository.UpdateAmount(collection.ProductID, collection.Amount, false);

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
