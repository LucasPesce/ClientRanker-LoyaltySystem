# 💎 Client Ranker: Sistema de Lealtad y Fidelización | Luc Pesce

![Estado](https://img.shields.io/badge/Estado-Desarrollo-orange)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0_LTS-512BD4?logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-UI_Framework-0078D4?logo=windows&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?logo=sqlite&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity_Framework-Core_8-3C8A3E?logo=nuget&logoColor=white)

Sistema de escritorio robusto desarrollado bajo la arquitectura MVVM para la gestión de clientes y programas de fidelización. Client Ranker automatiza la asignación de categorías según el nivel de consumo, gestiona el ciclo de vida de los datos históricos y opera de manera 100% offline, garantizando rendimiento y privacidad sin depender de servidores externos.

---

## 🚀 Características Principales

* **Sistema de Rangos Dinámicos:** Cálculo automático de puntos y asignación de categorías (Bronce, Plata, Oro, Platino, Diamante) en tiempo real según el gasto acumulado en el período activo.
* **Gestor de Períodos Inteligente:** Motor de rotación mensual automatizado. Al iniciar la aplicación, el sistema detecta cambios de mes calendario y desplaza los montos actuales al historial, purgando automáticamente los registros obsoletos para optimizar la base de datos.
* **Arquitectura Offline-First:** Persistencia de datos mediante SQLite y Entity Framework Core, creando un entorno autónomo y portable que no requiere de instalaciones complejas de motores SQL ni conexión a internet.
* **Seguridad de Datos (Soft Delete):** Implementación de borrado lógico para la gestión de clientes. Los registros eliminados son inhabilitados visualmente y excluidos de los cálculos, permitiendo su restauración ante errores humanos, hasta que el ciclo mensual los depura físicamente.
* **Patrón MVVM Estricto:** Código limpio y escalable. Separación total entre la lógica de negocio (ViewModels), los modelos de datos (Models) y la interfaz de usuario (Views).

---

## 🛠️ Stack Tecnológico

* **Lenguaje:** C# 12
* **Framework Core:** .NET 8.0 (LTS)
* **Interfaz de Usuario (UI):** WPF (Windows Presentation Foundation) con XAML.
* **Base de Datos Local:** SQLite.
* **ORM:** Entity Framework Core 8.0.
* **Patrones de Diseño:** MVVM (Model-View-ViewModel), Command Pattern (RelayCommand), Soft Delete.

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
