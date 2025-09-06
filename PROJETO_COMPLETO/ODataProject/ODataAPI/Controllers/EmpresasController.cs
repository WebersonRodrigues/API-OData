using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;
using System.Text.Json;

namespace ODataAPI.Controllers
{
    public class EmpresasController(ApplicationDbContext context) : ODataController
    {
        private readonly ApplicationDbContext _context = context;

        [EnableQuery]
        public IQueryable<Empresa> Get()
        {
            return _context.Empresas.Include(e => e.Lojas);
        }
    

        [EnableQuery]
        public SingleResult<Empresa> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Empresas.Where(e => e.Id == key).Include(e => e.Lojas));
        }

        public async Task<IActionResult> Post([FromBody] Empresa empresa)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return Created(empresa);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Empresa empresa)
        {
            if (!EmpresaExists(key))
                return NotFound("Empresa informado não existe na base.");

            empresa.Id = key;
            _context.Entry(empresa).State = EntityState.Modified;

            await _context.SaveChangesAsync();
 
            return Updated(empresa);
        }
        

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var empresa = await _context.Empresas.FindAsync(key);
            if (empresa == null)
                return NotFound();
            
            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<Empresa> empresaDelta)
        {
            var empresa = await _context.Empresas.FindAsync(key);

            if (empresa == null)
                return NotFound();
            
            // Aqui aplica somente as alterações enviadas.
            empresaDelta.Patch(empresa);

            await _context.SaveChangesAsync();

            return Updated(empresa);
        }

        private bool EmpresaExists(int id)
        {
            return _context.Empresas.Any(e => e.Id == id);
        }
    }
}