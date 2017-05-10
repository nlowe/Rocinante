var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");

    if(DirectoryExists("./dist"))
    {
        DeleteDirectory("./dist", true);
    }
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild("./Rocinante.sln");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach(var proj in GetFiles("./test/**/*.csproj"))
    {
        Information("Testing Project: " + proj);
        DotNetCoreTest(proj.FullPath);
    }
});

Task("Dist")
    .IsDependentOn("Test")
    .Does(() => 
{
    DotNetCorePublish("./src/Rocinante/Rocinante.csproj", new DotNetCorePublishSettings {
        OutputDirectory = "./dist"
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
