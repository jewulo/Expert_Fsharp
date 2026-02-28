module chapter_3

    /// Collection of code used in most examples in Chapter 3
    module http_stuff =
        open System.Net.Http
        open System.Threading.Tasks

        /// Get the contents of the URL via a web request using HttpClient (async)
        let http (url: string) =
            use client = new HttpClient()
            let task: Task<string> = client.GetStringAsync(url)
            task.Result

        let fetch url = 
            try Some (http url)
            with :? System.Net.WebException -> None

    module simple_types_and_literals =

        // squareAndAdd1 defaults to a:int -> b:int -> int
        let squareAndAdd1 a b = a * a + b

       // squareAndAdd2 defaults to a:float -> b:float -> float
       // because parameter 'a' was defined as a float. 
        let squareAndAdd2 (a: float) b = a * a + b

        // personally, I would rather declare my intent
        let squareAndAdd3 (a: float) (b: float) = a * a + b

        let run () = 
            printfn "%d" (squareAndAdd1 3 4)
            printfn "%f" (squareAndAdd2 3.0 4.0)
            printfn "%f" (squareAndAdd3 3.0 4.0)

    module arithmetic_conversions =

        let run () = 
            printfn "%d" (int 17.8)
            printfn "%d" (int -17.8)
            printfn "%A" (string 65)
            printfn "%f" (float 65)

    module simple_strings =

        let run () = 
            let s0 = "Couldn't put Humpty"
            printfn "%d" s0.Length
            printfn "%c" s0.[13]
            printfn "%s" s0.[13..16]

            // strings are immutable
            //s.[13] <- 'h'   // error FS0751: Invalid indexer expression
                            // error FS0810: Property 'Chars' cannot be set
                            // error FS0257: Invalid mutation of a constant expression.

            // simplest way to build strings is via concatenation using the + operator
            let s1 = "Couldn't put Humpty" + " " + "together again"
            printfn "%s" s1

    module working_with_conditionals = 
        let round1 x = 
            if x >= 100 then 100
            elif x < 0 then 0
            else x

        let round2 x = 
            match x with
            | _ when x >= 100 -> 100
            | _ when x < 0 -> 0
            | _ -> x

        let round3 (x, y) = 
            if x >= 100 || y > 100 then 100, 100
            elif x < 0 || y < 0 then 0, 0
            else x, y
            
        let run () =
            printfn "%d" (round1 80)
            printfn "%d" (round1 180)
            printfn "%d" (round1 -80)

            printfn "%d" (round2 80)
            printfn "%d" (round2 180)
            printfn "%d" (round2 -80)

            printfn "%A" (round3 (80, -40))
            printfn "%A" (round3 (180, 0))
            printfn "%A" (round3 (-180, 600))

    module defining_recursive_functions = 
        let rec factorial n = if n <= 1 then 1 else n * factorial (n - 1)

        let rec length l =
            match l with
            | [] -> 0
            | h :: t -> 1 + length t
        
        let rec repeatFetch url n =
            if n > 0 then
                let html = http_stuff.http url
                printfn "fetched <<< %s >>> on iteration %d" html n
                repeatFetch url (n - 1)

        let rec badFactorial n = if n <= 1 then 1 else n * badFactorial n

        // Defining mutually recursive functions with 'and' keyword
        let rec even1 n = (n = 0u) || odd1(n - 1u)
        and odd1 n = (n <> 0u) && even1(n - 1u)

        let even2 n = (n % 2u) = 0u
        let odd2 n = (n % 2u) = 1u

        let run() =
            printfn "%d" (factorial 5)
            printfn "%d" (factorial 6)
            printfn "%d" (factorial 7)

            printfn "%d" (length [])
            printfn "%d" (length [1])
            printfn "%d" (length [1; 2])
            printfn "%d" (length [1; 2; 3])

    module lists = 
        let run () =
            let oddPrimes = [3; 5; 7; 11]
            let morePrimes = [13; 17]
            let primes = 2 :: (oddPrimes @ morePrimes)

            printfn "%A" oddPrimes
            printfn "%A" morePrimes
            printfn "%A" primes

            let people = ["Adam"; "Dominic"; "James"]
            printfn "%A" people

            // append "Chris" to people using cons operator ':: '
            let extra_person = "Chris" :: ["Adam"; "Dominic"; "James"]
            printfn "%A" extra_person
            printfn "%A" people         // F# lists are immutable, so people is still ["Adam"; "Dominic"; "James"]  

            printfn "%d" (List.head [5; 4; 3])
            printfn "%A" (List.tail [5; 4; 3])

            let mapped_list = List.map (fun x -> x * x) [1; 2; 3]
            printfn "%A" mapped_list

            let filtered_list = List.filter (fun x -> x % 3 = 0) [2; 3; 5; 7; 9]
            printfn "%A" filtered_list

    module options = 
        let run_fetch () =
            match (http_stuff.fetch "http://www.nature.com") with
            | Some text -> printfn "text = %s" text
            | None -> printfn "**** no web page found"

        let run () =
            let people = [
                ("Adam", None);
                ("Eve",  None);
                ("Cain", Some("Adam", "Eve"));
                ("Abel", Some("Adam", "Eve"))]

            printfn "%A" people

            run_fetch()

    module getting_started_on_pattern_matching = 
        let isLikelySecretAgent url agent =
            match (url, agent) with
            | "http://www.control.org", 99 -> true
            | "http://www.control.org", 86 -> true
            | "http://www.kaos.org", _ -> true
            | _ -> false

        let printFirst primes =
            match primes with
            | h :: t -> printfn "The first prime in the list is %d" h
            | [] -> printfn "No primes found in the list"

        let showParents (name, parents) =
            match parents with
            | Some (dad,  mum) -> printfn "%s has father %s , mother %s" name dad mum
            | None -> printfn "%s has no parents!" name

        let run () =
            printfn "%A" (isLikelySecretAgent "http://www.control.org", 99)
            printfn "%A" (isLikelySecretAgent "http://www.control.org", 86)
            printfn "%A" (isLikelySecretAgent "http://www.control.org", 97)
            printfn "%A" (isLikelySecretAgent "http://www.control.org", 87)
            printfn "%A" (isLikelySecretAgent "http://www.kaos.org", 99)
            printfn "%A" (isLikelySecretAgent "http://www.kaos.org", 86)            
            printfn "%A" (isLikelySecretAgent "http://www.bbc.org", 99)
            printfn "%A" (isLikelySecretAgent "http://www.bbc.org", 86)

            let oddPrimes = [3; 5; 7; 11]
            printFirst oddPrimes

            showParents ("Adam", None)
            showParents ("Cain",Some ("Adam", "Eve"))

    module matching_on_structured_values =
        // decompose a tuple using match
        let highLow a b =
            match (a, b) with
            | ("lo", lo), ("hi", hi) -> (lo, hi)
            | ("hi", hi), ("lo", lo) -> (lo, hi)
            | _ -> failwith "expected a both a high and low value"

        // non-exhaustive match. #warning FS0025 '(_, 0)' is not covered
        let urlFilter3 url agent =
            match url, agent with
            | "http://www.control.org", 86 -> true
            | "http://www.kaos.org", _ -> false

        // to solve this add an extra exeception throwing clause
        let urlFilter4 url agent =
            match url, agent with
            | "http://www.control.org", 86 -> true
            | "http://www.kaos.org", _ -> false
            | _ -> false

        // redundant rules
        let urlFilter2 url agent =
            match url, agent with
            | "http://www.control.org", _ -> true
            | "http://www.control.org", 86 -> true  // this rule is covered by the first rule
            | _ -> false

        let run () =

            printfn "%A" (highLow ("hi", 300) ("lo", 100))
            printfn "%A" (highLow ("lo", 1000) ("hi", 3000))

            printfn "%A" (urlFilter3 "http://www.control.org" 86)
            printfn "%A" (urlFilter3 "http://www.control.org" 300)
            printfn "%A" (urlFilter3 "http://www.kaos.org" 86)
            printfn "%A" (urlFilter3 "http://www.koas.org" 300)
            printfn "%A" (urlFilter3 "http://www.bbc.org" 300)

            printfn "%A" (urlFilter4 "http://www.control.org", 86)
            printfn "%A" (urlFilter4 "http://www.control.org", 300)
            printfn "%A" (urlFilter4 "http://www.kaos.org", 86)
            printfn "%A" (urlFilter4 "http://www.koas.org", 300)
            printfn "%A" (urlFilter4 "http://www.bbc.org", 300)

            printfn "%A" (urlFilter2 "http://www.control.org", 86)
            printfn "%A" (urlFilter2 "http://www.control.org", 300)
            printfn "%A" (urlFilter2 "http://www.kaos.org", 86)
            printfn "%A" (urlFilter2 "http://www.koas.org", 300)
            printfn "%A" (urlFilter2 "http://www.bbc.org", 300)

    module guarding_rules_and_combining_patterns =
        let sign x = 
            match x with 
            | _ when x < 0 -> -1
            | _ when x > 0 ->  1
            | _ -> 0

        let getValue a = 
            match a with 
            | (("lo" | "low"), v) -> v
            | ("hi", v) | ("high", v) -> v
            | _ -> failwith "expected a both a high and low value"

        let run () =
            //printfn "%d" (sign 300)
            //printfn "%d" (sign -300)
            //printfn "%d" (sign 0)
            ()

    module further_ways_of_forming_patterns =        
        let run () = ()

    module introducing_function_values =        
        let run () =
            let sites = ["http://www.live.com"; "http://www.google.com"]
            let fetch url = (url, http_stuff.http url)
            let fetched_sites = List.map fetch sites
            printfn "%A" fetched_sites

    module using_anonymous_function_values_1 =
        let run () =
            let primes = [2; 3; 5; 7; 11]
            let primesCube = List.map (fun n -> n * n * n) primes
            printfn "%A" primesCube

    module using_anonymous_function_values_2 =
        let run () =
            let sites = ["http://www.live.com"; "http://www.google.com"]
            let resultOfFetch = List.map (fun url -> (url, http_stuff.http url)) sites
            let resultOfMapLength = List.map (fun (_,p) -> String.length p) resultOfFetch
            printfn "%A" resultOfMapLength

    module computing_with_aggregate_operators =
        /// Get the contents of the URL via a web request using HttpClient (async)
        let delimiters  = [| ' '; '\n'; '\t'; '<'; '>'; '='|]

        let getWords (s: string) = s.Split delimiters

        let getStats site =
            let url = "http://" + site
            let html = http_stuff.http url
            let hwords = html |> getWords
            let hrefs = html |> getWords |> Array.filter (fun s -> s = "href")
            (site, html.Length, hwords.Length, hrefs.Length)

        let run () =
            let sites = ["http://www.live.com"; "http://www.google.com"]
            let stats = sites |> List.map getStats;
            printfn "%A" stats    

    module building_functions_with_partial_application =
        let shift (dx, dy) (px, py) = (px + dx, py + dy)
        let shiftRight = shift (1, 0)
        let shiftUp = shift (0, 1)
        let shiftLeft = shift (-1, 0)
        let shiftDown = shift (0, -1)

        let run () =
            //let sr = 
            printfn "%A" (shiftRight (10, 10))
            printfn "%A" (shiftUp (10, 10))
            printfn "%A" (shiftLeft (10, 10))
            printfn "%A" (shiftDown (10, 10))

            // pass a partially applied shift function (shift(2,2)) to List.map
            // this will return shifted coordinates in a list
            let sc = List.map(shift (2, 2)) [(0, 0); (1, 0); (1, 1); (0, 1)] 
            printfn "%A" sc

            // pass a shift function to List.map
            // this will return functions for shifting by the various coordinates in a list
            let scf = List.map shift [(0, 0); (1, 0); (1, 1); (0, 1)] 
            printfn "%A" scf
            
    module using_local_functions =
        open System.Drawing

        let remap (r1: Rectangle) (r2: Rectangle) =
            let scalex = float r2.Width / float r1.Width
            let scaley = float r2.Height / float r1.Height
            let mapx x = int (float r2.Left + truncate (float (x - r1.Left) * scalex))
            let mapy y = int (float r2.Top + truncate (float (y - r1.Top) * scaley))
            let mapp (p:Point) = Point(mapx p.X, mapy p.Y)
            mapp

        let run () =
            let mapp = remap (Rectangle(100, 100,100,100)) (Rectangle(50, 50,200,200))
            printfn "%A" mapp
            printfn "%A" (mapp (Point(100, 100)))
            printfn "%A" (mapp (Point(150, 150)))
            printfn "%A" (mapp (Point(200, 200)))

    module abstracting_control_with_functions =
        open System

        let time f = 
            let start = DateTime.Now
            let res = f()
            let finish = DateTime.Now
            (res, finish - start)

        let run () =
            let ts = time (fun () -> (3 + 4))
            printfn "%A" ts

    module using_object_methods_as_first_class_functions =
        open System
        open System.IO

        let get_directories () =            
            let res = [@"C:\Program Files"; @"C:\Windows"] |> List.map Directory.GetDirectories
            printfn "%A" res

        let store_dot_net_object_method () =
            // let f = Console.WriteLine; //  error FS0041: A unique overload for method 'WriteLine' could not be determined based on type information 
            let f = (Console.WriteLine : string -> unit)
            f("Stored Console.WriteLine")

        let run () =
            get_directories()
            store_dot_net_object_method()

    module execute_modules =
        let run () =
            printfn "[---- Expert F#: START CHAPTER 3 ----]"

            //simple_types_and_literals.run()
            //arithmetic_conversions.run()
            //simple_strings.run()
            //working_with_conditionals.run()
            //defining_recursive_functions.run()
            //lists.run()
            //options.run()
            //getting_started_on_pattern_matching.run()
            //matching_on_structured_values.run()
            //guarding_rules_and_combining_patterns.run()
            //introducing_function_values.run()
            //using_anonymous_function_values_1.run()
            //using_anonymous_function_values_2.run()
            //computing_with_aggregate_operators.run()
            building_functions_with_partial_application.run()
            using_local_functions.run()
            abstracting_control_with_functions.run()
            using_object_methods_as_first_class_functions.run()

            printfn "[---- Expert F#: END CHAPTER 3 ----]"
            printfn ""



