using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIAeronave.Service;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Services;
using Newtonsoft.Json;

namespace APIAeronave.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AeronaveController : ControllerBase
    {
        private readonly AeronaveService _aeronaveService;

        public AeronaveController(AeronaveService personService)
        {
            _aeronaveService = personService;
        }

        [HttpGet]
        public ActionResult<List<Aeronave>> Get() =>
            _aeronaveService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Aeronave> Get(string id)
        {
            var cliente = _aeronaveService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Aeronave> GetPassageiroCPF(string codigo)
        {
            var cliente = _aeronaveService.GetCodigo(codigo);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Aeronave>> CreateAsync(Aeronave aeronave)
        {
            try
            {
                var usuario = await ConsultaAPI.BuscaUsuarioAsync(aeronave.LoginUser);
                if (usuario.Login == null)
                    return NotFound("Este usuario não existe");
                if (usuario.Funcao.Nome != "Administrador")
                    return BadRequest("Este usuario nao tem autorização para cadastrar precobase");
            }
            catch
            {
                return NotFound("API DE USUARIOS ESTA FORA DO AR");
            }

            if (!CodigoService.VerificaAeronaveSigla(aeronave.Codigo, _aeronaveService))
                return BadRequest("Já existe aeronave com a sigla digitada");

            ConsultaAPI.RegistraLog(new Log(aeronave.LoginUser, null, JsonConvert.SerializeObject(aeronave), "Create"));
            _aeronaveService.Create(aeronave);

            return CreatedAtRoute("GetCliente", new { id = aeronave.Id.ToString() }, aeronave);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Aeronave personIn)
        {
            var cliente = _aeronaveService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            ConsultaAPI.RegistraLog(new Log(personIn.LoginUser, JsonConvert.SerializeObject(cliente), JsonConvert.SerializeObject(personIn), "Update"));
            _aeronaveService.Update(id, personIn);


            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _aeronaveService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            ConsultaAPI.RegistraLog(new Log(person.LoginUser, JsonConvert.SerializeObject(person), null, "Delete"));
            _aeronaveService.Remove(person.Id);

            return NoContent();
        }
    }
}
