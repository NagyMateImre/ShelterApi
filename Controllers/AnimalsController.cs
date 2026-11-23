using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTO;

namespace ShelterApi.Controllers
{
    [ApiController]
    [Route("animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalshelterContext _context;

        public AnimalsController(AnimalshelterContext context)
        {
            _context = context;
        }

        [HttpPost("newAnimal")]
        public async Task<IActionResult> NewAnimal([FromBody] NewAnimalDTO newAnimalDTO)
        {
            if (string.IsNullOrWhiteSpace(newAnimalDTO.Name) || string.IsNullOrWhiteSpace(newAnimalDTO.Species) || newAnimalDTO.ShelterId == null)
            {
                return BadRequest(new { message = "A név, a faj és a menhely azonosító megadása kötelező.", result = (object?)null });
            }

            try
            {
                bool shelterExists = await _context.Shelters.AnyAsync(s => s.Id == newAnimalDTO.ShelterId.Value);
                if (!shelterExists)
                {
                    return NotFound(new { message = "A megadott shelter nem található.", result = (object?)null });
                }

                var animal = new Animal
                {
                    Id = Guid.NewGuid(),
                    Name = newAnimalDTO.Name,
                    Species = newAnimalDTO.Species,
                    Age = newAnimalDTO.Age,
                    Weight = newAnimalDTO.Weight,
                    ShelterId = newAnimalDTO.ShelterId.Value
                };

                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Az állat sikeresen rögzítve.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat rögzítése nem sikerült.", result = (object?)null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Váratlan hiba történt.",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("allAnimal")]
        public async Task<IActionResult> AllAnimal()
        {
            try
            {
                var animals = await _context.Animals.ToListAsync();
                if (animals.Count == 0)
                    return NotFound(new { message = "Még nincs állat.", result = (object?)null });
                return Ok(new { message = "Sikeres lekérés", result = animals });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Hiba történt az állatok lekérdezésekor.", 
                    result = ex.Message,
                    innerError = ex.InnerException?.Message,
                });
            }
        }

        [HttpGet("byId")]
        public async Task<IActionResult> ById([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "Ilyen azonosítójú állat nem található.", result = (object?)null });
                return Ok(new { message = "Az állat adatai.", result = animal });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Hiba történt az állat lekérdezésekor.", 
                    result = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] UpdateAnimaDTO updateAnimaDTO)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "A módosítandó állat nem található.", result = (object?)null });

                var shelter = await _context.Shelters.FindAsync(updateAnimaDTO.ShelterId);
                if (shelter == null)
                    return NotFound(new { message = "A megadott shelter nem található.", result = (object?)null });

                animal.Name = updateAnimaDTO.Name;
                animal.Species = updateAnimaDTO.Species;
                animal.Age = updateAnimaDTO.Age;
                animal.Weight = updateAnimaDTO.Weight;
                animal.ShelterId = updateAnimaDTO.ShelterId;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Az állat adatai sikeresen módosítva.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat módosítása nem sikerült.", result = (object?)null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Váratlan hiba történt az állat módosításakor.",
                    result = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        [HttpDelete("deleteAnimal")]
        public async Task<IActionResult> DeleteAnimal([FromQuery] Guid id)
        {
            try
            {
                var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == id);
                if (animal == null)
                    return NotFound(new { message = "Az állat nem található.", result = (object?)null });

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Az állat sikeresen törölve.", result = animal });
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "Az állat törlése nem sikerült.", result = (object?)null });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Váratlan hiba történt az állat törlésekor.",
                    result = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }
    }
}
