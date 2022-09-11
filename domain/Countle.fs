module Countle

    let add a b = a + b
    let multiply a b = a * b

    let operations = [ add; multiply]
    let rec permutations (values: int list) = 
        match values with
        | [] -> []
        | [v] -> [values]
        | _ -> values 
            |> List.collect (fun v -> 
                (permutations (values |> List.except [v])) 
                |> List.map (fun l -> [v]@l)) 
    
    let addFirst (values: int list) = 
        match values with
        | [] -> []
        | [v] -> values
        | _ -> [values[0] + values[1]]@values[2..]
    
    let operateFirst (values: int list) = 
        match values with
        | [] -> []
        | [v] -> [values]
        | _ -> operations |> List.map (fun o -> [o values[0] values[1]]@values[2..])

    let condenseOne values =
        permutations values |> List.collect operateFirst
    
    let rec condense values =
        match values with
        | [] -> []
        | [v] -> [values]
        | _ -> condenseOne values |> List.collect condense

    let condenseDistinct values = 
        condense values |> List.distinct
