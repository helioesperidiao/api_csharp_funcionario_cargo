using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using Api.Database;
using Api.Model;
using Org.BouncyCastle.Crypto.Generators;

namespace Api.DAO
{
    /// <summary>
    /// Classe responsável por realizar operações no banco de dados
    /// relacionadas à entidade Funcionario.
    /// </summary>
    public class FuncionarioDAO
    {
        private readonly MySqlDatabase _database;

        /// <summary>
        /// Construtor do DAO, recebe a instância de MySqlDatabase.
        /// </summary>
        public FuncionarioDAO(MySqlDatabase databaseInstance)
        {
            Console.WriteLine("⬆️  FuncionarioDAO.constructor()");
            _database = databaseInstance ?? throw new ArgumentNullException(nameof(databaseInstance));
        }

        /// <summary>
        /// Cria um novo funcionário no banco de dados.
        /// </summary>
        public async Task<int> Create(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Create()");

            string SQL = @"
                INSERT INTO Funcionario 
                (nomeFuncionario, email, senha, recebeValeTransporte, Cargo_idCargo) 
                VALUES (@nomeFuncionario, @email, @senha, @recebeValeTransporte, @Cargo_idCargo);";

            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            cmd.Parameters.AddWithValue("@nomeFuncionario", objFuncionario.NomeFuncionario);
            cmd.Parameters.AddWithValue("@email", objFuncionario.Email);
            cmd.Parameters.AddWithValue("@senha", objFuncionario.Senha);
            cmd.Parameters.AddWithValue("@recebeValeTransporte", objFuncionario.RecebeValeTransporte);
            cmd.Parameters.AddWithValue("@Cargo_idCargo", objFuncionario.Cargo.IdCargo);

            int insertedId = 0;
            await cmd.ExecuteNonQueryAsync();
            insertedId = (int)cmd.LastInsertedId;

            if (insertedId <= 0)
            {
                throw new Exception("Falha ao inserir funcionário");
            }

            return insertedId;
        }

        /// <summary>
        /// Remove um funcionário do banco de dados pelo ID.
        /// </summary>
        public async Task<bool> Delete(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Delete()");

            string SQL = "DELETE FROM Funcionario WHERE idFuncionario = @idFuncionario;";

            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@idFuncionario", objFuncionario.IdFuncionario);

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        /// <summary>
        /// Atualiza os dados de um funcionário existente.
        /// </summary>
        public async Task<bool> Update(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Update()");

            string SQL = @"
                UPDATE Funcionario 
                SET nomeFuncionario=@nomeFuncionario, email=@email, senha=@senha, 
                    recebeValeTransporte=@recebeValeTransporte, Cargo_idCargo=@Cargo_idCargo 
                WHERE idFuncionario=@idFuncionario;";

            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            cmd.Parameters.AddWithValue("@nomeFuncionario", objFuncionario.NomeFuncionario);
            cmd.Parameters.AddWithValue("@email", objFuncionario.Email);
            cmd.Parameters.AddWithValue("@senha", objFuncionario.Senha);
            cmd.Parameters.AddWithValue("@recebeValeTransporte", objFuncionario.RecebeValeTransporte);
            cmd.Parameters.AddWithValue("@Cargo_idCargo", objFuncionario.Cargo);
            cmd.Parameters.AddWithValue("@idFuncionario", objFuncionario.IdFuncionario);

            int affectedRows = await cmd.ExecuteNonQueryAsync();
            return affectedRows > 0;
        }

        /// <summary>
        /// Retorna todos os funcionários cadastrados no banco de dados.
        /// </summary>
        public async Task<List<Funcionario>> FindAll()
        {
            Console.WriteLine("🟢 FuncionarioDAO.FindAll()");
            string SQL = "SELECT * FROM funcionario JOIN cargo ON cargo.idCargo = funcionario.idFuncionario";

            List<Funcionario> result = new List<Funcionario>();

            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            await using MySqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Funcionario registro = new Funcionario();
                registro.IdFuncionario = reader.GetInt32("idFuncionario");
                registro.NomeFuncionario = reader.IsDBNull(reader.GetOrdinal("nomeFuncionario")) ? "" : reader.GetString("nomeFuncionario");
                registro.Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email");
                //row.Senha = reader.IsDBNull(reader.GetOrdinal("senha")) ? "" : reader.GetString("senha");
                registro.RecebeValeTransporte = reader.GetInt32("recebeValeTransporte");
                registro.Cargo.IdCargo = reader.GetInt32("idCargo");
                registro.Cargo.NomeCargo = reader.GetString("nomeCargo");

                result.Add(registro);
            }

            return result;
        }

