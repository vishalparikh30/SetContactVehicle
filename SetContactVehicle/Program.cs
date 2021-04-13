using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetContactVehicle
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Authtype = Office365; Url=your Dynamics URL; Username=UserName; Password=Password";
            CrmServiceClient client = new CrmServiceClient(connectionString);
            //OrganizationServiceContext context = new OrganizationServiceContext(client);
            EntityCollection entityCollection = null;
            EntityCollection resultCollection = new EntityCollection();
            int pageNumber = 1;
            string pageCookie = null;
            do
            {
                entityCollection = new EntityCollection();
                QueryExpression leadQuery = new QueryExpression();
                leadQuery.PageInfo = new PagingInfo();
                leadQuery.PageInfo.Count = 5000;
                leadQuery.PageInfo.PageNumber = pageNumber;
                leadQuery.PageInfo.PagingCookie = pageCookie;
                leadQuery.PageInfo.ReturnTotalRecordCount = true;
                leadQuery.EntityName = "af_contactvehicle";
                leadQuery.ColumnSet = new ColumnSet(new string[] { "af_contactid", "af_contactphoneno", "af_contactname", "af_contactemail" });
                entityCollection = client.RetrieveMultiple(leadQuery);
                pageCookie = entityCollection.PagingCookie;
                pageNumber++;
                resultCollection.Entities.AddRange(entityCollection.Entities);
            }
            while (entityCollection.MoreRecords);
            int count = 1;
            foreach (var entity in resultCollection.Entities)
            {
                Console.WriteLine(count);
                if (entity.Contains("af_contactid") && entity["af_contactid"] != null)
                {
                    EntityReference contactRef = entity["af_contactid"] as EntityReference;
                    Entity Contact =client.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet(new string[] {"lastname","emailaddress1","telephone1" }));
                    entity["af_contactphoneno"] = Contact.Contains("telephone1") && Contact["telephone1"] != null ? Convert.ToString(Contact["telephone1"]) : string.Empty;
                    entity["af_contactemail"] = Contact.Contains("emailaddress1") && Contact["emailaddress1"] != null ? Convert.ToString(Contact["emailaddress1"]) : string.Empty;
                    entity["af_contactname"] = Contact.Contains("lastname") && Contact["lastname"] != null ? Convert.ToString(Contact["lastname"]) : string.Empty;
                    client.Update(entity);
                }
                count++;
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
