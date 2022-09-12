module Countle

let add a b = Some(a + b)

let multiply a b = Some(a * b)

let divide a b =
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

let rec permutations values =
    match values with
    | [] -> []
    | [ _ ] -> [ values ]
    | _ ->
        values
        |> List.collect (fun value ->
            (permutations (values |> List.except [ value ]))
            |> List.map (fun l -> [ value ] @ l))

let combineFirstTwoBy state operation =
    let result = (fst operation) state.values[0] state.values[1]
    { values =
        match result with
        | Some value -> [ value ] @ state.values[2..]
        | None -> []
      rows =
        state.rows
        @ [ { left = state.values[0]
              right = state.values[1]
              operation = (snd operation)
              result = result } ] }

let combineFirstTwo state =
    match state.values with
    | [] -> []
    | [ _ ] -> [ state ]
    | _ -> operations |> List.map (combineFirstTwoBy state)

let getNextRow state =
    permutations state.values
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
    |> List.map (fun state-> state.rows)
    |> List.distinct
