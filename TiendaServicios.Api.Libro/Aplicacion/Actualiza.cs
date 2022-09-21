using FluentValidation;
using MediatR;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;

namespace TiendaServicios.Api.Libro.Aplicacion
{
    public class Actualiza
    {
        public class Ejecuta : IRequest {
            public Guid LibreriaMaterialId { get; set; }
            public string Titulo { get; set; }
            public DateTime? FechaPublicacion { get; set; }
            public Guid AutorLibro { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta> { 
            public EjecutaValidacion() {
                RuleFor(x => x.FechaPublicacion).NotEmpty();
                RuleFor(x => x.AutorLibro).NotEmpty();
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.LibreriaMaterialId).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            public ContextoLibreria _contexto;
            public Manejador(ContextoLibreria contexto) {
                _contexto = contexto;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var libro = new LibreriaMaterial
                {
                    LibreriaMaterialId = request.LibreriaMaterialId,
                    Titulo = request.Titulo,
                    FechaPublicacion = request.FechaPublicacion,
                    AutorLibro = request.AutorLibro
                };

                var libroToUpdate = _contexto.LibreriaMaterial.FindAsync(libro.LibreriaMaterialId);

                if (libroToUpdate == null) {
                    throw new Exception("El usuario que trata de modificar no existe en la base de datos!");
                }

                _contexto.LibreriaMaterial.Update(libro);

                var value = await _contexto.SaveChangesAsync();

                if (value > 0) {
                    return Unit.Value;
                }

                throw new Exception("Error al intentar modificar los datos!");
            }
        }
    }
}
