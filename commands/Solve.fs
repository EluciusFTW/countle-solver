namespace Commands

module Solve =
    open Spectre.Console.Cli
    open Countle
    open Output

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

            let results = expand [1;2;3] []
            results |> List.iter(fun r -> printMarkedUp r)
            0