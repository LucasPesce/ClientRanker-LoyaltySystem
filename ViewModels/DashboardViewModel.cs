using Client_Ranker.Data;
using Client_Ranker.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client_Ranker.ViewModels
{
    /// ViewModel para la ventana de Estadísticas/Dashboard.
    /// Carga y presenta datos históricos y métricas actuales para ofrecer una visión general del negocio.
    public class DashboardViewModel
    {
        #region Dependencias

        private readonly IDbContextFactory<AppDbContext>? _dbContextFactory;

        #endregion

        #region Propiedades de Gráficos

        /// Configuración para el gráfico de barras de evolución de ingresos mensuales.
        public ISeries[] RevenueSeries { get; set; }

        /// Configuración para los ejes X del gráfico de barras (mes/año).
        public Axis[] XAxes { get; set; }

        /// Configuración para el gráfico de torta que muestra la distribución de clientes por categoría.
        public IEnumerable<ISeries> CategorySeries { get; set; }

        #endregion

        #region Propiedades de Métricas

        /// Ingresos totales del último mes registrado.
        public decimal TotalRevenue { get; set; }

        /// Cantidad total de tickets del último mes registrado.
        public int TotalTickets { get; set; }

        /// Ticket promedio del último mes registrado.
        public decimal AverageTicket { get; set; }

        /// Mensaje dinámico generado por el motor de análisis inteligente.
        public string InsightMessage { get; set; }

        // Nota: Las cantidades de cada categoría se cargan directamente en el Constructor al llamar a LoadData()
        // Se podrían hacer públicas si la UI necesitara enlazarse individualmente, pero por ahora se usan internamente.
        // public int BronzeCount { get; set; }
        // public int SilverCount { get; set; }
        // public int GoldCount { get; set; }
        // public int PlatinumCount { get; set; }
        // public int DiamondCount { get; set; }

        #endregion

        #region Constructor e Inicialización

        /// Constructor principal del ViewModel de Dashboard.
        /// <param name="dbContextFactory">Fábrica de contextos de BD (DI).</param>
        public DashboardViewModel(IDbContextFactory<AppDbContext>? dbContextFactory = null)
        {
            _dbContextFactory = dbContextFactory;
            LoadData();
        }

        /// Método auxiliar para crear el contexto de la base de datos.
        private AppDbContext CreateContext()
        {
            return _dbContextFactory != null
                ? _dbContextFactory.CreateDbContext()
                : new AppDbContext();
        }

        /// Carga todos los datos necesarios para el Dashboard: métricas, gráficos e insights.
        private void LoadData()
        {
            using var context = CreateContext();

            // Obtenemos todos los resúmenes mensuales ordenados cronológicamente.
            var summaries = context.MonthlySummaries.OrderBy(m => m.Year).ThenBy(m => m.Month).ToList();

            // Si no hay datos históricos, no podemos cargar nada.
            if (!summaries.Any())
            {
                // Inicializar propiedades con valores vacíos para evitar errores en la UI
                RevenueSeries = Array.Empty<ISeries>();
                XAxes = Array.Empty<Axis>();
                CategorySeries = Array.Empty<ISeries>();
                TotalRevenue = 0;
                TotalTickets = 0;
                AverageTicket = 0;
                InsightMessage = "No hay datos históricos para mostrar el resumen.";
                return;
            }

            // Tomamos el último resumen para las métricas principales
            var lastSummary = summaries.Last();

            TotalRevenue = lastSummary.TotalRevenue;
            TotalTickets = lastSummary.TotalTickets;
            AverageTicket = TotalTickets > 0 ? TotalRevenue / TotalTickets : 0;

            // 1. Configurar Gráfico de Barras (Evolución de Ventas)
            RevenueSeries = new ISeries[] {
                new ColumnSeries<decimal> {
                    Values = summaries.Select(s => s.TotalRevenue).ToArray(),
                    Name = "Ventas $",
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue) // Color de las barras
                }
            };

            XAxes = new Axis[] {
                new Axis { Labels = summaries.Select(s => $"{s.Month:00}/{s.Year}").ToArray() } // Formato MM/YYYY
            };

            // 2. Configurar Gráfico de Torta (Clientes por Rango)
            CategorySeries = new ISeries[] {
                new PieSeries<int> { Values = new[] { lastSummary.BronzeCount }, Name = "Bronce", Fill = new SolidColorPaint(SKColor.Parse("#ac7e31")) },
                new PieSeries<int> { Values = new[] { lastSummary.SilverCount }, Name = "Plata", Fill = new SolidColorPaint(SKColor.Parse("#748cab")) },
                new PieSeries<int> { Values = new[] { lastSummary.GoldCount }, Name = "Oro", Fill = new SolidColorPaint(SKColor.Parse("#ffb800")) },
                new PieSeries<int> { Values = new[] { lastSummary.PlatinumCount }, Name = "Platino", Fill = new SolidColorPaint(SKColor.Parse("#0466c8")) },
                new PieSeries<int> { Values = new[] { lastSummary.DiamondCount }, Name = "Diamante", Fill = new SolidColorPaint(SKColor.Parse("#bf0603")) }
            };

            // 3. Motor de Sugerencias Inteligentes: Analiza los datos cargados
            GenerateInsights(lastSummary, summaries);
        }

        #endregion

        #region Motor de Sugerencias Inteligentes

        /// Analiza los datos históricos y del último mes para generar un mensaje de "insight" útil para el usuario.
        /// <param name="lastSummary">El resumen del último mes.</param>
        /// <param name="allSummaries">La lista completa de resúmenes mensuales.</param>
        private void GenerateInsights(MonthlySummary lastSummary, List<MonthlySummary> allSummaries)
        {
            // --- Análisis comparativo Año contra Año (Estacionalidad) ---
            var sameMonthLastYear = allSummaries.FirstOrDefault(m => m.Month == lastSummary.Month && m.Year == lastSummary.Year - 1);
            if (sameMonthLastYear != null)
            {
                // Evitar división por cero si la facturación del año pasado fue 0.
                if (sameMonthLastYear.TotalRevenue > 0)
                {
                    decimal growth = ((lastSummary.TotalRevenue - sameMonthLastYear.TotalRevenue) / sameMonthLastYear.TotalRevenue) * 100;

                    if (growth <= -15) // Si facturó un 15% menos o más
                    {
                        InsightMessage = $"🚨 Alerta estacional: Facturaste un {Math.Abs(growth):F1}% MENOS que el mismo mes del año pasado.";
                        return; // Prioridad máxima para alertas críticas
                    }
                    else if (growth >= 15) // Si facturó un 15% más o más
                    {
                        InsightMessage = $"🚀 Crecimiento real: Superaste en un {growth:F1}% la facturación del mismo mes del año pasado.";
                        return; // Mensaje positivo pero importante
                    }
                }
            }

            // --- Análisis de Fuga de Clientes (Mes contra Mes) ---
            // Buscamos el mes inmediatamente anterior
            var previousMonthSummary = allSummaries.Count > 1 ? allSummaries[allSummaries.Count - 2] : null;
            if (previousMonthSummary != null)
            {
                // Si la caída de clientes únicos es más del 15%
                if (lastSummary.UniqueCustomers < (previousMonthSummary.UniqueCustomers * 0.85))
                {
                    InsightMessage = "⚠️ Caída de tráfico: Este mes disminuyó considerablemente la cantidad de clientes únicos respecto al mes anterior.";
                    return;
                }
            }

            // --- Análisis de Calidad de Venta (Ticket Promedio) ---
            // El valor 2500 es un ejemplo, se podría configurar en AppConfig
            if (AverageTicket < 2500)
            {
                InsightMessage = "💡 Ticket Promedio bajo: Los clientes que vienen gastan poco. Considera estrategias de up-selling o combos.";
                return;
            }

            // --- Análisis de Fidelización y Categorías ---
            int totalFidelizados = lastSummary.SilverCount + lastSummary.GoldCount + lastSummary.PlatinumCount + lastSummary.DiamondCount;

            // Caso: Todos los clientes se quedan en Bronce
            if (lastSummary.BronzeCount > 0 && totalFidelizados == 0)
            {
                InsightMessage = "📊 Barrera de entrada alta: Todos tus clientes están en Bronce. Considera bajar los puntos necesarios para 'Plata' y que vean el progreso.";
            }
            // Caso: Alta concentración de VIPs
            else if (lastSummary.DiamondCount > (lastSummary.UniqueCustomers * 0.1)) // Si más del 10% de la base es Diamante
            {
                InsightMessage = "💎 Base VIP sólida: Tienes una alta proporción de clientes Diamante. ¡Excelente! Busca fidelizarlos aún más y segmenta ofertas para ellos.";
            }
            // Caso: Flujo saludable
            else
            {
                InsightMessage = "✅ Ecosistema saludable: La facturación, retención y distribución de categorías muestran un buen equilibrio.";
            }
        }

        #endregion
    }
}