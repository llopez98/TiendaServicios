﻿using AutoMapper;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaServicios.Api.Libro.Aplicacion;
using TiendaServicios.Api.Libro.Modelo;
using TiendaServicios.Api.Libro.Persistencia;
using Xunit;

namespace TiendaServicios.Api.Libro.Tests
{
    public class LibrosServiceTest
    {
        private IEnumerable<LibreriaMaterial> ObtenerDataPrueba() {
            A.Configure<LibreriaMaterial>()
                .Fill(x => x.Titulo).AsArticleTitle()
                .Fill(x => x.LibreriaMaterialId, () => { return new Guid(); });

            var lista = A.ListOf<LibreriaMaterial>(30);

            lista[0].LibreriaMaterialId = Guid.Empty;

            return lista;
        }

        private Mock<ContextoLibreria> CrearContexto() {
            var dataPrueba = ObtenerDataPrueba().AsQueryable();

            var dbSet = new Mock<DbSet<LibreriaMaterial>>();
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).Returns(dataPrueba.Provider);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Expression).Returns(dataPrueba.Expression);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.ElementType).Returns(dataPrueba.ElementType);
            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.GetEnumerator()).Returns(dataPrueba.GetEnumerator());

            dbSet.As<IAsyncEnumerable<LibreriaMaterial>>().Setup(x => x.GetAsyncEnumerator(new System.Threading.CancellationToken()))
                .Returns(new AsyncEnumerator<LibreriaMaterial>(dataPrueba.GetEnumerator()));

            dbSet.As<IQueryable<LibreriaMaterial>>().Setup(x => x.Provider).Returns(new AsyncQueryProvider<LibreriaMaterial>(dataPrueba.Provider));

            var contexto = new Mock<ContextoLibreria>();
            contexto.Setup(x => x.LibreriaMaterial).Returns(dbSet.Object);

            return contexto;
        }
        [Fact]
        public async void GetLibroxId() {
            var mockContexto = CrearContexto();

            var mapConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingTest());
            });

            var mapper = mapConfig.CreateMapper();

            var request = new ConsultaFiltro.LibroUnico();

            request.LibroId = Guid.Empty;

            var manejador = new ConsultaFiltro.Manejador(mockContexto.Object, mapper);

            var libro = await manejador.Handle(request,new System.Threading.CancellationToken());

            Assert.NotNull(libro);
            Assert.True(libro.LibreriaMaterialId == Guid.Empty);
        }

        [Fact]
        public async void GetLibros() {
            //System.Diagnostics.Debugger.Launch();
            // 1. Emular instacion de ef - Contexto
            var mockContexto = CrearContexto();

            // 2. Emular IMapper
            var mapConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingTest());
                });

            var mapper = mapConfig.CreateMapper();

            // 3. Instanciar clase Manejador con los Mocks
            Consulta.Manejador manejador = new Consulta.Manejador(mockContexto.Object, mapper);

            Consulta.Ejecuta request = new Consulta.Ejecuta();

            var lista = await manejador.Handle(request, new System.Threading.CancellationToken());

            Assert.True(lista.Any());
        }

        [Fact]
        public async void GuardarLibro() {

            //objeto que representa la configuracion de la base de datos en memoria
            var options = new DbContextOptionsBuilder<ContextoLibreria>().UseInMemoryDatabase(databaseName: "BaseDatosLibro")
                .Options;

            var contexto = new ContextoLibreria(options);

            var request = new Nuevo.Ejecuta();
            request.Titulo = "Libro de Microservices";
            request.AutorLibro = Guid.Empty;
            request.FechaPublicacion = DateTime.Now;

            var manejador = new Nuevo.Manejador(contexto);

            var libro = await manejador.Handle(request, new System.Threading.CancellationToken());

            Assert.True(libro != null);
        }
    }
}
