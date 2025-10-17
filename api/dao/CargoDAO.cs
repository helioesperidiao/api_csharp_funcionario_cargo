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
    /// 
    /// 🔹 Padrão utilizado: DAO (Data Access Object)
    /// O objetivo é isolar toda a lógica de acesso ao banco de dados.
    /// </summary>
    public class CargoDAO
    {
        private readonly MySqlDatabase _database;

        /// <summary>
        /// Construtor do DAO, recebe a instância de MySqlDatabase.
        /// Essa instância será usada para obter conexões com o banco.
        /// </summary>
        public CargoDAO(MySqlDatabase databaseInstance)
        {
            Console.WriteLine("⬆️  CargoDAO.constructor()");
            _database = databaseInstance ?? throw new ArgumentNullException(nameof(databaseInstance));
        }

        // ============================================================
        // MÉTODO CREATE
        // ============================================================

        /// <summary>
        /// Cria (insere) um novo cargo no banco de dados.
        /// </summary>
        public async Task<int> Create(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Create()");

            // 1️⃣ Definição do comando SQL (usando parâmetros para evitar SQL Injection)
            string SQL = "INSERT INTO cargo (nomeCargo) VALUES (@nomeCargo);";

            // 2️⃣ Abre uma nova conexão com o banco (método GetConnection já faz o OpenAsync)
            await using MySqlConnection conn = await _database.GetConnection();

            // 3️⃣ Cria o comando MySQL associando o SQL e a conexão
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 4️⃣ Substitui o parâmetro @nomeCargo pelo valor vindo do objeto Cargo
            cmd.Parameters.AddWithValue("@nomeCargo", objCargoModel.NomeCargo);

            // 5️⃣ Executa o comando SQL (INSERT) e não espera retorno de dados (apenas resultado)
            await cmd.ExecuteNonQueryAsync();

            // 6️⃣ Obtém o ID gerado automaticamente no banco (auto_increment)
            int insertedId = (int)cmd.LastInsertedId;

            // 7️⃣ Validação básica — se o ID não for gerado, algo deu errado
            if (insertedId <= 0)
            {
                throw new Exception("Falha ao inserir cargo");
            }

            // 8️⃣ Retorna o ID do novo registro inserido
            return insertedId;
        }

        // ============================================================
        // MÉTODO DELETE
        // ============================================================

        /// <summary>
        /// Remove um cargo do banco de dados pelo ID.
        /// </summary>
        public async Task<bool> Delete(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Delete()");

            // 1️⃣ Comando SQL com filtro pelo idCargo
            string SQL = "DELETE FROM cargo WHERE idCargo = @idCargo;";

            // 2️⃣ Criação da conexão e comando SQL
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 3️⃣ Substitui o parâmetro pelo valor do objeto Cargo
            cmd.Parameters.AddWithValue("@idCargo", objCargoModel.IdCargo);

            // 4️⃣ Executa o comando e retorna o número de linhas afetadas
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            // 5️⃣ Retorna true se ao menos uma linha foi excluída
            return affectedRows > 0;
        }

        // ============================================================
        // MÉTODO UPDATE
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um cargo existente.
        /// </summary>
        public async Task<bool> Update(Cargo objCargoModel)
        {
            Console.WriteLine("🟢 CargoDAO.Update()");

            // 1️⃣ SQL de atualização
            string SQL = "UPDATE cargo SET nomeCargo = @nomeCargo WHERE idCargo = @idCargo;";

            // 2️⃣ Conexão e comando
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);

            // 3️⃣ Substitui parâmetros pelos valores do objeto
            cmd.Parameters.AddWithValue("@nomeCargo", objCargoModel.NomeCargo);
            cmd.Parameters.AddWithValue("@idCargo", objCargoModel.IdCargo);

            // 4️⃣ Executa o UPDATE
            int affectedRows = await cmd.ExecuteNonQueryAsync();

            // 5️⃣ Retorna true se a atualização afetou pelo menos uma linha
            return affectedRows > 0;
        }

        // ============================================================
        // MÉTODO FIND ALL
        // ============================================================

        /// <summary>
        /// Retorna todos os cargos cadastrados no banco de dados.
        /// </summary>
        public async Task<List<Cargo>> FindAll()
        {
            Console.WriteLine("🟢 CargoDAO.FindAll()");

            // 1️⃣ Comando SQL para buscar todos os cargos
            string SQL = "SELECT * FROM cargo;";

            // 2️⃣ Lista onde serão armazenados os resultados
            List<Cargo> result = new List<Cargo>();

            // 3️⃣ Criação da conexão, comando e leitor de dados
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            await using MySqlDataReader registros = await cmd.ExecuteReaderAsync();

            // 4️⃣ Percorre cada linha retornada pelo SELECT
            while (await registros.ReadAsync())
            {
                // Cria um objeto Cargo e preenche com os dados do banco
                Cargo linha = new Cargo();
                linha.IdCargo = registros.GetInt32("idCargo");
                linha.NomeCargo = registros.GetString("nomeCargo");

                // Adiciona o objeto na lista
                result.Add(linha);
            }

            // 5️⃣ Retorna a lista de cargos preenchida
            return result;
        }

        // ============================================================
        // MÉTODO FIND BY ID
        // ============================================================

        /// <summary>
        /// Busca um cargo específico pelo seu ID.
        /// </summary>
        public async Task<Cargo?> FindById(int idCargo)
        {
            Console.WriteLine("🟢 CargoDAO.FindById()");

            // 🔸 Reutiliza o método genérico FindByField para não repetir código
            List<Cargo> listaCargos = await FindByField("idCargo", idCargo);

            // 🔸 Retorna o primeiro item, ou null se não encontrar
            return listaCargos.Count > 0 ? listaCargos[0] : null;
        }

        // ============================================================
        // MÉTODO FIND BY FIELD
        // ============================================================

        /// <summary>
        /// Busca cargos por um campo específico (idCargo ou nomeCargo).
        /// </summary>
        public async Task<List<Cargo>> FindByField(string field, object value)
        {
            Console.WriteLine($"🟢 CargoDAO.FindByField() - Campo: {field}, Valor: {value}");

            // 1️⃣ Validação do campo para evitar SQL Injection
            HashSet<string> allowedFields = new HashSet<string> { "idCargo", "nomeCargo" };
            if (!allowedFields.Contains(field))
                throw new ArgumentException($"Campo inválido para busca: {field}");

            // 2️⃣ Monta a query dinamicamente com o campo válido
            string SQL = $"SELECT * FROM cargo WHERE {field} = @value;";

            // 3️⃣ Lista para armazenar os resultados
            List<Cargo> listaCargos = new List<Cargo>();

            // 4️⃣ Cria a conexão e o comando SQL
            await using MySqlConnection conn = await _database.GetConnection();
            await using MySqlCommand cmd = new MySqlCommand(SQL, conn);
            cmd.Parameters.AddWithValue("@value", value);

            // 5️⃣ Executa o SELECT e lê os registros retornados
            await using MySqlDataReader registros = await cmd.ExecuteReaderAsync();

            while (await registros.ReadAsync())
            {
                // Monta o objeto Cargo a partir dos dados do banco
                Cargo cargo = new Cargo();
                cargo.IdCargo = registros.GetInt32("idCargo");
                cargo.NomeCargo = registros.GetString("nomeCargo");
                listaCargos.Add(cargo);
            }

            // 6️⃣ Retorna a lista de cargos encontrados
            return listaCargos;
        }
    }
}
