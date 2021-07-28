using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;
using Sgm.OC.Repository;

namespace Sgm.OC.Test
{
    [TestClass]
    public class UnitTest1
    {

        UnitOfWork unitOfWork1;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork1 = new UnitOfWork(new SgmOcContext());
        }


        [TestMethod]
        public void Test()
        {

            try
            {
                //string token = 
                //qO/g5bM93Cu7UM/yPEDGE9N/9LX8wq7C3WHFFNZFz5AXak3j+oEYARzphpo4HUPmyoMMjH1sXMlJTfVKMObvN64aeUxyHDcT4bYH5UhlX2p0oo7xOfuhawaig+J7E4OtX5+fiCqGKjCp+zYuH8uenr/4Fqh7/P3qWbPCbHHJXm0=
                //qO/g5bM93Cu7UM/yPEDGE9N/9LX8wq7C3WHFFNZFz5AZr57PE8dUWu0iedfzmlfTZQSIqkde1nsQ3TqVTRWk8+vOWXwdB4qPfcSib+LrZ7/t/rsF0mbHqooTT8/0TCpJoMuhSREvDxjuqtqSwLbIqUA1U7azrmv5I0Xp+ljD6Xk=

                //http://localhost:3000/auth/?token=qO/g5bM93Cu7UM/yPEDGE96j1RGsLOlfTAcaAnzn7GuekTLh6TyiVDVKPtpkL3D2M30v5/wYijZiKudjXIvDfpZYNG9bluZIgZ+qbFmMQG/RzhuvAsGuNKZuFQ4N6rZPtKAj9XYJyIlChMrgfCHnwEf9NZgSzxc3TSJZ63J4lnA=

                string user = AssemblyJson("christian");

                //qO/g5bM93Cu7UM/yPEDGE96j1RGsLOlfTAcaAnzn7GvqfgNo2f0Tlzx0vs7D9VpBlLzZ1lu1DxPxZB6R3IRwuKJX+Rt4NjH7eNAz/jIAuBucQmOf/vWIWSmURXoxBc5Iy2f2maUAqdutd/ANT8W2vtWbTTMsbtPPBRfD4unyMpQ=
                //string token = "qO/g5bM93Cu7UM/yPEDGE9N/9LX8wq7C3WHFFNZFz5AZr57PE8dUWu0iedfzmlfTZQSIqkde1nsQ3TqVTRWk8+vOWXwdB4qPfcSib+LrZ7/t/rsF0mbHqooTT8/0TCpJoMuhSREvDxjuqtqSwLbIqUA1U7azrmv5I0Xp+ljD6Xk=";

                string token = "GKpffUtA7TLp8wQCfP3yHr-bZoDPNPkeidjDADCnvS5FUcDzH3ZOz91D8OAGLHSyYClSNUx7RnEZ9loCDGB19PNO4T4yzmtRz1qY4a7Q8nChmbQS_-BVxU6GtIMIPvs10aidSHFCuIBwLQhpH7HpGJDRQJUv9406Q4VM9kC5rs81";

                //{"login":"christian","date":-8586024075591884610,"expirationDate":-8586024072591804508,"expiration":5}

                string result = Sgm.OC.Framework.Encription.EncryptedPasswordManager.ForPassword("Diarco").Decrypt(token);

                UserAuthInfo userAuthInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAuthInfo>(result);

                DateTime dateTime = DateTime.FromBinary(userAuthInfo.Date).AddMinutes(userAuthInfo.Expiration);

                Assert.IsTrue(dateTime > DateTime.Now);

            }
            catch (Exception ex)
            {
                var a = ex;
                Assert.Fail();
            }

        }

        class UserAuthInfo
        {
            public string Login { get; set; }
            public long Date { get; set; }
            public long ExpirationDate { get; set; }
            public long Expiration { get; set; }
        }


        private string AssemblyJson(string user)
        {
            int expMinutes = 5; //Pueden configurarlo en el appSetting.json
            var jsonObj = new
            {
                login = user,
                date = DateTime.Now.ToBinary(),
                expirationDate = DateTime.Now.AddMinutes(expMinutes).ToBinary(),
                expiration = expMinutes
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj).ToString();
        }


