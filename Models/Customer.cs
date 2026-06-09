using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    /// Entidad principal que representa a un cliente en la base de datos.
    /// Almacena su información personal y su comportamiento de consumo en el período actual y anterior.
    public class Customer
    {
        #region Identificación Personal

        /// Clave primaria en la base de datos.
        public int Id { get; set; }

        /// DNI, RUT o Pasaporte. Se usa como identificador único de negocio.
        public string DocumentId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        #endregion

        #region Métricas y Estado

        /// Acumulado de dinero gastado por el cliente durante el mes en curso.
        public decimal CurrentMonthSpending { get; set; }

        /// Gasto histórico del mes inmediatamente anterior.
        public decimal LastMonthSpending { get; set; }

        /// Cantidad de tickets o visitas realizadas en el mes actual.
        public int CurrentMonthVisits { get; set; }

        /// Borrado lógico: false indica que el cliente fue "eliminado" pero mantiene su historial.
        public bool IsActive { get; set; } = true;

        #endregion

        #region Comportamiento del Dominio (Domain Logic)

        /// Ejecuta el cierre de mes a nivel cliente: 
        /// Pasa el gasto actual al histórico y reinicia los contadores.
        public void RotatePeriod()
        {
            LastMonthSpending = CurrentMonthSpending;
            CurrentMonthSpending = 0;
            CurrentMonthVisits = 0; // Reiniciamos también las visitas al rotar el mes
        }

        #endregion
    }
}
