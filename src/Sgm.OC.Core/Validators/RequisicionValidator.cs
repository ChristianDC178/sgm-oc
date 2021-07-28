using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Sgm.OC.Framework;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;

namespace Sgm.OC.Core.Validators
{

    public abstract class DomainValidator<TEntity> : EntityBase
    {

        public abstract Validation Validate(TEntity entity);

    }



    public class RequisicionValidator : DomainValidator<Requisicion>
    {

        public override Validation Validate(Requisicion entity)
        {

            UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
            Validation validation = new Validation();

            if (entity.Items.Count == 0)
                validation.Add("Debe agregar productos");

            return validation;

        }

    }


}
