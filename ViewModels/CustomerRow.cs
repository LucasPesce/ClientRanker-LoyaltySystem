using System;

namespace Client_Ranker.ViewModels
{
    /// Objeto de Transferencia de Datos (DTO) que representa una fila en la tabla de la interfaz de usuario.
    /// Combina los datos puros del cliente con las reglas de negocio inyectadas para calcular 
    /// puntos y categorías en tiempo real, sin sobrecargar ni ensuciar la entidad de la base de datos.
    public class CustomerRow
    {
        #region Datos de la Entidad (Mapeo de Base de Datos)

        /// Identificador único del cliente.
        public int Id { get; set; }

        /// Documento Nacional de Identidad o RUT del cliente.
        public string DocumentId { get; set; } = string.Empty;

        /// Nombre del cliente.
        public string FirstName { get; set; } = string.Empty;

        /// Apellido del cliente.
        public string LastName { get; set; } = string.Empty;

        /// Dinero gastado por el cliente en el período seleccionado (Mes actual o Mes anterior).
        public decimal Spending { get; set; }

        /// Indica si el cliente está activo en el sistema o fue eliminado lógicamente (papelera).
        public bool IsActive { get; set; }

        /// Cantidad de veces que el cliente visitó el local/compró en el mes evaluado.
        public int CurrentMonthVisits { get; set; }

        #endregion

        #region Configuración Inyectada (Reglas de Negocio)

        // IMPORTANTE: Estos valores no le pertenecen al cliente en la base de datos.
        // El MainViewModel los extrae de 'AppConfig' y los "importa" aquí 
        // para que esta fila sepa cómo calcular su propia categoría.

        /// Ratio de conversión: Cuántos pesos equivalen a 1 punto.
        public decimal PesosPorPunto { get; set; }

        public int SilverThreshold { get; set; }
        public int GoldThreshold { get; set; }
        public int PlatinumThreshold { get; set; }
        public int DiamondThreshold { get; set; }

        #endregion

        #region Propiedades Calculadas (Lógica de Presentación)

        /// Puntos acumulados en base al gasto y el ratio de conversión actual.
        /// Previene la división por cero si la configuración de la BD estuviera corrupta.
        public int Points => PesosPorPunto > 0 ? (int)(Spending / PesosPorPunto) : 0;

        /// Categoría dinámica del cliente basada en la gamificación. 
        /// Evalúa los puntos calculados contra los umbrales configurados por el administrador.
        public string Category
        {
            get
            {
                // Evaluamos en cascada desde el rango más bajo al más alto
                if (Points < SilverThreshold) return "Bronce";
                if (Points < GoldThreshold) return "Plata";
                if (Points < PlatinumThreshold) return "Oro";
                if (Points < DiamondThreshold) return "Platino";

                // Si supera todos los umbrales anteriores, pertenece a la categoría máxima.
                return "Diamante";
            }
        }

        #endregion
    }
}