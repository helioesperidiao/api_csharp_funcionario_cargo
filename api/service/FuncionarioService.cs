using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Dao;
using Api.Model;
using Api.Utils;

namespace Api.Service
{
    /// <summary>
    /// Classe responsável pela camada de serviço para a entidade Funcionario.
    /// 🔹 Separa regras de negócio da camada de persistência (DAO).
    /// 🔹 Ideal para adicionar validações, verificações e lógica de autenticação.
    /// </summary>
    public class FuncionarioService
    {
        // 1️⃣ Atributo privado para acessar o DAO
        private readonly FuncionarioDAO _funcionarioDAO;

        /// <summary>
        /// Construtor com injeção de dependência do DAO.
        /// 🔹 Permite desacoplar a camada de serviço do DAO.
        /// </summary>
        public FuncionarioService(FuncionarioDAO funcionarioDAODependency)
        {
            Console.WriteLine("⬆️  FuncionarioService.constructor()");

            // 2️⃣ Valida e atribui o DAO
            _funcionarioDAO = funcionarioDAODependency ?? throw new ArgumentNullException(nameof(funcionarioDAODependency));
        }

        /// <summary>
        /// Cria um novo funcionário
        /// 🔹 Valida regras de domínio (setters do modelo)
        /// 🔹 Verifica duplicidade de email
        /// </summary>
        public async Task<int> CreateFuncionario(string nome, string email, string senha, int recebeValeTransporte, int cargoId)
        {
            Console.WriteLine("🟣 FuncionarioService.CreateFuncionario()");

            // 3️⃣ Cria objeto Funcionario e aplica validações do modelo
            Funcionario funcionario = new Funcionario();
            funcionario.NomeFuncionario = nome;
            funcionario.Email = email;
            funcionario.Senha = senha;
            funcionario.RecebeValeTransporte = recebeValeTransporte;
            funcionario.Cargo.IdCargo = cargoId;

            // 4️⃣ Valida regra de negócio: não permitir emails duplicados
            List<Funcionario> resultado = await _funcionarioDAO.FindByField("email", funcionario.Email);
            if (resultado.Count > 0)
            {
                throw new ErrorResponse(
                    400,
                    "Email já cadastrado",
                    $"O email {funcionario.Email} já está em uso"
                );
            }

            // 5️⃣ Chama DAO para inserir no banco
            return await _funcionarioDAO.Create(funcionario);
        }

        /// <summary>
        /// Retorna todos os funcionários
        /// 🔹 Simples delegação ao DAO
        /// </summary>
        public async Task<List<Funcionario>> FindAll()
        {
            Console.WriteLine("🟣 FuncionarioService.FindAll()");
            return await _funcionarioDAO.FindAll();
        }

        /// <summary>
        /// Retorna um funcionário por ID
        /// 🔹 Aplica validação do modelo antes de consultar
        /// </summary>
        public async Task<Funcionario?> FindById(int idFuncionario)
        {
            Console.WriteLine("🟣 FuncionarioService.FindById()");

            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = idFuncionario; // dispara validação do IdFuncionario

            return await _funcionarioDAO.FindById(funcionario.IdFuncionario);
        }

        /// <summary>
        /// Atualiza um funcionário existente
        /// 🔹 Aplica validação de campos do modelo antes de chamar DAO
        /// </summary>
        public async Task<bool> UpdateFuncionario(int idFuncionario, string nome, string email, string senha, int recebeValeTransporte, int cargoId)
        {
            Console.WriteLine("🟣 FuncionarioService.UpdateFuncionario()");

            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = idFuncionario;
            funcionario.NomeFuncionario = nome;
            funcionario.Email = email;
            funcionario.Senha = senha;
            funcionario.RecebeValeTransporte = recebeValeTransporte;
            funcionario.Cargo.IdCargo = cargoId;

            return await _funcionarioDAO.Update(funcionario);
        }

        /// <summary>
        /// Deleta um funcionário por ID
        /// 🔹 Valida ID antes de chamar DAO
        /// </summary>
        public async Task<bool> DeleteFuncionario(int idFuncionario)
        {
            Console.WriteLine("🟣 FuncionarioService.DeleteFuncionario()");

            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = idFuncionario; // dispara validação do IdFuncionario

            return await _funcionarioDAO.Delete(funcionario);
        }

        /// <summary>
        /// Autentica um funcionário pelo email e senha
        /// 🔹 Verifica se o usuário existe e valida senha
        /// 🔹 Gera token JWT e retorna objeto Usuario
        /// </summary>
        public async Task<Usuario> LoginFuncionario(string email, string senha)
        {
            Console.WriteLine("🟣 FuncionarioService.LoginFuncionario()");

            // 6️⃣ Cria objeto Funcionario apenas para consulta
            Funcionario funcionario = new Funcionario();
            funcionario.Email = email;
            funcionario.Senha = senha;

            // 7️⃣ Consulta no DAO
            Funcionario encontrado = await _funcionarioDAO.Login(funcionario);

            // 8️⃣ Se não encontrado, lança erro de autenticação
            if (encontrado == null)
            {
                throw new ErrorResponse(
                    401,
                    "Usuário ou senha inválidos",
                    new { message = "Não foi possível realizar autenticação" }
                );
            }
         

                // 9️⃣ Geração de token JWT com informações do usuário
                MeuTokenJWT jwt = new MeuTokenJWT();
                string token = jwt.GerarToken(new Dictionary<string, object>
            {
                { "email", encontrado.Email },
                { "role", encontrado.Cargo?.NomeCargo ?? "" },
                { "name", encontrado.NomeFuncionario ?? "" },
                { "idFuncionario", encontrado.IdFuncionario.ToString() }
            });

                // 🔟 Monta objeto Usuario para retorno
                Usuario usuario = new Usuario();
                usuario.Cargo = encontrado.Cargo;
                usuario.IdFuncionario = encontrado.IdFuncionario;
                usuario.NomeFuncionario = encontrado.NomeFuncionario;
                usuario.Token = token;

                return usuario;
            
        }
    }
}
