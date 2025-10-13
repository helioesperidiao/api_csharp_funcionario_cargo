using System;

namespace Api.Model
{
    /// <summary>
    /// Representa a entidade Funcionario do sistema.
    /// 
    /// Objetivo:
    /// - Encapsular os dados de um funcionário.
    /// - Garantir integridade dos atributos via getters e setters.
    /// </summary>
    public class Funcionario
    {
        // Atributos privados
        private int _idFuncionario;
        private string _nomeFuncionario = string.Empty;
        private string _email = string.Empty;
        private string _senha = string.Empty;
        private int _recebeValeTransporte;
        private Cargo _cargo = new Cargo();

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Funcionario()
        {
            // Console.WriteLine("⬆️ Funcionario.constructor()");
        }

        /// <summary>
        /// Identificador único do funcionário
        /// 🔹 Regra: número inteiro positivo
        /// </summary>
        public int IdFuncionario
        {
            get => this._idFuncionario;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("IdFuncionario deve ser maior que zero.");
                }
                this._idFuncionario = value;
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
                    this._nomeFuncionario = string.Empty;
                    return;
                }

                string nome = value.Trim();
                if (nome.Length < 3)
                {
                    throw new ArgumentException("NomeFuncionario deve ter pelo menos 3 caracteres.");
                }
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
                if (senha.Length > 64)
                {
                    throw new ArgumentException("Senha deve ter no máximo 64 caracteres.");
                }
                this._senha = senha;
            }
        }

        /// <summary>
        /// Indica se o funcionário recebe vale transporte
        /// 🔹 Regra: valor booleano
        /// </summary>
        public int RecebeValeTransporte
        {
            get => _recebeValeTransporte;
            set
            {
                if (value != 0 && value != 1)
                {
                    throw new ArgumentException("O valor de RecebeValeTransporte deve ser 0 ou 1.");
                }
                _recebeValeTransporte = value;
            }
        }

        /// <summary>
        /// Id do cargo associado ao funcionário
        /// 🔹 Regra: número inteiro positivo
        /// </summary>
        public Cargo Cargo
        {
            get => this._cargo;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Cargo não pode ser nulo.");
                }
                this._cargo = value;
            }
        }
    }
}
