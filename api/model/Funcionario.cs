using System;

namespace Api.Model
{
    /// <summary>
    /// Representa a entidade Funcionario do sistema.
    /// 
    /// Objetivo:
    /// - Encapsular os dados de um funcionário.
    /// - Garantir integridade dos atributos via getters e setters.
    /// 🔹 Demonstra conceitos de POO: encapsulamento, validação e composição.
    /// </summary>
    public class Funcionario
    {
        // 1️⃣ Atributos privados
        // 🔹 Mantêm o estado interno da classe e forçam o uso de propriedades para acesso
        private int _idFuncionario;
        private string _nomeFuncionario = string.Empty;
        private string _email = string.Empty;
        private string _senha = string.Empty;
        private int _recebeValeTransporte;
        private Cargo _cargo;

        /// <summary>
        /// Construtor padrão
        /// 🔹 Inicializa o objeto Funcionario e instancia o Cargo associado
        /// </summary>
        public Funcionario()
        {
            // Console.WriteLine("⬆️ Funcionario.constructor()");
            this._cargo = new Cargo(); // 2️⃣ Composição: cada funcionário tem um cargo
        }

        /// <summary>
        /// Identificador único do funcionário
        /// 🔹 Regra: número inteiro positivo
        /// </summary>
        public int IdFuncionario
        {
            get => this._idFuncionario; // 3️⃣ Getter
            set
            {
                // 4️⃣ Valida se o ID é positivo
                if (value <= 0)
                {
                    throw new ArgumentException("IdFuncionario deve ser maior que zero.");
                }

                this._idFuncionario = value; // 5️⃣ Setter
            }
        }

        /// <summary>
        /// Nome do funcionário
        /// 🔹 Regra: se fornecido, entre 3 e 128 caracteres
        /// </summary>
        public string NomeFuncionario
        {
            get => this._nomeFuncionario;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 6️⃣ Se não informado, atribui string vazia
                    this._nomeFuncionario = string.Empty;
                    return;
                }

                string nome = value.Trim();

                // 7️⃣ Valida tamanho mínimo
                if (nome.Length < 3)
                {
                    throw new ArgumentException("NomeFuncionario deve ter pelo menos 3 caracteres.");
                }

                // 8️⃣ Valida tamanho máximo
                if (nome.Length > 128)
                {
                    throw new ArgumentException("NomeFuncionario deve ter no máximo 128 caracteres.");
                }

                this._nomeFuncionario = nome;
            }
        }

        /// <summary>
        /// Email do funcionário
        /// 🔹 Regra: se fornecido, no máximo 64 caracteres
        /// </summary>
        public string Email
        {
            get => this._email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this._email = string.Empty;
                    return;
                }

                string email = value.Trim();

                // 9️⃣ Valida tamanho máximo
                if (email.Length > 64)
                {
                    throw new ArgumentException("Email deve ter no máximo 64 caracteres.");
                }

                this._email = email;
            }
        }

        /// <summary>
        /// Senha do funcionário
        /// 🔹 Regra: se fornecido, no máximo 64 caracteres
        /// </summary>
        public string Senha
        {
            get => this._senha;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this._senha = string.Empty;
                    return;
                }

                string senha = value.Trim();

                // 10️⃣ Valida tamanho máximo
                if (senha.Length > 64)
                {
                    throw new ArgumentException("Senha deve ter no máximo 64 caracteres.");
                }

                this._senha = senha;
            }
        }

        /// <summary>
        /// Indica se o funcionário recebe vale transporte
        /// 🔹 Regra: valor booleano (0 ou 1)
        /// </summary>
        public int RecebeValeTransporte
        {
            get => _recebeValeTransporte;
            set
            {
                // 11️⃣ Valida se é 0 ou 1
                if (value != 0 && value != 1)
                {
                    throw new ArgumentException("O valor de RecebeValeTransporte deve ser 0 ou 1.");
                }

                _recebeValeTransporte = value;
            }
        }

        /// <summary>
        /// Cargo associado ao funcionário
        /// 🔹 Regra: não pode ser nulo
        /// </summary>
        public Cargo Cargo
        {
            get => this._cargo;
            set
            {
                // 12️⃣ Valida não nulo
                if (value == null)
                {
                    throw new ArgumentException("Cargo não pode ser nulo.");
                }

                this._cargo = value;
            }
        }
    }
}
