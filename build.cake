// The following environment variables need to be set for Publish target:
// PACK1862_NETLIFY_TOKEN

#tool nuget:?package=Wyam&version=1.5.0
#addin nuget:?package=Cake.Wyam&version=1.5.0
#addin "NetlifySharp"
            
using NetlifySharp;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Does(() =>
    {
        Wyam();        
    });
    
Task("Preview")
    .Does(() =>
    {
        Wyam(new WyamSettings
        {
            Preview = true,
            Watch = true
        });        
    });

Task("Debug")
    .Does(() =>
    {
        StartProcess("../Wyam/src/clients/Wyam/bin/Debug/net462/wyam.exe",
            "-a \"../Wyam/tests/integration/Wyam.Examples.Tests/bin/Debug/net462/**/*.dll\" -p");
    });

Task("Deploy")
    .Does(() =>
    {
        var netlifyToken = EnvironmentVariable("PACK1862_NETLIFY_TOKEN");
        if(string.IsNullOrEmpty(netlifyToken))
        {
            throw new Exception("Could not get Netlify token environment variable");
        }

        Wyam();  

        Information("Deploying output to Netlify");
        var client = new NetlifyClient(netlifyToken);
        //client.UpdateSite("pack1862.netlify.com", MakeAbsolute(Directory("./output"))).SendAsync().Wait();
    });
    
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Preview");    
    
Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Deploy");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);