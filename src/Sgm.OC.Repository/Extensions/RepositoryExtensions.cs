using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.Filters;
using Sgm.OC.Domain.Views;
using System.IO;
using System;
using Microsoft.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Sgm.OC.Repository.Extensions
{
    public static class RepositoryExtensions
    {

        public static Requisicion GetRequisicionById(this GenericRepository<Requisicion> repository, int id)
        {
            return repository.DbSet
                     .Include(req => req.Items)
                      .ThenInclude(item => item.Producto)
                      .ThenInclude(p => p.UnidadMedida)
                        .FirstOrDefault(r => r.Id == id);
        }

        public static List<Producto> GetByFilter(this GenericRepository<Producto> repository, ExpressionStarter<Producto> predicate)
        {
            return repository.DbSet.Where(predicate)
                .Include(um => um.UnidadMedida)
                .ToList();
        }

        public static Pedido GetPedidoById(this GenericRepository<Pedido> repository, int id)
        {

            return repository.DbSet
                .Include(req => req.EstadoActual)
                .Include(req => req.Items)
                    .ThenInclude(item => item.Producto)
                .Include(req => req.Sucursal)
                .Include(req => req.Usuario)
                .FirstOrDefault(req => req.Id == id);

        }

        public static Usuario LoadUsuario(this GenericRepository<Usuario> repository, string loginName, int sucursalId)
        {
            return repository.DbSet
                .Include(usr => usr.Sucursal)
                .Include(usr => usr.Roles)
                 .ThenInclude(r => r.Rol)
                .FirstOrDefault(usr => usr.Login == loginName && usr.SucursalId == sucursalId);
        }

        public static List<Requisicion> GetRequisicionByFilter(this GenericRepository<Requisicion> repository, ExpressionStarter<Requisicion> predicate)
        {
            return repository.DbSet.Where(predicate)
                 .Include(r => r.EstadoActual)
                 .Include(r => r.Usuario)
                 .Include(r => r.Sucursal)
                .ToList();
        }

        public static List<ProveedorView> GetProveedoresByFilter(this QueryRepository<ProveedorView> repository, ProveedorFilters filter)
        {
            return repository.DbSet
                                  .FromSqlRaw("EXECUTE sp_select_Proveedor {0}, {1}, {2}, {3}",
                                         filter.PageSize,
                                         filter.Page,
                                         filter.IdInterno,
                                         filter.Descripcion)
                                  .ToList();
        }

        public static List<Pedido> GetPedidosByFilter(this GenericRepository<Pedido> repository, ExpressionStarter<Pedido> predicate)
        {
            return repository.DbSet.Where(predicate)
                 .Include(r => r.EstadoActual)
                 .Include(r => r.Usuario)
                 .Include(r => r.UsuarioModificacion)
                 .Include(r => r.Sucursal)
                .ToList();
        }

        public static List<Estado> GetEstadosByTipoEntidad(this GenericRepository<Estado> repository, int TipoEntidad)
        {
            return repository.DbSet.Where(it => it.TipoEntidadId == TipoEntidad)
                .ToList();
        }

        public static List<Sucursal> GetSucursales(this GenericRepository<Sucursal> repository)
        {
            return repository.DbSet
                .ToList();
        }

        public static List<Rubro> GetRubrosByFilter(this GenericRepository<Rubro> repository, ExpressionStarter<Rubro> predicate)
        {
            return repository.DbSet.Where(predicate)
                .OrderBy(i => i.Descripcion)
                .ToList();
        }

        public static List<Sucursal> GetSucursalesByFilter(this GenericRepository<Sucursal> repository, ExpressionStarter<Sucursal> predicate)
        {
            return repository.DbSet.Where(predicate)
                .OrderBy(i => i.Descripcion)
                .ToList();
        }


        public static List<PedidoItemView> GetPedidoItemsByFilter(this QueryRepository<PedidoItemView> repository, PedidoItemViewFilters filter)
        {
            var test = $"EXECUTE sp_select_vw_PedidoItems {filter.PageSize}, {filter.Page}, {filter.ProductoIdInterno}, {filter.ProductoDescripcion}, {filter.RubroIdInterno}, {filter.Recurrente}, {filter.RequisicionId}, {filter.EstadoRequisicion}, {filter.RequisicionAsignada}, {filter.FechaDesdeRequisicion}, {filter.FechaHastaRequisicion}";
            Console.Write(test);
            return repository.DbSet
                        .FromSqlInterpolated($"EXECUTE sp_select_vw_PedidoItems {filter.PageSize}, {filter.Page}, {filter.ProductoIdInterno}, {filter.ProductoDescripcion}, {filter.RubroIdInterno}, {filter.Recurrente}, {filter.RequisicionId}, {filter.EstadoRequisicion}, {filter.RequisicionAsignada}, {filter.FechaDesdeRequisicion}, {filter.FechaHastaRequisicion}")
                        .ToList();
        }
        public static List<RequisicionView> GetRequisicionByFilter(this QueryRepository<RequisicionView> repository, RequisicionFilters filter)
        {
            return repository.DbSet
                              .FromSqlRaw("EXECUTE sp_select_vw_Requisicion {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                                     filter.PageSize,
                                     filter.Page,
                                     filter.Id,
                                     filter.Desde,
                                     filter.Hasta,
                                     filter.Sucursal,
                                     filter.Usuario,
                                     filter.Estado,
                                     filter.Tipo
                                     )
                              .ToList();
        }
        public static List<RequisicionView> GetRequisicionByPedido(this QueryRepository<RequisicionView> repository, RequisicionFilters filter)
        {
            return repository.DbSet
                              .FromSqlRaw("EXECUTE sp_select_vw_Requisicion_by_pedido {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                                     filter.PageSize,
                                     filter.Page,
                                     filter.Id,
                                     filter.Desde,
                                     filter.Hasta,
                                     filter.Sucursal,
                                     filter.Usuario,
                                     filter.Estado,
                                     filter.Tipo
                                     )
                              .ToList();
        }

        public static List<PedidoView> GetPedidoByFilter(this QueryRepository<PedidoView> repository, PedidoFilters filter)
        {
            return repository.DbSet
                              .FromSqlRaw("EXECUTE sp_select_vw_Pedido {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                                     filter.PageSize,
                                     filter.Page,
                                     filter.Id,
                                     filter.Desde,
                                     filter.Hasta,
                                     filter.Sucursal,
                                     filter.UsuarioId,
                                     filter.Estado,
                                     filter.Tipo
                                     )
                              .ToList();
        }

        public static Presupuesto GetByRequisicionProveedorId(this GenericRepository<Presupuesto> repository, int requisicionId, int proveedorId)
        {
            return repository.DbSet
                    .FirstOrDefault(p => p.RequisicionId == requisicionId && p.ProveedorId == proveedorId);

        }

        public static List<PedidoItem> GetByRequisicionId(this GenericRepository<PedidoItem> repository, int requisicionId)
        {
            List<PedidoItem> items = repository.DbSet.Where(pi => pi.RequisicionId == requisicionId)
                     .Include(p => p.Pedido)
                       .ThenInclude(p => p.Usuario)
                     .Include(p => p.Pedido.Sucursal)  
                     .Include(p => p.Producto)
                      .ToList();

            return items;
        }


        //static string filePath = string.Empty;
        public static string SaveFileInDirectory(GenericRepository<Presupuesto> repository, Presupuesto presupuesto)
        {
            string presupuestoPath = Path.Combine(Sgm.OC.Framework.Settings.FilePath, presupuesto.Id.ToString());



            if (!Directory.Exists(presupuestoPath))
                Directory.CreateDirectory(presupuestoPath);

            string fullDirName = Path.Combine(presupuestoPath, "");

            if (!File.Exists(fullDirName))
            {
                File.WriteAllBytes(fullDirName, new byte[] { 1}) ;
                return fullDirName;
            }

            return null;

        }



    }
}
