using Microsoft.EntityFrameworkCore.Infrastructure;
using Sgm.OC.Domain;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.WF;
using Sgm.OC.Repository;
using Sgm.OC.Domain.Views;
using Microsoft.AspNetCore.Http.Connections;

namespace Sgm.OC.Repositories
{
    public class UnitOfWork
    {

        private readonly SgmOcContext _context;

        private GenericRepository<Sucursal> _sucursalRepo;
        private GenericRepository<Producto> _productoRepo;
        private GenericRepository<UnidadMedida> _unidadMedidaRepo;
        private GenericRepository<Requisicion> _requisicionRepo;
        private GenericRepository<RequisicionItem> _requisicionItemRepo;
        private GenericRepository<Pedido> _pedidoRepo;
        private GenericRepository<PedidoItem> _pedidoItemRepo;
        private GenericRepository<Usuario> _usuarioRepo;
        private GenericRepository<Proveedor> _proovedorRepo;
        private GenericRepository<Workflow> _workflowRepo;
        private GenericRepository<WorkflowEstado> _workflowestadoRepo;
        private GenericRepository<WorkflowEstadoEntidad> _workflowEstadoEntidadRepo;
        private GenericRepository<Estado> _estadoRepo;
        private GenericRepository<Rubro> _rubroRepo;
        private GenericRepository<Presupuesto> _presupuestoRepo;

        //Views
        private QueryRepository<PedidoItemView> _pedidoItemViewRepo;
        private QueryRepository<ProveedorView> _proveedorViewRepo;
        private QueryRepository<RequisicionView> _requisicionViewRepo;
        private QueryRepository<PedidoView> _pedidoViewRepo;

        public UnitOfWork(SgmOcContext context)
        {
            _context = context;
        }

        #region Repositories

        public GenericRepository<Usuario> UsuarioRepo
        {
            get
            {
                return _usuarioRepo ?? new GenericRepository<Usuario>(_context);
            }
        }

        public GenericRepository<Estado> EstadoRepo
        {
            get
            {
                return _estadoRepo ?? new GenericRepository<Estado>(_context);
            }
        }
        public GenericRepository<Sucursal> SucursalRepo
        {
            get
            {
                return _sucursalRepo ?? new GenericRepository<Sucursal>(_context);
            }
        }

        public GenericRepository<Rubro> RubroRepo
        {
            get
            {
                return _rubroRepo ?? new GenericRepository<Rubro>(_context);
            }
        }

        public GenericRepository<Producto> ProductoRepo
        {
            get
            {
                return _productoRepo ?? new GenericRepository<Producto>(_context);
            }
        }

        public GenericRepository<Proveedor> ProovedorRepo
        {
            get
            {
                return _proovedorRepo ?? new GenericRepository<Proveedor>(_context);
            }
        }

        public GenericRepository<UnidadMedida> UnidadMedidaRepo
        {
            get
            {
                return _unidadMedidaRepo ?? new GenericRepository<UnidadMedida>(_context);
            }
        }

        public GenericRepository<Requisicion> RequisicionRepo
        {
            get
            {
                return _requisicionRepo ?? new GenericRepository<Requisicion>(_context);
            }
        }

        public GenericRepository<RequisicionItem> RequisicionItemRepo
        {
            get
            {
                return _requisicionItemRepo ?? new GenericRepository<RequisicionItem>(_context);
            }
        }

        public GenericRepository<Pedido> PedidoRepo
        {
            get
            {
                return _pedidoRepo ?? new GenericRepository<Pedido>(_context);
            }
        }

        public GenericRepository<PedidoItem> PedidoItemRepo
        {
            get
            {
                return _pedidoItemRepo ?? new GenericRepository<PedidoItem>(_context);
            }
        }

        public GenericRepository<Workflow> WorkflowRepo
        {
            get
            {
                return _workflowRepo ?? new GenericRepository<Workflow>(_context);
            }

        }

        public GenericRepository<WorkflowEstado> WorkflowEstadoRepo
        {
            get
            {
                return _workflowestadoRepo ?? new GenericRepository<WorkflowEstado>(_context);
            }
        }

        public GenericRepository<WorkflowEstadoEntidad> WorkflowEstadoEntidadRepo
        {
            get
            {
                return _workflowEstadoEntidadRepo ?? new GenericRepository<WorkflowEstadoEntidad>(_context);
            }
        }
        
         public GenericRepository<Presupuesto> PresupuestoRepo
        {
            get
            {
                return _presupuestoRepo ?? new GenericRepository<Presupuesto>(_context);
            }
        }


        public QueryRepository<ProveedorView> ProveedorViewRepo
        {
            get
            {
                return _proveedorViewRepo ?? new QueryRepository<ProveedorView>(_context);
            }
        }

        public QueryRepository<PedidoItemView> PedidoItemViewRepo
        {
            get
            {
                return _pedidoItemViewRepo ?? new QueryRepository<PedidoItemView>(_context);
            }
        }

        public QueryRepository<RequisicionView> RequisicionViewRepo
        {
            get
            {
                return  _requisicionViewRepo  ?? new QueryRepository<RequisicionView>(_context);
            }
        }

        public QueryRepository<PedidoView> PedidoViewRepo
        {
            get
            {
                return _pedidoViewRepo ?? new QueryRepository<PedidoView>(_context);
            }
        }


        #endregion


        public static UnitOfWork Create()
        {
            return new UnitOfWork(new SgmOcContext());
        }

        public SgmOcContext Context
        {
            get
            {
                return _context;
            }
        }

        public Sgm.OC.Framework.DomainEnityState GetEntityState(object entity)
        {
            switch (_context.Entry(entity).State)
            {
                case Microsoft.EntityFrameworkCore.EntityState.Detached:
                    return Framework.DomainEnityState.Detached;
                case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                    return Framework.DomainEnityState.Unchanged;
                case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                    return Framework.DomainEnityState.Deleted;
                case Microsoft.EntityFrameworkCore.EntityState.Modified:
                    return Framework.DomainEnityState.Modified;
                case Microsoft.EntityFrameworkCore.EntityState.Added:
                    return Framework.DomainEnityState.Added;
            }

            return Framework.DomainEnityState.Unkknow;
        }


        public void SaveChanges()
        {
            _context.SaveChanges();
        }

    }

}
