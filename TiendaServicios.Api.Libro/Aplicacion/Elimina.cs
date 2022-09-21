using MediatR;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Elimina
    {
        public class Ejecuta : IRequest { 
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly ContextoLibreria _contexto;
            public Manejador(ContextoLibreria contexto) {
                _contexto = contexto;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libroToDelete = await _contexto.LibreriaMaterial.FindAsync(request.Id);

                if (libroToDelete == null) {
                    throw new Exception("El libro no existe!");
                }

                 _contexto.LibreriaMaterial.Remove(libroToDelete);

                var deleted = _contexto.SaveChanges();

                if (deleted == null)
                {
                    throw new Exception("Error al eliminar el libro");
                }

                return Unit.Value;
            }
        }
    }
}
