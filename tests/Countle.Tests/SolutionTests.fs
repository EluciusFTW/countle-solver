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
        |> List.forall (fun rows ->
            let stepsValid =
                rows
                |> List.forall (fun row -> row.result = apply row.operation row.left row.right)

            let endsInTarget = (List.last rows).result = Some target
            stepsValid && endsInTarget)
        |> Prop.trivial (List.isEmpty solutions)) //  |> Prop.collect $"{values.Length} numbers")

[<Property>]
let ``every puzzle has a solution`` () =
    Prop.forAll smallPuzzle (fun (values, target) -> getSolutions values target |> List.isEmpty |> not)

// --- Solvable puzzles ----------------------------------------------------------

// A puzzle that is solvable by construction: repeatedly combine two of the numbers with a
// randomly chosen valid operation until a single number is left, and use that as the target.
// The steps are kept so that a failing case shows how its target was built.
type SolvablePuzzle =
    {
        numbers: int list
        target: int
        steps: string list
    }

let solvablePuzzle count =
    let rec combine pool steps =
        gen {
            match pool with
            | [ result ] -> return result, List.rev steps
            | _ ->
                let! i = Gen.choose (0, pool.Length - 1)

                let! j =
                    Gen.choose (0, pool.Length - 2)
                    |> Gen.map (fun j -> if j >= i then j + 1 else j)

                let larger, smaller = max pool[i] pool[j], min pool[i] pool[j]

                let! f, symbol =
                    operations
                    |> List.filter (fun (f, _) -> (f larger smaller).IsSome)
                    |> Gen.elements

                let result = (f larger smaller).Value

                let rest =
                    pool
                    |> List.indexed
                    |> List.filter (fun (k, _) -> k <> i && k <> j)
                    |> List.map snd

                return! combine (result :: rest) ($"{larger} {symbol} {smaller} = {result}" :: steps)
        }

    gen {
        let! numbers = Gen.listOfLength count (Gen.choose (1, 25))
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
let ``any combination of two to four numbers is always found`` () =
    let puzzles = Gen.choose (2, 4) |> Gen.bind solvablePuzzle |> Arb.fromGen

    Prop.forAll puzzles (fun puzzle -> hasSolution puzzle |> Prop.collect $"{puzzle.numbers.Length} numbers")
