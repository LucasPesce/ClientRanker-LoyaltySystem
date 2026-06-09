using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Ranker.Models
{
    /// Entidad de configuración global del sistema.
    /// Actúa como un Singleton en la base de datos (solo debe existir un registro).
    /// Almacena las reglas dinámicas del negocio y credenciales para no tenerlas "hardcodeadas".
    public class AppConfig
    {
        #region Identificador Único

        public int Id { get; set; }

        #endregion

        #region Control de Sistema y Cierre

        /// Mes del último cierre procesado automáticamente por el sistema.
        public int LastClosedMonth { get; set; }

        /// Año del último cierre procesado automáticamente.
        public int LastClosedYear { get; set; }

        #endregion

        #region Motor de Gamificación (Reglas)

        /// Define la inflación de los puntos: Cuánto dinero debe gastar el cliente para ganar 1 punto.
        public decimal PesosPorPunto { get; set; } = 100m;

        // Umbrales para alcanzar cada nivel
        public int SilverThreshold { get; set; } = 100;
        public int GoldThreshold { get; set; } = 300;
        public int PlatinumThreshold { get; set; } = 600;
        public int DiamondThreshold { get; set; } = 900;

        #endregion

        #region Seguridad

        /// Contraseña encriptada o en texto plano (según el nivel de seguridad requerido) para el modo Supervisor.
        public string SupervisorPassword { get; set; } = "admin123";

        /// PIN de rescate usado en caso de que el administrador olvide la contraseña principal.
        public string RecoveryPin { get; set; } = "0000";

        #endregion
    }
}
