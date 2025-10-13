using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.DAO;
using Api.Model;
using Api.Utils;

namespace Api.Service
{
    /// <summary>
    /// Classe responsável pela camada de serviço para a entidade Funcionario.
    /// </summary>
    public class FuncionarioService
    {
        private readonly FuncionarioDAO _funcionarioDAO;

        /// <summary>
        /// Construtor com injeção de dependência do DAO.
        /// </summary>
        /// <param name="funcionarioDAODependency">Instância de FuncionarioDAO</param>
        public FuncionarioService(FuncionarioDAO funcionarioDAODependency)
        {
            Console.WriteLine("⬆️  FuncionarioService.constructor()");
            _funcionarioDAO = funcionarioDAODependency ?? throw new ArgumentNullException(nameof(funcionarioDAODependency));
        }

        /// <summary>
        /// Cria um novo funcionário
        /// </summary>
        /// <param name="nome">Nome do funcionário</param>
        /// <param name="email">Email</param>
        /// <param name="senha">Senha</param>
        /// <param name="recebeValeTransporte">Se recebe VT</param>
        /// <param name="cargoId">ID do cargo</param>
        /// <returns>ID do funcionário criado</returns>
        /// <exception cref="ErrorResponse">Se o email já existir</exception>
        public async Task<int> CreateFuncionario(string nome, string email, string senha, int recebeValeTransporte, int cargoId)
        {
            Console.WriteLine("🟣 FuncionarioService.CreateFuncionario()");

            Funcionario funcionario = new Funcionario();
            funcionario.NomeFuncionario = nome;
            funcionario.Email = email;
            funcionario.Senha = senha;
            funcionario.RecebeValeTransporte = recebeValeTransporte;
            funcionario.Cargo.IdCargo = cargoId;

            // Valida regra de negócio: não permitir emails duplicados
            List<Funcionario> resultado = await _funcionarioDAO.FindByField("email", funcionario.Email);

            if (resultado.Count > 0)
            {
                throw new ErrorResponse(
                    400,
                    "Email já cadastrado",
                    $"O email {funcionario.Email} já está em uso"
                );
            }

            return await _funcionarioDAO.Create(funcionario);
        }

        /// <summary>
        /// Retorna todos os funcionários
        /// </summary>
        public async Task<List<Funcionario>> FindAll()
        {
            Console.WriteLine("🟣 FuncionarioService.FindAll()");
            return await _funcionarioDAO.FindAll();
        }

        /// <summary>
        /// Retorna um funcionário por ID
        /// </summary>
        public async Task<Funcionario?> FindById(int idFuncionario)
        {
            Console.WriteLine("🟣 FuncionarioService.FindById()");

            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = idFuncionario; // valida regra de domínio

            return await _funcionarioDAO.FindById(funcionario.IdFuncionario);
        }

        /// <summary>
        /// Atualiza um funcionário existente
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
        /// </summary>
        public async Task<bool> DeleteFuncionario(int idFuncionario)
        {
            Console.WriteLine("🟣 FuncionarioService.DeleteFuncionario()");

            Funcionario funcionario = new Funcionario();
            funcionario.IdFuncionario = idFuncionario; // valida regra de domínio

            return await _funcionarioDAO.Delete(funcionario);
        }

        //LoginFuncionario

         /// <summary>
        /// Autentica um funcionário pelo email e senha
        /// </summary>
        /// <param name="email">Email do funcionário</param>
        /// <param name="senha">Senha do funcionário</param>
        /// <returns>Objeto contendo dados do funcionário e token JWT</returns>
        /// <exception cref="ErrorResponse">Se email ou senha estiverem incorretos</exception>
        public async Task<object> LoginFuncionario(string email, string senha)
        {
            Console.WriteLine("🟣 FuncionarioService.LoginFuncionario()");

            // Cria objeto Funcionario apenas para consulta
            Funcionario funcionario = new Funcionario();
            funcionario.Email = email;
            funcionario.Senha = senha;

            // Consulta no DAO
            Funcionario encontrado = await _funcionarioDAO.Login(funcionario);
            
            if (encontrado == null)
            {
                throw new ErrorResponse(
                    401,
                    "Usuário ou senha inválidos",
                    new { message = "Não foi possível realizar autenticação" }
                );
            }

            // Geração de token JWT
            MeuTokenJWT jwt = new MeuTokenJWT();
            var claims = new Dictionary<string, object>
            {
                { "email", encontrado.Email },
                { "role", encontrado.Cargo?.NomeCargo ?? "" },
                { "name", encontrado.NomeFuncionario ?? "" },
                { "idFuncionario", encontrado.IdFuncionario.ToString() }
            };

            string token = jwt.GerarToken(claims);

            // Monta o objeto de retorno
            var user = new
            {
                funcionario = new
                {
                    email = encontrado.Email,
                    role = encontrado.Cargo?.NomeCargo,
                    name = encontrado.NomeFuncionario,
                    idFuncionario = encontrado.IdFuncionario
                }
            };

            return new
            {
                user,
                token
            };
        }
    }
}
