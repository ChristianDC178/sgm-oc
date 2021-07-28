using System;
using System.Collections.Generic;
using System.Text;
using Sgm.OC.Domain;

namespace Sgm.OC.Repository
{
    public class JobsUnitOfWork
    {

        private readonly SgmOcContext _context;

        public JobsUnitOfWork(SgmOcContext context)
        {
            _context = context;
        }

        private QueryRepository<ConsolidacionPendiente> _reqAutoRepo; 

        public QueryRepository<ConsolidacionPendiente> RequisicionAutoRepo
        {
            get { return _reqAutoRepo ?? new QueryRepository<ConsolidacionPendiente>(_context); }
        }

        public static JobsUnitOfWork Create()
        {
            return new JobsUnitOfWork(new SgmOcContext());
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
