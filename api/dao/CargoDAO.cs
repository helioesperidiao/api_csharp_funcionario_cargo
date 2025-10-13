using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using Api.Database;
using Api.Model;

namespace Api.Dao
{
    /// <summary>
    /// Classe responsável por realizar operações no banco de dados
    /// relacionadas à entidade Cargo.
    /// </summary>
    public class CargoDAO
    {
        private readonly MySqlDatabase _database;

        /// <summary>
        /// Construtor do DAO, recebe a instância de MySqlDatabase.
        /// </summary>
        /// <param name="databaseInstance">Instância de MySqlDatabase injetada.</param>
        public CargoDAO(MySqlDatabase databaseInstance)
        {
            Console.WriteLine("⬆️  CargoDAO.constructor()");
            _database = databaseInstance ?? throw new ArgumentNullException(nameof(databaseInstance));
        }

        /// <summary>
        /// Cria um novo cargo no banco de dados.
        /// </summary>
        public async Task<int> Create(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Create()");

            string SQL = "INSERT INTO cargo (nomeCargo) VALUES (@nomeCargo);";

            await using var conn = await _database.GetConnection();
            await using var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@nomeCargo", objCargoModel.NomeCargo);

            int insertedId = 0;

            await cmd.ExecuteNonQueryAsync();
            insertedId = (int)cmd.LastInsertedId;

            if (insertedId <= 0)
            {
                throw new Exception("Falha ao inserir cargo");
            }

            return insertedId;
        }

        /// <summary>
        /// Remove um cargo do banco de dados pelo ID.
        /// </summary>
        public async Task<bool> Delete(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Delete()");

            string SQL = "DELETE FROM cargo WHERE idCargo = @idCargo;";

            await using var conn = await _database.GetConnection();
            await using var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@idCargo", objCargoModel.IdCargo);

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        /// <summary>
        /// Atualiza os dados de um cargo existente.
        /// </summary>
        public async Task<bool> Update(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Update()");

            string SQL = "UPDATE cargo SET nomeCargo = @nomeCargo WHERE idCargo = @idCargo;";

            await using var conn = await _database.GetConnection();
            await using var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@nomeCargo", objCargoModel.NomeCargo);
            cmd.Parameters.AddWithValue("@idCargo", objCargoModel.IdCargo);

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        /// <summary>
        /// Retorna todos os cargos cadastrados no banco de dados.
        /// </summary>
        public async Task<List<Cargo>> FindAll()
        {
            Console.WriteLine("🟢 CargoDAO.FindAll()");
            string SQL = "SELECT * FROM cargo;";

            List<Cargo> result = new List<Cargo>();

            await using var conn = await _database.GetConnection();
            await using var cmd = new MySqlCommand(SQL, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Cargo linha = new Cargo();
                linha.IdCargo = reader.GetInt32("idCargo");
                linha.NomeCargo = reader.GetString("nomeCargo");
                result.Add(linha);

            }

            return result;
        }

        /// <summary>
        /// Busca um cargo pelo ID.
        /// </summary>
        public async Task<Cargo?> FindById(int idCargo)
        {
            Console.WriteLine("🟢 CargoDAO.FindById()");
            var results = await FindByField("idCargo", idCargo);
            return results.Count > 0 ? results[0] : null;
        }

        /// <summary>
        /// Busca cargos por um campo específico.
        /// </summary>
        public async Task<List<Cargo>> FindByField(string field, object value)
        {
            Console.WriteLine($"🟢 CargoDAO.FindByField() - Campo: {field}, Valor: {value}");

            var allowedFields = new HashSet<string> { "idCargo", "nomeCargo" };
            if (!allowedFields.Contains(field))
                throw new ArgumentException($"Campo inválido para busca: {field}");

            string SQL = $"SELECT * FROM cargo WHERE {field} = @value;";

            List<Cargo> result = new List<Cargo>();

            await using var conn = await _database.GetConnection();
            await using var cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@value", value);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Cargo linha = new Cargo();
                linha.IdCargo = reader.GetInt32("idCargo");
                linha.NomeCargo = reader.GetString("nomeCargo");
                result.Add(linha);
            }

            return result;
        }
    }
}
