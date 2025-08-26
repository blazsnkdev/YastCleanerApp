namespace YastCleaner.Web.Utils
{
    public class PaginaResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalRegistros { get; set; }
        public int PaginaIndice { get; set; }
        public int TamanioPagina { get; set; }
        public int TotalPagias => (int)Math.Ceiling(TotalRegistros / (double)TamanioPagina);

        public bool TienePaginaAnterior => PaginaIndice > 1;
        public bool TienePaginaSiguiente => PaginaIndice < TotalPagias;

    }
}
