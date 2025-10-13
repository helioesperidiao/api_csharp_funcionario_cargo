using Microsoft.AspNetCore.Mvc;
using Api.Service;
using Api.Filters;
using Api.Model;
using System.Text.Json;

namespace Api.Control
{
    [ApiController]
    [Route("api/v1/cargos")]
    [ValidateToken]
    public class CargoController : ControllerBase
    {
        private readonly CargoService _cargoService;
        
        public CargoController(CargoService cargoService)
        {
            Console.WriteLine("⬆️ CargoController.CargoController()");
            this._cargoService = cargoService;
        }

        // GET: api/v1/cargos
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

        // GET: api/v1/cargos/{id}
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

        // POST: api/v1/cargos
        [HttpPost]
        [ValidateCargoBody]
        public async Task<IActionResult> Store([FromBody] JsonElement requestBody)
        {
            Console.WriteLine("🔵 CargoController.Store()");
            // Extrai o objeto "cargo" do corpo da requisição
            requestBody.TryGetProperty("cargo", out JsonElement cargoElem);

            // Extrai o nome do cargo
            string nomeCargo = cargoElem.GetProperty("nomeCargo").GetString() ?? "";

            // Cria o cargo e obtém o ID gerado
            int novoId = await _cargoService.CreateCargo(nomeCargo);

            // Monta o objeto de retorno
            Cargo novoCargo = new Cargo();
            novoCargo.IdCargo = novoId;
            novoCargo.NomeCargo = nomeCargo;

            object[] cargosArray = new object[] { novoCargo };

            object resposta = new
            {
                success = true,
                message = "Cadastro realizado com sucesso",
                data = new { cargos = cargosArray }
            };

            // Retorna 201 Created com os dados do novo cargo
            return StatusCode(201, resposta);
        }

        // PUT: api/v1/cargos/{id}
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



        // DELETE: api/v1/cargos/{id}
        [HttpDelete("{idCargo}")]
        public async Task<IActionResult> Destroy(int idCargo)
        {
            Console.WriteLine("🔵 CargoController.Destroy()");
            bool excluiu = await _cargoService.DeleteCargo(idCargo);

            Cargo cargoExcluido = new Cargo
            {
                IdCargo = idCargo
            };

            object[] cargosArray = new object[] { cargoExcluido };

            if (excluiu)
            {
                return NoContent();
            }
            else
            {
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
