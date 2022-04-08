﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIAeroporto.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Services;
using Newtonsoft.Json;

namespace APIAeroporto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AeroportoController : ControllerBase
    {
        private readonly AeroportoService _aeroportoService;

        public AeroportoController(AeroportoService personService)
        {
            _aeroportoService = personService;
        }

        [HttpGet]
        public ActionResult<List<Aeroporto>> Get() =>
            _aeroportoService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Aeroporto> Get(string id)
        {
            var cliente = _aeroportoService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Aeroporto> GetPassageiroCPF(string sigla)
        {
            var cliente = _aeroportoService.GetSigla(sigla);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Aeroporto>> CreateAsync(Aeroporto person)
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

            if (!SiglaService.VerificaAeroportoSigla(person.Sigla, _aeroportoService))
                return BadRequest("Já existe um aeroporto com a sigla escolhida");

            var end = await VerificaCep.CEPVerify(person.Endereco.Cep);
            if (end != null)
            {
                int num = person.Endereco.Numero;
                person.Endereco = new Endereco(end.Localidade, end.Logradouro, end.Bairro, end.Uf, end.Complemento, end.Cep);
                person.Endereco.Numero = num;
            }

            var airp = await AirportsQuery.GetAirportAsync(person.Sigla);
            if (airp != null)
            {
                person.Endereco.Localidade = airp.City;
                person.Endereco.Continente = airp.Continent;
                person.Endereco.Pais = airp.Country;
            }

            _aeroportoService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Aeroporto personIn)
        {
            var cliente = _aeroportoService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _aeroportoService.Update(id, personIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var person = _aeroportoService.Get(id);

            if (person == null)
            {
                return NotFound();
            }

            _aeroportoService.Remove(person.Id);

            return NoContent();
        }
    }
}
