using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIAeronave.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
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
        public async Task<ActionResult<Aeronave>> CreateAsync(Aeronave person)
        {
            HttpClient APIConnection = new HttpClient();
            try
            {
                HttpResponseMessage user = await APIConnection.GetAsync("https://localhost:44385/api/Usuario/busca?login=" + person.LoginUser);
                var usuario = JsonConvert.DeserializeObject<Usuario>(await user.Content.ReadAsStringAsync());

                if (usuario == null)
                    return NotFound("Este usuario não existe");
                if (usuario.Funcao.Nome != "Administrador")
                    return BadRequest("Este usuario nao tem autorização para cadastrar usuarios");
                ;
            }
            catch
            {
                return NotFound("API DE USUARIOS ESTA FORA DO AR");
            }
            if (!CodigoService.VerificaAeronaveSigla(person.Codigo, _aeronaveService))
                return BadRequest("Já existe aeronave com a sigla digitada");

            _aeronaveService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Aeronave personIn)
        {
            var cliente = _aeronaveService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

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

            _aeronaveService.Remove(person.Id);

            return NoContent();
        }
    }
}
