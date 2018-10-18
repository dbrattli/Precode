module Client

open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.Reaction
open Fable.Reaction.WebSocket
open Reaction
open Shared

open Fulma
open ReactLeaflet

// The model holds data that you want to keep track of while the application is running
type Ship = {
    Latitude: float
    Longitude: float
    Speed: float
    Heading: float
}

type Mmsi = int
type Model = { Ships: Map<Mmsi, Ship> }

// defines the initial state and initial command (= side-effect) of the application
let init () : Model =
    let initialModel = { Ships = Map.empty }

    initialModel

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model =

    // TODO: implement the new model based on the received message here

    currentModel

let safeComponents =
    let components =
        span [ ]
           [
             a [ Href "https://github.com/giraffe-fsharp/Giraffe" ] [ str "Giraffe" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io/elmish/" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://mangelmaxime.github.io/Fulma" ] [ str "Fulma" ]
           ]

    p [ ]
        [ strong [] [ str "SAFE Template" ]
          str " powered by: "
          components ]

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let view model dispatch =
    div [] [
        Navbar.navbar [ Navbar.Color IsPrimary ] [
            Navbar.Item.div [] [
                Heading.h2 [] [
                    str "SAFE Template"
                ]
            ]
        ]

        // TODO: the map needs to draw the ships from the model in the map.

        // TODO: Add hover on each ship to display speed and heading

        ReactLeaflet.map [
            MapProps.Center !^ (69.65, 18.57)
            MapProps.SetView true
            MapProps.Zoom (float 10)
            MapProps.ZoomSnap 0.1
            MapProps.Id "myMap"
            MapProps.Style [ CSSProp.Height "500px" ]
        ] [
            tileLayer [
                TileLayerProps.Attribution "&copy; <a href=\"https://www.openstreetmap.org/copyright\">OpenStreetMap</a> contributors"
                TileLayerProps.Url "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            ] []
          ]

        Footer.footer [] [
            Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [
                safeComponents
            ]
        ]
    ]

// Simple debug operator that prints messages to the console.
let Debug =
    AsyncObservable.map (fun msg ->
        printfn "%A" msg
        msg)

let query (msgs: IAsyncObservable<Msg>) =
    msgs
    |> msgChannel "ws://localhost:8085/ws" Msg.Encode Msg.Decode
    |> Debug

    // TODO: add operators her to calculate speed/heading per ship

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkSimple init update view
|> Program.withQuery query
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
