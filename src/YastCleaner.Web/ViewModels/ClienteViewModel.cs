namespace YastCleaner.Web.ViewModels
{
    public class ClienteViewModel
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NumeroCelular { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public List<PedidoViewModel> Pedidos { get; set; } = new();
        public ClienteViewModel()
        {
            
        }
        public ClienteViewModel(int clienteId, string nombre)
        {
            ClienteId = clienteId;
            Nombre = nombre;
        }
    }
}