        [TestMethod]
        public void Can_Create_Producto()
        {

            try
            {

                UnitOfWork unitOfWork = new UnitOfWork(new SgmOcContext());

                UnidadMedida unidadMedida = unitOfWork.UnidadMedidaRepo.DbSet.First(um => um.Id == 2);

                Producto producto = new Producto()
                {
                    IdInterno = 13,
                    Descripcion = "Gaseosa 1L",
                    Modificacion = DateTime.Now
                };

                producto.UnidadMedida = unidadMedida;

                unitOfWork.ProductoRepo.Create(producto);
                unitOfWork.SaveChanges();



            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Can_Create_Producto_UnidadMedida()
        {
            try
            {
                UnidadMedida unidadMedida = unitOfWork1.UnidadMedidaRepo.DbSet.First(um => um.Id == 1);

                Producto producto = new Producto()
                {
                    IdInterno = 16,
                    Descripcion = "Arroz 1kg",
                    Modificacion = DateTime.Now
                };

                producto.UnidadMedida = unidadMedida;

                unitOfWork1.ProductoRepo.Create(producto);
                unitOfWork1.SaveChanges();

                Assert.IsTrue(true);

            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        //Fede
        [TestMethod]
        public void Get_Producto_By_Id()
        {
            int id = 10;
            UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
            Producto producto = new Producto();

            //"SELECT"
            producto = unitOfWork.ProductoRepo.DbSet
                .Where(p => p.Id == id).First();
            Assert.IsNotNull(producto);
        }

        [TestMethod]
        public void Get_Producto_By_Filter()
        {
            var predicate = LinqKit.PredicateBuilder.New<Producto>();

            predicate = predicate.And(i => i.Descripcion.StartsWith("Tarima"));
            //predicate = predicate.And(i => i.Id == 1);
            
            UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
            List<Producto> productos = new List<Producto>();

            //"SELECT"
            var productos2 = unitOfWork.ProductoRepo.DbSet.Where(predicate).Select(i => i).ToList();
            
            //productos = unitOfWork.ProductoRepo.GetProductosByFilter(predicate);
        }
        [TestMethod]
        public void Can_Get_Producto()
        {
            try
            {

                Producto producto =
                    unitOfWork1.ProductoRepo.DbSet
                    .Include(p => p.UnidadMedida)
                    .First(p => p.Id == 2);

                Assert.IsNotNull(producto);
                Assert.IsNotNull(producto.UnidadMedidaId);
                Assert.IsNotNull(producto.UnidadMedida);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Can_Producto_And_UnidadMedida()
        {
            try
            {

                Producto producto = new Producto()
                {
                    IdInterno = 30,
                    Descripcion = "Nafta",
                    Modificacion = DateTime.Now
                };

                UnidadMedida unidadMedida = new UnidadMedida()
                {
                    Descripcion = "Galon",
                    Simbolo = "g"
                };

                producto.UnidadMedida = unidadMedida;

                unitOfWork1.ProductoRepo.Create(producto);
                unitOfWork1.UnidadMedidaRepo.Create(producto.UnidadMedida);

                unitOfWork1.SaveChanges();

                Assert.IsTrue(true);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                Assert.Fail();
            }

        }

        [TestMethod]
        public void Can_Create_Requisicion()
        {
            try
            {

                //te paso 2 productos 

                List<Producto> productos = unitOfWork1.ProductoRepo.DbSet
                    .Where(p => p.Id >= 1 && p.Id <= 2).ToList();

                Requisicion requisicion = new Requisicion(1, 1);

                foreach (var item in productos)
                {

                    RequisicionItem requisicionItem = new RequisicionItem(item.Id, 2, false);
                    requisicionItem.Requisicion = requisicion;
                    requisicionItem.Producto = item;

                    requisicion.Items.Add(requisicionItem);

                    unitOfWork1.RequisicionItemRepo.Create(requisicionItem);

                }

                unitOfWork1.RequisicionRepo.Create(requisicion);
                unitOfWork1.SaveChanges();

                Assert.IsTrue(true);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                Assert.Fail();
            }
        }

        [TestMethod]
        public void Can_Get_Requisicion()
        {
            try
            {

                Requisicion requisicion = unitOfWork1.RequisicionRepo.DbSet
                     .Include(req => req.Items)
                      .ThenInclude(item => item.Producto)
                      .ThenInclude(p => p.UnidadMedida)
                       .First(r => r.Id == 6);

                Assert.IsTrue(requisicion.Items.Count > 0);
                Assert.IsNotNull(requisicion.Items.First().Producto);
                Assert.IsNotNull(requisicion.Items.First().Producto.UnidadMedida);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.ToString());
                Assert.Fail();
            }
        }

    }


}
