open Spectre.Console.Cli
open Commands

[<EntryPoint>]
let main argv =

    let app = CommandApp()
    app.Configure(fun config ->
        config.AddCommand<Solve.Solve>("solve")
            .WithAlias("s")
            |> ignore)

    app.Run(argv)