// <copyright file="FSharpIntro.fsx" company="Engie University">
//  Copyright (c) Engie, all rights reserved.
// </copyright>

open System
open System.IO

(*
-------------------------------------------------------------------------
F# syntax basics
-------------------------------------------------------------------------
*)

// The 'let' keyword maps a name to a value.
let c = 'a'
let s = "this is a string"
printfn "Length of the string: %i" s.Length // printf is statically type checked

// Primitive types, and type inference
let v1 : float = 1.0    // type 'float' in F# means 'System.Double' !
let v2 = 1.0            // val v2:float

let v3 = 1.0f           // val v3:float32 (equiv. of 'System.Single')

let b1 = true           // val b1:bool
// b1 <- false          // error FS0027: This value is not mutable

let nameTuple = ("James", "Bond")
fst nameTuple |> printfn "First name: %s"   // James
snd nameTuple |> printfn "Last name: %s"    // Bond

let thd (_, _, c) = c
("James", "Bond", 7) |> (thd >> printfn "Agent: %03i")    // 007

(*
-------------------------------------------------------------------------
F# is an eager functional language (whereas Haskell is lazy)
-------------------------------------------------------------------------
*)

let test b t f = if b then t else f

// Eager evaluation
test true (printf "true") (printf "false")  // "truefalse"

// Lazy evaluation
let f = test true (lazy (printf "true")) (lazy (printf "false"))
f.Force()                                   // "true"

(*
-------------------------------------------------------------------------
Boolean operators
-------------------------------------------------------------------------
*)

let And = true && false
let Or = true || false
let Not = not true

(*
-------------------------------------------------------------------------
Arithmetic
-------------------------------------------------------------------------
*)

// From Microsoft.FSharp.Core.Operators : redefines common operators for arithmetic overflow checks
open Checked

1.0 + 2.0  |> printfn "%A"  // 3.0
3.0 - 1.0  |> printfn "%A"  // 2.0
2.0 * 3.0  |> printfn "%A"  // 6.0
1.0 / 2.0  |> printfn "%A"  // 0.5
2.0 ** 3.0 |> printfn "%A"  // 8.0
7 % 3      |> printfn "%A"  // 1

(*
-------------------------------------------------------------------------
Common math functions
-------------------------------------------------------------------------
*)

abs -1              |> printfn "%A" // 1
sign -5             |> printfn "%A" // -1
ceil 9.1            |> printfn "%A" // 10.0
floor 9.9           |> printfn "%A" // 9.0

sqrt 4.0            |> printfn "%A" // 2.0
exp 1.0             |> printfn "%A" // 2.718281828
1.0 |> (exp >> log) |> printfn "%A" // 1.0
log10 10.0          |> printfn "%A" // 1.0
pown 2L 10          |> printfn "%A" // 1024L

let pi = Math.PI
pi / 2.0 |> sin     |> printfn "%A" // 1.0
cos pi              |> printfn "%A" // -1.0
tan pi              |> printfn "%A" // 0.0

(*
-------------------------------------------------------------------------
F# functions
-------------------------------------------------------------------------
*)

let inline add n x = x + n
let add1 = add 1    // partial application
printfn "Adding 1 to 2 should be 3 : %b" ((add1 2) = 3) // true

//normal version
let printTwoParameters x y = 
    printfn "x=%i y=%i" x y

//explicitly curried version
let curriedPrintTwoParameters x  =    // only one parameter!
    let subFunction y = 
        printfn "x=%i y=%i" x y  // new function with one param
    subFunction 

// Function composition
let inline times n x = x * n
let add1Times2 = add1 >> times 2
let add5Times3 = add 5 >> times 3

printfn "result is %d" (add5Times3 1) // 18

// Add 5, then multiply by 3
let add5ThenMultiplyBy3 = (+) 5 >> (*) 3

add5ThenMultiplyBy3 1 |> printfn "result is %d" // 18

// Recursive functions
let rec factorial i =       // val factorial : uint32 -> uint64
    match i with
    | 0u | 1u -> 1UL
    | _  -> factorial (i-1u) * (uint64 i)

10u |> factorial |> printfn "10! = %d" // 3628800 (uint64)

let rec fib i =
    match i with
    | 0 | 1 -> i
    | n when n > 1 -> fib(n-2) + fib(n-1) // use of 'when' guards
    | n when n < 0 -> fib(n+2) - fib(n+1)

fib 10  |> printfn "fib(10) = %d"  // 55
fib -10 |> printfn "fib(-10) = %d" // -55

(*
-------------------------------------------------------------------------
Modules
-------------------------------------------------------------------------
*)

module Arithmetic = 

    let inline add x y = x + y
    let inline mult x y = x * y


module MathCalculus = 

    let add1 x = x |> Arithmetic.add 1 // or "add 1 x"

printfn "1 + 1 = %d" <| MathCalculus.add1 1

(*
-------------------------------------------------------------------------
Combinators and Operators
-------------------------------------------------------------------------
*)

let (|>) x f = f x             // forward pipe
let (<|) f x = f x             // reverse pipe
let (>>) f g x = g (f x)       // forward composition
let (<<) g f x = g (f x)       // reverse composition

/// Combines two paths
let (@@) path1 path2 = Path.Combine(path1, path2)
let myFile = @"D:\trainings\FSharp" @@ "FSharp4Quants.fsx"

(*
-------------------------------------------------------------------------
Algebraic types and TDD
-------------------------------------------------------------------------
*)

module Ast =

    /// Defines tokens
    type Token =
    //Comparaison tokens
    | Eql | Neq | Lsq | Lst | Grq | Grt
    //Function token
    | Fun of string
    
    /// Any term of constant or variable value
    and Expression =
    | Constant      of float
    | Add           of left:Expression * right:Expression
    | Subtract      of left:Expression * right:Expression
    | Multiply      of left:Expression * right:Expression
    | Divide        of left:Expression * right:Expression
    | Negative      of expr:Expression
    | Compare       of cmpr:Token * left:Expression * right:Expression
    | FunCall       of funName:Token * args:Expression list

(*
-------------------------------------------------------------------------
Collections
-------------------------------------------------------------------------
*)

// List comprehension
let multipleOf x = [ for i in 1 .. 10 do yield x * i]
let multipleOf5 = multipleOf 5 // [ 5; 10; 15; 20; 25; 30; 35; 40; 45; 50 ]


(*
-------------------------------------------------------------------------
F# units of measure
-------------------------------------------------------------------------
*)

type CurrencyRate<[<Measure>]'u, [<Measure>]'v> =
    { Rate: float<'u/'v>; Date: System.DateTime }

[<Measure>] type EUR
[<Measure>] type USD
[<Measure>] type GBP

let date = System.DateTime(2015, 11, 25)
let eurToUsdAtDate = { Rate= 1.2<USD/EUR>; Date= date }
let eurToGbpAtDate = { Rate= 1.2<GBP/EUR>; Date= date }

let tenEur = 10.0<EUR>
let tenEurInUsd = eurToUsdAtDate.Rate * tenEur

(*
-------------------------------------------------------------------------
Strategy Pattern
-------------------------------------------------------------------------
*)

type Animal(noiseMakingStrategy) = 
   member this.MakeNoise = 
      noiseMakingStrategy() |> printfn "Making noise %s" 
   
// now create a cat 
let meowing() = "Meow"
let cat = Animal(meowing)
cat.MakeNoise

// .. and a dog
let woofOrBark() = if (DateTime.Now.Second % 2 = 0) 
                   then "Woof" else "Bark"
let dog = Animal(woofOrBark)
dog.MakeNoise
