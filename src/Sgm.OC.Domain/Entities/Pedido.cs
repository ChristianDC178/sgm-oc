using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Localization;
using Microsoft.VisualBasic;
using Sgm.OC.Domain.WF;
using Sgm.OC.Framework;

namespace Sgm.OC.Domain.Entities
{
    public class Pedido : EntityBussiness
    {

        public Pedido(int sucursalId, int usuarioId)
        {
            UsuarioId = usuarioId;
            SucursalId = sucursalId;
        }

        private Pedido() { }

        public string PrefijoId 
        { 
            get
            {
                return "SOL-" + Id;
            } 
        }

        public int EstadoId { get; set; }
        public int UsuarioId { get; private set; }
        public bool Recurrente { get; set; }
        public bool Prevalidar { get; set; }
        public int SucursalId { get; set; }

        public Sucursal Sucursal { get; set; }
        public Usuario Usuario { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public Usuario UsuarioModificacion { get; set; }
        public Estado EstadoActual { get; set; }
        public List<PedidoItem> Items { get; set; } = new List<PedidoItem>();
        public Workflow Workflow { get; set; }
        public List<WorkflowEstadoEntidad> WorkflowEstados { get; set; } = new List<WorkflowEstadoEntidad>();

        public bool Rechazado
        {
            get
            {
                return EstadoId == Constants.PEDIDO_RECHAZADO_ESTADO_ID;
            }
        }

        public ChangeWorkflowEstadoResult ApproveWorkflowEstado(Usuario usuario, string comentario)
        {

            ChangeWorkflowEstadoResult result = new ChangeWorkflowEstadoResult();

            if (!WorkflowEstados.Any())
                result.Validation.Add("No contiene estados");

            //WorkflowEstadoEntidad current = WorkflowEstados.FirstOrDefault(we => we.EstadoId == EstadoId);
            WorkflowEstadoEntidad current = WorkflowEstados.FirstOrDefault(we => we.IsCurrent);

            if (current.Final && current.Aprobado.HasValue)
            {
                result.Validation.Add("El proceso de aprobación esta finalizado");
                return result;
            }

            if (usuario.IsInRole(current.CodigoRol))
            {

                if (current.ComentarioRequerido)
                {
                    if (!string.IsNullOrEmpty(comentario))
                    {
                        current.Comentario = comentario;
                    }
                    else
                    {
                        result.Validation.Add("Comentario obligatorio");
                    }
                }

                current.Aprobado = true;
                current.UsuarioId = usuario.Id;
                current.FechaEstado = DateAndTime.Now;
                result.EstadoAprobado = current;
                EstadoId = current.EstadoId;
                Modificacion = DateAndTime.Now;
                UsuarioModificacionId = usuario.Id;

                if (!current.Final)
                {
                    result.EstadoSiguiente = WorkflowEstados.FirstOrDefault(we => we.Secuencia == (current.Secuencia + 1));
                    result.EstadoSiguiente.IsCurrent = true;
                    current.IsCurrent = false;
                }
                
            }
            else
            {
                result.Validation.Add("No está autorizado para cambiar el estado");
            }

            return result;
        }

        public ChangeWorkflowEstadoResult RejectWorkFlowEstado(Usuario usuario, string comentario)
        {

            ChangeWorkflowEstadoResult result = new ChangeWorkflowEstadoResult();

            if (!WorkflowEstados.Any())
                result.Validation.Add("No contiene estados");

            WorkflowEstadoEntidad current = WorkflowEstados.FirstOrDefault(we => we.IsCurrent);

            if (current.Final && current.Aprobado.HasValue)
            {
                result.Validation.Add("El proceso de aprobación esta finalizado");
                return result;
            }

            if (usuario.IsInRole(current.CodigoRol) && current.Rechazar)
            {

                current.UsuarioId = usuario.Id;
                current.FechaEstado = DateAndTime.Now;
                current.Aprobado = false;
                current.Comentario = comentario;

                Modificacion = DateTime.Now;
                UsuarioModificacionId = usuario.Id;
                EstadoId = Constants.PEDIDO_RECHAZADO_ESTADO_ID;

                result.EstadoAprobado = current;

            }
            else
            {
                result.Validation.Add("No está autorizado para cambiar el estado");
            }

            return result;

        }

    }



}

