namespace YastCleaner.Web.ViewModels
{
    public class PedidosTemporalViewModel
    {
        public int Id { get; set; }//esto es del servicio
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public double Importe => Cantidad * Precio;
    }
}
