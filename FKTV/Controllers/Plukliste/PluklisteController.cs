using AspNetCoreGeneratedDocument;
using FKTV.Models.Plukliste;
using FKTV_BLL;
using FKTV_DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FKTV.Controllers.Plukliste
{
    public class PluklisteController : Controller
    {
        // GET: PluklistController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PluklistController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PluklistController/Create
        public ActionResult Create()
        {
            
            
            return View();
        }

        // POST: PluklistController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Plukliste.Plukliste plukseddel)
        {
            string inputData = JsonSerializer.Serialize(plukseddel);
            DataAccess access = new DataAccess();
            //Update storage
            ItemsRepository itemsRepository = new ItemsRepository();
            string exportDir = access.GetPluklistExportFolder;
            var orderNum = DateTime.Now.ToString("yyyyMMddHHmmss");
            System.IO.File.WriteAllText(Path.Combine(exportDir, $"{orderNum}_export.json"), inputData);
            return RedirectToAction(nameof(Index));
        }

        // GET: PluklistController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PluklistController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: PluklistController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PluklistController/Delete/5
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
