module Countle.Tests.DomainProperties

open FsCheck
open FsCheck.FSharp
open FsCheck.Xunit
open Countle.Domain

// --- Arithmetic operations ---------------------------------------------------

[<Property>]
let ``add is commutative`` (a: int) (b: int) =
    add a b = add b a

[<Property>]
let ``subtract never yields a negative number`` (a: int) (b: int) =
    match subtract a b with
    | Some r -> r >= 0 && r = a - b
    | None -> a < b

[<Property>]
let ``divide only succeeds when the division is exact`` (a: int) (b: int) =
    match divide a b with
    | Some q -> b <> 0 && q * b = a
    | None -> b = 0 || a % b <> 0

// --- Solver --------------------------------------------------------------------

// The search space grows very fast, so keep puzzles small: 2-4 numbers in 1..25.
let smallPuzzle =
    gen {
        let! count = Gen.choose (2, 4)
        let! values = Gen.listOfLength count (Gen.choose (1, 25))
        let! target = Gen.choose (1, 200)
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
        getSolutions values target
        |> List.forall (fun rows ->
            let stepsValid =
                rows |> List.forall (fun row -> row.result = apply row.operation row.left row.right)

            let endsInTarget = (List.last rows).result = Some target
            stepsValid && endsInTarget))

[<Property>]
let ``the sum of two numbers is always found`` (PositiveInt a) (PositiveInt b) =
    (a + b > 0) ==> lazy (getSolutions [ a; b ] (a + b) |> List.isEmpty |> not)
