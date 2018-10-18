namespace Shared

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg = {
    Mmsi: int;
    Time: string;
    Latitude: float;
    Longitude: float;
}

type Msg with
    static member Encode (msg: Msg) : string =
        let encoder = Encode.object [
              "mmsi", Encode.int msg.Mmsi
              "time", Encode.string msg.Time
              "lat", Encode.float msg.Latitude
              "lon", Encode.float msg.Longitude
            ]

        Encode.toString 4 encoder

    static member Decode (json: string) : Msg option =
        let decoder = Decode.object (fun get ->
                {
                  Mmsi = get.Required.Field "mmsi" Decode.int
                  Time = get.Required.Field "time" Decode.string
                  Latitude = get.Required.Field "lat" Decode.float
                  Longitude = get.Required.Field "lon" Decode.float
                }
            )

        let result = Decode.fromString decoder json
        match result with
        | Ok msg -> Some msg
        | Error _ -> None
