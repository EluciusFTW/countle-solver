module Countle

let add a b = Some(a + b)

let multiply a b = Some(a * b)

let divide a b =
    match b with
    | 0 -> None
    | _ -> 
        match (a % b) with
        | 0 -> Some(a / b)
        | _ -> None

let subtract a b =
    match (a - b) with
    | x when x >= 0 -> Some(a - b)
    | _ -> None

let operations =
    [ (add, '+')
      (multiply, '*')
      (subtract, '-')
      (divide, '/') ]

type row =
    { left: int
      right: int
      operation: char
      result: Option<int> }

type intermediate = { values: int list; rows: row list }

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
            for col in row + 1 .. length - 1 -> [row; col]
    } 
    |> Seq.toList
    |> List.map (fun pair -> [values[pair[0]]; values[pair[1]]]@(exceptPositions values pair)) 
    
let combineFirstTwoBy state operation =
    let result = (fst operation) state.values[1] state.values[0]
    { values =
        match result with
        | Some value -> [ value ] @ state.values[2..]
        | None -> []
      rows =
        state.rows
        @ [ { left = state.values[1]
              right = state.values[0]
              operation = (snd operation)
              result = result } ] }

let combineFirstTwo state =
    match state.values with
    | [] -> []
    | [ _ ] -> [ state ]
    | _ -> operations |> List.map (combineFirstTwoBy state)

let getNextRow state =
    state.values 
    |> List.sort
    |> pickFromOrdered
    |> List.map (fun permuted -> { values = permuted; rows = state.rows })
    |> List.collect combineFirstTwo

let rec getRows state target =
    match state.values with
    | [] -> []
    | [ v ] -> [ state ]
    | _ ->
        getNextRow state
        |> List.collect (fun s ->
            match List.contains target s.values with
            | true -> [ s ]
            | false -> getRows s target)
        |> List.filter (fun s -> s.values.Length > 0)

let getSolutions values target =
    getRows { values = values; rows = [] } target
    |> List.filter (fun state -> 
        match (List.tryLast state.rows) with 
        | Some row -> 
            match row.result with
            | Some result -> result = target
            | _ -> false
        | _ -> false)
    |> List.sortBy (fun state -> state.rows.Length)
    |> List.map (fun state -> state.rows)
    |> List.distinct