        /// <summary>
        /// Busca um funcionário pelo ID.
        /// </summary>
        public async Task<Funcionario?> FindById(int idFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.FindById()");
            List<Funcionario> results = await FindByField("idFuncionario", idFuncionario);
            return results.Count > 0 ? results[0] : null;
        }

        /// <summary>
        /// Busca funcionários por um campo específico.
        /// </summary>
        public async Task<List<Funcionario>> FindByField(string field, object value)
        {
            Console.WriteLine($"🟢 FuncionarioDAO.FindByField() - Campo: {field}, Valor: {value}");

            HashSet<string> allowedFields = new HashSet<string> { "idFuncionario", "nomeFuncionario", "email", "Cargo_idCargo" };
            if (!allowedFields.Contains(field))
                throw new ArgumentException($"Campo inválido para busca: {field}");

            string SQL = $"SELECT * FROM Funcionario WHERE {field} = @value;";

            List<Funcionario> result = new List<Funcionario>();

            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@value", value);

            await using MySqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Funcionario row = new Funcionario();
                row.IdFuncionario = reader.GetInt32("idFuncionario");
                row.NomeFuncionario = reader.IsDBNull(reader.GetOrdinal("nomeFuncionario")) ? "" : reader.GetString("nomeFuncionario");
                row.Email = reader.IsDBNull(reader.GetOrdinal("email")) ? "" : reader.GetString("email");
                row.Senha = reader.IsDBNull(reader.GetOrdinal("senha")) ? "" : reader.GetString("senha");
                row.RecebeValeTransporte = reader.GetInt32("recebeValeTransporte");
                row.Cargo.IdCargo = reader.GetInt32("idCargo");
                row.Cargo.NomeCargo = reader.GetString("nomeCargo");

                result.Add(row);
            }

            return result;
        }

        /// <summary>
        /// Consulta funcionário pelo email e senha
        /// </summary>
        /// <param name="funcionarioModel">Objeto com email e senha</param>
        /// <returns>Objeto Funcionario completo ou null</returns>
        public async Task<Funcionario?> Login(Funcionario funcionarioModel)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Login()");

            string SQL = @"
                SELECT f.idFuncionario, f.nomeFuncionario, f.email, f.senha, f.recebeValeTransporte,
                       c.idCargo, c.nomeCargo
                FROM funcionario f
                JOIN cargo c ON c.idCargo = f.Cargo_idCargo
                WHERE f.email = @Email;";

            await using var connection = await _database.GetConnection();
            await using var command = new MySqlCommand(SQL, connection);
            command.Parameters.AddWithValue("@Email", funcionarioModel.Email);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                Console.WriteLine("❌ Funcionário não encontrado");
                return null;
            }

            string senhaHash = reader.GetString("senha");
            bool senhaValida = BCrypt.Net.BCrypt.Verify(funcionarioModel.Senha, senhaHash);
            if (!senhaValida)
            {
                Console.WriteLine("❌ Senha inválida");
                return null;
            }

            // Monta objeto Cargo
            Cargo cargo = new Cargo();
            cargo.IdCargo = reader.GetInt32("idCargo");
            cargo.NomeCargo = reader.GetString("nomeCargo");

            // Monta objeto Funcionario
            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = reader.GetInt32("idFuncionario");
            funcionario.NomeFuncionario = reader.GetString("nomeFuncionario");
            funcionario.Email = reader.GetString("email");
            funcionario.RecebeValeTransporte = reader.GetInt32("recebeValeTransporte");
            funcionario.Cargo.IdCargo = reader.GetInt32("idCargo");

            return funcionario;
        }
    }
}
