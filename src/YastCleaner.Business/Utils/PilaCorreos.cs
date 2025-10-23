namespace YastCleaner.Business.Utils
{
    public class PilaCorreos
    {
        private readonly Stack<Correo> _pila = new Stack<Correo>();
        public void Agregar(Correo correo) => _pila.Push(correo);
        public Correo? Obtener() => _pila.Count > 0 ? _pila.Pop() : null;
        public bool Pendientes() => _pila.Count > 0;
    }
}
