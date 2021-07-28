using System;
using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Enums;
using Sgm.OC.Domain.Views;
using Sgm.OC.Framework;
using Sgm.OC.Repositories;
using Sgm.OC.Repository.Extensions;
using Sgm.OC.Security.Entities;

namespace Sgm.OC.Core.Bussiness
{
    public class QueryBussiness
    {

        public UserLogged UserLogged { get; set; }


        public List<Estado> GetByTipoEntidad(TipoEntidad tipoEntidad)
        {

            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                List<Estado> estado = unitOfWork.EstadoRepo.GetEstadosByTipoEntidad(tipoEntidad.GetHashCode());

                return estado;

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public List<Rubro> GetRubroByFiltro(RubroFilters filtro)
        {
            int minPageSize = 10;
            int maxPageSize = 50;
            int minPageNumber = 1;

            int pagesize = minPageSize;
            int pageNumber = minPageNumber;
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                var predicate = PredicateBuilder.New<Rubro>(true);


                if (int.TryParse(filtro.PageSize, out int salida))
                {
                    pagesize = int.Parse(filtro.PageSize);
                }

                if (int.TryParse(filtro.Page, out int salida2))
                {
                    pageNumber = int.Parse(filtro.Page);
                }

                if (!string.IsNullOrEmpty(filtro.Descripcion))
                {
                    predicate = predicate.And(i => i.Descripcion.Contains(filtro.Descripcion));
                }

                if (!string.IsNullOrEmpty(filtro.Id))
                {
                    predicate = predicate.And(i => i.IdInterno == int.Parse(filtro.Id));
                }

                List<Rubro> rubros = unitOfWork.RubroRepo.GetRubrosByFilter(predicate)
                             .Skip((pageNumber - 1) * pagesize)
                             .Take(pagesize)
                             .ToList();

                return rubros;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Sucursal> GetBySucursalByFilter(SucursalFilters filtro)
        {
            int minPageSize = 10;
            int maxPageSize = 50;
            int minPageNumber = 1;

            int pagesize = minPageSize;
            int pageNumber = minPageNumber;
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                var predicate = PredicateBuilder.New<Sucursal>(true);


                if (int.TryParse(filtro.PageSize, out int salida))
                {
                    pagesize = int.Parse(filtro.PageSize);
                }

                if (int.TryParse(filtro.Page, out int salida2))
                {
                    pageNumber = int.Parse(filtro.Page);
                }

                if (!string.IsNullOrEmpty(filtro.Descripcion))
                {
                    predicate = predicate.And(i => i.Descripcion.Contains(filtro.Descripcion));
                }

                if (!string.IsNullOrEmpty(filtro.Id))
                {
                    predicate = predicate.And(i => i.Id == int.Parse(filtro.Id));
                }

                List<Sucursal> sucursales = unitOfWork.SucursalRepo.GetSucursalesByFilter(predicate)
                             .Skip((pageNumber - 1) * pagesize)
                             .Take(pagesize)
                             .ToList();

                return sucursales;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ViewResult<ProveedorView> GetProveedorByFilter(ProveedorFilters filtro)
        {
            int minPageSize = 10;
            int maxPageSize = 50;

            int minPageNumber = 1;

            int pagesize = minPageSize;
            int pageNumber = minPageNumber;

            int id;

            try
            {
                UnitOfWork unitOfWork = UnitOfWork.Create();

                if (int.TryParse(filtro.PageSize, out pagesize))
                {
                    pagesize = int.Parse(filtro.PageSize);
                }
                if (pagesize < minPageSize || pagesize > maxPageSize)
                    pagesize = minPageSize;

                filtro.PageSize = pagesize.ToString();

                if (int.TryParse(filtro.Page, out pageNumber))
                {
                    pageNumber = int.Parse(filtro.Page);
                }

                if (pageNumber < minPageNumber)
                    pageNumber = minPageNumber;

                filtro.Page = pageNumber.ToString();

                List<ProveedorView> proveedores = unitOfWork.ProveedorViewRepo.GetProveedoresByFilter(filtro);
                if (proveedores.Any())
                {
                    return new ViewResult<ProveedorView>(proveedores, proveedores.First().TotalPages, proveedores.First().TotalRows);

                }

                return new ViewResult<ProveedorView>(proveedores, 0, 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Usuario GetUsuario()
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                Usuario usuario = unitOfWork.UsuarioRepo.LoadUsuario(UserLogged.Login, UserLogged.SucursalId);

                return usuario;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
