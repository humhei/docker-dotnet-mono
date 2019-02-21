#if FAKE
#r "paket: groupref build //"
#endif
#if !FAKE
#r "Facades/netstandard"
#r "netstandard"
#endif
#load "./.fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.Core.TargetOperators
open FPublisher.FakeHelper.CommandHelper

let dockerTool = platformTool "docker"

let docker = exec dockerTool "./"

let dockerUser = Environment.environVarOrFail "docker_user"
let dockerPassword = Environment.environVarOrFail "docker_password"

let repo = "humhei/docker-dotnet-mono"
let repoWithTag = repo + ":lastest"

TraceSecrets.register "<docker_user>" dockerUser
TraceSecrets.register "<docker_password>" dockerPassword

Target.create "Login" (fun _ ->
     docker 
          [ yield "login"
            yield sprintf "--username=%s" dockerUser
            yield sprintf "--password=%s" dockerPassword ]
)

Target.create "Build" (fun _ ->
     docker 
          [ yield "build"
            yield "-t"
            yield repoWithTag ]
)

Target.create "Push" (fun _ ->
     docker 
          [ yield "push"
            yield repoWithTag ]
)

"Login" 
     ==> "Build"
     ==> "Push"

Target.runOrDefault "Push"