using Microsoft.Test.Apex.VisualStudio.Solution;
using Xunit;

namespace NuGet.Tests.Apex
{
    public class NuGetUITestCase : SharedVisualStudioHostTestClass, IClassFixture<VisualStudioHostFixtureFactory>
    {
        public NuGetUITestCase(VisualStudioHostFixtureFactory visualStudioHostFixtureFactory) 
            : base(visualStudioHostFixtureFactory)
        {
        }

        [StaFact]
        public void SearchPackageFromUI()
        {
            // Arrange
            EnsureVisualStudioHost();
            var dte = VisualStudio.Dte;
            var solutionService = VisualStudio.Get<SolutionService>();

            solutionService.CreateEmptySolution();
            solutionService.AddProject(ProjectLanguage.CSharp, ProjectTemplate.ClassLibrary, ProjectTargetFramework.V46, "TestProject");

            dte.ExecuteCommand("Project.ManageNuGetPackages");
            var nugetTestService = GetNuGetTestService();
            nugetTestService.SeachPackgeFromUI("newtonsoft.json", "TestProject");
        }

        [StaFact]
        public void InstallPackageFromUI()
        {
            // Arrange
            EnsureVisualStudioHost();
            var dte = VisualStudio.Dte;
            var solutionService = VisualStudio.Get<SolutionService>();

            solutionService.CreateEmptySolution();
            solutionService.AddProject(ProjectLanguage.CSharp, ProjectTemplate.ClassLibrary, ProjectTargetFramework.V46, "TestProject");

            dte.ExecuteCommand("Project.ManageNuGetPackages");
            var nugetTestService = GetNuGetTestService();
            nugetTestService.InstallPackageFromUI("newtonsoft.json","9.0.1", "TestProject");
        }

        [StaFact]
        public void UninstallPackageFromUI()
        {
            // Arrange
            EnsureVisualStudioHost();
            var dte = VisualStudio.Dte;
            var solutionService = VisualStudio.Get<SolutionService>();

            solutionService.CreateEmptySolution();
            solutionService.AddProject(ProjectLanguage.CSharp, ProjectTemplate.ClassLibrary, ProjectTargetFramework.V46, "TestProject");

            dte.ExecuteCommand("Project.ManageNuGetPackages");
            var nugetTestService = GetNuGetTestService();
            nugetTestService.InstallPackageFromUI("newtonsoft.json", "9.0.1", "TestProject");
            nugetTestService.UninstallPackageFromUI("newtonsoft.json", "TestProject");
        }

        [StaFact]
        public void UpdatePackageFromUI()
        {
            // Arrange
            EnsureVisualStudioHost();
            var dte = VisualStudio.Dte;
            var solutionService = VisualStudio.Get<SolutionService>();

            solutionService.CreateEmptySolution();
            solutionService.AddProject(ProjectLanguage.CSharp, ProjectTemplate.ClassLibrary, ProjectTargetFramework.V46, "TestProject");

            dte.ExecuteCommand("Project.ManageNuGetPackages");
            var nugetTestService = GetNuGetTestService();
            nugetTestService.InstallPackageFromUI("newtonsoft.json", "9.0.1", "TestProject");
            nugetTestService.UpdatePackageFromUI("newtonsoft.json", "10.0.3", "TestProject");
        }
    }
}
