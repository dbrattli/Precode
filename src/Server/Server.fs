module Server.Main

open System
open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging

open FSharp.Control.Tasks.V2.ContextInsensitive

open Giraffe
open Giraffe.Serialization

open Reaction.Giraffe.Middleware
open Reaction
open Reaction.AsyncObservable
open NAisParser

open Shared
open Server.Ais

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let webApp =
    // Not used, ignore
    route "/ping" >=> text "pong"

let query (connectionId: ConnectionId) (_: IAsyncObservable<Msg*ConnectionId>) : IAsyncObservable<Msg*ConnectionId> =
    // TODO: Replace with the AIS observable (ofAis) in Ais.fs
    AsyncObservable.empty ()

    // TODO: Map AIS messages of type MessageType123 to Msg. PS: You can use ais2Msg in Ais.fs

    // TODO: Need to do GEO filtering here. PS: you can do this later

    // TODO: Uncomment line below once the AIS stream is in place to map in the connectionId.
    // |> map (fun msg -> (msg, connectionId))

let configureApp (app : IApplicationBuilder) =
    app.UseWebSockets()
       .UseReaction<Msg>(fun options ->
       { options with
           Query = query
           Encode = Msg.Encode
           Decode = Msg.Decode
       })
       .UseDefaultFiles()
       .UseStaticFiles()
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore
    let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
    fableJsonSettings.Converters.Add(Fable.JsonConverter())
    services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings) |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.SetMinimumLevel(LogLevel.Debug)
    |> ignore

WebHost
    .CreateDefaultBuilder()
    .UseWebRoot(publicPath)
    .UseContentRoot(publicPath)
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
    .ConfigureLogging(configureLogging)
    .Build()
    .Run()