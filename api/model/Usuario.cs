using System;

namespace Api.Model
{
    /// <summary>
    /// Representa um usuário do sistema.
    /// 
    /// Objetivo:
    /// - Estender a classe Funcionario com propriedades específicas de autenticação.
    /// - Garantir integridade dos dados via getters e setters.
    /// 🔹 Demonstra conceitos de herança e encapsulamento.
    /// </summary>
    public class Usuario : Funcionario
    {
        // 1️⃣ Atributo privado específico do usuário (token JWT)
        // 🔹 Mantém o token seguro e força o uso da propriedade para acesso
        private string _token = "";

        /// <summary>
        /// Construtor padrão.
        /// 🔹 Inicializa a instância do usuário.
        /// </summary>
        public Usuario() { }

        /// <summary>
        /// Token de autenticação do usuário.
        /// 🔹 Regra: string não nula ou vazia.
        /// 🔹 Demonstra validação de propriedades e herança de Funcionario.
        /// </summary>
        public string Token
        {
            get => _token; // 2️⃣ Getter do token
            set
            {
                // 3️⃣ Valida se o token não é nulo ou vazio
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Token não pode ser nulo ou vazio.");
                }

                // 4️⃣ Remove espaços extras e atribui ao atributo privado
                _token = value.Trim();
            }
        }
    }
}
