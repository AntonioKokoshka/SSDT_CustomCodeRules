using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace CustomCodeAnalysisRule
{
    [ExportCodeAnalysisRule(RuleId
               , RuleDisplayName
               , Description = "Each Warehouse table has DWHTimeStamp column"
               , Category = "Naming"
               , RuleScope = SqlRuleScope.Element)]
    public class DimensionWithIdentityRule1 : SqlCodeAnalysisRule
    {
        public const string RuleId = "Public.Dac.Samples.SR8001";
        public const string RuleDisplayName = "Each Warehouse table has DWHTimeStamp column";
        public const string Message = "No DWHTimestamp column in {0} table";
        public DimensionWithIdentityRule1()
        {
            SupportedElementTypes = new[] { Table.TypeClass };
        }

        public override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            var problems = new List<SqlRuleProblem>();

            var objectName = ruleExecutionContext.ModelElement;

            var objectRealName = GetElementName(ruleExecutionContext, objectName);

            int IsCorrect = 0;

            if (objectName != null)
            {
                foreach (var child in objectName.GetReferencedRelationshipInstances(Table.Columns))
                {
                    var type = child.Object.GetReferenced(Column.DataType).FirstOrDefault();
                    //var propertyValue = child.GetReferencing().Where(x => x.ObjectType.Name == "ExtendedProperty").First().GetProperty(ExtendedProperty.Value);

                    //child.Object.GetReferenced();
                    var isNullable = type.GetProperty<bool?>(DataType.UddtNullable);
                    var length = type.GetProperty<int?>(DataType.UddtLength);
                    
                    problems.Add(new SqlRuleProblem(string.Format("The dimension table {0} has no column", length), objectName));

                    //do something useful with this information!
                }

                if (!IsDWHTimeStamp(objectName.Name))
                {
                    
                    IsCorrect++;
                    
                }
            }

            if (IsCorrect > 0)
            {
                var displayServices = ruleExecutionContext.SchemaModel.DisplayServices;

                var formattedName = displayServices.GetElementName(objectName, ElementNameStyle.FullyQualifiedName);

                var problemDescription = string.Format(Message, formattedName);

                var problem = new SqlRuleProblem(problemDescription, objectName);

                problems.Add(problem);
            }
                //var columns = tableElement.GetReferenced(Table.Columns);
                //if (columns.Count() == 0)
                //    problems.Add(new SqlRuleProblem(string.Format("The dimension table {0} has no column", tableName), tableElement));




                return problems;
            
        }

        private static string GetElementName(SqlRuleExecutionContext ruleExecutionContext, TSqlObject modelElement)
        {
            // Get the element name using the built in DisplayServices. This provides a number of 
            // useful formatting options to
            // make a name user-readable
            var displayServices = ruleExecutionContext.SchemaModel.DisplayServices;
            string elementName = displayServices.GetElementName(modelElement, ElementNameStyle.EscapedFullyQualifiedName);
            return elementName;
        }
        private static bool IsDWHTimeStamp(ObjectIdentifier id)
        {
            return id.HasName && ValidationHelpers.IsDWHTimeStamp(id.Parts.Last());
        }
    }
}
