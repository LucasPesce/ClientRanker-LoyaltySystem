# 💎 Client Ranker: Sistema de Lealtad y Fidelización | Luc Pesce

![Estado](https://img.shields.io/badge/Estado-MVP-success)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0_LTS-512BD4?logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-UI_Framework-0078D4?logo=windows&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?logo=sqlite&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity_Framework-Core_8-3C8A3E?logo=nuget&logoColor=white)

Sistema de escritorio robusto desarrollado bajo la arquitectura **MVVM estricta y contenedores de Inyección de Dependencias (DI)**. Client Ranker automatiza la asignación de categorías según el nivel de consumo, gestiona el ciclo de vida de datos históricos, ofrece análisis de negocio en tiempo real y opera de manera 100% offline.

---

## 🚀 Características Principales

* **Sistema de Rangos Dinámicos:** Cálculo automático de puntos y asignación de categorías (Bronce, Plata, Oro, Platino, Diamante) en tiempo real según el gasto acumulado y reglas configurables.
* **Gestor de Períodos Inteligente:** Motor de rotación mensual automatizado. El sistema detecta cambios de mes calendario y desplaza los montos actuales al historial, purgando y guardando fotos contables de manera automática.
* **Dashboard y Motor de Insights:** Panel de estadísticas visuales (LiveCharts) con un algoritmo que detecta anomalías estacionales, caídas de tráfico y oportunidades de up-selling basándose en el historial de ventas.
* **Exportación a Excel:** Generación de reportes comerciales formateados utilizando ClosedXML.
* **Arquitectura Offline-First:** Persistencia mediante SQLite y Entity Framework Core, creando un entorno autónomo y portable sin requerir motores SQL externos ni conexión a internet.
* **Seguridad de Datos (Soft Delete):** Implementación de borrado lógico. Los registros eliminados son inhabilitados visualmente permitiendo su restauración ante errores humanos.
* **Desacoplamiento Estricto:** Aplicación del patrón de Inyección de Dependencias de Microsoft para la gestión de ciclos de vida de base de datos y servicios de navegación UI.

---

## 🛠️ Stack Tecnológico

* **Lenguaje:** C# 12
* **Framework Core:** .NET 8.0 (LTS)
* **Interfaz de Usuario (UI):** WPF (Windows Presentation Foundation) con XAML.
* **Base de Datos & ORM:** SQLite + Entity Framework Core 8.0
* **Librerías Adicionales:** LiveChartsCore (Gráficos), ClosedXML (Reportes Excel), Microsoft.Extensions.DependencyInjection.
* **Patrones de Diseño:** MVVM, Inyección de Dependencias (DI), Service Locator (DialogService), Command Pattern, Soft Delete.

---

## 🔑 Acceso Administrativo y Modo Demo
Para facilitar la revisión técnica del proyecto, el sistema incluye funciones de prueba y accesos por defecto:
* **Configuración / Modo Supervisor:** Para acceder a las métricas del sistema y cambiar reglas de gamificación, utiliza la contraseña por defecto: `admin123`. (PIN de rescate: `0000`).
* **Generar Datos Ficticios:** En el panel principal, existe un botón oculto/de sistema (ícono de prueba) que inyecta **12 meses de datos históricos aleatorios**. Esto permite visualizar inmediatamente el funcionamiento de los gráficos y el motor de insights en el Dashboard sin necesidad de cargar ventas manualmente durante meses.

---

## ⚙️ Instalación y Uso Local

Sigue estos pasos para clonar, compilar y ejecutar este proyecto en tu entorno de desarrollo local:

### 1. Requisitos Previos
* Instalar **Visual Studio 2022** con la carga de trabajo: *Desarrollo de escritorio de .NET*.
* Asegurar que el runtime de **.NET 8.0** esté instalado.
* Tener instalada la herramienta global de Entity Framework para la consola:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 2. Clonar el repositorio
```bash
git clone [https://github.com/lucaspesce/ClientRanker.git](https://github.com/lucaspesce/ClientRanker.git)
cd ClientRanker
```

### 3. Abrir la Solución
* Haz doble clic en el archivo `Client Ranker.sln` para abrir el proyecto en Visual Studio.
* Compila la solución completa presionando `Ctrl + Shift + B` para restaurar los paquetes NuGet automáticamente.

### 4. Construir la Base de Datos
Antes de ejecutar el programa por primera vez, debes aplicar las migraciones para generar el archivo físico de SQLite (`loyalty_database.db`). Abre la Terminal Integrada de Visual Studio (`Ctrl + ñ`) y ejecuta:
```bash
dotnet ef database update
```

### 5. Iniciar la Aplicación
* Presiona `F5` o haz clic en el botón "Iniciar" en Visual Studio.

---

## 📁 Estructura del Proyecto

Breve desglose de la distribución arquitectónica del sistema:

```text
Client Ranker/
 ├── Data/             # Contexto de base de datos (AppDbContext) y conexión SQLite
 ├── Migrations/       # Planos históricos de Entity Framework para versionado de DB
 ├── Models/           # Entidades de dominio (Customer, AppConfig)
 ├── ViewModels/       # Lógica de presentación y comandos (MainViewModel, RelayCommand)
 ├── Views/            # Interfaces XAML (MainWindow, AddCustomerWindow, AddPurchaseWindow)
 └── App.xaml          # Punto de entrada de la aplicación WPF
```

---

## 📁 Autor

**Luc Pesce**
* Analista en Sistemas | Frontend & .NET Developer
* [LinkedIn](https://www.linkedin.com/in/lucaspesce/)
* [GitHub](https://github.com/lucaspesce)
