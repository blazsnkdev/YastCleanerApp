using YastCleaner.Web.Utils;

namespace YastCleaner.Web.Helpers
{
    public static class PaginacionHelper
    {
        public static PaginaResult<T> Paginacion<T>(IEnumerable<T> lista, int indicePagina, int tamanioPagina)
        {
            var total = lista.Count();//obtenemos el total
            var items = lista.Skip((indicePagina - 1) * tamanioPagina)//realizar los items por pagina
                .Take(tamanioPagina)
                .ToList();

            var resultadoPaginacion = new PaginaResult<T>() { //crear el objeto list paginado
                Items = items,
                TotalRegistros = total,
                PaginaIndice = indicePagina,
                TamanioPagina = tamanioPagina
            };
            return resultadoPaginacion;
        }
    }
}
