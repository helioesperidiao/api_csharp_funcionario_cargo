using System;

namespace Api.Model
{
    /// <summary>
    /// Representa a entidade Cargo do sistema.
    /// 
    /// Objetivo:
    /// - Encapsular os dados de um cargo.
    /// - Garantir integridade dos atributos via getters e setters.
    /// 🔹 Demonstra conceitos de POO: encapsulamento e validação.
    /// </summary>
    public class Cargo
    {
        // 1️⃣ Atributos privados
        // 🔹 Mantém o estado interno da classe e força uso de propriedades para acesso
        private int _idCargo;
        private string _nomeCargo = string.Empty;

        /// <summary>
        /// Construtor padrão da classe Cargo.
        /// 🔹 Inicializa a instância e permite log para debug se necessário.
        /// </summary>
        public Cargo()
        {
            //Console.WriteLine("⬆️  Cargo.constructor()");
        }

        /// <summary>
        /// Identificador único do cargo
        /// 🔹 Regra: número inteiro positivo
        /// 🔹 Demonstra validação de entrada via setter
        /// </summary>
        public int IdCargo
        {
            get => this._idCargo; // 2️⃣ Retorna o valor do atributo privado
            set
            {
                // 3️⃣ Valida se o valor é positivo
                if (value <= 0)
                {
                    throw new ArgumentException("IdCargo deve ser maior que zero.");
                }

                // 4️⃣ Atribui o valor ao atributo privado
                this._idCargo = value;
            }
        }

        /// <summary>
        /// Nome do cargo
        /// 🔹 Regra: string não vazia, entre 3 e 64 caracteres
        /// 🔹 Demonstra validação de string e trimming
        /// </summary>
        public string NomeCargo
        {
            get => this._nomeCargo; // 5️⃣ Retorna o valor do atributo privado
            set
            {
                // 6️⃣ Verifica se o valor não é nulo ou vazio
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("NomeCargo não pode ser nulo ou vazio.");
                }

                // 7️⃣ Remove espaços extras
                string nome = value.Trim();

                // 8️⃣ Valida tamanho mínimo
                if (nome.Length < 3)
                {
                    throw new ArgumentException("NomeCargo deve ter pelo menos 3 caracteres.");
                }

                // 9️⃣ Valida tamanho máximo
                if (nome.Length > 64)
                {
                    throw new ArgumentException("NomeCargo deve ter no máximo 64 caracteres.");
                }

                // 🔹 Atribui o valor validado ao atributo privado
                this._nomeCargo = nome;
            }
        }
    }
}
