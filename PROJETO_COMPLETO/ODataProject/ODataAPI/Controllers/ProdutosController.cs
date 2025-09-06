using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Data;
using ODataAPI.Models;

namespace ODataAPI.Controllers
{
    public class ProdutosController(ApplicationDbContext context) : ODataController
    {
        private readonly ApplicationDbContext _context = context;

        [EnableQuery]
        public IQueryable<Produto> Get()
        {
            return _context.Produtos.Include(p => p.Loja).Include(p => p.Avaliacoes);
        }

        [EnableQuery]
        public SingleResult<Produto> Get([FromRoute] int key)
        {
            return SingleResult.Create(_context.Produtos.Where(p => p.Id == key)
                .Include(p => p.Loja).Include(p => p.Avaliacoes));
        }

        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            if (produto.LojaId <= 0)
                return BadRequest("LojaId é obrigatório e deve ser maior que zero.");
        
            var loja = await _context.Lojas.FindAsync(produto.LojaId);
            if (loja == null)
                return BadRequest($"Loja com Id {produto.LojaId} não encontrada.");

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return Created(produto);
        }

        public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(key))
                {
                    return NotFound();
                }
                throw;
            }

            return Updated(produto);
        }

        public async Task<IActionResult> Delete([FromRoute] int key)
        {
            var produto = await _context.Produtos.FindAsync(key);
            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(p => p.Id == id);
        }
    }
}