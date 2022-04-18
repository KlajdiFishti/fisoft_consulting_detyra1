using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace FISOFT_ConsultingPlugins
{
    public class DeleteUniversityCheck : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            tracingService.Trace("1");
            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)

            {
                tracingService.Trace("2");
                // Obtain the target entity from the input parameters.  
                EntityReference entity = (EntityReference)context.InputParameters["Target"];
                tracingService.Trace("3");
                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    tracingService.Trace("4");

                    string university = entity.Id.ToString();
                    QueryExpression query = new QueryExpression("fisoft_students"); // creating new query wich gathers elements from entity students
                    query.ColumnSet = new ColumnSet(new[] { "fisoft_universityid" }); // set new column of the query named fisoft_email
                    query.Criteria.AddCondition("fisoft_universityid", ConditionOperator.Equal, university);// adding condition to the column of the query to be equal to the university guid converted to string
                    EntityCollection collection = service.RetrieveMultiple(query); // retrieving myltiple values of the query into collection
                    tracingService.Trace("collection.Entities.Count: "+ collection.Entities.Count);
                    if (collection.Entities.Count > 0) // if collection  has more than one value 
                    {
                        throw new InvalidPluginExecutionException("University "+(entity.Name+" has already students linked to it.")); // cause an error to let the user know that the university record has students and therfore wont be deleted
                    }

                    tracingService.Trace("5");

                    
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }

            
        }

    }
}
