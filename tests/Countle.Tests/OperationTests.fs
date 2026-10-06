module Countle.Tests.OperationProperties

open FsCheck
open FsCheck.FSharp
open FsCheck.Xunit
open Countle.Domain

[<Property>]
let ``add is commutative`` (a: int) (b: int) = add a b = add b a

[<Property>]
let ``subtract never yields a negative number`` (a: int) (b: int) =
    match subtract a b with
    | Some r -> r >= 0 && r = a - b
    | None -> a < b

[<Property>]
let ``divide only succeeds when the division is exact`` (a: int) (b: int) =
    let holds =
        match divide a b with
        | Some q -> b <> 0 && q * b = a
        | None -> b = 0 || a % b <> 0

    holds
    |> Prop.classify (b = 0) "division by zero"
    |> Prop.classify (b <> 0 && a % b = 0) "exact"
    |> Prop.classify (b <> 0 && a % b <> 0) "with remainder"
