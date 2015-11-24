// <copyright file="FSharpIntro.fsx" company="Engie University">
//  Copyright (c) Engie, all rights reserved.
// </copyright>

open System

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

// normal version
let printTwoParameters x y = 
    printfn "x=%i y=%i" x y

// explicitly curried version
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

(*
-------------------------------------------------------------------------
Modules
-------------------------------------------------------------------------
*)

module Arithmetic = 

    let inline add x y = x + y
    let inline mult x y = x * y


module MathCalculus = 

    let add1 = Arithmetic.add 1 // partial application


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
open System.IO
let (@@) path1 path2 = Path.Combine(path1, path2)
let myFile = @"D:\trainings\FSharp" @@ "FSharp4Quants.fsx"

(*
-------------------------------------------------------------------------
Recursive functions
-------------------------------------------------------------------------
*)

open System.Numerics

let rec factorial i =       // val factorial : byte -> BigInteger
    match i with
    | 0uy | 1uy -> 1I
    | _   -> (bigint (i |> int)) * factorial (i-1uy)

30uy |> factorial |> printfn "30! = %A" // 265252859812191058636308480000000

let rec fib i =
    match i with
    | 0 | 1        -> i
    | n when n > 1 -> fib(n-2) + fib(n-1) // use of 'when' guards
    | n when n < 0 -> fib(n+2) - fib(n+1)

fib 30  |> printfn "fib(30) = %i"  // 832040
fib -30 |> printfn "fib(-30) = %i" // -832040

(*
-------------------------------------------------------------------------
Design Patterns
-------------------------------------------------------------------------
*)

//// Decorator Pattern

(*
let loggingCalculator innerCalculator input = 
   printfn "input is %A" input
   let result = innerCalculator input
   printfn "result is %A" result
   result
*)

/// A generic wrapper logger function that works with any function
let genericLogger anyFunc input =       // val genericLogger : ('a -> 'b) -> 'a -> 'b
    printfn "input is %A" input         // log the input
    let result = anyFunc input          // evaluate the function
    printfn "result is %A" result       // log the result
    result                              // return the result

let times2 input = input * 2            // val times2 : int -> int

let add1WithLogging = genericLogger add1        // val add1WithLogging : (int -> int)
let times2WithLogging = genericLogger times2    // val times2WithLogging : (int -> int)

// test
add1WithLogging 3
times2WithLogging 3

[1..5] |> List.map add1WithLogging

/// A generic wrapper timer function that works with any function
let genericTimer anyFunc input =        // val genericTimer : ('a -> 'b) -> 'a -> 'b
   let stopwatch = System.Diagnostics.Stopwatch()
   stopwatch.Start() 
   let result = anyFunc input           // evaluate the function
   printfn "elapsed ms is %A." stopwatch.ElapsedMilliseconds
   result                               // return the result

let add1WithTimer = genericTimer add1WithLogging        // val add1WithTimer : (int -> int)
let times2WithTimer = genericTimer times2WithLogging    // val times2WithTimer : (int -> int)

// test
add1WithTimer 3
times2WithTimer 3


//// Strategy Pattern

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

(*
-------------------------------------------------------------------------
Algebraic types and TDD
-------------------------------------------------------------------------
*)

// Representing Stock Options

/// The option kind
type OptionKind = Put | Call

/// An option is either American, European or a combination of options
type OptionAEC = 
    /// An American option
    | American of OptionKind * float
    /// A European option
    | European of OptionKind * float
    /// A combination of options
    | Combine  of OptionAEC * OptionAEC

/// Display the description of a stock option
let rec display (option) =
    match option with
    | OptionAEC.American (k, v) -> sprintf "an american option of kind %A and value %f" k v
    | OptionAEC.European (k, v) -> sprintf "a european option of kind %A and value %f" k v
    | OptionAEC.Combine  (l, r) -> sprintf "A combination of %s, and %s" (display l) (display r)

let american = OptionAEC.American (OptionKind.Call, 0.5)
let european = OptionAEC.European (OptionKind.Put, 0.75)
let option = OptionAEC.Combine (american, european)
display option

(*
-------------------------------------------------------------------------
Pattern matching
-------------------------------------------------------------------------
*)

// Matching tuples directly
let first, second, _ =  (1,2,3)  // underscore means ignore

// Matching lists directly
let e1::e2::rest = [1..10]       // ignore the warning for now

// Matching lists inside a match..with
let listMatcher list = 
    match list with
    | []              -> printfn "the list is empty" 
    | [first]         -> printfn "the list has one element %A " first 
    | [first; second] -> printfn "the list contains %A and %A" first second 
    | _               -> printfn "the list has more than two elements"

listMatcher [1;2;3;4]
listMatcher [1;2]
listMatcher [1]
listMatcher []

/// Gets the length of a list
let rec len list = 
    match list with
    | []  -> 0
    | [_] -> 1
    | head :: tail -> 1 + len tail

// create some types
type Address = { Street: string; City: string; }   
type Customer = { ID: int; Name: string; Address: Address}   

// create a customer 
let customer1 = { ID = 1; Name = "Bob"; 
      Address = { Street = "123 Main"; City = "NY" } }     

