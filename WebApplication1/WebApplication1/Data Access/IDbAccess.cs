using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApplication1.Data_Access
{
    internal interface IDbAccess<TEntity,in TPk> where TEntity : class
    {
        ActionResult getAllData(TEntity entity);
    }
}
