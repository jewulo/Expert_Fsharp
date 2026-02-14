namespace chapter_3
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

        // I think the above is equivalent to this really, the x becomes redundant
        let round2b x = 
            match x with
            | x when x >= 100 -> 100
            | x when x < 0 -> 0
            | x -> x

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

            printfn " (round2b)"
            printfn "%d" (round2b 80)
            printfn "%d" (round2b 180)
            printfn "%d" (round2b -80)
            printfn " (round2b)"

            printfn "%A" (round3 (80, -40))
            printfn "%A" (round3 (180, 0))
            printfn "%A" (round3 (-180, 600))

    module defining_recursive_functions = 
        open System.Net.Http
        open System.Threading.Tasks

        let rec factorial n = if n <= 1 then 1 else n * factorial (n - 1)

        let rec length l =
            match l with
            | [] -> 0
            | h :: t -> 1 + length t
        
        /// Get the contents of the URL via a web request using HttpClient (async)
        let http (url: string) =
            use client = new HttpClient()
            let task: Task<string> = client.GetStringAsync(url)
            task.Result

        let rec repeatFetch url n =
            if n > 0 then
                let html = http url
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

        let run_fetch () =
            match (fetch "http://www.nature.com") with
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

    module pattern_matching = 
        let run () =
            ()

    module execute_module =
        let run () =
            printfn "[---- Expert F#: START CHAPTER 3 ----]"

            simple_types_and_literals.run()
            arithmetic_conversions.run()
            simple_strings.run()
            working_with_conditionals.run()
            defining_recursive_functions.run()
            lists.run()
            options.run()

            printfn "[---- Expert F#: END CHAPTER 3 ----]"
            printfn ""



