namespace Commands

module Solve =
    open Spectre.Console.Cli
    open Countle
    open Output

    let printList values =
        values |> Seq.iter (fun value -> printf "%i " value)
        printfn ""

    type SolveSettings() =
        inherit CommandSettings()

        // [<CommandOption("-n|--name")>]
        // member val name = "friend" with get, set

        // override _.Validate() =
        //     match self.name.Length with
        //     | 1 -> Spectre.Console.ValidationResult.Error($"That's an awfully short name, I don't buy it.")
        //     | _ -> Spectre.Console.ValidationResult.Success()
    
    type Solve() =
        inherit Command<SolveSettings>()
        interface ICommandLimiter<SolveSettings>

        override _.Execute(_context, settings) = 
            printMarkedUp $"Solving ..."

            let input = [200; 3; 11; 15; 7]
            printf "Input: "
            printList input

            // printf "Add first: "
            // printList (addFirst input)

            // printfn "Operate first: "
            // (operateFirst input) |> List.iter printList

            // printf "Condense One: "
            // condenseOne input |> List.iter printList

            printf "Condense Fully: "
            let results = condenseDistinct input
            printfn "Input condenses to %i numbers." results.Length
            results |> List.iter printList

            // let results = permutations input 
            // printfn "Found %i permutations." results.Length
            // results |> List.iter printList
            0