using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using Api.Database;
using Api.Model;
using Org.BouncyCastle.Crypto.Generators;

namespace Api.Dao
{
    /// <summary>
    /// Classe responsável por realizar operações no banco de dados
    /// relacionadas à entidade Funcionario.
    /// 
    /// 🔹 Padrão DAO (Data Access Object)
    /// Separa a lógica de acesso a dados da lógica de negócio.
    /// </summary>
    public class FuncionarioDAO
    {
        private readonly MySqlDatabase _database;

        /// <summary>
        /// Construtor do DAO.
        /// Recebe a instância de MySqlDatabase que gerencia as conexões com o banco.
        /// </summary>
        public FuncionarioDAO(MySqlDatabase databaseInstance)
        {
            Console.WriteLine("⬆️  FuncionarioDAO.constructor()");
            _database = databaseInstance ?? throw new ArgumentNullException(nameof(databaseInstance));
        }

        // ============================================================
        // MÉTODO CREATE (INSERÇÃO DE FUNCIONÁRIO)
        // ============================================================
        public async Task<int> Create(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Create()");

            // 1️⃣ SQL com parâmetros nomeados para evitar SQL Injection
            string SQL = @"
                INSERT INTO Funcionario 
                (nomeFuncionario, email, senha, recebeValeTransporte, Cargo_idCargo) 
                VALUES (@nomeFuncionario, @email, @senha, @recebeValeTransporte, @Cargo_idCargo);";

            // 2️⃣ Abre conexão com o banco de dados
            await using MySqlConnection conn = await _database.GetConnection();

            // 3️⃣ Cria comando SQL associado à conexão
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 4️⃣ Adiciona os valores dos parâmetros vindos do objeto Funcionario
            cmd.Parameters.AddWithValue("@nomeFuncionario", objFuncionario.NomeFuncionario);
            cmd.Parameters.AddWithValue("@email", objFuncionario.Email);
            cmd.Parameters.AddWithValue("@senha", objFuncionario.Senha);
            cmd.Parameters.AddWithValue("@recebeValeTransporte", objFuncionario.RecebeValeTransporte);
            cmd.Parameters.AddWithValue("@Cargo_idCargo", objFuncionario.Cargo.IdCargo);

            // 5️⃣ Executa o comando no banco
            await cmd.ExecuteNonQueryAsync();

            // 6️⃣ Obtém o ID do registro recém-inserido
            int insertedId = (int)cmd.LastInsertedId;

            // 7️⃣ Validação básica: se ID <= 0, algo deu errado
            if (insertedId <= 0)
            {
                throw new Exception("Falha ao inserir funcionário");
            }

            // 8️⃣ Retorna o ID do funcionário inserido
            return insertedId;
        }

        // ============================================================
        // MÉTODO DELETE (EXCLUSÃO DE FUNCIONÁRIO)
        // ============================================================
        public async Task<bool> Delete(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Delete()");

            // 1️⃣ SQL para deletar funcionário pelo ID
            string SQL = "DELETE FROM Funcionario WHERE idFuncionario = @idFuncionario;";

            // 2️⃣ Abre conexão e cria comando
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 3️⃣ Substitui parâmetro pelo ID do funcionário
            cmd.Parameters.AddWithValue("@idFuncionario", objFuncionario.IdFuncionario);

            // 4️⃣ Executa o comando e retorna o número de linhas afetadas
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            // 5️⃣ Retorna true se alguma linha foi deletada
            return affectedRows > 0;
        }

        // ============================================================
        // MÉTODO UPDATE (ATUALIZAÇÃO DE FUNCIONÁRIO)
        // ============================================================
        public async Task<bool> Update(Funcionario objFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Update()");

            // 1️⃣ SQL de atualização com parâmetros nomeados
            string SQL = @"
                UPDATE Funcionario 
                SET nomeFuncionario=@nomeFuncionario, email=@email, senha=@senha, 
                    recebeValeTransporte=@recebeValeTransporte, Cargo_idCargo=@Cargo_idCargo 
                WHERE idFuncionario=@idFuncionario;";

            // 2️⃣ Abre conexão e cria comando
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 3️⃣ Define os valores dos parâmetros
            cmd.Parameters.AddWithValue("@nomeFuncionario", objFuncionario.NomeFuncionario);
            cmd.Parameters.AddWithValue("@email", objFuncionario.Email);
            cmd.Parameters.AddWithValue("@senha", objFuncionario.Senha);
            cmd.Parameters.AddWithValue("@recebeValeTransporte", objFuncionario.RecebeValeTransporte);
            cmd.Parameters.AddWithValue("@Cargo_idCargo", objFuncionario.Cargo.IdCargo);
            cmd.Parameters.AddWithValue("@idFuncionario", objFuncionario.IdFuncionario);

            // 4️⃣ Executa o comando e obtém número de linhas afetadas
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            // 5️⃣ Retorna true se algum registro foi atualizado
            return affectedRows > 0;
        }

