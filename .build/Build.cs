using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.PowerShell.PowerShellTasks;
using Nuke.Common.Tools.ReportGenerator;
using Nuke.Common.Tools.Xunit;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Nuke.Common.Tools.Xunit.XunitTasks;
using Nuke.Common.Git;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Utilities;
using JetBrains.Annotations;


//[GitHubActions("ci",
//    GitHubActionsImage.MacOsLatest,
//    AutoGenerate = true,
//    OnPushBranches = new[] { "main" },
//    OnPullRequestBranches = new[] { "main" },
//    InvokedTargets = new[] { nameof(Pipeline) })]
//[GitHubActions("ci",
//    GitHubActionsImage.WindowsLatest,
//    AutoGenerate = true,
//    OnPushBranches = new[] { "main" },
//    OnPullRequestBranches = new[] { "main" },
//    InvokedTargets = new[] { nameof(Pipeline) })]
[GitHubActions("ci",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    EnableGitHubToken = true,
    OnPushBranches = new[]
    {
        "main",
        "release/**"
    },
    OnPullRequestBranches = new[]
    {
        "main",
        "release/**"
    },
    InvokedTargets = new[] { nameof(Pipeline) })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    //[Parameter("Copyright Details")]
    readonly string Copyright = $"Copyright {DateTime.Now.Year} Truthshield, Inc";

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    static string Version = new Version(0, 2, 0).ToString();

    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";
    const string HotfixBranchPrefix = "hotfix";

    AbsolutePath ArtifactsDirectory   => RootDirectory / "Artifacts";
    AbsolutePath NugetDirectory       => RootDirectory / "NuGet";
    AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";

    static GitHubActions GitHubActions => GitHubActions.Instance;

    static readonly string PackageContentType = "application/octet-stream";
    static string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    string GithubNugetFeed => GitHubActions != null
         ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
         : null;

    [GitRepository]
    readonly GitRepository GitRepository;

    T From<T>()
        where T : INukeBuild
        => (T)(object)this;

    GitVersion GitVersion => From<IWithGitVersion>().Versioning;

    //Target ConfigureCert => _ => _
    //.Before(Compile)
    //.Executes(() =>
    //{
    //    //Process2.
    //    PowerShell("./configure-cert.ps1");
    //     //.se
    //     //.SetCommand()
    //     //.SetExecutionPolicy("Unrestricted"));
    //});

    Target Pipeline => _ => _
        .DependsOn(Test)
        .Executes();

    Target RestoreNuke => _ => _
        .OnlyWhenStatic(() => !IsLocalBuild)
        .Executes(() =>
        {
            DotNetTasks
                .DotNetToolUpdate(configuration =>
                    configuration
                        .SetPackageName("Nuke.GlobalTool")
                        .EnableGlobal());
                        //.SetArgumentConfigurator(args => args.Add("--version={0}", "0.25.0-alpha0377")));
        });

    //Target RestoreWyam => _ => _
    //    .Executes(() =>
    //    {
    //        DotNetTasks
    //            .DotNetToolUpdate(configuration =>
    //                configuration
    //                    .SetPackageName("Wyam.Tool")
    //                    .EnableGlobal());
    //                    //.SetArgumentConfigurator(args => args.Add("--version={0}", "2.2.9")));
    //    });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(TestResultsDirectory);
            EnsureCleanDirectory(NugetDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        //.DependsOn(RestoreWyam)
        .DependsOn(RestoreNuke)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .SetVerbosity(DotNetVerbosity.Quiet)
                .EnableNoCache());
                //.SetConfigFile(RootDirectory / "nuget.config"));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetVerbosity(DotNetVerbosity.Quiet)
                .SetConfiguration(Configuration)
                .EnableNoLogo()
                .EnableNoRestore());
                    //.SetAssemblyVersion(GitVersion.AssemblySemVer)
                    //.SetFileVersion(GitVersion.AssemblySemFileVer)
                    //.SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Triggers(PublishToGithub)
        .Executes(() =>
        {
            //Project[] projects = new[]
            //{
            //    Solution.Specs.FluentAssertions_Specs,
            //    Solution.Specs.FluentAssertions_Equivalency_Specs
            //};

            //if (EnvironmentInfo.IsWin)
            //{
            //    Xunit2(s =>
            //    {
            //        IEnumerable<string> testAssemblies = projects
            //            .SelectMany(project => GlobFiles(project.Directory, "bin/Debug/net47/*.Specs.dll"));

            //        Assert.NotEmpty(testAssemblies.ToList());

            //        return s
            //            .SetFramework("net47")
            //            .AddTargetAssemblies(testAssemblies);
            //    });
            //}

            Environment.SetEnvironmentVariable("CICD", "true");

            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetProjectFile(Solution)
                .EnableNoBuild()
                .SetVerbosity(DotNetVerbosity.Quiet)
                .SetDataCollector("XPlat Code Coverage")
                .SetResultsDirectory(TestResultsDirectory)
                .AddRunSetting(
                    "DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.DoesNotReturnAttribute",
                    "DoesNotReturnAttribute")
                //.CombineWith(
                //    projects,
                //    (_, project) => _
                //        .SetProjectFile(project)
                //        .CombineWith(
                //            project.GetTargetFrameworks().Except(new[] { "net47" }),
                //            (_, framework) => _.SetFramework(framework)
                //        )
                //)
            );
        });


    Target Pack => _ => _
      .DependsOn(Test)
      .Executes(() =>
      {
          DotNetPack(s => s
            .SetProject(Solution.GetProject("Applinate.Microservice.Foundation"))
            .SetConfiguration(Configuration)
            .EnableNoBuild()
            .EnableNoRestore()
            .SetVersion(Version)
            .SetAssemblyVersion(Version)
            .SetFileVersion(Version)
            .SetInformationalVersion(Version)
            //.SetCopyright(Copyright) // TODO: use copyright
            //.SetVersion(GitVersion.NuGetVersionV2) // TODO: use gitversion 
            //.SetAssemblyVersion(GitVersion.AssemblySemVer)
            //.SetInformationalVersion(GitVersion.InformationalVersion)
            //.SetFileVersion(GitVersion.AssemblySemFileVer)
            .SetPackageTags("microservice cqrs tdd bdd framework applinate productivity solid")
            .SetIncludeSymbols(false)
            .SetNoDependencies(true)
            .SetOutputDirectory(NugetDirectory)
            );
      });

    Target PublishToGithub => _ => _
       .Description($"Publishing to Github for Development only.")
       .DependsOn(Pack)
       .Requires(() => Configuration.Equals(Configuration.Release) && IsMaineBranch())
       .OnlyWhenStatic(() => Configuration.Equals(Configuration.Release) && IsMaineBranch())
       .Executes(() =>
       {
           GlobFiles(NugetDirectory, "*.nupkg")
               .Where(x => !x.EndsWith("symbols.nupkg"))
               .ForEach(x =>
               {
                   DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .EnableSkipDuplicate()
                       .SetSource(GithubNugetFeed)
                       .SetApiKey(GitHubActions.Token)
                       .EnableSkipDuplicate()
                   );

               });
       });

    public bool IsOnDevelopBranch() => 
        (GitRepository.Branch?.EqualsOrdinalIgnoreCase("dev") ?? false) ||
        (GitRepository.Branch?.EqualsOrdinalIgnoreCase("develop") ?? false) ||
        (GitRepository.Branch?.EqualsOrdinalIgnoreCase("development") ?? false);

    public bool IsReleaseBranch() =>
        GitRepository.Branch?.ContainsOrdinalIgnoreCase("release") ?? false;

    public bool IsMaineBranch() =>
        GitRepository.Branch?.EqualsOrdinalIgnoreCase("main") ?? false;

    //Target PublishToMyGet => _ => _
    //   .Description($"Publishing to MyGet for PreRelese only.")
    //   .Requires(() => Configuration.Equals(Configuration.Release))
    //   .Triggers(Pack)
    //   .OnlyWhenStatic(() => GitRepository.IsOnReleaseBranch())
    //   .Executes(() =>
    //   {
    //       GlobFiles(ArtifactsDirectory, ArtifactsType)
    //           .Where(x => !x.EndsWith(ExcludedArtifactsType))
    //           .ForEach(x =>
    //           {
    //               DotNetNuGetPush(s => s
    //                   .SetTargetPath(x)
    //                   .SetSource(MyGetNugetFeed)
    //                   .SetApiKey(MyGetApiKey)
    //                   .EnableSkipDuplicate()
    //               );
    //           });
    //   });

    //Target PublishToNuGet => _ => _
    //   .Description($"Publishing to NuGet with the version.")
    //   //.Triggers(CreateRelease)
    //   .Requires(() => Configuration.Equals(Configuration.Release))
    //   .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch())
    //   .Executes(() =>
    //   {
    //       GlobFiles(NugetDirectory, "*.nupkg")
    //           .Where(x => !x.EndsWith("symbols.nupkg"))
    //           .ForEach(x =>
    //           {
    //               DotNetNuGetPush(s => s
    //                   .SetTargetPath(x)
    //                   .EnableSkipDuplicate()
    //                   .SetSource(NugetFeed)
    //                   .SetApiKey(NuGetApiKey)
    //                   .EnableSkipDuplicate()
    //               );

    //           });
    //   });
}



[PublicAPI]
public interface IWithGitVersion : INukeBuild
{
    [GitVersion(Framework = "net7.0", NoFetch = true)]
    [Required]
    GitVersion Versioning => TryGetValue(() => Versioning);
}
