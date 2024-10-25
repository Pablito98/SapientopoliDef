using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace Sapientopoli.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<DataSet> QueryAsync(FormattableString formattableQuery);
    }
}