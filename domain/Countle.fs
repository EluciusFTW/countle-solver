module Countle.Domain

let add a b = Some(a + b)

let multiply a b = Some(a * b)

let divide a b =
    match b with
    | 0 -> None
    | _ ->
        match a % b with
        | 0 -> Some(a / b)
        | _ -> None

let subtract a b =
    match a - b with
    | x when x >= 0 -> Some(a - b)
    | _ -> None

let operations = [ (add, '+'); (multiply, '*'); (subtract, '-'); (divide, '/') ]

type row =
    {
        left: int
        right: int
        operation: char
        result: int
    }

type intermediate = { values: int list; rows: row list }

let initialState values = { values = values; rows = [] }

let exceptPositions values positions =
    values
    |> Seq.mapi (fun index value ->
        match positions |> List.contains index with
        | true -> -1
        | false -> value)
    |> Seq.filter (fun value -> value > 0)
    |> Seq.toList

let pickFromOrdered (values: int list) =
    let length = values.Length

    seq {
        for row in 0 .. length - 2 do
            for col in row + 1 .. length - 1 -> [ row; col ]
    }
    |> Seq.toList
    |> List.map (fun pair -> [ values[pair[0]]; values[pair[1]] ] @ (exceptPositions values pair))

let combineFirstTwoBy state operation =
    fst operation state.values[1] state.values[0]
    |> Option.map (fun newValue ->
        {
            values = [ newValue ] @ state.values[2..]
            rows =
                state.rows
                @ [
                    {
                        left = state.values[1]
                        right = state.values[0]
                        operation = snd operation
                        result = newValue
                    }
                ]
        })

let combineFirstTwo state =
    match state.values with
    | [] -> []
    | [ _ ] -> [ Some state ]
    | _ -> operations |> List.map (combineFirstTwoBy state)

let getNextRow state =
    state.values
    |> List.sort
    |> pickFromOrdered
    |> List.map (fun permuted -> { state with values = permuted })
    |> List.collect combineFirstTwo

let rec getStates (state: intermediate) target =
    match state.values with
    | [] -> []
    | [ v ] -> [ state ]
    | _ ->
        getNextRow state
        |> List.choose id
        |> List.collect (fun s ->
            match List.contains target s.values with
            | true -> [ s ]
            | false -> getStates s target)

let getSolutions values target =
    match List.contains target values with
    | true -> [ [] ]
    | false ->
        getStates (initialState values) target
        |> List.filter (fun state ->
            match List.tryLast state.rows with
            | Some row -> row.result = target
            | _ -> false)
        |> List.sortBy (fun state -> state.rows.Length)
        |> List.map (fun state -> state.rows)
        |> List.distinct