        // ============================================================
        // MÉTODO FIND ALL (LISTAR TODOS FUNCIONÁRIOS)
        // ============================================================
        public async Task<List<Funcionario>> FindAll()
        {
            Console.WriteLine("🟢 FuncionarioDAO.FindAll()");

            // 1️⃣ SQL para buscar todos os funcionários, incluindo dados do cargo
            string SQL = "SELECT * FROM funcionario JOIN cargo ON cargo.idCargo = funcionario.idFuncionario";

            // 2️⃣ Lista que armazenará os registros lidos do banco
            List<Funcionario> listaFuncionarios = new List<Funcionario>();

            // 3️⃣ Abre conexão e cria comando
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 4️⃣ Executa o SELECT e obtém um leitor de dados
            await using MySqlDataReader registros = await cmd.ExecuteReaderAsync();

            // 5️⃣ Itera cada linha retornada pelo banco
            while (await registros.ReadAsync())
            {
                // 5.1️⃣ Cria objeto Funcionario e preenche seus atributos
                Funcionario registro = new Funcionario();
                registro.IdFuncionario = registros.GetInt32("idFuncionario");
                registro.NomeFuncionario = registros.GetString("nomeFuncionario");
                registro.Email = registros.GetString("email");
                registro.RecebeValeTransporte = registros.GetInt32("recebeValeTransporte");
                
                registro.Cargo.IdCargo = registros.GetInt32("idCargo");
                registro.Cargo.NomeCargo = registros.GetString("nomeCargo");

                // 5.2️⃣ Adiciona o objeto à lista de resultados
                listaFuncionarios.Add(registro);
            }

            // 6️⃣ Retorna a lista completa de funcionários
            return listaFuncionarios;
        }

        // ============================================================
        // MÉTODO FIND BY ID (BUSCAR FUNCIONÁRIO POR ID)
        // ============================================================
        public async Task<Funcionario?> FindById(int idFuncionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.FindById()");

            // 1️⃣ Reutiliza método FindByField para não duplicar código
            List<Funcionario> results = await FindByField("idFuncionario", idFuncionario);

            // 2️⃣ Retorna o primeiro registro encontrado ou null
            return results.Count > 0 ? results[0] : null;
        }

        // ============================================================
        // MÉTODO FIND BY FIELD (BUSCA FUNCIONÁRIOS POR QUALQUER CAMPO)
        // ============================================================
        public async Task<List<Funcionario>> FindByField(string field, object value)
        {
            Console.WriteLine($"🟢 FuncionarioDAO.FindByField() - Campo: {field}, Valor: {value}");

            // 1️⃣ Validação de campos permitidos para evitar SQL Injection
            HashSet<string> allowedFields = new HashSet<string>
            { "idFuncionario", "nomeFuncionario", "email", "Cargo_idCargo" };

            if (!allowedFields.Contains(field))
                throw new ArgumentException($"Campo inválido para busca: {field}");

            // 2️⃣ SQL de busca com parâmetro
            string SQL = $"SELECT * FROM Funcionario join cargo on cargo.idcargo = funcionario.cargo_idCargo WHERE {field} = @value;";

            // 3️⃣ Lista que armazenará os registros encontrados
            List<Funcionario> result = new List<Funcionario>();

            // 4️⃣ Cria conexão e comando
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@value", value);

            // 5️⃣ Executa SELECT e obtém leitor de dados
            await using MySqlDataReader registros = await cmd.ExecuteReaderAsync();

            // 6️⃣ Itera os registros retornados
            while (await registros.ReadAsync())
            {
                Funcionario row = new Funcionario();
                row.IdFuncionario = registros.GetInt32("idFuncionario");
                row.NomeFuncionario = registros.GetString("nomeFuncionario");
                row.Email = registros.GetString("email");
                row.Senha = registros.GetString("senha");
                row.RecebeValeTransporte = registros.GetInt32("recebeValeTransporte");
                row.Cargo.IdCargo = registros.GetInt32("idCargo");
                row.Cargo.NomeCargo = registros.GetString("nomeCargo");

                result.Add(row);
            }

            // 7️⃣ Retorna a lista de funcionários encontrados
            return result;
        }

        // ============================================================
        // MÉTODO LOGIN (AUTENTICAÇÃO)
        // ============================================================
        public async Task<Funcionario?> Login(Funcionario funcionario)
        {
            Console.WriteLine("🟢 FuncionarioDAO.Login()");

            // 1️⃣ SQL para buscar funcionário pelo email, incluindo o cargo
            string SQL = @"
                SELECT * 
                FROM funcionario
                JOIN cargo on cargo.idCargo = funcionario.Cargo_idCargo
                WHERE email = @Email;";

            // 2️⃣ Abre conexão e cria comando
            await using MySqlConnection connection = await _database.GetConnection();
            await using MySqlCommand command = new MySqlCommand(SQL, connection);
            command.Parameters.AddWithValue("@Email", funcionario.Email);

            // 3️⃣ Executa SELECT e obtém leitor
            await using MySqlDataReader registros = await command.ExecuteReaderAsync();

            // 4️⃣ Se não encontrou registro, retorna null
            if (!await registros.ReadAsync())
            {
                Console.WriteLine("❌ Funcionário não encontrado");
                return null;
            }

            // 5️⃣ Valida a senha usando BCrypt
            string senhaHash = registros.GetString("senha");
            bool senhaValida = BCrypt.Net.BCrypt.Verify(funcionario.Senha, senhaHash);
            if (!senhaValida)
            {
                Console.WriteLine("❌ Senha inválida");
                return null;
            }

            // 6️⃣ Monta objeto Cargo
            Cargo cargo = new Cargo();
            cargo.IdCargo = registros.GetInt32("idCargo");
            cargo.NomeCargo = registros.GetString("nomeCargo");

            // 7️⃣ Preenche objeto Funcionario com os dados do banco
            funcionario.IdFuncionario = registros.GetInt32("idFuncionario");
            funcionario.NomeFuncionario = registros.GetString("nomeFuncionario");
            funcionario.RecebeValeTransporte = registros.GetInt32("recebeValeTransporte");
            funcionario.Cargo.IdCargo = registros.GetInt32("idCargo");

            // 8️⃣ Retorna o funcionário autenticado
            return funcionario;
        }
    }
}
