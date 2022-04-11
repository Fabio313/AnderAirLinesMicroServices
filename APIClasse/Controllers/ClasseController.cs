using System;
using System.Collections.Generic;
using APIClasse.Service;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace APIClasse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClasseController : ControllerBase
    {
        private readonly ClasseService _classeService;

        public ClasseController(ClasseService personService)
        {
            _classeService = personService;
        }

        [HttpGet]
        public ActionResult<List<Classe>> Get() =>
            _classeService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Classe> Get(string id)
        {
            var cliente = _classeService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Classe> GetClasseDescricao(string descricao)
        {
            var cliente = _classeService.GetDescricao(descricao);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public ActionResult<Classe> Create(Classe person)
        {

            _classeService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Classe personIn)
        {
            var cliente = _classeService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _classeService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _classeService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _classeService.Remove(person.Id);

            return NoContent();
        }
    }
}
