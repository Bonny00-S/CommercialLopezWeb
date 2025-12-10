using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

// ASEGÚRATE DE QUE ESTE NAMESPACE COINCIDA CON TU PROYECTO
namespace ProyectoWebCommercialLopez.Services
{
    public class PdfService
    {
        // Define un color primario (Azul oscuro)
        private static readonly string PrimaryColor = Colors.Blue.Darken2;

        public byte[] CreateReport(string title, string generatedBy, object tableData)
        {
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);

                    // --- Encabezado ---
                    page.Header().Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
                        {
                            // Logo
                            // Asegúrate de que esta ruta exista o comenta esta línea si no tienes la imagen aún
                            row.ConstantItem(70).Image("wwwroot/images/logoComercial.jpg");

                            // Texto del encabezado
                            row.RelativeItem().PaddingLeft(15).Column(col =>
                            {
                                col.Item().Text("Commercial López").FontSize(24).SemiBold().FontColor(PrimaryColor);
                                col.Item().Text(title).FontSize(16).SemiBold();
                                col.Item().PaddingTop(5).Text(text =>
                                {
                                    text.Span("Generado: ").SemiBold();
                                    text.Span(now);
                                });
                                col.Item().Text(text =>
                                {
                                    text.Span("Por: ").SemiBold();
                                    text.Span(generatedBy);
                                });
                            });
                        });

                        // Línea separadora (El grosor es el argumento: 2)
                        headerCol.Item().PaddingTop(10).LineHorizontal(2).LineColor(PrimaryColor);
                    });

                    // --- Contenido (Tabla) ---
                    page.Content().PaddingVertical(20).Element(BuildTable(tableData));

                    // --- Pie de Página ---
                    page.Footer().Column(footerCol =>
                    {
                        footerCol.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        footerCol.Item().PaddingTop(5).AlignCenter().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                            x.EmptyLine();
                            x.Span("© Commercial López - Reporte Automático").FontSize(8);
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        private Action<IContainer> BuildTable(object data)
        {
            // Validación de datos
            if (data == null || !(data is IEnumerable<object> enumerableData))
            {
                return container => container.PaddingVertical(10)
                    .AlignCenter()
                    .Text("No hay datos para mostrar en la tabla.")
                    .Italic();
            }

            // Obtenemos las propiedades usando Reflection
            // Nota: data.GetType().GetGenericArguments() puede fallar si data no es genérico directo.
            // Para mayor seguridad usamos el primer elemento de la lista si existe.
            var firstItem = System.Linq.Enumerable.FirstOrDefault(enumerableData);
            if (firstItem == null)
            {
                return container => container.PaddingVertical(10).Text("La lista está vacía.");
            }

            var props = firstItem.GetType().GetProperties();

            return container =>
            {
                container.Table(table =>
                {
                    // 1. Definición de Columnas
                    table.ColumnsDefinition(columns =>
                    {
                        // Columna para el índice #
                        columns.ConstantColumn(30);

                        foreach (var _ in props)
                        {
                            columns.RelativeColumn();
                        }
                    });

                    // 2. Encabezado de la Tabla
                    table.Header(header =>
                    {
                        // Estilo base para celdas del header
                        IContainer HeaderStyle(IContainer container)
                        {
                            return container
                                .BorderBottom(2).BorderColor(PrimaryColor)
                                .Background(PrimaryColor)
                                .Padding(5)
                                .AlignCenter();
                        }

                        // Celda Índice (#)
                        header.Cell().Element(HeaderStyle).Text("#").SemiBold().FontColor(Colors.White);

                        // Celdas de Propiedades
                        foreach (var p in props)
                        {
                            // .FontColor se aplica al Text, no a la celda
                            header.Cell().Element(HeaderStyle).Text(p.Name).SemiBold().FontColor(Colors.White);
                        }

                        // Nota: HeaderOnEveryPage() no es necesario, es el comportamiento por defecto.
                    });

                    // 3. Filas
                    var rowIndex = 1;
                    foreach (var item in enumerableData)
                    {
                        var backgroundColor = rowIndex % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                        // Celda Índice
                        table.Cell().Background(backgroundColor).Padding(5).AlignCenter().Text(rowIndex.ToString());

                        // Celdas de Propiedades
                        foreach (var prop in props)
                        {
                            var value = prop.GetValue(item)?.ToString() ?? string.Empty;

                            table.Cell().Background(backgroundColor).Padding(5)
                                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                                .Text(value);
                        }
                        rowIndex++;
                    }
                });
            };
        }
    }
}