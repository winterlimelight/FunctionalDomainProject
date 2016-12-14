module AmApi.Railway

// Combination of http://fsharpforfunandprofit.com/posts/recipe-part2/ and http://blog.ploeh.dk/2016/03/21/composition-with-an-either-computation-expression/
// An implementation may exist in Chessie, but I'm getting package manager errors for that, and at this stage I think there is benefits to understanding the 
// implementation myself, so here it is.
type Result<'TSuccess,'TFailure> = 
    | Success of 'TSuccess
    | Failure of 'TFailure

type RailwayBuilder () =
    member this.Bind(x, f) = 
        match x with
        | Success s -> f s
        | Failure f -> Failure f

    member this.Return x = Success x
    member this.ReturnFrom x = x

let railway = new RailwayBuilder()

let OptionToResult opt defaultFailure =
    match opt with
    | Some s -> Success(s)
    | None -> Failure(defaultFailure)