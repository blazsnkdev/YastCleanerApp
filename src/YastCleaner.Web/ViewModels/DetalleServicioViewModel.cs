namespace YastCleaner.Web.ViewModels
{
    public class DetalleServicioViewModel
    {
        public int ServicioId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double Precio { get; set; }
        public string Estado { get; set; }
        public int CantidadDetalles { get; set; }
        public double MontoGenerado { get; set; }
    }
}

