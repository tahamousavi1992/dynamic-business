using DynamicBusiness.BPMS.CodePanel;
using DynamicBusiness.BPMS.Domain;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DynamicCodeEngine : BaseEngine, IDynamicCodeEngine
    {
        public DynamicCodeEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public static void SetToErrorMessage(CodeResultModel result, ResultOperation resultOperation)
        {
            if (result != null)
            {
                if (!result.Result)
                {
                    if (resultOperation != null)
                    {
                        resultOperation.SetHasError();
                        if (!(result?.CodeBaseShared.MessageList?.Any() ?? false))
                            resultOperation.AddError(LangUtility.Get("Failed.Text", "Engine"));
                    }
                }
            }
        }

        /// <returns>check condition and single line</returns>
        public bool ExecuteBooleanCode(DesignCodeModel designCode, FormModel formModel = null)
        {
            if (string.IsNullOrWhiteSpace(designCode?.Code))
                return true;
            CodeBase myInstance = DynamicCodeEngine.LoadClass(designCode.ID, base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID);

            myInstance.InitialProperties(base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentThreadID, base.EngineSharedModel.CurrentApplicationPageID, base.UnitOfWork, base.EngineSharedModel.BaseQueryModel, GetCurrentUserID(base.EngineSharedModel.CurrentUserName), base.EngineSharedModel.CurrentUserName, formModel, base.EngineSharedModel.ApiSessionID, null);
            bool result = Convert.ToBoolean(myInstance.ExecuteCode());

            return result;
        }

        /// <summary>
        /// mostly used in service task and execute runCode command in grid column.
        /// </summary>
        public CodeResultModel ExecuteScriptCode(DesignCodeModel designCodeModel, CodeBaseSharedModel codeBaseShared)
        {
            if (string.IsNullOrWhiteSpace(designCodeModel?.Code))
                return null;
            CodeBase myInstance = DynamicCodeEngine.LoadClass(designCodeModel.ID, base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID);

            myInstance.InitialProperties(base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentThreadID, null, base.UnitOfWork, base.EngineSharedModel.BaseQueryModel, GetCurrentUserID(base.EngineSharedModel.CurrentUserName), base.EngineSharedModel.CurrentUserName, null, base.EngineSharedModel.ApiSessionID, codeBaseShared);
            bool result = Convert.ToBoolean(myInstance.ExecuteCode());

            return new CodeResultModel(result, myInstance.UrlHelper.RedirectUrlModel, myInstance.CodeBaseShared);
        }

        //before having showed form,it will execute form's OnEntryFormCode.
        public CodeResultModel ExecuteOnEntryFormCode(DesignCodeModel designCodeModel, FormModel formModel, CodeBaseSharedModel codeBaseShared)
        {
            if (string.IsNullOrWhiteSpace(designCodeModel?.Code))
                return null;

            CodeBase codeBase = DynamicCodeEngine.LoadClass(designCodeModel.ID, base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID);
            codeBase.InitialProperties(base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentThreadID, base.EngineSharedModel.CurrentApplicationPageID, base.UnitOfWork, base.EngineSharedModel.BaseQueryModel, GetCurrentUserID(base.EngineSharedModel.CurrentUserName), base.EngineSharedModel.CurrentUserName, formModel, base.EngineSharedModel.ApiSessionID, codeBaseShared);

            return new CodeResultModel(Convert.ToBoolean(codeBase.ExecuteCode()), codeBase.UrlHelper.RedirectUrlModel, codeBase.CodeBaseShared);
        }

        //after having posted form,it will execute form's OnExitFormCode
        public CodeResultModel ExecuteOnExitFormCode(DesignCodeModel designCodeModel, FormModel formModel, CodeBaseSharedModel codeBaseShared)
        {
            if (string.IsNullOrWhiteSpace(designCodeModel?.Code))
                return null;
            CodeBase codeBase = new CodeBase();
            //CodeBase codeBase = DynamicCodeEngine.LoadClass(designCodeModel.ID, base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID);
            codeBase.InitialProperties(base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentThreadID, base.EngineSharedModel.CurrentApplicationPageID, base.UnitOfWork, base.EngineSharedModel.BaseQueryModel, GetCurrentUserID(base.EngineSharedModel.CurrentUserName), base.EngineSharedModel.CurrentUserName, formModel, base.EngineSharedModel.ApiSessionID, codeBaseShared);
            foreach (DCSetVariableModel dCBase in designCodeModel.CodeObjects)
            {
                dCBase.Execute(codeBase, base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID, base.UnitOfWork);
            }
            return new CodeResultModel(true, codeBase.UrlHelper.RedirectUrlModel, codeBase.CodeBaseShared);
            //return new CodeResultModel(Convert.ToBoolean(codeBase.ExecuteCode()), codeBase.UrlHelper.RedirectUrlModel, codeBase.CodeBaseShared);
        }

        public CodeResultModel SaveButtonCode(ButtonHtml buttonHtml, ResultOperation resultOperation,
             FormModel formModel, CodeBaseSharedModel codeBaseShared)
        {
            DesignCodeModel designCodeModel = null;
            if (!string.IsNullOrWhiteSpace(buttonHtml.BackendCoding))
                designCodeModel = DesignCodeUtility.GetDesignCodeFromXml(buttonHtml.BackendCoding);

            if (designCodeModel != null && !string.IsNullOrWhiteSpace(designCodeModel.Code))
            {
                var result = this.ExecuteOnExitFormCode(designCodeModel, formModel, codeBaseShared);
                if (!result.Result)
                {
                    resultOperation.SetHasError();
                    if (!(result?.CodeBaseShared.MessageList?.Any() ?? false))
                        resultOperation.AddError(LangUtility.Get("Failed.Text", "Engine"));
                }

                return result;
            }
            return null;
        }

        public void SaveExternalVariable(CodeResultModel codeResultModel)
        {
            //If in script task any variable is set, it Will save them all at the end
            if (codeResultModel?.CodeBaseShared?.ListSetVariable?.Any() == true)
                foreach (var variableModel in codeResultModel.CodeBaseShared.ListSetVariable)
                    new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(variableModel, null);

        }

        #region ..::private properties ::..

        private static Guid? GetCurrentUserID(string Username)
        {
            return new UserService().GetInfo(Username ?? "")?.ID;
        }

        private static CodeBase LoadClass(string className, Guid? processID, Guid? applicationPageID)
        {
            AppDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
            string compiled = BPMSResources.FilesRoot + BPMSResources.AssemblyRoot + "\\" + BPMSResources.AssemblyCompiledRoot;
            string compiledFile = compiled + "\\" + (processID.ToStringObjNull() ?? applicationPageID.ToStringObj());

            if (!System.IO.Directory.Exists(compiled))
                System.IO.Directory.CreateDirectory(compiled);

            if (System.IO.File.Exists(compiledFile))
            {
                return (DynamicCodeEngine.GetAssembly(compiledFile).CreateInstance("c_" + className.Replace("-", "_")) as CodeBase);
            }
            else
            {
                CompileAssemblies(processID.HasValue ?
                    new ProcessService().GetInfo(processID.Value).SourceCode :
                    new DynamicFormService().GetInfoByPageID(applicationPageID.Value).SourceCode,
                    processID.ToStringObjNull() ?? applicationPageID.ToStringObj());

                return (DynamicCodeEngine.GetAssembly(compiledFile).CreateInstance("c_" + className.Replace("-", "_")) as CodeBase);
            }
        }

        private static void CompileAssemblies(string sourceScript, string assemblyID)
        {
            sourceScript = @"using System;
                            using System.Linq;
                            using System.Collections.Generic;
                            using DynamicBusiness.BPMS.Domain;
                            using DotNetNuke.Entities.Users;
                            using DotNetNuke.Security.Membership;
                            using DynamicBusiness.BPMS.CodePanel;
                             " + Environment.NewLine + sourceScript;
            string compiled = BPMSResources.FilesRoot + BPMSResources.AssemblyRoot + "\\" + BPMSResources.AssemblyCompiledRoot;
            string compiledFile = compiled + "\\" + assemblyID;
            List<string> allAssembly = new List<string>();
            System.CodeDom.Compiler.CompilerParameters dynamicParams =
                          new System.CodeDom.Compiler.CompilerParameters();
            System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            string path = Path.GetDirectoryName(currentAssembly.EscapedCodeBase.Replace("file:///", ""));

            allAssembly.Add(currentAssembly.Location);   // Reference the current assembly from within dynamic one
            allAssembly.AddRange(Directory.GetFiles(path, "*.dll").Where(dll => currentAssembly.GetReferencedAssemblies().Any(c => dll.Contains(c.Name + ".dll"))).ToList());
            //add system.dll 
            allAssembly.Add(typeof(System.ComponentModel.IListSource).Assembly.Location);

            allAssembly.AddRange(currentAssembly.GetReferencedAssemblies().Where(c =>
            !c.Name.Contains("Microsoft.CodeAnalysis") &&
            !c.Name.Contains("System.Collections.Immutable")
            && (!allAssembly.Any(d => d.Contains(c.Name)) || c.Name == "System.Web"))
                .Select(c => System.Reflection.Assembly.ReflectionOnlyLoad(c.FullName).Location).ToList());

            allAssembly = allAssembly.Where(c =>
            !c.Contains("Stimulsoft.Base.dll") &&
            !c.Contains("Stimulsoft.Report.dll") &&
            !c.Contains("Stimulsoft.Report.Web.dll")).ToList();

            if (System.IO.Directory.Exists(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot))
            {
                allAssembly.AddRange(new DirectoryInfo(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot).GetFiles("*.dll").Select(c => c.FullName).ToList());
            }

            RoslynCompiler.compile(FixSourceCode(sourceScript), allAssembly, compiledFile);
            Assembly.Load(File.ReadAllBytes(compiledFile));
        }

        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            // Ignore missing resources
            if (args.Name.Contains(".resources"))
                return null;

            if (!AppDomain.CurrentDomain.GetAssemblies().Any(c => c.GetName().Name == args.Name.Split(',')[0]))
            {
                try
                {
                    string fileName = BPMSResources.FilesRoot + BPMSResources.AssemblyRoot + "\\" + BPMSResources.AssemblyCompiledRoot + "\\" + args.Name.Split(',')[0];
                    if (System.IO.File.Exists(fileName))
                        return GetAssembly(fileName);
                    else
                    {
                        if (System.IO.File.Exists(fileName.Replace($"\\{BPMSResources.AssemblyCompiledRoot}", "") + ".dll"))
                            return GetAssembly(fileName.Replace($"\\{BPMSResources.AssemblyCompiledRoot}", "") + ".dll");
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(c => c.GetName().Name == args.Name.Split(',')[0]);
        }

        private static string FixSourceCode(string sourceCode)
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                entityDefService.GetList(true).ForEach(c =>
                {
                    sourceCode = DomainUtility.ReplaceRegularValue($" {c}(", ")", $"{c}(this.UnitOfWork)", sourceCode, true);
                });
            }
            return sourceCode;
        }

        private static Assembly GetAssembly(string path)
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().LastOrDefault(c => path.Replace(".dll", "").EndsWith(c.FullName.Split(',')[0]));
            if (assembly != null)
                return assembly;

            return Assembly.Load(File.ReadAllBytes(path));
        }

        public static void GenerateProcessAssembly(sysBpmsProcess process)
        {
            CompileAssemblies(process.SourceCode, process.ID.ToString());
        }

        public static void GenerateAppPageAssembly(sysBpmsDynamicForm dynamicForm)
        {
            CompileAssemblies(dynamicForm.SourceCode, dynamicForm.ApplicationPageID.ToString());
        }

        public static string MakeClass(string classCode, string className)
        {
            return $"public class c_{className.Replace("-", "_")}: DynamicBusiness.BPMS.CodePanel.CodeBase{{" + Environment.NewLine
                        + $@"public override object ExecuteCode(){{
{classCode + Environment.NewLine + "return true;"}
}}" +
                        Environment.NewLine + "}"
                       + Environment.NewLine;
        }

        #endregion

    }
}
