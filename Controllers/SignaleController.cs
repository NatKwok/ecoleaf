using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asp_mvc_webmap_vs.Data;
using asp_mvc_webmap_vs.Models;
using Newtonsoft.Json;
using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm;

namespace asp_mvc_webmap_vs.Controllers
{
    public class SignaleController : Controller
    {
        private readonly MvcWebmapContext _context;

        public SignaleController(MvcWebmapContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalementsCoyote>>> GetSignalementsCoyotes()
        {
            var feature = await _context.SignalementsCoyotes.ToListAsync();

            var features = feature.Select(record =>
            {

                if (record.Geom == null)
                {
                    return null;
                }

                if (record.Geom is Point Point)

                {
                    var geojsonPoint = new
                    {
                        type = "Point",
                        coordinates = new[] {record.Geom.X, record.Geom.Y}
                    };

                    var properties = new Dictionary<string, object>
                    {
                        { "Id", record.Id },
                        { "Date Observed", record.DatObs },
                        {"Hour Observed" , record.HrObs },
                        { "Area", record.Territoire },
                        {"# of Coyotes", record.NbCoyotes }
                    };

                    // Create a GeoJSON Feature
                    return new
                    {
                        type = "Feature",
                        geometry = geojsonPoint,
                        properties
                    };
                }

                return null;
            })
            .Where(feature => feature != null)
            .ToList();

            //Create a FeatureCollection
            var featureCollection = new
            {
                type = "FeatureCollection",
                features
            };
            //Serialize to GeoJSON
            var geoJson = JsonConvert.SerializeObject(featureCollection, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            // Return GeoJSON with appropriate content type
            return Content(geoJson, "application/json");
        }

        // GET: Signale
        public async Task<IActionResult> Index()
        {
            return View(await _context.SignalementsCoyotes.ToListAsync());
        }

        // GET: Signale/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signalementsCoyote = await _context.SignalementsCoyotes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (signalementsCoyote == null)
            {
                return NotFound();
            }

            return View(signalementsCoyote);
        }

        // GET: Signale/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Signale/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Geom,EntryId,ObjId,DatObs,HrObs,NbCoyotes,Alimentation,StatutAnimal,Periode,CompClass,ComCode,Cote,Territoire,StatutMention,Provenance,Verif,X,Y,Lat,Long")] SignalementsCoyote signalementsCoyote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(signalementsCoyote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(signalementsCoyote);
        }

        // GET: Signale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signalementsCoyote = await _context.SignalementsCoyotes.FindAsync(id);
            if (signalementsCoyote == null)
            {
                return NotFound();
            }
            return View(signalementsCoyote);
        }

        // POST: Signale/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Geom,EntryId,ObjId,DatObs,HrObs,NbCoyotes,Alimentation,StatutAnimal,Periode,CompClass,ComCode,Cote,Territoire,StatutMention,Provenance,Verif,X,Y,Lat,Long")] SignalementsCoyote signalementsCoyote)
        {
            if (id != signalementsCoyote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(signalementsCoyote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SignalementsCoyoteExists(signalementsCoyote.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(signalementsCoyote);
        }

        // GET: Signale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var signalementsCoyote = await _context.SignalementsCoyotes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (signalementsCoyote == null)
            {
                return NotFound();
            }

            return View(signalementsCoyote);
        }

        // POST: Signale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var signalementsCoyote = await _context.SignalementsCoyotes.FindAsync(id);
            if (signalementsCoyote != null)
            {
                _context.SignalementsCoyotes.Remove(signalementsCoyote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SignalementsCoyoteExists(int id)
        {
            return _context.SignalementsCoyotes.Any(e => e.Id == id);
        }
    }
}
