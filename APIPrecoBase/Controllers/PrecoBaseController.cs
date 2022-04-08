using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIPrecoBase.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace APIPrecoBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrecoBaseController : ControllerBase
    {
        private readonly PrecoBaseService _precoBaseService;

        public PrecoBaseController(PrecoBaseService personService)
        {
            _precoBaseService = personService;
        }

        [HttpGet]
        public ActionResult<List<PrecoBase>> Get() =>
            _precoBaseService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<PrecoBase> Get(string id)
        {
            var cliente = _precoBaseService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<PrecoBase> GetVooChaves(string siglaorigem, string sigladestino)
        {
            var cliente = _precoBaseService.GetPrecoBase(siglaorigem, sigladestino);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<PrecoBase>> CreateAsync(PrecoBase precobase)
        {
            HttpClient APIConnection = new HttpClient();

            try
            {
                HttpResponseMessage aeroporto = await APIConnection.GetAsync("https://localhost:44321/api/Aeroporto/busca?sigla=" + precobase.Origem.Sigla);
                var origem = JsonConvert.DeserializeObject<Aeroporto>(await aeroporto.Content.ReadAsStringAsync());
                if (origem.Sigla == null)
                    return NotFound("Não existe aeroporto com a sigla de origem escolhida!");
                precobase.Origem = origem;
            }
            catch
            {
                return NotFound("API DOS AEROPORTOS ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage aeroporto = await APIConnection.GetAsync("https://localhost:44321/api/Aeroporto/busca?sigla=" + precobase.Destino.Sigla);
                var destino = JsonConvert.DeserializeObject<Aeroporto>(await aeroporto.Content.ReadAsStringAsync());
                if (destino.Sigla == null)
                    return NotFound("Não existe aeroporto com a sigla de destino escolhida!");
                precobase.Destino = destino;
            }
            catch
            {
                return NotFound("API DOS AEROPORTOS ESTA FORA DO AR");
            }

            _precoBaseService.Create(precobase);

            return CreatedAtRoute("GetCliente", new { id = precobase.Id.ToString() }, precobase);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, PrecoBase personIn)
        {
            var cliente = _precoBaseService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _precoBaseService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _precoBaseService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _precoBaseService.Remove(person.Id);

            return NoContent();
        }
    }
}
