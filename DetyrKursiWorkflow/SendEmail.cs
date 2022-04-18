using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System.Activities;


namespace FISOFT_ConsultingWorkflow
{
    public class SendEmail : CodeActivity
    {
        // setting up inputs for the custom workflow
        [RequiredArgument]
        [Input("fisoft_fullname")]
        public InArgument<string> Fisoft_fullname { get; set; } // input argument

        [Input("fisoft_universityid")]
        [ReferenceTarget("fisoft_university")]
        public InArgument<EntityReference> Fisoft_universityid { get; set; } // input argument

        // setting up the outputs for the custom workflow
        [Output("Name")]
        public OutArgument<string> Name { get; set; } // output arguments

        [Output("University")]
        public OutArgument<string> University { get; set; } // output argument
        protected override void Execute(CodeActivityContext executionContext)
        {
            //Create the tracing service
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            //Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // creating the variables wich will contain values of the input arguments
            string fisoft_fullname = Fisoft_fullname.Get(executionContext);
            string fisoft_universityid = Fisoft_universityid.Get(executionContext).Id.ToString(); // converting the guid to string

            QueryByAttribute query = new QueryByAttribute("fisoft_students"); // creating new query that will get elements from the students entity
            

            query.ColumnSet = new ColumnSet(new string[] { "fisoft_fullname", "fisoft_universityid" }); // setting new columns taken from entity's fields named fisoft_fullname and fisoft_university
            

            query.AddAttributeValue("fisoft_fullname", fisoft_fullname);  // adding the attribute value from fisoft_fullname variable 
            query.AddAttributeValue("fisoft_universityid", fisoft_universityid); // adding the attribute vaule from fisoft_universityid

            EntityCollection collection = service.RetrieveMultiple(query); // creating a collection with multiple elements from query
            Entity config = collection.Entities.FirstOrDefault(); // creating entity config with first elements from collection enity

            Name.Set(executionContext, config!=null? config.Attributes["fisoft_fullname"].ToString() :""); // setting the output argument named name with value taken from elemnt config entites
            University.Set(executionContext, ((EntityReference)config.Attributes["fisoft_universityid"]).Name); // setting the output argument named University with value taken from element config entites
            
        }

    }
}