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
    /// <summary>
    /// Controlador responsável por gerenciar as operações relacionadas à entidade <see cref="Funcionario"/>.
    /// 
    /// Essa classe lida com requisições HTTP relacionadas a funcionários:
    /// - Listagem (GET)
    /// - Consulta por ID (GET /{id})
    /// - Criação (POST)
    /// - Atualização (PUT /{id})
    /// - Exclusão (DELETE /{id})
    /// - Autenticação (POST /login)
    /// 
    /// O controlador utiliza o <see cref="FuncionarioService"/> para executar as regras de negócio
    /// e comunica-se com a camada DAO (acesso ao banco de dados) indiretamente.
    /// </summary>
    [ApiController]
    [Route("api/v1/funcionarios")]
    public class FuncionarioController : ControllerBase
    {
        // Serviço responsável por lidar com a lógica de negócio da entidade Funcionario
        private readonly FuncionarioService _funcionarioService;

        /// <summary>
        /// Construtor do controlador. 
        /// Inicializa a injeção de dependência do serviço FuncionarioService.
        /// </summary>
        /// <param name="funcionarioService">Instância de <see cref="FuncionarioService"/> injetada pelo container do ASP.NET Core.</param>
        public FuncionarioController(FuncionarioService funcionarioService)
        {
            Console.WriteLine("⬆️ FuncionarioController.constructor()");
            this._funcionarioService = funcionarioService;
        }

        /// <summary>
        /// Retorna todos os funcionários cadastrados.
        /// </summary>
        /// <returns>Lista de funcionários e status HTTP 200 (OK).</returns>
        [HttpGet]
        [ValidateToken] // Middleware que valida o token JWT
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("🔵 FuncionarioController.Index()");

            // Busca todos os funcionários através do serviço
            List<Funcionario> listaFuncionarios = await _funcionarioService.FindAll();

            // Monta o corpo da resposta padronizado
            object resposta = new
            {
                success = true,
                message = "Busca realizada com sucesso",
                data = new { funcionarios = listaFuncionarios }
            };

            return Ok(resposta);
        }

        /// <summary>
        /// Retorna um funcionário específico com base no ID informado.
        /// </summary>
        /// <param name="idFuncionario">ID do funcionário.</param>
        /// <returns>Objeto do funcionário encontrado, ou mensagem de erro caso não exista.</returns>
        [HttpGet("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioId] // Middleware que valida se o ID é válido
        public async Task<IActionResult> Show(int idFuncionario)
        {
            Console.WriteLine("🔵 FuncionarioController.Show()");

            // Busca o funcionário pelo ID
            Funcionario? funcionarioEncontrado = await _funcionarioService.FindById(idFuncionario);

            List<Funcionario> funcionariosArray = new List<Funcionario>();

            // Caso encontrado, adiciona na lista para manter o padrão de resposta
            if (funcionarioEncontrado != null)
            {
                funcionariosArray = new List<Funcionario> { funcionarioEncontrado };
            }

            // Monta o corpo da resposta
            object resposta = new
            {
                success = true,
                message = "Executado com sucesso",
                data = new { funcionarios = funcionariosArray }
            };

            return Ok(resposta);
        }

        /// <summary>
        /// Cria um novo funcionário com base nos dados enviados no corpo da requisição.
        /// </summary>
        /// <param name="requestBody">JSON contendo os dados do novo funcionário.</param>
        /// <returns>Status 201 (Created) com os dados do funcionário criado.</returns>
        [HttpPost]
        [ValidateToken]
        [ValidateFuncionarioBody] // Middleware que valida o corpo JSON da requisição
        public async Task<IActionResult> Store([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Store()");

            // Extrai o objeto "funcionario" do corpo da requisição JSON
            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);

            // Extrai os campos básicos
            string nome = funcionarioElem.GetProperty("nomeFuncionario").GetString() ?? "";
            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";

            // Extrai e valida o campo recebeValeTransporte (0 ou 1)
            int recebeValeTransporte = funcionarioElem.GetProperty("recebeValeTransporte").GetInt32();

            // Extrai o ID do cargo associado
            int cargoId = funcionarioElem.GetProperty("cargo").GetProperty("idCargo").GetInt32();

            // Chama o serviço para criar o funcionário e obter o novo ID
            int novoId = await _funcionarioService.CreateFuncionario(nome, email, senha, recebeValeTransporte, cargoId);

            // Monta o objeto do funcionário criado
            Funcionario novoFuncionario = new Funcionario
            {
                IdFuncionario = novoId,
                NomeFuncionario = nome,
                Email = email,
                Senha = senha,
                RecebeValeTransporte = recebeValeTransporte,
                Cargo = new Cargo { IdCargo = cargoId }
            };

            // Cria array para manter padrão da resposta
            Funcionario[] funcionariosArray = new Funcionario[] { novoFuncionario };

            object resposta = new
            {
                success = true,
                message = "Cadastro realizado com sucesso",
                data = new { funcionarios = funcionariosArray }
            };

            // Retorna 201 Created
            return StatusCode(201, resposta);
        }

        /// <summary>
        /// Atualiza os dados de um funcionário existente.
        /// </summary>
        /// <param name="idFuncionario">ID do funcionário a ser atualizado.</param>
        /// <param name="requestBody">JSON contendo os novos dados.</param>
        /// <returns>Status 200 (OK) se atualizado, ou 404 se não encontrado.</returns>
        [HttpPut("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioBody]
        public async Task<IActionResult> Update(int idFuncionario, [FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Update()");

            // Extrai o objeto "funcionario" do corpo JSON
            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);

            // Lê as propriedades enviadas
            string nome = funcionarioElem.GetProperty("nomeFuncionario").GetString() ?? "";
            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";
            int recebeValeTransporte = funcionarioElem.GetProperty("recebeValeTransporte").GetInt32();
            int cargoId = funcionarioElem.GetProperty("cargo").GetProperty("idCargo").GetInt32();

            // Atualiza o funcionário via serviço
            bool atualizou = await _funcionarioService.UpdateFuncionario(idFuncionario, nome, email, senha, recebeValeTransporte, cargoId);

            // Monta o objeto atualizado
            Funcionario funcionarioAtualizado = new Funcionario
            {
                IdFuncionario = idFuncionario,
                NomeFuncionario = nome,
                Email = email,
                Senha = senha,
                RecebeValeTransporte = recebeValeTransporte,
                Cargo = new Cargo { IdCargo = cargoId }
            };

            Funcionario[] funcionariosArray = new Funcionario[] { funcionarioAtualizado };

            // Retorna a resposta adequada
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

        /// <summary>
        /// Remove um funcionário com base no ID informado.
        /// </summary>
        /// <param name="idFuncionario">ID do funcionário a ser excluído.</param>
        /// <returns>Status 204 (No Content) se excluído, ou 404 se não encontrado.</returns>
        [HttpDelete("{idFuncionario}")]
        [ValidateToken]
        [ValidateFuncionarioId]
        public async Task<IActionResult> Destroy(int idFuncionario)
        {
            Console.WriteLine("🔵 FuncionarioController.Destroy()");

            // Solicita exclusão via serviço
            bool excluiu = await _funcionarioService.DeleteFuncionario(idFuncionario);

            Funcionario funcionarioExcluido = new Funcionario { IdFuncionario = idFuncionario };

            if (excluiu)
            {
                // Retorna 204 sem corpo
                return NoContent();
            }
            else
            {
                object resposta = new
                {
                    success = false,
                    message = "Funcionário não encontrado para exclusão",
                    data = new { funcionarios = new[] { funcionarioExcluido } }
                };
                return NotFound(resposta);
            }
        }

        /// <summary>
        /// Realiza o login de um funcionário e retorna o token JWT em caso de sucesso.
        /// </summary>
        /// <param name="requestBody">JSON contendo o email e a senha.</param>
        /// <returns>Status 200 (OK) com o token, ou 401 (Unauthorized) se inválido.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 FuncionarioController.Login()");

            // Extrai os dados do JSON
            requestBody.TryGetProperty("funcionario", out JsonElement funcionarioElem);
            string email = funcionarioElem.GetProperty("email").GetString() ?? "";
            string senha = funcionarioElem.GetProperty("senha").GetString() ?? "";

            // Valida o login via serviço
            var resultadoFuncionario = await _funcionarioService.LoginFuncionario(email, senha);

            // Caso não encontre, retorna 401
            if (resultadoFuncionario == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Email ou senha inválidos"
                });
            }

            // Caso sucesso, retorna token e dados do funcionário
            return Ok(new
            {
                success = true,
                message = "Login efetuado com sucesso!",
                data = resultadoFuncionario
            });
        }
    }
}
