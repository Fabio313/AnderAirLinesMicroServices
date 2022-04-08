using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIUsuarios.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Services;
using Newtonsoft.Json;

namespace APIUsuarios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService personService)
        {
            _usuarioService = personService;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> Get() =>
            _usuarioService.Get();

        [HttpGet("{id:length(24)}", Name = "GetCliente")]
        public ActionResult<Usuario> Get(string id)
        {
            var cliente = _usuarioService.Get(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        [HttpGet("busca")]
        public ActionResult<Usuario> GetUsuarioLogin(string login)
        {
            var cliente = _usuarioService.GetLogin(login);

            if (cliente == null)
            {
                return NotFound();
                throw new Exception();
            }

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Usuario>> CreateAsync(Usuario person)
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
;            }
            catch
            {
                return NotFound("API DE USUARIOS ESTA FORA DO AR");
            }

            if (!LoginService.VerificaUsuarioLogin(person.Login, _usuarioService))
                return BadRequest("Login ja esta sendo utlizado!");

            try
            {
                HttpResponseMessage funcao = await APIConnection.GetAsync("https://localhost:44386/api/Aeronave/busca?login=" + person.Funcao.Nome);
                var funcaoObject = JsonConvert.DeserializeObject<Funcao>(await funcao.Content.ReadAsStringAsync());

                if (funcaoObject == null)
                    return NotFound("Esta função não existe");
                person.Funcao = funcaoObject;
            }
            catch
            {
                return NotFound("API DAS FUNÇÕES ESTA FORA DO AR");
            }

            var end = await VerificaCep.CEPVerify(person.Endereco.Cep);
            if (end.Localidade != null)
            {
                int num = person.Endereco.Numero;
                person.Endereco = new Endereco(end.Localidade, end.Logradouro, end.Bairro, end.Uf, end.Complemento, end.Cep);
                person.Endereco.Numero = num;
            }

            _usuarioService.Create(person);

            return CreatedAtRoute("GetCliente", new { id = person.Id.ToString() }, person);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Usuario personIn)
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
