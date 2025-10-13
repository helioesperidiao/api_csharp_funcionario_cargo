using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.DAO;
using Api.Model;
using Api.Utils;
namespace Api.Service
{
    /// <summary>
    /// Classe responsável pela camada de serviço para a entidade Cargo.
    /// </summary>
    public class CargoService
    {
        private readonly CargoDAO _cargoDAO;

        /// <summary>
        /// Construtor com injeção de dependência do DAO.
        /// </summary>
        /// <param name="cargoDAODependency">Instância de CargoDAO</param>
        public CargoService(CargoDAO cargoDAODependency)
        {
            Console.WriteLine("⬆️  CargoService.constructor()");
            _cargoDAO = cargoDAODependency ?? throw new ArgumentNullException(nameof(cargoDAODependency));
        }

        /// <summary>
        /// Cria um novo cargo
        /// </summary>
        /// <param name="nomeCargo">Nome do cargo</param>
        /// <returns>ID do cargo criado</returns>
        /// <exception cref="ErrorResponse">Se o cargo já existir</exception>
        public async Task<int> CreateCargo(string nomeCargo)
        {
            Console.WriteLine("🟣 CargoService.CreateCargoAsync()");

            Cargo cargo = new Cargo();
            cargo.NomeCargo = nomeCargo; // valida regra de domínio no set

            // Valida regra de negócio: não permitir nomes duplicados
            List<Cargo> resultado = await _cargoDAO.FindByField("nomeCargo", cargo.NomeCargo);

            if (resultado.Count > 0)
            {
                throw new ErrorResponse(
                    400,
                    "Cargo já existe",
                    $"O cargo {cargo.NomeCargo} já existe"
                );
            }

            return await _cargoDAO.Create(cargo);
        }

        /// <summary>
        /// Retorna todos os cargos
        /// </summary>
        public async Task<List<Cargo>> FindAll()
        {
            Console.WriteLine("🟣 CargoService.FindAll()");
            return await _cargoDAO.FindAll();
        }

        /// <summary>
        /// Retorna um cargo por ID
        /// </summary>
        public async Task<Cargo?> FindById(int idCargo)
        {
            Console.WriteLine("🟣 CargoService.FindById()");

            Cargo cargo = new Cargo();
            cargo.IdCargo= idCargo; // valida regra de domínio no set
        
            return await _cargoDAO.FindById(cargo.IdCargo);
        }

        /// <summary>
        /// Atualiza um cargo existente
        /// </summary>
        public async Task<bool> UpdateCargo(int idCargo, string nomeCargo)
        {
            Console.WriteLine("🟣 CargoService.UpdateCargo()");

            Cargo cargo = new Cargo();
            cargo.IdCargo = idCargo;    // valida regra de domínio no set
            cargo.NomeCargo = nomeCargo; // valida regra de domínio no set

            return await _cargoDAO.Update(cargo);
        }

        /// <summary>
        /// Deleta um cargo por ID
        /// </summary>
        public async Task<bool> DeleteCargo(int idCargo)
        {
            Console.WriteLine("🟣 CargoService.DeleteCargo()");

            Cargo cargo = new Cargo();
            cargo.IdCargo = idCargo;    // valida regra de domínio no set
            return await _cargoDAO.Delete(cargo);
        }
    }

}
