module Countle

    let add a b = 
        Some(a + b)

    let multiply a b = 
        Some(a * b)

    let divide a b =
        match (a % b) with
        | 0 -> Some (a/b)
        | _ -> None

    let subtract a b =
        match (a - b) with
        | x when x >= 0 -> Some (a - b)
        | _ -> None

    let operations = [ 
        add
        multiply
        subtract
        divide
    ]

    type row = 
     { left: int; right: int; operation: string }
     
    type intermediate =
        { values: int list;  }

    let rec permutations values = 
        match values with
        | [] -> []
        | [_] -> [values]
        | _ -> values 
            |> List.collect (fun value -> 
                (permutations (values |> List.except [value])) 
                |> List.map (fun l -> [value]@l)) 
    
    let operate (values: int list) = 
        match values with
        | [] -> []
        | [_] -> [values]
        | _ -> operations 
            |> List.map (fun o -> 
                match (o values[0] values[1]) with 
                | Some x -> [x]@values[2..]
                | _ -> [])

    let condenseOne values =
        permutations values |> List.collect operate
    
    let rec condense values =
        match values with
        | [] ->  []
        | [v] -> [values]
        | _ -> condenseOne values |> List.collect condense

    let condenseDistinct values = 
        condense values 
        |> List.distinct 
        |> List.sort
