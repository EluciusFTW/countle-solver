namespace Commands

module Solve =
    open Spectre.Console.Cli
    open Countle
    open Output

    let printList values =
        values |> Seq.iter (fun value -> printf "%i " value)
        printfn ""
    
    let printIntermediates (rows: row list) = 
        printfn "Solution:"
        rows
        |> List.iter (fun row -> 
            match row.result with
            | Some value -> printfn $"{row.left} {row.operation} {row.right} = {value}"
            | None -> failwith "Expected finite result, but there was none.")
        printfn ""

    type SolveSettings() =
        inherit CommandSettings()

        [<CommandOption("-n|--numbers")>]
        member val numbers = "" with get, set

        [<CommandOption("-t|--target")>]
        member val target = 0 with get, set

        [<CommandOption("-m|--maxSolutions")>]
        member val maxSolutions = 5 with get, set
    
    type Solve() =
        inherit Command<SolveSettings>()
        interface ICommandLimiter<SolveSettings>

        override _.Execute(_context, settings) = 

            let values = 
                settings.numbers.Split(',') 
                |> Array.map int 
                |> Array.toList 

            printfn "Input: %A" values
            printfn "Target: %i" settings.target
            printfn "Limit to solutions: %i" settings.maxSolutions

            printfn "Calculating solutions ... "
            getSolutions values settings.target
                |> List.truncate settings.maxSolutions
                |> List.iter printIntermediates
            0