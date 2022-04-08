using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIVoo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace APIVoo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VooController : ControllerBase
    {
        private readonly VooService _vooService;

        public VooController(VooService personService)
        {
            _vooService = personService;
        }

        [HttpGet]
        public ActionResult<List<Voo>> Get() =>
            _vooService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Voo> Get(string id)
        {
            var cliente = _vooService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Voo> GetVooChaves(string siglaorigem, string sigladestino)
        {
            var cliente = _vooService.GetVoo(siglaorigem, sigladestino);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Voo>> CreateAsync(Voo voo)
        {
            HttpClient APIConnection = new HttpClient();
            try
            {

                HttpResponseMessage aeroporto = await APIConnection.GetAsync("https://localhost:44321/api/Aeroporto/busca?sigla=" + voo.Origem.Sigla);
                var origem = JsonConvert.DeserializeObject<Aeroporto>(await aeroporto.Content.ReadAsStringAsync());

                if (origem.Sigla == null)
                    return NotFound("Não existe aeroporto com a sigla de origem escolhida!");
                voo.Origem = origem;
            }
            catch
            {
                return NotFound("API DOS AEROPORTOS ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage aeroporto = await APIConnection.GetAsync("https://localhost:44321/api/Aeroporto/busca?sigla=" + voo.Destino.Sigla);
                var destino = JsonConvert.DeserializeObject<Aeroporto>(await aeroporto.Content.ReadAsStringAsync());
                if (destino.Sigla == null)
                    return NotFound("Não existe aeroporto com a sigla de destino escolhida!");
                voo.Destino = destino;
            }
            catch
            {
                return NotFound("API DOS AEROPORTOS ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage aeronave = await APIConnection.GetAsync("https://localhost:44359/api/Aeronave/busca?codigo=" + voo.Aeronave.Codigo);
                var aeronaveobject = JsonConvert.DeserializeObject<Aeronave>(await aeronave.Content.ReadAsStringAsync());
                if (aeronaveobject.Codigo == null)
                    return NotFound("Não existe aeronave com o codigo procurado!");
                voo.Aeronave = aeronaveobject;
            }
            catch
            {
                return NotFound("API DOS AEROPORTOS ESTA FORA DO AR");
            }

            _vooService.Create(voo);

            return CreatedAtRoute("GetCliente", new { id = voo.Id.ToString() }, voo);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Voo personIn)
        {
            var cliente = _vooService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _vooService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _vooService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _vooService.Remove(person.Id);

            return NoContent();
        }
    }
}
