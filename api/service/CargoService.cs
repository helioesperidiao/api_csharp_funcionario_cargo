using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dao;
using Api.Model;
using Api.Utils;

namespace Api.Service
{
    /// <summary>
    /// Classe responsável pela camada de serviço para a entidade Cargo.
    /// 🔹 Demonstra a separação de responsabilidades: regras de negócio separadas do DAO e do controller.
    /// </summary>
    public class CargoService
    {
        // 1️⃣ Atributo privado para acessar o DAO
        private readonly CargoDAO _cargoDAO;

        /// <summary>
        /// Construtor com injeção de dependência do DAO.
        /// 🔹 Permite desacoplar a camada de serviço da implementação do DAO.
        /// </summary>
        /// <param name="cargoDAODependency">Instância de CargoDAO</param>
        public CargoService(CargoDAO cargoDAODependency)
        {
            Console.WriteLine("⬆️  CargoService.constructor()");

            // 2️⃣ atribui o DAO
            _cargoDAO = cargoDAODependency;
        }

        /// <summary>
        /// Cria um novo cargo
        /// 🔹 Valida regras de domínio e impede duplicidade
        /// </summary>
        /// <param name="nomeCargo">Nome do cargo</param>
        /// <returns>ID do cargo criado</returns>
        /// <exception cref="ErrorResponse">Se o cargo já existir</exception>
        public async Task<int> CreateCargo(string nomeCargo)
        {
            Console.WriteLine("🟣 CargoService.CreateCargoAsync()");

            // 3️⃣ Cria objeto Cargo e aplica validações do modelo (setter)
            Cargo cargo = new Cargo();
            cargo.NomeCargo = nomeCargo; // dispara validação do NomeCargo

            // 4️⃣ Valida regra de negócio: não permitir nomes duplicados
            List<Cargo> resultado = await _cargoDAO.FindByField("nomeCargo", cargo.NomeCargo);

            if (resultado.Count > 0)
            {
                // 5️⃣ Lança exceção customizada se já existir
                throw new ErrorResponse(
                    400,
                    "Cargo já existe",
                    $"O cargo {cargo.NomeCargo} já existe"
                );
            }

            // 6️⃣ Chama DAO para inserir no banco
            return await _cargoDAO.Create(cargo);
        }

        /// <summary>
        /// Retorna todos os cargos
        /// 🔹 Simples delegação ao DAO
        /// </summary>
        public async Task<List<Cargo>> FindAll()
        {
            Console.WriteLine("🟣 CargoService.FindAll()");
            return await _cargoDAO.FindAll();
        }

        /// <summary>
        /// Retorna um cargo por ID
        /// 🔹 Aplica validação do modelo antes de chamar o DAO
        /// </summary>
        public async Task<Cargo?> FindById(int idCargo)
        {
            Console.WriteLine("🟣 CargoService.FindById()");

            Cargo cargo = new Cargo();
            cargo.IdCargo = idCargo; // dispara validação do IdCargo

            return await _cargoDAO.FindById(cargo.IdCargo);
        }

        /// <summary>
        /// Atualiza um cargo existente
        /// 🔹 Valida campos e chama DAO para persistir alterações
        /// </summary>
        public async Task<bool> UpdateCargo(int idCargo, string nomeCargo)
        {
            Console.WriteLine("🟣 CargoService.UpdateCargo()");

            Cargo cargo = new Cargo();
            cargo.IdCargo = idCargo;    // dispara validação do IdCargo
            cargo.NomeCargo = nomeCargo; // dispara validação do NomeCargo

            return await _cargoDAO.Update(cargo);
        }

        /// <summary>
        /// Deleta um cargo por ID
        /// 🔹 Valida ID antes de chamar DAO para exclusão
        /// </summary>
        public async Task<bool> DeleteCargo(int idCargo)
        {
            Console.WriteLine("🟣 CargoService.DeleteCargo()");

            Cargo cargo = new Cargo();
            cargo.IdCargo = idCargo;    // dispara validação do IdCargo
            return await _cargoDAO.Delete(cargo);
        }
    }
}
