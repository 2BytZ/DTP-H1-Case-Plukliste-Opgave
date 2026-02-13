using AspNetCoreGeneratedDocument;
using FKTV_DAL;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FKTV.Controllers.Plukliste
{
    public class PluksedlerController : Controller
    {
        public IActionResult Pluksedler(int index)
        {
            if (index == null)
            {
                index = 0;
            }
            var data = new DataAccess();

            // Ensure import folder exists (the view used this previously)
            Directory.CreateDirectory(data.PluklistImportLocation);

            // Ensure export folder exists (original behaviour)
            if (!Directory.Exists(data.GetPluklistExportFolder))
            {
                return RedirectToAction(nameof(Views_Shared_Error));
            }

            var files = Directory.EnumerateFiles(data.GetPluklistExportFolder).ToList();

            // Provide files and index to the view via ViewBag
            ViewBag.Files = files;
            var safeIndex = files.Count == 0 ? 0 : Math.Clamp(index, 0, files.Count - 1);
            ViewBag.Index = safeIndex;

            // If no files, return an empty model (view will render empty state)
            if (files.Count == 0)
            {
                return View(new Models.Plukliste.Plukliste());
            }

            var path = files[safeIndex];

            // Try to deserialize JSON plukliste (console app produces JSON)
            if (Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var json = System.IO.File.ReadAllText(path);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var model = JsonSerializer.Deserialize<Models.Plukliste.Plukliste>(json, options) ?? new Models.Plukliste.Plukliste();
                    return View(model);
                }
                catch (Exception ex)
                {
                    // If deserialization fails, surface a helpful message in the view
                    ViewBag.Error = $"Failed to read plukliste from '{Path.GetFileName(path)}': {ex.Message}";
                    return View(new Models.Plukliste.Plukliste());
                }
            }

            // Non-JSON files are unsupported here; surface a message and return empty model
            ViewBag.Error = $"Unsupported plukliste file format: {Path.GetExtension(path)}";
            return View(new Models.Plukliste.Plukliste());
        }
        public IActionResult Next(int index)
        {
            index++;
            return RedirectToAction(nameof(Pluksedler), new { index });
        }
        public IActionResult Previous(int index)
        {
            if (index > 0)
            {
                index--;
            }
            return RedirectToAction(nameof(Pluksedler), new { index });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AfslutPlukseddel(List<string> files, int index)
        {
            DataAccess access = new DataAccess();
            var _files = Directory.EnumerateFiles(access.GetPluklistExportFolder).ToList();
            System.IO.File.Move(_files[index], Path.Combine(access.PluklistImportLocation, _files[index]));
            files.Remove(_files[index]);
            if (index == _files.Count) index--;
            return RedirectToAction(nameof(Pluksedler), new { index });
        }
    }
}
