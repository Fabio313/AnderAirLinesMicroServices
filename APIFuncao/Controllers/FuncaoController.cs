using System;
using System.Collections.Generic;
using APIFuncao.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace APIFuncao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuncaoController : ControllerBase
    {
        private readonly FuncaoService _usuarioService;

        public FuncaoController(FuncaoService personService)
        {
            _usuarioService = personService;
        }

        [HttpGet]
        public ActionResult<List<Funcao>> Get() =>
            _usuarioService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Funcao> Get(string id)
        {
            var cliente = _usuarioService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Funcao> GetUsuarioLogin(string login)
        {
            var cliente = _usuarioService.GetNome(login);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public ActionResult<Funcao> Create(Funcao person)
        {
            if (!NomeService.VerificaFuncaoNome(person.Nome, _usuarioService))
                return BadRequest("Login ja esta sendo utlizado!");

            _usuarioService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Funcao personIn)
        {
            var cliente = _usuarioService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _usuarioService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _usuarioService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _usuarioService.Remove(person.Id);

            return NoContent();
        }
    }
}
