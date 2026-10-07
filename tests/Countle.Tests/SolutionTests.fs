module Countle.Tests.SolutionProperties

open FsCheck
open FsCheck.FSharp
open FsCheck.Xunit
open Countle.Domain

let smallPuzzle =
    gen {
        let! count = Gen.choose (2, 5)
        let! values = Gen.listOfLength count (Gen.choose (1, 50))
        let! target = Gen.choose (1000, 2000)
        return values, target
    }
    |> Arb.fromGen

let private apply op left right =
    operations
    |> List.find (fun (_, symbol) -> symbol = op)
    |> fun (f, _) -> f left right

[<Property>]
let ``every reported solution ends in the target and each step is valid arithmetic`` () =
    Prop.forAll smallPuzzle (fun (values, target) ->
        let solutions = getSolutions values target

        solutions
        |> List.forall (fun states ->
            let allStatesValid =
                states
                |> List.forall (fun state ->
                    match apply state.operation state.left state.right with
                    | Some rowResult -> rowResult = state.result
                    | _ -> false)

            let endsInTarget =
                match List.tryLast states with
                | Some row -> row.result = target
                | None -> List.contains target values

            allStatesValid && endsInTarget)
        |> Prop.trivial (List.isEmpty solutions)
        |> Prop.collect $"{values.Length} numbers")

// --- Solvable puzzles ----------------------------------------------------------

// A puzzle that is solvable by construction: repeatedly combine two of the numbers with a
// randomly chosen valid operation until a single number is left, and use that as the target.
type SolvablePuzzle =
    {
        numbers: int list
        target: int
        steps: string list
    }

let twoDistinctRandomIndices length =
    gen {
        let! i = Gen.choose (0, length - 1)
        let! j = Gen.choose (0, length - 2)
        return i, (if j >= i then j + 1 else j)
    }

let getValuesDescending i j (values: int list) =
    max values[i] values[j], min values[i] values[j]

let solvablePuzzle count =
    let rec combine availableNumbers steps =
        gen {
            match availableNumbers with
            | [ value ] -> return value, List.rev steps
            | _ ->
                let! i, j = twoDistinctRandomIndices availableNumbers.Length
                let larger, smaller = getValuesDescending i j availableNumbers

                let! operation, symbol =
                    operations
                    |> List.filter (fun (op, _) -> (op larger smaller).IsSome)
                    |> Gen.elements

                let result = (operation larger smaller).Value

                let rest =
                    availableNumbers
                    |> List.indexed
                    |> List.filter (fun (k, _) -> k <> i && k <> j)
                    |> List.map snd

                return! combine (result :: rest) ($"{larger} {symbol} {smaller} = {result}" :: steps)
        }

    gen {
        let! numbers = Gen.listOfLength count (Gen.choose (1, 100))
        let! target, steps = combine numbers []

        return
            {
                numbers = numbers
                target = target
                steps = steps
            }
    }

let private hasSolution puzzle =
    getSolutions puzzle.numbers puzzle.target |> List.isEmpty |> not

[<Property>]
let ``any operation on two numbers is always found`` () =
    Prop.forAll (solvablePuzzle 2 |> Arb.fromGen) hasSolution

[<Property>]
let ``any operation on three numbers is always found`` () =
    Prop.forAll (solvablePuzzle 3 |> Arb.fromGen) hasSolution

[<Property>]
let ``any operation on four numbers is always found`` () =
    Prop.forAll (solvablePuzzle 4 |> Arb.fromGen) hasSolution

[<Property>]
let ``any operation on five numbers is always found`` () =
    Prop.forAll (solvablePuzzle 5 |> Arb.fromGen) hasSolution
