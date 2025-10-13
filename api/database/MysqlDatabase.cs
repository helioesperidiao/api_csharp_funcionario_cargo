using System;
using System.Threading.Tasks;
using MySqlConnector;

namespace Api.Database
{
    /// <summary>
    /// Classe responsável por gerenciar a conexão com o banco MySQL.
    /// - Mantém um pool estático (singleton), compartilhado entre todas as instâncias.
    /// - Garante que a conexão retornada esteja aberta.
    /// </summary>
    public class MySqlDatabase
    {
        private static MySqlConnectionPool? _pool;

        private readonly string _host;
        private readonly string _user;
        private readonly string _password;
        private readonly string _database;
        private readonly int _port;
        private readonly uint _connectionLimit;

        public MySqlDatabase(
            string host = "127.0.0.1",
            string user = "root",
            string password = "",
            string database = "gestao_rh",
            int port = 3306,
            uint connectionLimit = 10
        )
        {
             Console.WriteLine("⬆️  MySqlDatabase.MySqlDatabase()");
            _host = host;
            _user = user;
            _password = password;
            _database = database;
            _port = port;
            _connectionLimit = connectionLimit;
        }

        /// <summary>
        /// Inicializa o pool se necessário e retorna a conexão aberta
        /// </summary>
        private async Task<MySqlConnection> Connect()
        {
            if (_pool == null)
            {
                var connString = new MySqlConnectionStringBuilder
                {
                    Server = _host,
                    UserID = _user,
                    Password = _password,
                    Database = _database,
                    Port = (uint)_port,
                    MaximumPoolSize = _connectionLimit,
                    ConnectionTimeout = 30
                }.ToString();

                _pool = new MySqlConnectionPool(connString);

                // Testa conexão inicial
                try
                {
                    using var testConn = new MySqlConnection(connString);
                    await testConn.OpenAsync();
                    Console.WriteLine("⬆️  Conectado ao MySQL com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("❌ Falha ao conectar ao MySQL: " + ex.Message);
                    Environment.Exit(1);
                }
            }

            var conn = _pool.GetConnection();
            if (conn.State != System.Data.ConnectionState.Open)
            {
                await conn.OpenAsync(); // garante que a conexão esteja aberta
            }

            return conn;
        }

        /// <summary>
        /// Retorna a conexão aberta
        /// </summary>
        public async Task<MySqlConnection> GetConnection()
        {
            return await Connect();
        }
    }

    /// <summary>
    /// Classe auxiliar simples para simular pool de conexões
    /// </summary>
    public class MySqlConnectionPool
    {
        private readonly string _connectionString;

        public MySqlConnectionPool(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
