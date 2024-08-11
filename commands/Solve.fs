namespace Countle.Commands

open Spectre.Console.Cli
open SpectreCoff
open Countle.Domain

module Print =
    let rowOutput row result = 
        Many [
            Calm $"{row.left}"
            Pumped $"{row.operation}"
            Calm $"{row.right}"
            Calm "="
            Edgy $"{result}"
        ]

    let printRows (rows: row list) = 

        alignedRule Left "Solution" |> toConsole
        rows
        |> List.map (fun row -> 
            match row.result with
            | Some value -> rowOutput row value
            | None -> E "Expected finite result, but there was none.")
        |> Many 
        |> toConsole

type SolveSettings() =
    inherit CommandSettings()

    [<CommandOption("-n|--numbers")>]
    member val numbers = "" with get, set

    [<CommandOption("-t|--target")>]
    member val target = 0 with get, set

    [<CommandOption("-m|--maxSolutions")>]
    member val maxSolutions = 5 with get, set

    [<CommandOption("-e|--exactlyIn")>]
    member val steps = 0 with get, set

type Solve() =
    inherit Command<SolveSettings>()
    interface ICommandLimiter<SolveSettings>

    override _.Execute(_context, settings) = 
        let ofLenght (rows: row list) =
            match settings.steps with
            | 0 -> true
            | i -> rows.Length = i

        let values = 
            settings.numbers.Split(',') 
            |> Array.map int 
            |> Array.toList
        
        getSolutions values settings.target
            |> List.filter ofLenght 
            |> List.truncate settings.maxSolutions
            |> List.iter Print.printRows
        0