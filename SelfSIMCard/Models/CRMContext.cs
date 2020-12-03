using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;

namespace SelfSIMCard.Models
{
    public class CRMContext: DbContext
    {
        public CRMContext() : base("CrmDbContext")
        {
            //DbConfiguration.SetConfiguration(new Oracle.)
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbStock GetStock(string ICCID)
        {
            return SqlQuery<DbStock>(string.Format(
@"SELECT DISTINCT sim.RES_CODE ICCID,
                  sim.IMSI,
                  CASE WHEN pack.RES_ID IS NULL THEN sim_org.ORG_NAME ELSE pack_org.ORG_NAME END ORG_NAME,
                  CASE WHEN pack.RES_ID IS NULL THEN sim.MODEL_ID ELSE pack.MODEL_ID END MODEL_ID,
                  CASE WHEN pack.RES_ID IS NULL THEN sim.RES_STATUS_ID ELSE pack.RES_STATUS_ID END RES_STATUS_ID,
                  pack.MSISDN, sub.SUB_ID, sub.CUST_ID, sub.MSISDN SUB_MSISDN, sim.PUK1
FROM INVENTORY.RES_SIM sim
LEFT JOIN INVENTORY.RES_STARTPACK pack ON (sim.RES_CODE = pack.RES_CODE)
LEFT JOIN CCARE.INF_SUBSCRIBER_ALL sub ON (sub.ICCID = sim.RES_CODE AND sub.SUB_STATE <> 'B02')
LEFT JOIN CRMPUB.SYS_ORG sim_org ON (sim.DEPT_ID = sim_org.ORG_ID)
LEFT JOIN CRMPUB.SYS_ORG pack_org ON (pack.DEPT_ID = pack_org.ORG_ID)
WHERE sim.RES_CODE = '{0}'", ICCID));
        }

        private T SqlQuery<T>(string query) where T : class
        {
            T result = null;
            bool success = false;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    result = Database.SqlQuery<T>(query).FirstOrDefault();

                    success = true;
                    break;
                }
                catch (DbEntityValidationException exp)
                {
                    throw new Exception(exp.EntityValidationErrors.FirstOrDefault().ToString());
                }
            }

            if (!success)
                throw new Exception(string.Format("Failed query for {0}", typeof(T).Name));

            return result;
        }

    }
}