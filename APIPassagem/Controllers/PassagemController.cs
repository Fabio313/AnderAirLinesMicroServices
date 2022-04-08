using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIPassagem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace APIPassagem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassagemController : ControllerBase
    {
        private readonly PassagemService _passagemService;

        public PassagemController(PassagemService personService)
        {
            _passagemService = personService;
        }

        [HttpGet]
        public ActionResult<List<Passagem>> Get() =>
            _passagemService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Passagem> Get(string id)
        {
            var cliente = _passagemService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Passagem>> CreateAsync(Passagem passagem)
        {

            HttpClient APIConnection = new HttpClient();
            try
            {
                HttpResponseMessage buscavoo = await APIConnection.GetAsync("https://localhost:44307/api/Voo/busca?siglaorigem=" + passagem.Voo.Origem.Sigla + "&sigladestino=" + passagem.Voo.Destino.Sigla);
                var voo = JsonConvert.DeserializeObject<Voo>(await buscavoo.Content.ReadAsStringAsync());
                if (voo.Origem == null || voo.Destino == null)
                    return NotFound("Não existe voo com as informações fornecidas escolhido!");
                passagem.Voo = voo;
            }
            catch
            {
                return NotFound("API DOS VOOS ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage buscapassageiro = await APIConnection.GetAsync("https://localhost:44340/api/Passageiro/busca?cpf=" + passagem.Passageiro.Cpf);
                var passageiro = JsonConvert.DeserializeObject<Passageiro>(await buscapassageiro.Content.ReadAsStringAsync());
                if (passageiro.Cpf == null)
                    return NotFound("Não existe passageiro com este cpf!");
                passagem.Passageiro = passageiro;
            }
            catch
            {
                return NotFound("API DOS PASSAGEIROS ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage buscaclasse = await APIConnection.GetAsync("https://localhost:44305/api/Aeronave/busca?descricao=" + passagem.Classe.Descricao);
                var classe = JsonConvert.DeserializeObject<Classe>(await buscaclasse.Content.ReadAsStringAsync());
                if (classe.Descricao == null)
                    return NotFound("Não existe classe com esta descricao!");
                passagem.Classe = classe;
            }
            catch
            {
                return NotFound("API DAS CLASSES ESTA FORA DO AR");
            }

            try
            {
                HttpResponseMessage buscaprecobase = await APIConnection.GetAsync("https://localhost:44364/api/PrecoBase/busca?siglaorigem=" + passagem.Voo.Origem.Sigla + "&sigladestino=" + passagem.Voo.Destino.Sigla);
                var precobase = JsonConvert.DeserializeObject<PrecoBase>(await buscaprecobase.Content.ReadAsStringAsync());
                if (precobase.Origem == null || precobase.Destino == null)
                    return NotFound("Não existe preco base com origem e destino do voo escolhido!");
                passagem.PrecoBase = precobase;
            }
            catch
            {
                return NotFound("API DOS PRECOS BASES DO AR");
            }

            passagem.ValorTotal = (passagem.PrecoBase.Valor * ((passagem.Classe.Valor - 100) / 100) * -1) * (((passagem.PromocaoPorcentagem - 100) / 100) * -1);

            _passagemService.Create(passagem);

            return CreatedAtRoute("GetCliente", new { id = passagem.Id.ToString() }, passagem);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Passagem personIn)
        {
            var cliente = _passagemService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _passagemService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _passagemService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _passagemService.Remove(person.Id);

            return NoContent();
        }
    }
}