// extract name only
let { Name=name1 } = customer1 
printfn "The customer is called %s" name1

(*
-------------------------------------------------------------------------
Active Patterns
-------------------------------------------------------------------------
*)

// First example
// create an active pattern
let (|Int|_|) str =
    match System.Int32.TryParse(str) with
    | (true,int) -> Some(int)
    | _ -> None

// create an active pattern
let (|Bool|_|) str =
    match System.Boolean.TryParse(str) with
    | (true,bool) -> Some(bool)
    | _ -> None

// create a function to call the patterns
let testParse str = 
    match str with
    | Int i -> printfn "The value is an int '%i'" i
    | Bool b -> printfn "The value is a bool '%b'" b
    | _ -> printfn "The value '%s' is something else" str

// test
testParse "12"
testParse "true"
testParse "abc"


// Second example: the FizzBuzz challenge
// setup the active patterns
let (|MultOf3|_|) i = if i % 3 = 0 then Some MultOf3 else None
let (|MultOf5|_|) i = if i % 5 = 0 then Some MultOf5 else None

// the main function
let fizzBuzz i = 
  match i with
  | MultOf3 & MultOf5 -> printf "FizzBuzz, " 
  | MultOf3 -> printf "Fizz, " 
  | MultOf5 -> printf "Buzz, " 
  | _ -> printf "%i, " i
  
// test
[1..20] |> List.iter fizzBuzz


// Third example: file or directory (from FAKE/FakeLib/FileHelper.fs)
/// Active pattern which discriminates between files and directories.
let (|File|Directory|) (fileSysInfo : FileSystemInfo) = 
    match fileSysInfo with
    | :? FileInfo      as file -> File(file)
    | :? DirectoryInfo as dir  -> Directory(dir, dir.EnumerateFileSystemInfos())
    | _ -> failwith "No file or directory given."

// test
let file = FileInfo(myFile)
match file with
| File f -> sprintf "File of name '%s'" f.Name
| Directory dir -> sprintf "Directory of name '%s'" (fst dir |> fun d -> d.Name )

(*
-------------------------------------------------------------------------
F# completeness
-------------------------------------------------------------------------
*)

// impure code when needed
let mutable counter = 0
 
// create C# compatible classes and interfaces
type IEnumerator<'a> =
    abstract member Current : 'a
    abstract MoveNext : unit -> bool
 
// extension methods
type System.Int32 with
    member this.IsEven = (this % 2 = 0)
 
let i = 20
match i.IsEven with
| true  -> printfn "'%i' is even." i
| false -> printfn "'%i' is not even." i


// UI code
open System.Windows.Forms
let form = new Form(Width = 400, Height = 300
    , Visible = true, Text = "Hello World")
form.TopMost <- true
form.Click.Add (fun args -> printfn "Clicked!")
form.Show()

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
Collections
-------------------------------------------------------------------------
*)

// List comprehension
let multipleOf x = [ for i in 1 .. 10 do yield x * i ]

// [ 5; 10; 15; 20; 25; 30; 35; 40; 45; 50 ]
multipleOf 5
|> List.iteri (fun i r -> printfn "Result at index '%i' is '%i'" i r)

(*
-------------------------------------------------------------------------
Async and Parallel programming
-------------------------------------------------------------------------
*)

//// Pattern #1: Parallel CPU Asyncs

let fibs =
    [ for i in 0..40 -> async { return fib(i) } ]
    |> Async.Parallel           // Execute multiple asynchronous operations in parallel.
    |> Async.RunSynchronously   // Execute an asynchronous operation and wait for its result.


//// Pattern #2:  Parallel I/O Asyncs

// open System
// open System.IO
open System.Net
open Microsoft.FSharp.Control.WebExtensions

let http (name, url) =
    async {
        try 
            let uri = System.Uri(url)
            let webClient = new WebClient()
            let! html = webClient.AsyncDownloadString(uri) // let! : binding is made to the result of an asynchronous primitive.
            printfn "Read %d characters for %s" html.Length name
        with
            | ex -> printfn "%s" (ex.Message)
    }

let sites = ["Bing",   "http://www.bing.com";
             "Google", "http://www.google.com";
             "Yahoo",  "http://www.yahoo.com"]

let htmlOfSites =
    [for site in sites -> http site ]
    |> Async.Parallel           // Execute multiple asynchronous operations in parallel.
    |> Async.RunSynchronously   // Execute an asynchronous operation and wait for its result.

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


//// Combining units

[<Measure>] type m
[<Measure>] type sec
[<Measure>] type kg

let distance = 1.0<m>    
let time = 2.0<sec>    
let speed = distance/time 
let acceleration = speed/time
let mass = 5.0<kg>    
let force = mass * speed/time


//// Conversions

/// Degrees Celsius
[<Measure>] type degC

/// Degrees Farenheit
[<Measure>] type degF

/// Convert degrees Celsius to Farenheits
let convertDegCToF c = 
    c * 1.8<degF/degC> + 32.0<degF>

// test    
printfn "0°C is equivalent to %A°F." <| convertDegCToF 0.0<degC>
