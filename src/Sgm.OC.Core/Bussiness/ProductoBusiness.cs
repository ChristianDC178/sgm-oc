using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;
using Sgm.OC.Repository.Extensions;

namespace Sgm.OC.Core.Bussiness
{
    public class ProductoBusiness
    {

        public ProductoBusiness() { }


        public Producto GetById(int id)
        {
            return new Producto();
        }

        public Producto GetProducto(int id)
        {

            try
            {
                Producto producto = new Producto();
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());

                Usuario usuario = new Usuario();

                producto = unitOfWork.ProductoRepo.DbSet
                    .Include(um => um.UnidadMedida)
                    .Where(p => p.Id == id).First();
                return producto;
            }
            catch (Exception ex)
            {

                Sgm.OC.Framework.SgmApplication.Logger.Error($"Ocurrio un error al obtener el producto { id}", ex);
                throw ex;
            }
        }

        public List<Producto> GetProductoByFilter(ProductoFilters filtro)
        {
            int minPageSize = 10;
            int maxPageSize = 50;

            int minPageNumber = 1;

            int pagesize = minPageSize;
            int pageNumber = minPageNumber;


            try
            {
                UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
                var predicate = LinqKit.PredicateBuilder.New<Producto>(true);

                if (int.TryParse(filtro.PageSize, out pagesize))
                {
                    pagesize = int.Parse(filtro.PageSize);
                }

                if (int.TryParse(filtro.Page, out pageNumber))
                {
                    pageNumber = int.Parse(filtro.Page);
                }

                if (int.TryParse(filtro.IdInterno, out int id))
                {
                    predicate = predicate.And(i => i.IdInterno == id);
                }

                if (!string.IsNullOrEmpty(filtro.Rubro))
                {
                    if (IsDouble(filtro.Rubro))
                    {

                        predicate = predicate.And(i => i.Rubro.Id == filtro.Rubro);
                    }
                    else
                    {
                        predicate = predicate.And(i => i.Rubro.Descripcion.Contains(filtro.Rubro));
                    }
                }

                if (!string.IsNullOrEmpty(filtro.Recurrente))
                {
                    predicate = predicate.And(i => i.Recurrente  == Boolean.Parse(filtro.Recurrente));
                }

                if (!string.IsNullOrEmpty(filtro.Descripcion))
                {
                    predicate = predicate.And(i => i.Descripcion.Contains(filtro.Descripcion));
                }

                if (filtro.Creacion.HasValue)
                {
                    predicate = predicate.And(i => i.Creacion.Date.CompareTo(filtro.Creacion.Value) == 1);
                }

                if (!(pagesize >= minPageSize && pagesize <= maxPageSize))
                {
                    pagesize = minPageSize;
                }

                if (!(pageNumber >= minPageNumber))
                {
                    pageNumber = minPageNumber;
                }

                List<Producto> productos = new List<Producto>();

                productos = unitOfWork.ProductoRepo.GetByFilter(predicate)
                             .Skip((pageNumber - 1) * pagesize)
                             .Take(pagesize)
                             .ToList();


                return productos;
            }
            catch (Exception ex)
            {
                Sgm.OC.Framework.SgmApplication.Logger.Error("Error al buscar los productos por filtro", ex);
                throw ex;
            }
        }
        public bool IsDouble(string str)
        {
            if (double.TryParse(str, out double salida))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
