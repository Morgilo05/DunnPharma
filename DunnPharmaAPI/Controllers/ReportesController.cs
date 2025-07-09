using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DunnPharmaAPI.Data;
using DunnPharmaAPI.DTOs;
using QuestPDF.Fluent;

namespace DunnPharmaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly DunnPharmaDbContext _context;

        public ReportesController(DunnPharmaDbContext context)
        {
            _context = context;
        }

        // GET: api/reportes/ventas?desde=2025-05-01&hasta=2025-05-31&idCliente=3
        [HttpGet("ventas")]
        public async Task<ActionResult<IEnumerable<VentaReporteDto>>> ObtenerVentas(
            DateTime desde, DateTime hasta, int? idCliente = null)
        {
            var query = _context.Cardex
                .Include(c => c.Producto)
                .Include(c => c.Cliente)
                .Where(c => c.Movimiento == 'S' &&
                            c.FechaMovimiento >= desde &&
                            c.FechaMovimiento <= hasta);

            if (idCliente.HasValue)
                query = query.Where(c => c.IdCliente == idCliente.Value);

            var resultado = await query
                .OrderByDescending(c => c.FechaMovimiento)
                .Select(c => new VentaReporteDto
                {
                    Fecha = c.FechaMovimiento,
                    Cliente = c.Cliente.Nombre,
                    Producto = c.Producto.Nombre,
                    Piezas = c.Piezas,
                    PrecioUnitario = c.PrecioVenta ?? 0,
                    CostoUnitario = c.Costo ?? 0
                })
                .ToListAsync();

            return Ok(resultado);
        }

        // GET: api/reportes/ventas/pdf?desde=2025-05-01&hasta=2025-05-31&idCliente=3
        [HttpGet("ventas/pdf")]
        public async Task<IActionResult> ExportarVentasPdf(DateTime desde, DateTime hasta, int? idCliente = null)
        {
            var query = _context.Cardex
                .Include(c => c.Producto)
                .Include(c => c.Cliente)
                .Where(c => c.Movimiento == 'S' &&
                            c.FechaMovimiento >= desde &&
                            c.FechaMovimiento <= hasta);

            if (idCliente.HasValue)
                query = query.Where(c => c.IdCliente == idCliente.Value);

            var ventas = await query
                .OrderBy(c => c.FechaMovimiento)
                .ToListAsync();

            using var stream = new MemoryStream();

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Reporte de Ventas").FontSize(18).Bold();
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Desde: {desde:dd/MM/yyyy}   Hasta: {hasta:dd/MM/yyyy}");
                        if (idCliente.HasValue)
                        {
                            var nombre = ventas.FirstOrDefault()?.Cliente?.Nombre;
                            if (!string.IsNullOrEmpty(nombre))
                                col.Item().Text($"Cliente: {nombre}");
                        }

                        col.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(90); // Fecha
                                cols.RelativeColumn();   // Cliente
                                cols.RelativeColumn();   // Producto
                                cols.ConstantColumn(40); // Piezas
                                cols.ConstantColumn(60); // Precio
                                cols.ConstantColumn(60); // Subtotal
                                cols.ConstantColumn(60); // Utilidad
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Text("Fecha").Bold();
                                header.Cell().Text("Cliente").Bold();
                                header.Cell().Text("Producto").Bold();
                                header.Cell().Text("Pzs").Bold();
                                header.Cell().Text("Precio").Bold();
                                header.Cell().Text("Subtotal").Bold();
                                header.Cell().Text("Utilidad").Bold();
                            });

                            foreach (var v in ventas)
                            {
                                decimal precio = v.PrecioVenta ?? 0;
                                decimal costo = v.Costo ?? 0;
                                decimal subtotal = precio * v.Piezas;
                                decimal utilidad = (precio - costo) * v.Piezas;

                                tabla.Cell().Text(v.FechaMovimiento.ToShortDateString());
                                tabla.Cell().Text(v.Cliente?.Nombre ?? "-");
                                tabla.Cell().Text(v.Producto?.Nombre ?? "-");
                                tabla.Cell().Text(v.Piezas.ToString());
                                tabla.Cell().Text($"{precio:C}");
                                tabla.Cell().Text($"{subtotal:C}");
                                tabla.Cell().Text($"{utilidad:C}");
                            }
                        });
                    });
                    page.Footer().AlignCenter().Text("DüNN Pharma - Reporte generado automáticamente");
                });
            });

            document.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"ReporteVentas_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.pdf");
        }

        // GET: api/reportes/ventas/excel?desde=2025-05-01&hasta=2025-05-31&idCliente=3
        [HttpGet("ventas/excel")]
        public async Task<IActionResult> ExportarVentasExcel(DateTime desde, DateTime hasta, int? idCliente = null)
        {
            var query = _context.Cardex
                .Include(c => c.Producto)
                .Include(c => c.Cliente)
                .Where(c => c.Movimiento == 'S' &&
                            c.FechaMovimiento >= desde &&
                            c.FechaMovimiento <= hasta);

            if (idCliente.HasValue)
                query = query.Where(c => c.IdCliente == idCliente.Value);

            var ventas = await query.OrderBy(c => c.FechaMovimiento).ToListAsync();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.AddWorksheet("Ventas");

            // Encabezado
            ws.Cell(1, 1).Value = "Fecha";
            ws.Cell(1, 2).Value = "Cliente";
            ws.Cell(1, 3).Value = "Producto";
            ws.Cell(1, 4).Value = "Piezas";
            ws.Cell(1, 5).Value = "Precio";
            ws.Cell(1, 6).Value = "Subtotal";
            ws.Cell(1, 7).Value = "Utilidad";

            int row = 2;
            foreach (var v in ventas)
            {
                decimal precio = v.PrecioVenta ?? 0;
                decimal costo = v.Costo ?? 0;
                decimal subtotal = precio * v.Piezas;
                decimal utilidad = (precio - costo) * v.Piezas;

                ws.Cell(row, 1).Value = v.FechaMovimiento.ToShortDateString();
                ws.Cell(row, 2).Value = v.Cliente?.Nombre;
                ws.Cell(row, 3).Value = v.Producto?.Nombre;
                ws.Cell(row, 4).Value = v.Piezas;
                ws.Cell(row, 5).Value = precio;
                ws.Cell(row, 6).Value = subtotal;
                ws.Cell(row, 7).Value = utilidad;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteVentas_{desde:yyyyMMdd}_{hasta:yyyyMMdd}.xlsx");
        }

        // GET: api/reportes/inventario
        [HttpGet("inventario")]
        public async Task<ActionResult<IEnumerable<InventarioReporteDto>>> ObtenerInventarioActual()
        {
            var lotes = await _context.Lotes
                .Include(l => l.Producto)
                .Where(l => l.Piezas > 0)
                .OrderBy(l => l.Producto.Nombre)
                .ToListAsync();

            var inventario = lotes.Select(l => new InventarioReporteDto
            {
                Producto = l.Producto.Nombre,
                Lote = l.CodigoLote,
                Piezas = l.Piezas,
                Costo = l.Costo,
                FechaCaducidad = l.FechaCaducidad,
                Factura = l.Factura
            }).ToList();

            return Ok(inventario);
        }

        // GET: api/reportes/inventario/pdf
        [HttpGet("inventario/pdf")]
        public async Task<IActionResult> ExportarInventarioPdf()
        {
            var lotes = await _context.Lotes
                .Include(l => l.Producto)
                .Where(l => l.Piezas > 0)
                .OrderBy(l => l.Producto.Nombre)
                .ToListAsync();

            var stream = new MemoryStream();

            var doc = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Inventario Actual").Bold().FontSize(18);
                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(60);
                            cols.ConstantColumn(100);
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Text("Producto").Bold();
                            header.Cell().Text("Pzs").Bold();
                            header.Cell().Text("Costo").Bold();
                            header.Cell().Text("Caducidad").Bold();
                            header.Cell().Text("Factura").Bold();
                        });

                        foreach (var l in lotes)
                        {
                            tabla.Cell().Text($"{l.Producto.Nombre} - {l.CodigoLote}");
                            tabla.Cell().Text(l.Piezas.ToString());
                            tabla.Cell().Text($"{l.Costo:C}");
                            tabla.Cell().Text(l.FechaCaducidad.ToShortDateString());
                            tabla.Cell().Text(l.Factura ?? "-");
                        }
                    });
                    page.Footer().AlignCenter().Text("DüNN Pharma - Inventario generado automáticamente");
                });
            });

            doc.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", "InventarioActual.pdf");
        }

        // GET: api/reportes/inventario/excel
        [HttpGet("inventario/excel")]
        public async Task<IActionResult> ExportarInventarioExcel()
        {
            var lotes = await _context.Lotes
                .Include(l => l.Producto)
                .Where(l => l.Piezas > 0)
                .OrderBy(l => l.Producto.Nombre)
                .ToListAsync();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.AddWorksheet("Inventario");

            ws.Cell(1, 1).Value = "Producto";
            ws.Cell(1, 2).Value = "Lote";
            ws.Cell(1, 3).Value = "Piezas";
            ws.Cell(1, 4).Value = "Costo";
            ws.Cell(1, 5).Value = "Caducidad";
            ws.Cell(1, 6).Value = "Factura";

            int row = 2;
            foreach (var l in lotes)
            {
                ws.Cell(row, 1).Value = l.Producto.Nombre;
                ws.Cell(row, 2).Value = l.CodigoLote;
                ws.Cell(row, 3).Value = l.Piezas;
                ws.Cell(row, 4).Value = l.Costo;
                ws.Cell(row, 5).Value = l.FechaCaducidad.ToShortDateString();
                ws.Cell(row, 6).Value = l.Factura ?? "-";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InventarioActual.xlsx");
        }


    }
}
