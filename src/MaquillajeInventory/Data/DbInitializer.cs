using MakeupInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace MakeupInventory.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();

            // 1. Semilla de Categorías
            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new Categoria { Nombre = "Labiales", Descripcion = "Labiales mate, cremosos, tintas y brillos hidratantes" },
                    new Categoria { Nombre = "Sombras", Descripcion = "Paletas de sombras nude, coloridas y glitter de alta pigmentación" },
                    new Categoria { Nombre = "Bases y Correctores", Descripcion = "Bases líquidas, correctores y polvos matificantes" },
                    new Categoria { Nombre = "Rubor e Iluminador", Descripcion = "Rubores en crema, polvo compacto e iluminadores satinados" },
                    new Categoria { Nombre = "Delineadores y Máscaras", Descripcion = "Delineadores a prueba de agua y máscaras para pestañas" },
                    new Categoria { Nombre = "Cuidado Facial", Descripcion = "Fijadores de maquillaje, sueros hidratantes y primers" }
                );
                context.SaveChanges();
            }

            // 2. Semilla de Usuarios
            if (!context.Usuarios.Any())
            {
                context.Usuarios.AddRange(
                    new Usuario { Nombre = "Propietaria Maquillaje SV", Email = "admin@maquillajesv.com", PasswordHash = "AQAAAAIAAYagAAAAEAdminHash123==", Rol = "Admin" },
                    new Usuario { Nombre = "Vendedora de Turno", Email = "vendedora@maquillajesv.com", PasswordHash = "AQAAAAIAAYagAAAAEVendedoraHash123==", Rol = "Vendedor" },
                    new Usuario { Nombre = "Clienta Frecuente", Email = "cliente@maquillajesv.com", PasswordHash = "AQAAAAIAAYagAAAAEClienteHash123==", Rol = "Cliente" }
                );
                context.SaveChanges();
            }

            // 3. Semilla de Clientes
            if (!context.Clientes.Any())
            {
                context.Clientes.AddRange(
                    new Cliente { Nombre = "Valeria Martínez", Telefono = "+503 7234-5678", Email = "valeria.martinez@gmail.com" },
                    new Cliente { Nombre = "Sofía Ramos", Telefono = "+503 7890-1234", Email = "sofia.ramos@hotmail.com" },
                    new Cliente { Nombre = "Camila Hernández", Telefono = "+503 7654-3210", Email = "camila.hernandez@outlook.com" }
                );
                context.SaveChanges();
            }

            // 4. Semilla de Productos
            if (!context.Productos.Any())
            {
                var catLabiales = context.Categorias.FirstOrDefault(c => c.Nombre == "Labiales")?.Id ?? 1;
                var catSombras = context.Categorias.FirstOrDefault(c => c.Nombre == "Sombras")?.Id ?? 2;
                var catBases = context.Categorias.FirstOrDefault(c => c.Nombre.StartsWith("Bases"))?.Id ?? 3;
                var catRubor = context.Categorias.FirstOrDefault(c => c.Nombre.StartsWith("Rubor"))?.Id ?? 4;
                var catDelineadores = context.Categorias.FirstOrDefault(c => c.Nombre.StartsWith("Delineador"))?.Id ?? 5;

                context.Productos.AddRange(
                    new Producto
                    {
                        Nombre = "Labial Rojo Pasión Mate",
                        Descripcion = "Labial líquido de larga duración con acabado terciopelo mate indeleble. Ideal para piel mixta o grasa en eventos nocturnos.",
                        Precio = 8.50m,
                        Stock = 25,
                        CategoriaId = catLabiales,
                        Proveedor = "Maybelline New York",
                        ImagenUrl = "https://images.unsplash.com/photo-1586495777744-4413f21062fa?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-15)
                    },
                    new Producto
                    {
                        Nombre = "Labial Nude Hidratante Rosa",
                        Descripcion = "Fórmula enriquecida con manteca de karité y vitamina E. Tono rosa suave ideal para maquillaje diario y piel seca.",
                        Precio = 7.00m,
                        Stock = 30,
                        CategoriaId = catLabiales,
                        Proveedor = "L'Oréal Paris",
                        ImagenUrl = "https://images.unsplash.com/photo-1625093742435-6fa192b6fb10?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-14)
                    },
                    new Producto
                    {
                        Nombre = "Paleta de Sombras Nude Glam",
                        Descripcion = "Paleta con 16 tonos neutros, cálidos y satinados de ultra alta pigmentación para looks tanto de oficina como de noche.",
                        Precio = 15.99m,
                        Stock = 12,
                        CategoriaId = catSombras,
                        Proveedor = "NYX Professional",
                        ImagenUrl = "https://images.unsplash.com/photo-1512496015851-a90fb38ba796?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-12)
                    },
                    new Producto
                    {
                        Nombre = "Base Líquida Control Grasa Tono Medio",
                        Descripcion = "Base unificadora con niacinamida y acabado semimate. Controla el sebo y minimiza poros hasta por 16 horas.",
                        Precio = 12.75m,
                        Stock = 18,
                        CategoriaId = catBases,
                        Proveedor = "L'Oréal Paris",
                        ImagenUrl = "https://images.unsplash.com/photo-1631729371254-42c2892f0e6e?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-10)
                    },
                    new Producto
                    {
                        Nombre = "Rubor Compacto Tono Coral Glow",
                        Descripcion = "Rubor sedoso micropulverizado que brinda un resplandor saludable y natural en pómulos con sutil reflejo dorado.",
                        Precio = 9.25m,
                        Stock = 4,
                        CategoriaId = catRubor,
                        Proveedor = "NYX Professional",
                        ImagenUrl = "https://images.unsplash.com/photo-1522337360788-8b13dee7a37e?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-8)
                    },
                    new Producto
                    {
                        Nombre = "Delineador Plumón Waterproof Negro Intenso",
                        Descripcion = "Punta fina de precisión milimétrica resistente al agua, sudor y transferencias durante todo el día.",
                        Precio = 5.50m,
                        Stock = 40,
                        CategoriaId = catDelineadores,
                        Proveedor = "Maybelline New York",
                        ImagenUrl = "https://images.unsplash.com/photo-1631214524020-7e18db9a8f92?auto=format&fit=crop&w=600&q=80",
                        FechaIngreso = DateTime.Now.AddDays(-5)
                    }
                );
                context.SaveChanges();
            }

            // 5. Semilla de Ventas y Cuotas iniciales
            if (!context.Ventas.Any())
            {
                var primerProducto = context.Productos.FirstOrDefault();
                var clienta = context.Clientes.FirstOrDefault();

                if (primerProducto != null)
                {
                    var ventaDemo = new Venta
                    {
                        Fecha = DateTime.Now.AddDays(-3),
                        MetodoPago = "Pago en Cuotas",
                        Total = 30.00m,
                        ClienteId = clienta?.Id,
                        ClienteNombre = clienta?.Nombre ?? "Valeria Martínez",
                        ClienteEmail = clienta?.Email ?? "valeria.martinez@gmail.com",
                        ClienteTelefono = "+503 7234-5678",
                        DireccionEntrega = "Colonia Escalón, San Salvador",
                        Estado = "En Cuotas"
                    };
                    context.Ventas.Add(ventaDemo);
                    context.SaveChanges();

                    context.DetalleVentas.Add(new DetalleVenta
                    {
                        VentaId = ventaDemo.Id,
                        ProductoId = primerProducto.Id,
                        Cantidad = 2,
                        PrecioUnitario = primerProducto.Precio,
                        Subtotal = primerProducto.Precio * 2
                    });

                    context.MovimientosInventario.Add(new MovimientoInventario
                    {
                        ProductoId = primerProducto.Id,
                        Cantidad = -2,
                        Fecha = DateTime.Now.AddDays(-3),
                        Motivo = $"Venta en Cuotas #{ventaDemo.Id}"
                    });
                    context.SaveChanges();

                    // Cuotas de ejemplo para Mockup 5.1.4.f
                    context.Cuotas.AddRange(
                        new Cuota
                        {
                            VentaId = ventaDemo.Id,
                            ClienteId = clienta?.Id,
                            Monto = 15.00m,
                            FechaVencimiento = DateTime.Now.AddDays(-5),
                            Estado = "Pagada",
                            FechaPago = DateTime.Now.AddDays(-4)
                        },
                        new Cuota
                        {
                            VentaId = ventaDemo.Id,
                            ClienteId = clienta?.Id,
                            Monto = 15.00m,
                            FechaVencimiento = DateTime.Now.AddDays(3),
                            Estado = "Pendiente"
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}
