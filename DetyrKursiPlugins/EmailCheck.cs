using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace FISOFT_ConsultingPlugins
{
    public class EmailCheck : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =(ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity fisoft_student = (Entity)context.InputParameters["Target"];

                try
                {
                    // Plug-in business logic goes here.
                    string email = string.Empty; //declaring string named email and setting it to empty
                    
                    if (fisoft_student.Attributes.Contains("fisoft_email")) // chechking if user entered an email in his new record
                    { 
                        email = fisoft_student.Attributes["fisoft_email"].ToString(); // setting variable email to containg the string of email added

                        QueryExpression query = new QueryExpression("fisoft_students"); // creating new query wich gathers elements from entity students
                        query.ColumnSet = new ColumnSet(new[] {"fisoft_email"}); // set new column of the query named fisoft_email
                        query.Criteria.AddCondition("fisoft_email",ConditionOperator.Equal, email); // adding condition to the column of the query to be equal to the email variable
                        EntityCollection collection = service.RetrieveMultiple(query); // retrieving myltiple values of the query into collection
                        if(collection.Entities.Count >0) // if collection  has more than one vale
                        {
                            throw new InvalidPluginExecutionException("Student with this email already exist"); // cause an error
                        }
                        // setting up new variables to create a new attribute named full name wich will be needed when formain the email to send to the student
                        string firstName = string.Empty; 
                        string lastName = string.Empty;
                        // populating the variables 
                        if (fisoft_student.Attributes.Contains("fisoft_name")) 
                        {
                            firstName = fisoft_student.Attributes["fisoft_name"].ToString();
                        }
                        if (fisoft_student.Attributes.Contains("fisoft_lastname"))
                        {
                            lastName = fisoft_student.Attributes["fisoft_lastname"].ToString();
                        }
                        // adding the new atribute wich its value will be the value of the two other atributes combined
                        fisoft_student.Attributes.Add("fisoft_fullname",firstName + " " + lastName);

                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Can't continue without an email");
                    }
                    
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
