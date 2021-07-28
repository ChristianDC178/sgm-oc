using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Domain.WF;
using Sgm.OC.Repositories;
using Sgm.OC.Repository;


namespace Sgm.OC.Test
{
    [TestClass]
    public class WorkflowTest
    {

        UnitOfWork unitOfWork1;

        [TestInitialize]
        public void Initialize()
        {
            Sgm.OC.Framework.Settings.ConnectionString = "Data Source=localhost; Initial Catalog=SgmOc_Diarco; User Id=sgmoc; Password=sgmocdev;";
            unitOfWork1 = new UnitOfWork(new SgmOcContext());
        }


        [TestMethod]
        public void Test()
        {
            try
            {
                Workflow workflow = unitOfWork1.WorkflowRepo.GetById(1);
                unitOfWork1.Context.Entry(workflow).Collection(w => w.Estados).Load();
                Assert.IsTrue(workflow.Estados.Count > 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                Assert.Fail();
            }
        }



        [TestMethod]
        public void Can_Set_Workflow_To_Entidad()
        {
            try
            {
                Requisicion requisicion = unitOfWork1.RequisicionRepo.GetById(1);
                Workflow workflow = unitOfWork1.WorkflowRepo.GetById(1);

                workflow.Estados = unitOfWork1.WorkflowEstadoRepo.DbSet.Where(est => est.WorkflowId == workflow.Id).ToList();

                requisicion.WorkflowEstados = workflow.GetWorkflowToEntity();

                requisicion.WorkflowEstados.ForEach((est) =>
                {
                    unitOfWork1.WorkflowEstadoEntidadRepo.Create(est);
                });

                WorkflowEstadoEntidad workflowEstadoEntidad = requisicion.WorkflowEstados.First(est => est.Secuencia == 1);
                
                workflowEstadoEntidad.EstadoId = 1;
                workflowEstadoEntidad.FechaEstado = DateTime.Now;
                workflowEstadoEntidad.UsuarioId = 1;
                workflowEstadoEntidad.Aprobado = true;
                workflowEstadoEntidad.RequisicionId = requisicion.Id;

                unitOfWork1.SaveChanges();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                Assert.Fail();
            }
        }



    }
}
