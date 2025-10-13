using System;

namespace Api.Model
{
    /// <summary>
    /// Representa a entidade Cargo do sistema.
    /// 
    /// Objetivo:
    /// - Encapsular os dados de um cargo.
    /// - Garantir integridade dos atributos via getters e setters.
    /// </summary>
    public class Cargo
    {
        // Atributos privados
        private int _idCargo;
        private string _nomeCargo = string.Empty;


        /// <summary>
        /// Construtor da classe Cargo.
        /// </summary>
        public Cargo()
        {
            //Console.WriteLine("⬆️  Cargo.constructor()");
        }

        /// <summary>
        /// Identificador único do cargo
        /// 🔹 Regra: número inteiro positivo
        /// </summary>
        public int IdCargo
        {
            get => this._idCargo;
            set
            {
                // Verifica se é maior que zero
                if (value <= 0)
                {
                    throw new ArgumentException("IdCargo deve ser maior que zero.");
                }

                this._idCargo = value;
            }
        }

        /// <summary>
        /// Nome do cargo
        /// 🔹 Regra: string não vazia, entre 3 e 64 caracteres
        /// </summary>
        public string NomeCargo
        {
            get => this._nomeCargo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("NomeCargo não pode ser nulo ou vazio.");
                }

                string nome = value.Trim();

                if (nome.Length < 3)
                {
                    throw new ArgumentException("NomeCargo deve ter pelo menos 3 caracteres.");
                }

                if (nome.Length > 64)
                {
                    throw new ArgumentException("NomeCargo deve ter no máximo 64 caracteres.");
                }

                this._nomeCargo = nome;
            }
        }
    }
}
