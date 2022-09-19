namespace TiendaServicios.Api.CarritoCompra.Aplicacion
{
    public class CarritoDto
    {
        public int CarritoId { get; set; }
        public DateTime? FechaCreacionCarrito { get; set; }
        public List<CarritoDetalleDto> ListaProductos { get; set; }
    }
}
