// <copyright file="FSharp4Quants.fsx" company="Engie University">
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
printfn "Length of the string: %i" s.Length

// Primitive types, and type inference
let v1 : float = 1.0    // type 'float' in F# means 'System.Double' !
let v2 = 1.0            // val v2:float

let v3 = 1.0f           // val v3:float32 (equiv. of 'System.Single')

let b1 = true           // val b1:bool
// b1 <- false          // Value are immutable by default! Compile-time error

let nameTuple = ("James", "Bond")
let firstName = fst nameTuple   // James
let lastName = snd nameTuple    // Bond

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

// Note the use of double-backticks
let ``result is 3.0`` = 1.0 + 2.0
let ``result is 2.0`` = 3.0 - 1.0
let ``result is 6.0`` = 2.0 * 3.0
let ``result is 0.5`` = 1.0 / 2.0
let ``result is 8.0`` = 2.0 ** 3.0
let ``result is 1`` = 7 % 3

(*
-------------------------------------------------------------------------
Common math functions
-------------------------------------------------------------------------
*)

let ``abs(-1) = 1`` = abs -1
let ``sign(-5) = -1`` = sign -5
let ``ceil(9.1) = 10.0`` = ceil 9.1
let ``floor(9.9) = 9.0`` = floor 9.9

let ``sqrt(4.0) = 2.0`` = sqrt 4.0
let ``exp(1.0) ≃ 2.7182818`` = exp 1.0
let ``ln(exp(1.0)) = 1.0`` = 1.0 |> (exp >> log) // composition and piping
let ``log(10.0) = 1.0`` = log10 10.0
let ``2^10 = 1024`` = pown 2L 10

let pi = Math.PI
let ``sin(pi/2) = 1.0`` = pi / 2.0 |> sin   // forward-pipe
let ``cos(pi) = -1.0`` = cos pi
let ``tan(pi) = 0`` = tan pi

(*
-------------------------------------------------------------------------
F# functions
-------------------------------------------------------------------------
*)

let inline add n x = x + n
let add1 = add 1    // partial application
printfn "Adding 1 to 2 should be 3 : %A" ((add1 2) = 3) // true

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

printfn "add5Times3 1 is %d" (add5Times3 1) // 18

// Add 5, then multiply by 3
let add5ThenMultiplyBy3 = (+) 5 >> (*) 3

printfn "add5ThenMultiplyBy3 1 is %d" <| add5ThenMultiplyBy3 1 // 18

// Recursive functions
let rec factorial i =
    match i with
    | 0L | 1L -> 1L
    | _  -> i * factorial (i-1L)

factorial 10L |> printfn "10! = %d" // 3628800

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
F# collections
-------------------------------------------------------------------------
*)

// List comprehension
let multipleOf x = [ for i in 1 .. 10 do yield x * i]
let multipleOf5 = multipleOf 5 // [ 5; 10; 15; 20; 25; 30; 35; 40; 45; 50 ]



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
