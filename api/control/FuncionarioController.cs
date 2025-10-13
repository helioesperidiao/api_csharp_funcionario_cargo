using Microsoft.AspNetCore.Mvc;
using Api.Service;
using Api.Filters;
using Api.Model;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Api.Utils;

namespace Api.Control
{
    [ApiController]
    [Route("api/v1/funcionarios")]
    public class FuncionarioController : ControllerBase
    {
        private readonly FuncionarioService _funcionarioService;

        public FuncionarioController(FuncionarioService funcionarioService)
        {
            Console.WriteLine("⬆️ FuncionarioController.constructor()");
            this._funcionarioService = funcionarioService;
        }

        // GET: api/v1/funcionarios
        [HttpGet]
        [ValidateToken]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("🔵 FuncionarioController.Index()");
            List<Funcionario> listaFuncionarios = await _funcionarioService.FindAll();

            object resposta = new
            {
                success = true,
                message = "Busca realizada com sucesso",
                data = new { funcionarios = listaFuncionarios }
            };

            return Ok(resposta);
        }

        // GET: api/v1/funcionarios/{id}
        [HttpGet("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioId]
        public async Task<IActionResult> Show(int idFuncionario)
        {
            Console.WriteLine("🔵 FuncionarioController.Show()");
            Funcionario? funcionarioEncontrado = await _funcionarioService.FindById(idFuncionario);

            List<Funcionario> funcionariosArray = new List<Funcionario>();

            if (funcionarioEncontrado != null)
            {
                funcionariosArray = new List<Funcionario> { funcionarioEncontrado };
            }

            object resposta = new
            {
                success = true,
                message = "Executado com sucesso",
                data = new { funcionarios = funcionariosArray }
            };

            return Ok(resposta);
        }

        // POST: api/v1/funcionarios
        [HttpPost]
        [ValidateToken]
        [ValidateFuncionarioBody]
        public async Task<IActionResult> Store([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Store()");

            // Extrai o objeto "funcionario"
            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);

            string nome = funcionarioElem.GetProperty("nomeFuncionario").GetString() ?? "";
            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";

            // Valida recebeValeTransporte
            int recebeValeTransporte = funcionarioElem.GetProperty("recebeValeTransporte").GetInt32();
            if (recebeValeTransporte != 0 && recebeValeTransporte != 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "O campo 'recebeValeTransporte' deve ser 0 ou 1."
                });
            }

            // Extrai o id do cargo corretamente
            int cargoId = funcionarioElem.GetProperty("cargo").GetProperty("idCargo").GetInt32();

            int novoId = await _funcionarioService.CreateFuncionario(nome, email, senha, recebeValeTransporte, cargoId);

            Funcionario novoFuncionario = new Funcionario();
            novoFuncionario.IdFuncionario = novoId;
            novoFuncionario.NomeFuncionario = nome;
            novoFuncionario.Email = email;
            novoFuncionario.Senha = senha;
            novoFuncionario.RecebeValeTransporte = recebeValeTransporte;
            novoFuncionario.Cargo.IdCargo = cargoId;

            object[] funcionariosArray = new object[] { novoFuncionario };

            object resposta = new
            {
                success = true,
                message = "Cadastro realizado com sucesso",
                data = new { funcionarios = funcionariosArray }
            };

            return StatusCode(201, resposta);
        }

        // PUT: api/v1/funcionarios/{id}
        [HttpPut("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioBody]
        public async Task<IActionResult> Update(int idFuncionario, [FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Update()");

            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);

            string nome = funcionarioElem.GetProperty("nomeFuncionario").GetString() ?? "";
            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";
            int recebeValeTransporte = funcionarioElem.GetProperty("recebeValeTransporte").GetInt32();
            int cargoId = funcionarioElem.GetProperty("idCargo").GetInt32();

            bool atualizou = await _funcionarioService.UpdateFuncionario(idFuncionario, nome, email, senha, recebeValeTransporte, cargoId);

            Funcionario funcionarioAtualizado = new Funcionario();
            funcionarioAtualizado.IdFuncionario = idFuncionario;
            funcionarioAtualizado.NomeFuncionario = nome;
            funcionarioAtualizado.Email = email;
            funcionarioAtualizado.Senha = senha;
            funcionarioAtualizado.RecebeValeTransporte = recebeValeTransporte;
            funcionarioAtualizado.Cargo.IdCargo = cargoId;

            object[] funcionariosArray = new object[] { funcionarioAtualizado };

            if (atualizou)
            {
                object resposta = new
                {
                    success = true,
                    message = "Atualizado com sucesso",
                    data = new { funcionarios = funcionariosArray }
                };

                return Ok(resposta);
            }
            else
            {
                object resposta = new
                {
                    success = false,
                    message = "Funcionário não encontrado para atualização",
                    data = new { funcionarios = funcionariosArray }
                };

                return NotFound(resposta);
            }
        }

        // DELETE: api/v1/funcionarios/{id}
        [HttpDelete("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioId]
        public async Task<IActionResult> Destroy(int idFuncionario)
        {
            Console.WriteLine("🔵 FuncionarioController.Destroy()");
            bool excluiu = await _funcionarioService.DeleteFuncionario(idFuncionario);

            Funcionario funcionarioExcluido = new Funcionario();
            funcionarioExcluido.IdFuncionario = idFuncionario;

            object[] funcionariosArray = new object[] { funcionarioExcluido };

            if (excluiu)
            {
                return NoContent();
            }
            else
            {
                object resposta = new
                {
                    success = false,
                    message = "Funcionário não encontrado para exclusão",
                    data = new { funcionarios = funcionariosArray }
                };

                return NotFound(resposta);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Login()");
            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);

            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";

            var resultadoFuncionario = await _funcionarioService.LoginFuncionario(email, senha);

            if (resultadoFuncionario == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Email ou senha inválidos"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Login efetuado com sucesso!",
                data = resultadoFuncionario
            });

        }
    }
}
