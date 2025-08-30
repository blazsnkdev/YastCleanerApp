namespace YastCleaner.Web.ViewModels
{
    public class ClienteViewModel
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }

        public ClienteViewModel(int clienteId, string nombre)
        {
            ClienteId = clienteId;
            Nombre = nombre;
        }
    }
}
