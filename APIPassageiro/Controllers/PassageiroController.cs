using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Services;

namespace APIPassageiro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassageiroController : ControllerBase
    {
        private readonly PassageiroService _passageiroService;

        public PassageiroController(PassageiroService personService)
        {
            _passageiroService = personService;
        }

        [HttpGet]
        public ActionResult<List<Passageiro>> Get() =>
            _passageiroService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Passageiro> Get(string id)
        {
            var cliente = _passageiroService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Passageiro> GetPassageiroCPF(string cpf)
        {
            var cliente = _passageiroService.GetCPF(cpf);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Passageiro>> CreateAsync(Passageiro person)
        {
            if (!CPFService.ValidaCPF(person.Cpf, _passageiroService))
                return BadRequest("CPF Inválido!");
            if (!CPFService.VerificaPassageiroCPF(person.Cpf, _passageiroService))
                return BadRequest("Passageiro ja cadastrado");

            var end = await VerificaCep.CEPVerify(person.Endereco.Cep);
            if (end.Localidade != null)
            {
                int num = person.Endereco.Numero;
                person.Endereco = new Endereco(end.Localidade, end.Logradouro, end.Bairro, end.Uf, end.Complemento, end.Cep);
                person.Endereco.Numero = num;
            }

            _passageiroService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Passageiro personIn)
        {
            var cliente = _passageiroService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _passageiroService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _passageiroService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _passageiroService.Remove(person.Id);

            return NoContent();
        }
    }
}
