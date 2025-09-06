using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Controllers
{
    public class LojasController(ApplicationDbContext context) : ODataController
    {
        private readonly ApplicationDbContext _context = context;

        [EnableQuery]
        public IQueryable<Loja> Get()
        {
            return _context.Lojas;
        }

        [EnableQuery]
        public SingleResult<Loja> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Lojas.Where(l => l.Id == key));
        }

        public async Task<IActionResult> Post([FromBody] Loja loja)
        {
            if (loja.EmpresaId <= 0)
                return BadRequest("EmpresaId é obrigatório e deve ser maior que zero.");
            
            var empresa = await _context.Empresas.FindAsync(loja.EmpresaId);   
            if (empresa == null)
                return BadRequest($"Empresa com Id {loja.EmpresaId} não encontrada."); 

            _context.Lojas.Add(loja);
            await _context.SaveChangesAsync();

            return Created(loja);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Loja loja)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != loja.Id)
            {
                return BadRequest();
            }

            _context.Entry(loja).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LojaExists(key))
                {
                    return NotFound();
                }
                throw;
            }

            return Updated(loja);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var loja = await _context.Lojas.FindAsync(key);
            if (loja == null)
            {
                return NotFound();
            }

            _context.Lojas.Remove(loja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LojaExists(int id)
        {
            return _context.Lojas.Any(l => l.Id == id);
        }
    }
}