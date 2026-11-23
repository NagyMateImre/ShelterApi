using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;

namespace ShelterApi.Controllers
{

    [ApiController]
    [Route("(queries")]
    public class QueriesController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public QueriesController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpGet("allSheltersWithAnimals")]
        public async Task<IActionResult> AllSheltersWithAnimals()
        {
            try
            {
                var shelters = await _context.Shelters.Include(s => s.Animals).ToListAsync();
                return Ok(new { message = "A menhelyek és a hozzájuk tartozó állatok listája.", result = shelters });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { message = "Hiba történt a menhelyek lekérdezésekor.", 
                    result = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("allAnimalsWithShelter")]
        public async Task<IActionResult> AllAnimalsWithShelter()
        {
            try
            {
                var animals = await (from animal in _context.Animals
                    join shelter in _context.Shelters on animal.ShelterId equals shelter.Id
                    select new
                    {
                        animal.Id,
                        animal.Name,
                        animal.Species,
                        animal.Age,
                        animal.Weight,
                        animal.RegTime,
                        animal.ShelterId,
                        ShelterName = shelter.Name
                    }).ToListAsync();
                return Ok(new { message = "Az állatok a menhelyük nevével.", result = animals });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { message = "Hiba történt az állatok és menhelyek összekapcsolásakor.",
                   result = ex.Message,
                   innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("animalsOfShelter")]
        public async Task<IActionResult> AnimalsOfShelter([FromQuery] Guid shelterId)
        {
            try
            {
                var shelter = await _context.Shelters
                    .Include(s => s.Animals)
                    .Where(s => s.Id == shelterId)
                    .FirstOrDefaultAsync();

                if (shelter == null)
                    return NotFound(new { message = "A megadott menhely nem található.", result = (object?)null });

                return Ok(new { message = "A menhely adatai és a hozzá tartozó állatok.", result = shelter });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { message = "Hiba történt a menhely állatainak lekérdezésekor.",
                   result = ex.Message,
                   innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("animalCountByShelter")]
        public async Task<IActionResult> AnimalCountByShelter()
        {
            try
            {
                var counts = await _context.Shelters
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.City,
                        AnimalCount = s.Animals.Count()
                    })
                    .OrderByDescending(s => s.AnimalCount)
                    .ToListAsync();

                return Ok(new { message = "Menhelyenkénti állatlétszám, csökkenő sorrendben.", result = counts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { message = "Hiba történt az állatok darabszámának lekérdezésekor.",
                  result = ex.Message,
                  innerError = ex.InnerException?.Message
                });
            }
        }
    }
}
