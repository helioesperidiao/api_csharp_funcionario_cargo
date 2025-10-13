using Microsoft.AspNetCore.Mvc;
using Api.Service;
using Api.Filters;
using Api.Model;
using System.Text.Json;

namespace Api.Control
{
    /// <summary>
    /// Controlador responsável pelo gerenciamento de cargos.
    /// Fornece endpoints para criar, listar, atualizar e excluir cargos no sistema.
    /// </summary>
    [ApiController]
    [Route("api/v1/cargos")]
    [ValidateToken] // Middleware personalizado para validar o token JWT em todas as requisições
    public class CargoController : ControllerBase
    {
        private readonly CargoService _cargoService;

        /// <summary>
        /// Construtor do controlador CargoController.
        /// Recebe o serviço de cargos via injeção de dependência.
        /// </summary>
        /// <param name="cargoService">Serviço responsável pela lógica de negócio dos cargos.</param>
        public CargoController(CargoService cargoService)
        {
            Console.WriteLine("⬆️ CargoController.CargoController()");
            this._cargoService = cargoService;
        }

        // ============================================================
        // GET: api/v1/cargos
        // ============================================================

        /// <summary>
        /// Retorna a lista de todos os cargos cadastrados no sistema.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     GET /api/v1/cargos
        /// 
        /// </remarks>
        /// <response code="200">Retorna todos os cargos encontrados.</response>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("🔵 CargoController.Index()");
            List<Cargo> listaCargos = await _cargoService.FindAll();

            object resposta = new
            {
                success = true,
                message = "Busca realizada com sucesso",
                data = new { cargos = listaCargos }
            };

            return Ok(resposta);
        }

        // ============================================================
        // GET: api/v1/cargos/{id}
        // ============================================================

        /// <summary>
        /// Retorna os dados de um cargo específico, baseado no ID fornecido.
        /// </summary>
        /// <param name="idCargo">ID do cargo a ser buscado.</param>
        /// <response code="200">Cargo encontrado e retornado com sucesso.</response>
        /// <response code="404">Cargo não encontrado.</response>
        [HttpGet("{idCargo}")]
        [ValidateCargoId]
        public async Task<IActionResult> Show(int idCargo)
        {
            Console.WriteLine("🔵 CargoController.Show()");
            Cargo? cargoEncontrado = await _cargoService.FindById(idCargo);

            List<Cargo> cargosArray = new List<Cargo>();

            if (cargoEncontrado != null)
            {
                cargosArray = new List<Cargo> { cargoEncontrado };
            }

            object resposta = new
            {
                success = true,
                message = "Executado com sucesso",
                data = new { cargos = cargosArray }
            };
            return Ok(resposta);
        }

        // ============================================================
        // POST: api/v1/cargos
        // ============================================================

        /// <summary>
        /// Cria um novo cargo no sistema.
        /// </summary>
        /// <param name="requestBody">
        /// Corpo JSON contendo o nome do cargo a ser criado.
        /// Exemplo:
        /// 
        /// { "cargo": { "nomeCargo": "Gerente de Projetos" } }
        /// </param>
        /// <response code="201">Cargo criado com sucesso.</response>
        /// <response code="400">Requisição inválida ou dados ausentes.</response>
        [HttpPost]
        [ValidateCargoBody]
        public async Task<IActionResult> Store([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 CargoController.Store()");
            requestBody.TryGetProperty("cargo", out JsonElement cargoElem);
            string nomeCargo = cargoElem.GetProperty("nomeCargo").GetString() ?? "";

            int novoId = await _cargoService.CreateCargo(nomeCargo);

            Cargo novoCargo = new Cargo
            {
                IdCargo = novoId,
                NomeCargo = nomeCargo
            };

            object[] cargosArray = new object[] { novoCargo };

            object resposta = new
            {
                success = true,
                message = "Cadastro realizado com sucesso",
                data = new { cargos = cargosArray }
            };

            return StatusCode(201, resposta);
        }

        // ============================================================
        // PUT: api/v1/cargos/{id}
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um cargo existente.
        /// </summary>
        /// <param name="idCargo">ID do cargo a ser atualizado.</param>
        /// <param name="requestBody">
        /// Corpo JSON contendo o novo nome do cargo.
        /// Exemplo:
        /// 
        /// { "cargo": { "nomeCargo": "Supervisor de Produção" } }
        /// </param>
        /// <response code="200">Cargo atualizado com sucesso.</response>
        /// <response code="404">Cargo não encontrado.</response>
        [HttpPut("{idCargo}")]
        [ValidateCargoBody]
        public async Task<IActionResult> Update(int idCargo, [FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 CargoController.Update()");
            if (!requestBody.TryGetProperty("cargo", out JsonElement cargoElem))
                throw new Exception("JSON inválido: 'cargo' ausente");

            string nomeCargo = cargoElem.GetProperty("nomeCargo").GetString() ?? "";

            bool atualizou = await _cargoService.UpdateCargo(idCargo, nomeCargo);

            Cargo cargoAtualizado = new Cargo
            {
                IdCargo = idCargo,
                NomeCargo = nomeCargo
            };

            object[] cargosArray = new object[] { cargoAtualizado };

            if (atualizou)
            {
                object resposta = new
                {
                    success = true,
                    message = "Atualizado com sucesso",
                    data = new { cargos = cargosArray }
                };

                return Ok(resposta);
            }
            else
            {
                object resposta = new
                {
                    success = false,
                    message = "Cargo não encontrado para atualização",
                    data = new { cargos = cargosArray }
                };

                return NotFound(resposta);
            }
        }

        // ============================================================
        // DELETE: api/v1/cargos/{id}
        // ============================================================

        /// <summary>
        /// Remove um cargo existente pelo seu ID.
        /// </summary>
        /// <param name="idCargo">ID do cargo a ser removido.</param>
        /// <response code="204">Cargo removido com sucesso.</response>
        /// <response code="404">Cargo não encontrado.</response>
        [HttpDelete("{idCargo}")]
        public async Task<IActionResult> Destroy(int idCargo)
        {
            Console.WriteLine("🔵 CargoController.Destroy()");
            bool excluiu = await _cargoService.DeleteCargo(idCargo);

            Cargo cargoExcluido = new Cargo();
            cargoExcluido.IdCargo = idCargo;

            if (excluiu)
            {
                return NoContent();
            }
            else
            {
                Cargo[] cargosArray = new Cargo[] { cargoExcluido };
                object resposta = new
                {
                    success = false,
                    message = "Cargo não encontrado para exclusão",
                    data = new { cargos = cargosArray }
                };

                return NotFound(resposta);
            }
        }
    }
}
