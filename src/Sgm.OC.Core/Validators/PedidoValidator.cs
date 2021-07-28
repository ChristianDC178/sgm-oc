using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Sgm.OC.Framework;
using Sgm.OC.Domain.Entities;
using Sgm.OC.Repositories;

namespace Sgm.OC.Core.Validators
{
    public class PedidoValidator : DomainValidator<Pedido>
    {

        public override Validation Validate(Pedido entity)
        {

            UnitOfWork unitOfWork = new UnitOfWork(new Repository.SgmOcContext());
            Validation validation = new Validation();

            if (entity.Items.Count == 0)
                validation.Add("Debe agregar productos");

            return validation;

        }
    }
}
