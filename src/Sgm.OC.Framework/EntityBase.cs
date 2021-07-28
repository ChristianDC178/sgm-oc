using System;

namespace Sgm.OC.Framework
{
    public class EntityBase
    {

        public int Id { get; set; }
        public string Descripcion { get; set; }

    }

    public class EntityBussiness : EntityBase
    {

        public EntityBussiness()
        {
            Creacion = DateTime.Now;
        }

        public DateTime Creacion { get; set; }
        public DateTime? Modificacion { get; set; }

        //public DateTime Creacion { get; set; }
        //public DateTime? Modificacion { get; set; }

    }

}