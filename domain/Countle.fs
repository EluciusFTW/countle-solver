module Countle

    let rec expand values results = 
        match values with
        | [] -> results
        | [v] -> results
        | first :: rest -> 
            let newResults = rest |> List.map (fun v -> v + first )
            expand rest results@newResults

    let rec all (values: list<list<int>>) = 
        let len = values |> List.minBy (fun v -> v.Length)

        match len.Length with
        | 0 -> values
        | 1 -> values
        | 2 -> values
        | _ -> all values 
