open Spectre.Console.Cli
open Countle.Commands

[<EntryPoint>]
let main argv =

    let app = CommandApp()
    app.Configure(fun config ->
        config.AddCommand<Solve>("solve")
            .WithAlias("s")
            |> ignore)

    app.Run(argv)